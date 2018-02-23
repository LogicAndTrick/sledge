using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Tools.VMTool.Actions;
using Sledge.Editor.Tools.VMTool.Controls;
using Sledge.Editor.Tools.VMTool.SubTools;
using Sledge.Settings;

namespace Sledge.Editor.Tools.VMTool
{
    public class VMTool : BaseDraggableTool
    {
        private readonly VMSidebarPanel _controlPanel;
        private readonly VMErrorsSidebarPanel _errorPanel;
        
        private readonly BoxDraggableState _boxState;

        private ShowPoints ShowPoints { get; set; }
        private List<VMPoint> Points { get; set; }
        private List<VMSolid> Solids { get; set; }
        
        public VMTool()
        {
            _errorPanel = new VMErrorsSidebarPanel();
        }

        #region Tool switching

        internal VMSubTool CurrentSubTool
        {
            get { return Children.OfType<VMSubTool>().FirstOrDefault(x => x.Active); }
            set
            {
                foreach (var tool in Children.Where(x => x != value && x.Active))
                {
                    tool.ToolDeselected(false);
                    tool.Active = false;
                }
                if (value != null)
                {
                    value.Active = true;
                    value.ToolSelected(false);
                }
            }
        }

        private void AddTool(VMSubTool tool)
        {
            _controlPanel.AddTool(tool);
            Children.Add(tool);
        }

        private void VMToolSelected(object sender, VMSubTool tool)
        {
            if (CurrentSubTool == tool) return;

            _controlPanel.SetSelectedTool(tool);
            CurrentSubTool = tool;

            Mediator.Publish(EditorMediator.ContextualHelpChanged);
            Invalidate();
        }

        private void VMStandardMode()
        {
            VMToolSelected(this, Children.OfType<VMStandardTool>().FirstOrDefault());
        }

        private void VMScalingMode()
        {
            //VMToolSelected(this, Children.First(x => x is ScaleTool));
        }

        private void VMFaceEditMode()
        {
            //VMToolSelected(this, Children.First(x => x is EditFaceTool));
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.HistoryUndo:
                    MessageBox.Show("Exit the VM tool to undo changes.");
                    //if (Document.History.CanUndo()) Document.History.Undo();
                    //else MessageBox.Show("Nothing to undo in the VM tool"); // todo pop the stack and so on?
                    return HotkeyInterceptResult.Abort;
                case HotkeysMediator.HistoryRedo:
                    // if (Document.History.CanRedo()) Document.History.Redo();
                    return HotkeyInterceptResult.Abort;
                case HotkeysMediator.OperationsPaste:
                case HotkeysMediator.OperationsPasteSpecial:
                    return HotkeyInterceptResult.SwitchToSelectTool;
                case HotkeysMediator.SwitchTool:
                    if (parameters is HotkeyTool && (HotkeyTool)parameters == GetHotkeyToolType())
                    {
                        CycleShowPoints();
                        return HotkeyInterceptResult.Abort;
                    }
                    return HotkeyInterceptResult.Continue;
                case HotkeysMediator.SelectionClear:
                    Cancel();
                    return HotkeyInterceptResult.Abort;
                case HotkeysMediator.VMFaceEditMode:
                case HotkeysMediator.VMScalingMode:
                case HotkeysMediator.VMSplitFace:
                case HotkeysMediator.VMStandardMode:
                    return HotkeyInterceptResult.Continue;
            }
            return HotkeyInterceptResult.Abort; // Don't allow stuff to happen when inside the VM tool. todo: fix/make this more generic?
        }
        #region Commit VM changes

        private void Reset(object sender)
        {
            Clear();
            SelectionChanged();
        }

        protected void CommitChanges()
        {
            CommitChanges(Solids);
        }

        protected void CommitChanges(IEnumerable<VMSolid> solids)
        {
            var list = solids.ToList();
            if (!list.Any()) return;

            // Reset all changes
            foreach (var s in list)
            {
                Solids.Remove(s);
                Points.RemoveAll(x => x.Solid == s);
                s.Original.IsCodeHidden = s.Copy.IsCodeHidden = false;
                s.Original.Faces.ForEach(x => x.IsSelected = false);
                s.Copy.Faces.ForEach(x => x.IsSelected = false);
            }

            // Commit the changes
            if (list.Any(x => x.IsDirty))
            {
                var dirty = list.Where(x => x.IsDirty).ToList();
                var edit = new ReplaceObjects(dirty.Select(x => x.Original), dirty.Select(x => x.Copy));
                PerformAndCommitAction("Vertex manipulation on " + dirty.Count + " solid" + (dirty.Count == 1 ? "" : "s"), edit);
            }

            // Notify that we've unhidden some stuff
            if (list.Any(x => !x.IsDirty))
            {
                Mediator.Publish(EditorMediator.SceneObjectsUpdated, list.Where(x => !x.IsDirty).Select(x => x.Original));
            }

            Invalidate();
        }

        public void PerformAction(VMAction action)
        {
            try
            {
                action.Redo(Document);
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var frames = st.GetFrames() ?? new StackFrame[0];
                var msg = "Action exception: " + action.Name + " (" + action + ")";
                foreach (var frame in frames)
                {
                    var method = frame.GetMethod();
                    msg += "\r\n    " + method.ReflectedType.FullName + "." + method.Name;
                }
                Logging.Logger.ShowException(new Exception(msg, ex), "Error performing action");
            }

            Document.History.AddHistoryItem(action);
        }

