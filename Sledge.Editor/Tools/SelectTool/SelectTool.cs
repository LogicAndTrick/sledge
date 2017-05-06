using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Operations.EditOperations;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.Clipboard;
using Sledge.Editor.Properties;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Editor.Tools.SelectTool.TransformationHandles;
using Sledge.Editor.Tools.Widgets;
using Sledge.Rendering.Cameras;
using Sledge.Settings;
using KeyboardState = Sledge.Editor.UI.KeyboardState;
using View = Sledge.Settings.View;

namespace Sledge.Editor.Tools.SelectTool
{
    /// <summary>
    /// The select tool is used to select objects in several different ways:
    /// 1. Single click in the 2D view will perform edge-detection selection
    /// 2. Single click in the 3D view allows ray-casting selection (with mouse wheel cycling)
    /// 3. Drawing a box in the 2D view and confirming it will select everything in the box
    /// </summary>
    public class SelectTool : BaseDraggableTool
    {
        private readonly BoxDraggableState _emptyBox;
        private readonly SelectionBoxDraggableState _selectionBox;

        private MapObject ChosenItemFor3DSelection { get; set; }
        private List<MapObject> IntersectingObjectsFor3DSelection { get; set; }

        private readonly SelectToolSidebarPanel _sidebarPanel;

        public SelectTool()
        {
            _selectionBox = new SelectionBoxDraggableState(this);
            _selectionBox.BoxColour = Color.Yellow;
            _selectionBox.FillColour = Color.FromArgb(View.SelectionBoxBackgroundOpacity, Color.White);
            _selectionBox.Stippled = Sledge.Settings.View.DrawBoxDashedLines;
            _selectionBox.State.Changed += SelectionBoxChanged;
            States.Add(_selectionBox);
            Children.AddRange(_selectionBox.Widgets);

            _emptyBox = new BoxDraggableState(this);
            _emptyBox.BoxColour = Color.Yellow;
            _emptyBox.FillColour = Color.FromArgb(View.SelectionBoxBackgroundOpacity, Color.White);
            _emptyBox.Stippled = Sledge.Settings.View.DrawBoxDashedLines;
            _emptyBox.State.Changed += EmptyBoxChanged;
            _emptyBox.DragEnded += (sender, args) =>
            {
                if (Sledge.Settings.Select.AutoSelectBox) Confirm();
            };
            States.Add(_emptyBox);

            Usage = ToolUsage.Both;

            _sidebarPanel = new SelectToolSidebarPanel();
            _sidebarPanel.ChangeTransformationMode += (sender, mode) =>
            {
                _selectionBox.SetTransformationMode(mode);
            };
            _sidebarPanel.ToggleShow3DWidgets += (sender, show) =>
            {
                Sledge.Settings.Select.Show3DSelectionWidgets = show;
                _selectionBox.Update();
            };
        }

        public void TransformationModeChanged(SelectionBoxDraggableState.TransformationMode mode)
        {
            _sidebarPanel.TransformationToolChanged(mode);
        }

        public override string GetContextualHelp()
        {
            return "*Click* to select an object.\n" +
                   "In the 3D view, *click and hold* and use the *mouse wheel* to cycle through objects behind the cursor.\n" +
                   "In the 2D view, *click the selection box* to cycle between manipulation modes.";
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Select;
        }

        public override string GetName()
        {
            return "SelectTool";
        }

        public override IEnumerable<KeyValuePair<string, Control>> GetSidebarControls()
        {
            yield return new KeyValuePair<string, Control>(GetName(), _sidebarPanel);
        }

        public override void ToolSelected(bool preventHistory)
        {
            IgnoreGroupingChanged();

            Mediator.Subscribe(EditorMediator.SelectionChanged, this);
            Mediator.Subscribe(EditorMediator.DocumentTreeStructureChanged, this);
            Mediator.Subscribe(EditorMediator.DocumentTreeObjectsChanged, this);
            Mediator.Subscribe(EditorMediator.IgnoreGroupingChanged, this);

            SelectionChanged();

            base.ToolSelected(preventHistory);
        }

        #region Selection/document changed
        private void DocumentTreeStructureChanged()
        {
            SelectionChanged();
        }

        private void DocumentTreeObjectsChanged(IEnumerable<MapObject> objects)
        {
            if (objects.Any(x => x.IsSelected))
            {
                SelectionChanged();
            }
        }

