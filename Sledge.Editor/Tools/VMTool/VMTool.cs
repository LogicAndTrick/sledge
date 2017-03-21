using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.Properties;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Editor.Tools.VMTool.Actions;
using Sledge.Editor.Tools.VMTool.Controls;
using Sledge.Editor.Tools.VMTool.SubTools;
using Sledge.Editor.UI;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Renderables;
using Sledge.Settings;
using Line = Sledge.DataStructures.Geometric.Line;
using View = Sledge.Settings.View;

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
            _controlPanel = new VMSidebarPanel();
            _controlPanel.ToolSelected += VMToolSelected;
            _controlPanel.DeselectAll += x => DeselectAll();
            _controlPanel.Reset += Reset;

            _errorPanel = new VMErrorsSidebarPanel();

            Points = new List<VMPoint>();
            Solids = new List<VMSolid>();

            Usage = ToolUsage.Both;

            _boxState = new BoxDraggableState(this);
            _boxState.BoxColour = Color.Orange;
            _boxState.FillColour = Color.FromArgb(View.SelectionBoxBackgroundOpacity, Color.DodgerBlue);
            _boxState.DragStarted += (sender, args) =>
            {
                if (!KeyboardState.Ctrl)
                {
                    DeselectAll();
                }
            };
            
            States.Add(new VMPointsState(this));
            States.Add(_boxState);

            AddTool(new VMStandardTool(this));
            AddTool(new VMScaleTool(this));
            AddTool(new VMFaceEditTool(this));

            UseValidation = true;
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

        public override void DocumentChanged()
        {
            _controlPanel.Document = Document;
        }

        #endregion

        #region Default tool overrides

        public override Image GetIcon()
        {
            return Resources.Tool_VM;
        }

        public override string GetName()
        {
            return "Vertex Manipulation Tool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.VM;
        }

        public override string GetContextualHelp()
        {
            return Children.Where(x => x.Active).Select(x => x.GetContextualHelp()).FirstOrDefault() ?? "Select a VM mode for more information";
        }

        public override IEnumerable<KeyValuePair<string, Control>> GetSidebarControls()
        {
            yield return new KeyValuePair<string, Control>(GetName(), _controlPanel);
            yield return new KeyValuePair<string, Control>("VM Errors", _errorPanel);
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

        #endregion

        public override void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Confirm();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Cancel();
                e.Handled = true;
            }
            base.KeyDown(viewport, e);
        }

        #region Box confirm / cancel
        private void Confirm()
        {
            if (_boxState.State.Action != BoxAction.Drawn) return;
            var bbox = _boxState.State.GetSelectionBox();
            if (bbox != null && !bbox.IsEmpty())
            {
                SelectPointsInBox(bbox, KeyboardState.Ctrl);
                _boxState.RememberedDimensions = bbox;
            }
            _boxState.State.Action = BoxAction.Idle;

            Invalidate();
        }

        private void Cancel()
        {
            if (_boxState.State.Action != BoxAction.Idle)
            {
                _boxState.RememberedDimensions = new Box(_boxState.State.Start, _boxState.State.End);
                _boxState.State.Action = BoxAction.Idle;
            }
            else
            {
                DeselectAll();
            }

            Invalidate();
        }
        #endregion

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

        #region Selection - 3D

        /// <summary>
        /// When the mouse is pressed in the 3D view, we want to select the clicked object.
        /// </summary>
        /// <param name="viewport">The viewport that was clicked</param>
        /// <param name="camera"></param>
        /// <param name="e">The click event</param>
        protected override void MouseDown(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            // Do not perform selection if space is down
            if (View.Camera3DPanRequiresMouseClick && KeyboardState.IsKeyDown(Keys.Space)) return;

            if (Try3DPointSelection(viewport, camera, e)) return;
            if (Try3DObjectSelection(viewport, camera, e)) return;

            base.MouseDown(viewport, camera, e);
        }

        protected bool Try3DPointSelection(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            var toggle = KeyboardState.Ctrl;

            var l = camera.EyeLocation;
            var pos = new Coordinate((decimal)l.X, (decimal)l.Y, (decimal)l.Z);
            var p = new Coordinate(e.X, e.Y, 0);
            const int d = 5;
            var clicked = (from point in GetVisiblePoints()
                           let c = viewport.WorldToScreen(point.Position)
                           where c != null && c.Z <= 1
                           where p.X >= c.X - d && p.X <= c.X + d && p.Y >= c.Y - d && p.Y <= c.Y + d
                           orderby (pos - point.Position).LengthSquared()
                           select point).ToList();
            Select(clicked, toggle);
            return clicked.Any();
        }

        private Coordinate GetIntersectionPoint(MapObject obj, Line line)
        {
            if (obj == null) return null;

            var solid = obj as Solid;
            if (solid == null) return obj.GetIntersectionPoint(line);

            return solid.Faces.Where(x => x.Opacity > 0 && !x.IsHidden)
                .Select(x => x.GetIntersectionPoint(line))
                .Where(x => x != null)
                .OrderBy(x => (x - line.Start).VectorMagnitude())
                .FirstOrDefault();
        }

        protected bool Try3DObjectSelection(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            // First, get the ray that is cast from the clicked point along the viewport frustrum
            var ray = viewport.CastRayFromScreen(e.X, e.Y);

            // Grab all the elements that intersect with the ray
            var hits = Document.Map.WorldSpawn.GetAllNodesIntersectingWith(ray, true);

            // Sort the list of intersecting elements by distance from ray origin
            var solid = hits
                .OfType<Solid>()
                .Select(x => new { Item = x, Intersection = GetIntersectionPoint(x, ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .Select(x => x.Item)
                .FirstOrDefault();

            if (solid != null)
            {
                if (solid.IsSelected && KeyboardState.Ctrl)
                {
                    // deselect solid
                    var select = new MapObject[0];
                    var deselect = new[] { solid };
                    Document.PerformAction("Deselect solid", new ChangeSelection(select, deselect));
                }
                else if (!solid.IsSelected)
                {
                    // select solid
                    var select = new[] { solid };
                    var deselect = !KeyboardState.Ctrl ? Document.Selection.GetSelectedObjects() : new MapObject[0];
                    Document.PerformAction("Select solid", new ChangeSelection(select, deselect));
                }

                return true;
            }

            return false;
        }

        #endregion

        #region Selection - 2D

        protected override void MouseDown(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (CurrentDraggable == null || CurrentDraggable == _boxState)
            {
                if (!KeyboardState.Ctrl) DeselectAll();
                if (Try2DObjectSelection(viewport, e)) return;
            }

            base.MouseDown(viewport, camera, e);
        }

        /*
         * Selection in 2D:
         * MouseDown:
         *   If ctrl is not down, deselect all
         *   Points <- all vertices under cursor
         *   If any Points is standard Points <- Points where standard
         *   Topmost <- closest Points to viewport
         *   If shift is down, Points <- { Topmost }
         *   Points <- Points where Solid = Topmost.Solid
         *   Val <- true
         *   If ctrl is down, Val <- !TopMost.IsSelected
         *   Topmost:
         *     If null, try 2D object selection instead
         *     Otherwise, Points each IsSelected <- Val
         * 
         */

        public List<VMPoint> GetPoints(MapViewport viewport, Coordinate position, bool allowMixed, bool topmostOnly, bool oneSolidOnly)
        {
            var p = viewport.Flatten(position);
            var d = 5 / (decimal)viewport.Zoom; // Tolerance value = 5 pixels

            // Order by the unused coordinate in the view (which is the up axis) descending to get the "closest" point
            var points = (from pp in GetVisiblePoints()
                          let c = viewport.Flatten(pp.Position)
                          where p.X >= c.X - d && p.X <= c.X + d && p.Y >= c.Y - d && p.Y <= c.Y + d
                          let unused = viewport.GetUnusedCoordinate(pp.Position)
                          orderby unused.X + unused.Y + unused.Z descending
                          select pp).ToList();

            if (!allowMixed && points.Any(x => !x.IsMidpoint)) points.RemoveAll(x => x.IsMidpoint);
            if (points.Count <= 0) return points;

            var first = points[0];
            if (topmostOnly) points = new List<VMPoint> { first };
            if (oneSolidOnly) points.RemoveAll(x => x.Solid != first.Solid);

            return points;
        }

        protected bool Try2DObjectSelection(MapViewport viewport, ViewportEvent e)
        {
            // Create a box to represent the click, with a tolerance level
            var unused = viewport.GetUnusedCoordinate(new Coordinate(100000, 100000, 100000));
            var tolerance = 4 / (decimal)viewport.Zoom; // Selection tolerance of four pixels
            var used = viewport.Expand(new Coordinate(tolerance, tolerance, 0));
            var add = used + unused;
            var click = viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y));
            var box = new Box(click - add, click + add);

            var centerHandles = Sledge.Settings.Select.DrawCenterHandles;
            var centerOnly = Sledge.Settings.Select.ClickSelectByCenterHandlesOnly;
            // Get the first element that intersects with the box, selecting or deselecting as needed
            var solid = Document.Map.WorldSpawn.GetAllNodesIntersecting2DLineTest(box, centerHandles, centerOnly).OfType<Solid>().FirstOrDefault();

            if (solid != null)
            {
                if (solid.IsSelected && KeyboardState.Ctrl)
                {
                    // deselect solid
                    var select = new MapObject[0];
                    var deselect = new[] { solid };
                    Document.PerformAction("Deselect solid", new ChangeSelection(select, deselect));
                }
                else if (!solid.IsSelected)
                {
                    // select solid
                    var select = new[] { solid };
                    var deselect = !KeyboardState.Ctrl ? Document.Selection.GetSelectedObjects() : new MapObject[0];
                    Document.PerformAction("Select solid", new ChangeSelection(select, deselect));
                }

                return true;
            }

            return false;
        }

        private bool _selectOnClick;

        public void PointMouseDown(MapViewport viewport, VMPoint point)
        {
            if (_boxState.State.Action != BoxAction.Idle)
            {
                _boxState.RememberedDimensions = new Box(_boxState.State.Start, _boxState.State.End);
                _boxState.State.Action = BoxAction.Idle;
            }

            var vtxs = GetPoints(viewport, point.Position, false, KeyboardState.Shift, true);
            if (!vtxs.Any()) return;

            _selectOnClick = true;
            if (!vtxs.Any(x => x.IsSelected))
            {
                Select(vtxs, KeyboardState.Ctrl);
                _selectOnClick = false;
            }
        }

        public void PointClick(MapViewport viewport, VMPoint point)
        {
            if (!_selectOnClick) return;
            _selectOnClick = false;

            var vtxs = GetPoints(viewport, point.Position, false, KeyboardState.Shift, true);
            if (!vtxs.Any()) return;
            Select(vtxs, KeyboardState.Ctrl);
        }

        #endregion

        #region Point dragging

        public void StartPointDrag(MapViewport viewport, ViewportEvent e, Coordinate startLocation)
        {
            foreach (var child in Children.OfType<VMSubTool>().Where(x => x.Active))
            {
                child.StartPointDrag(viewport, e, startLocation);
            }
        }

        public void PointDrag(MapViewport viewport, ViewportEvent viewportEvent, Coordinate lastPosition, Coordinate position)
        {
            foreach (var child in Children.OfType<VMSubTool>().Where(x => x.Active))
            {
                child.PointDrag(viewport, viewportEvent, lastPosition, position);
            }
        }

        public void EndPointDrag(MapViewport viewport, ViewportEvent e, Coordinate endLocation)
        {
            foreach (var child in Children.OfType<VMSubTool>().Where(x => x.Active))
            {
                child.EndPointDrag(viewport, e, endLocation);
            }
        }

        #endregion

        #region Errors

        public IEnumerable<VMError> GetErrors()
        {
            return Solids.SelectMany(x => x.GetErrors());
        }

        #endregion

        protected override IEnumerable<SceneObject> GetSceneObjects()
        {
            // todo !async using .Result
            var objs = Solids.SelectMany(x => MapObjectConverter.Convert(Document, x.Copy).Result).ToList();
            foreach (var so in objs.OfType<RenderableObject>())
            {
                so.ForcedRenderFlags |= RenderFlags.Wireframe;
                //so.IsSelected = true;
                so.TintColor = Sledge.Common.Colour.Blend(Color.FromArgb(128, Color.Green), so.TintColor);
                so.AccentColor = Color.White;
            }
            objs.AddRange(base.GetSceneObjects());
            return objs;
        }

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
