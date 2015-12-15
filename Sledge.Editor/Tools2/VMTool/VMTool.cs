using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.History;
using Sledge.Editor.Properties;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools;
using Sledge.Editor.Tools2.DraggableTool;
using Sledge.Editor.Tools2.VMTool.Actions;
using Sledge.Editor.UI;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Settings;
using View = Sledge.Settings.View;

namespace Sledge.Editor.Tools2.VMTool
{
    public class VMTool : BaseDraggableTool
    {
    //    private readonly VMSidebarPanel _controlPanel;
    //    private readonly VMErrorsSidebarPanel _errorPanel;

        private readonly VMPointsDraggableState _pointState;
        private readonly BoxDraggableState _boxState;

        private ShowPoints _showPoints;

        public VMTool()
        {
            //_controlPanel = new VMSidebarPanel();
            //_errorPanel = new VMErrorsSidebarPanel();

            Usage = ToolUsage.Both;

            _pointState = new VMPointsDraggableState(this);

            _boxState = new BoxDraggableState(this);
            _boxState.BoxColour = Color.Orange;
            _boxState.FillColour = Color.FromArgb(View.SelectionBoxBackgroundOpacity, Color.DodgerBlue);

            States.Add(_pointState);
            States.Add(_boxState);

            UseValidation = true;
        }

        public override Image GetIcon()
        {
            return Resources.Tool_VM;
        }

        public override string GetName()
        {
            return "VMTool";
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
            //yield return new KeyValuePair<string, Control>(GetName(), _controlPanel);
            //yield return new KeyValuePair<string, Control>("VM Errors", _errorPanel);
            yield break;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.HistoryUndo:
                    if (Document.History.CanUndo()) Document.History.Undo();
                    else MessageBox.Show("Nothing to undo in the VM tool"); // todo pop the stack and so on?
                    return HotkeyInterceptResult.Abort;
                case HotkeysMediator.HistoryRedo:
                    if (Document.History.CanRedo()) Document.History.Redo();
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
                case HotkeysMediator.VMFaceEditMode:
                case HotkeysMediator.VMScalingMode:
                case HotkeysMediator.VMSplitFace:
                case HotkeysMediator.VMStandardMode:
                    return HotkeyInterceptResult.Continue;
            }
            return HotkeyInterceptResult.Abort; // Don't allow stuff to happen when inside the VM tool. todo: fix/make this more generic?
        }

        public override void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Confirm(viewport);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                // todo: escape (or any other existing hotkey) doesn't get sent to tools because they are consumed by the form
                Cancel(viewport);
                e.Handled = true;
            }
            base.KeyDown(viewport, e);
        }

        private void Confirm(MapViewport viewport)
        {
            if (_boxState.State.Action != BoxAction.Drawn) return;
            var bbox = _boxState.State.GetSelectionBox();
            if (bbox != null && !bbox.IsEmpty())
            {
                _pointState.SelectPointsInBox(bbox, KeyboardState.Ctrl);
                _boxState.RememberedDimensions = bbox;
            }
            _boxState.State.Action = BoxAction.Idle;

            Invalidate();
        }

        private void Cancel(MapViewport viewport)
        {
            _boxState.RememberedDimensions = new Box(_boxState.State.Start, _boxState.State.End);
            _boxState.State.Action = BoxAction.Idle;

            Invalidate();
        }

        /// <summary>
        /// When the mouse is pressed in the 3D view, we want to select the clicked object.
        /// </summary>
        /// <param name="viewport">The viewport that was clicked</param>
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
            return _pointState.SelectPointsIn3D(viewport, camera, e, KeyboardState.Ctrl);
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
                    Document.PerformAction("Deselect VM solid", new ChangeSelection(select, deselect));
                }
                else if (!solid.IsSelected)
                {
                    // select solid
                    var select = new[] { solid };
                    var deselect = !KeyboardState.Ctrl ? Document.Selection.GetSelectedObjects() : new MapObject[0];
                    Document.PerformAction("Select VM solid", new ChangeSelection(select, deselect));
                }

                return true;
            }

            return false;
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
            Document.History.PopStack();
            Document.PerformAction(name, action);
            Document.History.PushStack("VM Tool");
        }

        public new void Invalidate()
        {
            base.Invalidate();
        }

        private void CycleShowPoints()
        {
            var side = (int)_showPoints;
            side = (side + 1) % (Enum.GetValues(typeof(ShowPoints)).Length);
            _showPoints = (ShowPoints)side;
        }

        private void SelectionChanged()
        {
            _pointState.SelectionChanged();
        }

        public override void ToolSelected(bool preventHistory)
        {
            Document.History.PushStack("VM Tool");

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

            _pointState.Commit();
            _pointState.Clear();

            Document.History.PopStack(); // todo push history collection

            base.ToolDeselected(preventHistory);
        }
    }
}