        private void SelectionChanged()
        {
            if (Document == null) return;
            UpdateBoxBasedOnSelection();
            foreach (var widget in Children.OfType<Widget>())
            {
                widget.SelectionChanged();
            }
        }

        /// <summary>
        /// Updates the box based on the currently selected objects.
        /// </summary>
        private void UpdateBoxBasedOnSelection()
        {
            if (Document.Selection.IsEmpty())
            {
                _emptyBox.State.Start = _emptyBox.State.End = null;
                if (_emptyBox.State.Action == BoxAction.Drawn) _emptyBox.State.Action = BoxAction.Idle;
                _selectionBox.State.Start = _selectionBox.State.End = null;
                _selectionBox.State.Action = BoxAction.Idle;
            }
            else
            {
                _emptyBox.State.Start = _emptyBox.State.End = null;
                _emptyBox.State.Action = BoxAction.Idle;

                var box = Document.Selection.GetSelectionBoundingBox();
                _selectionBox.State.Start = box.Start;
                _selectionBox.State.End = box.End;
                _selectionBox.State.Action = BoxAction.Drawn;
            }
            _selectionBox.Update();
        }
        #endregion

        #region Ignore Grouping
        private bool IgnoreGrouping()
        {
            return Document.Map.IgnoreGrouping;
        }

        private void IgnoreGroupingChanged()
        {
            var selected = Document.Selection.GetSelectedObjects().ToList();
            var select = new List<MapObject>();
            var deselect = new List<MapObject>();
            if (Document.Map.IgnoreGrouping)
            {
                deselect.AddRange(selected.Where(x => x.HasChildren));
            }
            else
            {
                var parents = selected.Select(x => x.FindTopmostParent(y => y is Group || y is Entity) ?? x).Distinct();
                foreach (var p in parents)
                {
                    var children = p.FindAll();
                    var leaves = children.Where(x => !x.HasChildren);
                    if (leaves.All(selected.Contains)) select.AddRange(children.Where(x => !selected.Contains(x)));
                    else deselect.AddRange(children.Where(selected.Contains));
                }
            }
            if (deselect.Any() || select.Any())
            {
                Document.PerformAction("Apply group selection", new ChangeSelection(select, deselect));
            }
        }
        #endregion

        #region Perform selection

        /// <summary>
        /// If ignoreGrouping is disabled, this will convert the list of objects into their topmost group or entity.
        /// If ignoreGrouping is enabled, this will remove objects that have children from the list.
        /// </summary>
        /// <param name="objects">The object list to normalise</param>
        /// <param name="ignoreGrouping">True if grouping is being ignored</param>
        /// <returns>The normalised list of objects</returns>
        private static IEnumerable<MapObject> NormaliseSelection(IEnumerable<MapObject> objects, bool ignoreGrouping)
        {
            return ignoreGrouping
                       ? objects.Where(x => !x.HasChildren)
                       : objects.Select(x => x.FindTopmostParent(y => y is Group || y is Entity) ?? x).Distinct().SelectMany(x => x.FindAll());
        }

        /// <summary>
        /// Deselect (first) a list of objects and then select (second) another list.
        /// </summary>
        /// <param name="objectsToDeselect">The objects to deselect</param>
        /// <param name="objectsToSelect">The objects to select</param>
        /// <param name="deselectAll">If true, this will ignore the objectToDeselect parameter and just deselect everything</param>
        /// <param name="ignoreGrouping">If true, object groups will be ignored</param>
        private void SetSelected(IEnumerable<MapObject> objectsToDeselect, IEnumerable<MapObject> objectsToSelect, bool deselectAll, bool ignoreGrouping)
        {
            if (objectsToDeselect == null) objectsToDeselect = new MapObject[0];
            if (objectsToSelect == null) objectsToSelect = new MapObject[0];

            if (deselectAll)
            {
                objectsToDeselect = Document.Selection.GetSelectedObjects();
                // _lastTool = null;
            }

            // Normalise selections
            objectsToDeselect = NormaliseSelection(objectsToDeselect.Where(x => x != null), ignoreGrouping);
            objectsToSelect = NormaliseSelection(objectsToSelect.Where(x => x != null), ignoreGrouping);

            // Don't bother deselecting the objects we're about to select
            objectsToDeselect = objectsToDeselect.Where(x => !objectsToSelect.Contains(x));

            // Perform selections
            var deselected = objectsToDeselect.ToList();
            var selected = objectsToSelect.ToList();

            Document.PerformAction("Selection changed", new ChangeSelection(selected, deselected));
        }