        public void PerformAndCommitAction(string name, IAction action)
        {
            //Document.History.PopStack();
            Document.PerformAction(name, action);
            //Document.History.PushStack("VM Tool");
        }
        #endregion

        #region Selection changed
        private void SelectionChanged()
        {
            // Find the solids that were selected and now aren't
            var sel = Document.Selection.GetSelectedObjects().SelectMany(x => x.FindAll()).OfType<Solid>().Distinct().ToList();
            var diff = Solids.Where(x => !sel.Contains(x.Original));

            // Commit the difference
            CommitChanges(diff);
            Clear();

            // Find the solids that are now selected and weren't before
            var newSolids = sel.Where(x => Solids.All(y => y.Original.ID != x.ID)).ToList();
            foreach (var so in newSolids)
            {
                var vmsolid = new VMSolid(this, so);
                so.IsCodeHidden = true;
                Solids.Add(vmsolid);
                Points.AddRange(vmsolid.Points);
            }

            Points.Sort((a, b) => b.IsMidpoint.CompareTo(a.IsMidpoint));

            // Notify if we've changed anything
            if (newSolids.Any())
            {
                Mediator.Publish(EditorMediator.SceneObjectsUpdated, newSolids);
            }

            foreach (var sub in Children.OfType<VMSubTool>().Where(x => x.Active))
            {
                sub.SelectionChanged();
            }
            Invalidate();
        }
        #endregion

        #region Points / Solids

        public IEnumerable<VMSolid> GetSolids()
        {
            return Solids;
        }

        public VMSolid GetVmSolid(Solid s)
        {
            return Solids.FirstOrDefault(x => ReferenceEquals(x.Original, s));
        }

        public void Clear()
        {
            Solids.Clear();
            Points.Clear();
            Invalidate();
        }

        public VMPoint GetPointByID(long objectId, int pointId)
        {
            return Points.FirstOrDefault(x => x.ID == pointId && x.Solid.Original.ID == objectId);
        }

        internal IEnumerable<VMPoint> GetVisiblePoints()
        {
            switch (ShowPoints)
            {
                case ShowPoints.All:
                    return Points;
                case ShowPoints.Vertices:
                    return Points.Where(x => !x.IsMidpoint);
                case ShowPoints.Midpoints:
                    return Points.Where(x => x.IsMidpoint);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void RefreshPoints(IList<VMSolid> solids)
        {
            var newPoints = Points.Where(x => !solids.Contains(x.Solid)).ToList();
            foreach (var solid in solids) solid.RefreshPoints();
            Points.Clear();
            Points.AddRange(newPoints.Union(solids.SelectMany(x => x.Points)));
            Points.Sort((a, b) => b.IsMidpoint.CompareTo(a.IsMidpoint));
        }

        public void UpdatePoints(IList<VMSolid> solids)
        {
            foreach (var solid in solids) solid.UpdatePoints();
        }

        public void UpdateSolids(IList<VMSolid> solids, bool refreshPoints)
        {
            if (!solids.Any()) return;

            foreach (var solid in solids)
            {
                solid.IsDirty = true;
                foreach (var face in solid.Copy.Faces)
                {
                    if (face.Vertices.Count >= 3) face.Plane = new Plane(face.Vertices[0].Location, face.Vertices[1].Location, face.Vertices[2].Location);
                    face.UpdateBoundingBox();
                }
            }

            if (refreshPoints) RefreshPoints(solids);
            else UpdatePoints(solids);
            Invalidate();
        }

        private void Select(List<VMPoint> points, bool toggle)
        {
            if (!points.Any()) return;
            if (!toggle) Points.ForEach(x => x.IsSelected = false);
            var first = points[0];
            var val = !toggle || !first.IsSelected;
            points.ForEach(x => x.IsSelected = val);

            Invalidate();
        }

        public bool SelectPointsInBox(Box box, bool toggle)
        {
            var inBox = GetVisiblePoints().Where(x => box.CoordinateIsInside(x.Position)).ToList();
            Select(inBox, toggle);
            return inBox.Any();
        }

        public void DeselectAll()
        {
            Points.ForEach(x => x.IsSelected = false);

            Invalidate();
        }

        public bool CanDragPoint(VMPoint point)
        {
            return Children.Where(x => x.Active).OfType<VMSubTool>().All(x => x.CanDragPoint(point));
        }

        #endregion

        #region Errors

        public IEnumerable<VMError> GetErrors()
        {
            return Solids.SelectMany(x => x.GetErrors());
        }

        #endregion

        public new void Invalidate()
        {
            _errorPanel.SetErrorList(GetErrors());
            base.Invalidate();
        }

        private void CycleShowPoints()
        {
            var side = (int)ShowPoints;
            side = (side + 1) % (Enum.GetValues(typeof(ShowPoints)).Length);
            ShowPoints = (ShowPoints)side;
            Invalidate();
        }

        public override void ToolSelected(bool preventHistory)
        {
            //Document.History.PushStack("VM Tool");

            SelectionChanged();

            Mediator.Subscribe(EditorMediator.SelectionChanged, this);
            Mediator.Subscribe(HotkeysMediator.VMStandardMode, this);
            Mediator.Subscribe(HotkeysMediator.VMScalingMode, this);
            Mediator.Subscribe(HotkeysMediator.VMFaceEditMode, this);

            base.ToolSelected(preventHistory);
        }

        public override void ToolDeselected(bool preventHistory)
        {
            Mediator.UnsubscribeAll(this);

            CommitChanges();
            Clear();

            //Document.History.PopStack();

            base.ToolDeselected(preventHistory);
        }
    }
}