        #endregion

        #region 3D interaction

        protected override void MouseDoubleClick(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            // Don't show Object Properties while navigating the view, because mouse cursor will be hidden
            if (KeyboardState.IsKeyDown(Keys.Space)) return;

            if (Sledge.Settings.Select.DoubleClick3DAction == DoubleClick3DAction.Nothing) return;
            if (!Document.Selection.IsEmpty())
            {
                if (Sledge.Settings.Select.DoubleClick3DAction == DoubleClick3DAction.ObjectProperties)
                {
                    Mediator.Publish(HotkeysMediator.ObjectProperties);
                }
                else if (Sledge.Settings.Select.DoubleClick3DAction == DoubleClick3DAction.TextureTool)
                {
                    Mediator.Publish(HotkeysMediator.SwitchTool, HotkeyTool.Texture);
                }
            }
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

        /// <summary>
        /// When the mouse is pressed in the 3D view, we want to select the clicked object.
        /// </summary>
        /// <param name="viewport">The viewport that was clicked</param>
        /// <param name="e">The click event</param>
        protected override void MouseDown(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            // Do not perform selection if space is down
            if (View.Camera3DPanRequiresMouseClick && KeyboardState.IsKeyDown(Keys.Space)) return;

            // First, get the ray that is cast from the clicked point along the viewport frustrum
            var ray = viewport.CastRayFromScreen(e.X, e.Y);

            // Grab all the elements that intersect with the ray
            var hits = Document.Map.WorldSpawn.GetAllNodesIntersectingWith(ray);

            // Sort the list of intersecting elements by distance from ray origin
            IntersectingObjectsFor3DSelection = hits
                .Select(x => new { Item = x, Intersection = GetIntersectionPoint(x, ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .Select(x => x.Item)
                .ToList();

            // By default, select the closest object
            ChosenItemFor3DSelection = IntersectingObjectsFor3DSelection.FirstOrDefault();

            // If Ctrl is down and the object is already selected, we should deselect it instead.
            var list = new[] { ChosenItemFor3DSelection };
            var desel = ChosenItemFor3DSelection != null && KeyboardState.Ctrl && ChosenItemFor3DSelection.IsSelected;
            SetSelected(desel ? list : null, desel ? null : list, !KeyboardState.Ctrl, IgnoreGrouping());
        }

        protected override void MouseUp(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            IntersectingObjectsFor3DSelection = null;
            ChosenItemFor3DSelection = null;
            viewport.ReleaseInputLock(this);
        }

        protected override void MouseWheel(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            // If we're not in 3D cycle mode, carry on
            if (IntersectingObjectsFor3DSelection == null || ChosenItemFor3DSelection == null)
            {
                return;
            }

            var desel = new List<MapObject>();
            var sel = new List<MapObject>();

            // Select (or deselect) the current element
            if (ChosenItemFor3DSelection.IsSelected) desel.Add(ChosenItemFor3DSelection);
            else sel.Add(ChosenItemFor3DSelection);

            // Get the index of the current element
            var index = IntersectingObjectsFor3DSelection.IndexOf(ChosenItemFor3DSelection);
            if (index < 0) return;

            // Move the index in the mouse wheel direction, cycling if needed
            var dir = e.Delta / Math.Abs(e.Delta);
            index = (index + dir) % IntersectingObjectsFor3DSelection.Count;
            if (index < 0) index += IntersectingObjectsFor3DSelection.Count;

            ChosenItemFor3DSelection = IntersectingObjectsFor3DSelection[index];

            // Select (or deselect) the new current element
            if (ChosenItemFor3DSelection.IsSelected) desel.Add(ChosenItemFor3DSelection);
            else sel.Add(ChosenItemFor3DSelection);

            SetSelected(desel, sel, false, IgnoreGrouping());
        }
        
        /// <summary>
        /// The select tool captures the mouse wheel when the mouse is down in the 3D viewport
        /// </summary>
        /// <returns>True if the select tool is capturing wheel events</returns>
        public override bool IsCapturingMouseWheel()
        {
            return IntersectingObjectsFor3DSelection != null
                   && IntersectingObjectsFor3DSelection.Any()
                   && ChosenItemFor3DSelection != null;
        }

        #endregion

        #region 2D interaction

        protected override void OnDraggableClicked(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position, IDraggable draggable)
        {
            var ctrl = KeyboardState.Ctrl;
            if (draggable == _emptyBox || ctrl)
            {
                var desel = new List<MapObject>();
                var sel = new List<MapObject>();
                var seltest = SelectionTest(viewport, camera, e);
                if (seltest != null)
                {
                    if (!ctrl || !seltest.IsSelected) sel.Add(seltest);
                    else desel.Add(seltest);
                }
                SetSelected(desel, sel, !ctrl, IgnoreGrouping());
            }
            else if (_selectionBox.State.Action == BoxAction.Drawn && draggable is ResizeTransformHandle && ((ResizeTransformHandle) draggable).Handle == ResizeHandle.Center)
            {
                _selectionBox.Cycle();
            }
            e.Handled = !ctrl || draggable == _emptyBox;
        }

        protected override void OnDraggableDragStarted(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position, IDraggable draggable)
        {
            var ctrl = KeyboardState.Ctrl;
            if (draggable == _emptyBox && !ctrl && !Document.Selection.IsEmpty())
            {
                SetSelected(null, null, true, IgnoreGrouping());
            }
        }

        protected override void OnDraggableDragMoved(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate previousPosition, Coordinate position, IDraggable draggable)
        {
            base.OnDraggableDragMoved(viewport, camera, e, previousPosition, position, draggable);
            if (_selectionBox.State.Action == BoxAction.Resizing && draggable is ITransformationHandle)
            {
                var tform = _selectionBox.GetTransformationMatrix(viewport, camera, Document);
                if (tform.HasValue)
                {
                    Document.SetSelectListTransform(tform.Value);
                    var box = new Box(_selectionBox.State.OrigStart, _selectionBox.State.OrigEnd);
                    var trans = CreateMatrixMultTransformation(tform.Value);
                    Mediator.Publish(EditorMediator.SelectionBoxChanged, box);//.Transform(trans));
                }
            }
        }

        protected override void OnDraggableDragEnded(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position, IDraggable draggable)
        {
            var tt = draggable as ITransformationHandle;
            if (_selectionBox.State.Action == BoxAction.Resizing && tt != null)
            {
                // Execute the transform on the selection
                var tform = _selectionBox.GetTransformationMatrix(viewport, camera, Document);
                if (tform.HasValue)
                {
                    var createClone = KeyboardState.Shift && draggable is ResizeTransformHandle && ((ResizeTransformHandle)draggable).Handle == ResizeHandle.Center;
                    ExecuteTransform(tt.Name, CreateMatrixMultTransformation(tform.Value), createClone);
                }
            }
            Document.EndSelectionTransform();
            base.OnDraggableDragEnded(viewport, camera, e, position, draggable);
        }

        private void EmptyBoxChanged(object sender, EventArgs e)
        {
            if (_emptyBox.State.Action != BoxAction.Idle && _selectionBox.State.Action != BoxAction.Idle)
            {
                _selectionBox.State.Action = BoxAction.Idle;
                // We're drawing a selection box, so clear the current tool
                // SetCurrentTool(null);
            }
            if (_emptyBox.State.Action == BoxAction.Drawn && Sledge.Settings.Select.AutoSelectBox)
            {
                // BoxDrawnConfirm(emptyBox.State.Viewport);
            }
        }

        private void SelectionBoxChanged(object sender, EventArgs e)
        {
            
        }

        private MapObject SelectionTest(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            // Create a box to represent the click, with a tolerance level
            var unused = viewport.GetUnusedCoordinate(new Coordinate(100000, 100000, 100000));
            var tolerance = 4 / (decimal) viewport.Zoom; // Selection tolerance of four pixels
            var used = viewport.Expand(new Coordinate(tolerance, tolerance, 0));
            var add = used + unused;
            var click = viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y));
            var box = new Box(click - add, click + add);

            var centerHandles = Sledge.Settings.Select.DrawCenterHandles;
            var centerOnly = Sledge.Settings.Select.ClickSelectByCenterHandlesOnly;
            // Get the first element that intersects with the box, selecting or deselecting as needed
            return Document.Map.WorldSpawn.GetAllNodesIntersecting2DLineTest(box, centerHandles, centerOnly).FirstOrDefault();
        }

        protected override void KeyDown(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            var nudge = GetNudgeValue(e.KeyCode);
            if (nudge != null && (_selectionBox.State.Action == BoxAction.Drawn) && !Document.Selection.IsEmpty())
            {
                var translate = viewport.Expand(nudge);
                var transformation = Matrix4.CreateTranslation((float)translate.X, (float)translate.Y, (float)translate.Z);
                ExecuteTransform("Nudge", CreateMatrixMultTransformation(transformation), KeyboardState.Shift);
                SelectionChanged();
            }
        }

        #endregion

        #region Box confirm/cancel

        public override void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Confirm();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Cancel();
            }
            base.KeyDown(viewport, e);
        }

        /// <summary>
        /// Once a box is confirmed, we select all element intersecting with the box (contained within if shift is down).
        /// </summary>
        private void Confirm()
        {
            // Only confirm the box if the empty box is drawn
            if (_selectionBox.State.Action != BoxAction.Idle || _emptyBox.State.Action != BoxAction.Drawn) return;

            var boundingbox = _emptyBox.State.GetSelectionBox();
            if (boundingbox != null)
            {
                // If the shift key is down, select all brushes that are fully contained by the box
                // If select by handles only is on, select all brushes with centers inside the box
                // Otherwise, select all brushes that intersect with the box
                Func<Box, IEnumerable<MapObject>> selector = x => Document.Map.WorldSpawn.GetAllNodesIntersectingWith(x);
                if (Sledge.Settings.Select.BoxSelectByCenterHandlesOnly) selector = x => Document.Map.WorldSpawn.GetAllNodesWithCentersContainedWithin(x);
                if (KeyboardState.Shift) selector = x => Document.Map.WorldSpawn.GetAllNodesContainedWithin(x);

                var nodes = selector(boundingbox).ToList();
                SetSelected(null, nodes, false, IgnoreGrouping());
            }

            SelectionChanged();
        }

        private void Cancel()
        {
            if (_selectionBox.State.Action != BoxAction.Idle && !Document.Selection.IsEmpty())
            {
                SetSelected(null, null, true, IgnoreGrouping());
            }
            _selectionBox.State.Action = _emptyBox.State.Action = BoxAction.Idle;
            SelectionChanged();
        }

        #endregion

        #region Transform stuff

        /// <summary>
        /// Runs the transform on all the currently selected objects
        /// </summary>
        /// <param name="transformationName">The name of the transformation</param>
        /// <param name="transform">The transformation to apply</param>
        /// <param name="clone">True to create a clone before transforming the original.</param>
        private void ExecuteTransform(string transformationName, IUnitTransformation transform, bool clone)
        {
            if (clone) transformationName += "-clone";
            var objects = Document.Selection.GetSelectedParents().ToList();
            var name = String.Format("{0} {1} object{2}", transformationName, objects.Count, (objects.Count == 1 ? "" : "s"));

            var cad = new CreateEditDelete();
            var action = new ActionCollection(cad);

            if (clone)
            {
                // Copy the selection, transform it, and reselect
                var copies = ClipboardManager.CloneFlatHeirarchy(Document, Document.Selection.GetSelectedObjects()).ToList();
                foreach (var mo in copies)
                {
                    mo.Transform(transform, Document.Map.GetTransformFlags());
                    if (Sledge.Settings.Select.KeepVisgroupsWhenCloning) continue;
                    foreach (var o in mo.FindAll()) o.Visgroups.Clear();
                }
                cad.Create(Document.Map.WorldSpawn.ID, copies);
                var sel = new ChangeSelection(copies.SelectMany(x => x.FindAll()), Document.Selection.GetSelectedObjects());
                action.Add(sel);
            }
            else
            {
                // Transform the selection
                cad.Edit(objects, new TransformEditOperation(transform, Document.Map.GetTransformFlags()));
            }

            // Execute the action
            Document.PerformAction(name, action);
        }

        private IUnitTransformation CreateMatrixMultTransformation(Matrix4 mat)
        {
            return new UnitMatrixMult(mat);
        }

        #endregion

        public override void ToolDeselected(bool preventHistory)
        {
            Mediator.UnsubscribeAll(this);
            base.ToolDeselected(preventHistory);
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Selection;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            return HotkeyInterceptResult.Continue;
        }
    }
}