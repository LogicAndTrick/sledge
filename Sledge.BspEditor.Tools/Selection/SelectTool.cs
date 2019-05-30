using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Mutation;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Properties;
using Sledge.BspEditor.Tools.Selection.TransformationHandles;
using Sledge.BspEditor.Tools.Widgets;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Tools.Selection
{
    /// <summary>
    /// The select tool is used to select objects in several different ways:
    /// 1. Single click in the 2D view will perform edge-detection selection
    /// 2. Single click in the 3D view allows ray-casting selection (with mouse wheel cycling)
    /// 3. Drawing a box in the 2D view and confirming it will select everything in the box
    /// </summary>
    [Export(typeof(ITool))]
    [Export(typeof(ISettingsContainer))]
    [OrderHint("A")]
    [DefaultHotkey("Shift+S")]
    [AutoTranslate]
    public class SelectTool : BaseDraggableTool, ISettingsContainer
    {
        private readonly BoxDraggableState _emptyBox;
        private readonly SelectionBoxDraggableState _selectionBox;

        private IMapObject ChosenItemFor3DSelection { get; set; }
        private List<IMapObject> IntersectingObjectsFor3DSelection { get; set; }

        // Settings

        [Setting] public int SelectionBoxBackgroundOpacity { get;set; } = 64;
        [Setting] public bool AutoSelectBox { get; set; } = false;
        [Setting] public bool Show3DWidgets { get; set; } = false;
        [Setting] public bool SelectByCenterHandles { get; set; } = true;
        [Setting] public bool OnlySelectByCenterHandles { get; set; } = false;
        [Setting] public bool SelectionBoxOnlySelectsByCenterHandles { get; set; } = false;
        [Setting] public bool SelectionClickCycles { get; set; } = true;
        [Setting] public bool KeepVisgroupsWhenCloning { get; set; } = true;

        string ISettingsContainer.Name => "Sledge.BspEditor.Tools.SelectTool";

        IEnumerable<SettingKey> ISettingsContainer.GetKeys()
        {
            yield return new SettingKey("Tools/Selection", "SelectionBoxBackgroundOpacity", typeof(int));
            yield return new SettingKey("Tools/Selection", "AutoSelectBox", typeof(bool));
            yield return new SettingKey("Tools/Selection", "Show3DWidgets", typeof(bool));
            yield return new SettingKey("Tools/Selection", "SelectByCenterHandles", typeof(bool));
            yield return new SettingKey("Tools/Selection", "OnlySelectByCenterHandles", typeof(bool));
            yield return new SettingKey("Tools/Selection", "SelectionBoxOnlySelectsByCenterHandles", typeof(bool));
            yield return new SettingKey("Tools/Selection", "SelectionClickCycles", typeof(bool));
            yield return new SettingKey("Tools/Selection", "KeepVisgroupsWhenCloning", typeof(bool));
        }

        void ISettingsContainer.LoadValues(ISettingsStore store)
        {
            store.LoadInstance(this);
            Oy.Publish("SelectTool:SetShow3DWidgets", Show3DWidgets ? "1" : "0");
        }

        void ISettingsContainer.StoreValues(ISettingsStore store)
        {
            store.StoreInstance(this);
        }

        public string Align { get; set; }
        public string Flip { get; set; }
        public string Rotate { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
        public string Top { get; set; }
        public string Bottom { get; set; }
        public string Vertically { get; set; }
        public string Horizontally { get; set; }
        public string Clockwise { get; set; }
        public string AntiClockwise { get; set; }

        public SelectTool()
        {
            _selectionBox = new SelectionBoxDraggableState(this);
            _selectionBox.BoxColour = Color.Yellow;
            _selectionBox.FillColour = Color.FromArgb(SelectionBoxBackgroundOpacity, Color.White);
            _selectionBox.State.Changed += SelectionBoxChanged;
            States.Add(_selectionBox);
            Children.AddRange(_selectionBox.Widgets);

            _emptyBox = new BoxDraggableState(this);
            _emptyBox.BoxColour = Color.Yellow;
            _emptyBox.FillColour = Color.FromArgb(SelectionBoxBackgroundOpacity, Color.White);
            _emptyBox.State.Changed += EmptyBoxChanged;
            _emptyBox.DragEnded += (sender, args) =>
            {
                var document = GetDocument();
                if (document != null && AutoSelectBox) Confirm(document);
            };
            States.Add(_emptyBox);

            Usage = ToolUsage.Both;

            Oy.Subscribe<String>("SelectTool:TransformationModeChanged", x =>
            {
                if (Enum.TryParse(x, out SelectionBoxDraggableState.TransformationMode mode))
                {
                    if (mode != _selectionBox.CurrentTransformationMode)
                        _selectionBox.SetTransformationMode(mode);
                }
            });
            Oy.Subscribe<string>("SelectTool:Show3DWidgetsChanged", x =>
            {
                Show3DWidgets = x == "1";
                _selectionBox.ShowWidgets = Show3DWidgets;
                _selectionBox.Update();
            });
        }

        public void TransformationModeChanged(SelectionBoxDraggableState.TransformationMode mode)
        {
            Oy.Publish("SelectTool:TransformationModeChanged", mode.ToString());
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Select;
        }

        public override string GetName()
        {
            return "SelectTool";
        }

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<IDocument>("MapDocument:SelectionChanged", x =>
            {
                var document = GetDocument();
                if (x == document) SelectionChanged(document);
            });
            yield return Oy.Subscribe<Change>("MapDocument:Changed", x =>
            {
                var document = GetDocument();
                if (x.Document == document)
                {
                    if (x.HasObjectChanges) UpdateBoxBasedOnSelection(document);
                    if (x.HasDataChanges && x.AffectedData.Any(d => d is SelectionOptions)) IgnoreGroupingPossiblyChanged(document);
                }
            });
            yield return Oy.Subscribe<RightClickMenuBuilder>("MapViewport:RightClick", b =>
            {
                if (!(b.Viewport.Viewport.Camera is OrthographicCamera camera)) return;
                var document = GetDocument();
                if (document == null) return;

                var selectionBoundingBox = document.Selection.GetSelectionBoundingBox();
                var point = camera.Flatten(camera.ScreenToWorld(b.Event.X, b.Event.Y));
                var start = camera.Flatten(selectionBoundingBox.Start);
                var end = camera.Flatten(selectionBoundingBox.End);

                if (point.X < start.X || point.X > end.X || point.Y < start.Y || point.Y > end.Y) return;

                // Clicked inside the selection bounds
                b.Clear();

                b.AddCommand("BspEditor:Edit:Cut");
                b.AddCommand("BspEditor:Edit:Copy");
                b.AddCommand("BspEditor:Edit:Delete");
                b.AddCommand("BspEditor:Edit:Paste");
                b.AddCommand("BspEditor:Edit:PasteSpecial");
                b.AddSeparator();

                b.AddCommand("BspEditor:Tools:Transform");
                b.AddSeparator();

                b.AddCommand("BspEditor:Edit:Undo");
                b.AddCommand("BspEditor:Edit:Redo");
                b.AddSeparator();

                b.AddCommand("BspEditor:Tools:Carve");
                b.AddCommand("BspEditor:Tools:Hollow");
                b.AddSeparator();

                b.AddCommand("BspEditor:Edit:Group");
                b.AddCommand("BspEditor:Edit:Ungroup");
                b.AddSeparator();

                b.AddCommand("BspEditor:Tools:TieToEntity");
                b.AddCommand("BspEditor:Tools:MoveToWorld");
                b.AddSeparator();

                if (b.Viewport.Is2D)
                {
                    var f = camera.Flatten(new Vector3(1, 2, 3));
                    var e = camera.Expand(f);
                    var flat = new {X = (int) f.X, Y = (int) f.Y, Z = (int) f.Z};
                    var expand = new {X = (int) e.X, Y = (int) e.Y, Z = (int) e.Z};

                    var left = flat.X == 1 ? "AlignXMin" : (flat.X == 2 ? "AlignYMin" : "AlignZMin");
                    var right = flat.X == 1 ? "AlignXMax" : (flat.X == 2 ? "AlignYMax" : "AlignZMax");
                    var bottom = flat.Y == 1 ? "AlignXMin" : (flat.Y == 2 ? "AlignYMin" : "AlignZMin");
                    var top = flat.Y == 1 ? "AlignXMax" : (flat.Y == 2 ? "AlignYMax" : "AlignZMax");

                    var group = b.AddGroup(Align);

                    var l = b.CreateCommandItem($"BspEditor:Tools:{left}");
                    l.Text = Left;
                    group.DropDownItems.Add(l);

                    var r = b.CreateCommandItem($"BspEditor:Tools:{right}");
                    r.Text = Right;
                    group.DropDownItems.Add(r);

                    var u = b.CreateCommandItem($"BspEditor:Tools:{top}");
                    u.Text = Top;
                    group.DropDownItems.Add(u);

                    var d = b.CreateCommandItem($"BspEditor:Tools:{bottom}");
                    d.Text = Bottom;
                    group.DropDownItems.Add(d);

                    var horizontal = flat.X == 1 ? "FlipX" : (flat.X == 2 ? "FlipY" : "FlipZ");
                    var vertical = flat.Y == 2 ? "FlipY" : (flat.Y == 3 ? "FlipZ" : "FlipX");

                    group = b.AddGroup(Flip);

                    var h = b.CreateCommandItem($"BspEditor:Tools:{horizontal}");
                    h.Text = Horizontally;
                    group.DropDownItems.Add(h);

                    var v = b.CreateCommandItem($"BspEditor:Tools:{vertical}");
                    v.Text = Vertically;
                    group.DropDownItems.Add(v);

                    group = b.AddGroup(Rotate);

                    var axis = expand.X == 0 ? Vector3.UnitX : (expand.Y == 0 ? -Vector3.UnitY : Vector3.UnitZ);

                    var cw = b.CreateCommandItem("BspEditor:Tools:Rotate", new { Axis = axis, Angle = -90f });
                    cw.Text = Clockwise;
                    group.DropDownItems.Add(cw);

                    var acw = b.CreateCommandItem("BspEditor:Tools:Rotate", new { Axis = axis, Angle = 90f });
                    acw.Text = AntiClockwise;
                    group.DropDownItems.Add(acw);
                }

                b.AddCommand("BspEditor:Map:Properties");
            });
        }

        private bool _lastIgnoreGroupingValue;
        private void IgnoreGroupingPossiblyChanged(MapDocument document)
        {
            var igVal = IgnoreGrouping();
            if (igVal == _lastIgnoreGroupingValue) return;
            IgnoreGroupingChanged(document);
        }

        public override async Task ToolSelected()
        {
            TransformationModeChanged(SelectionBoxDraggableState.TransformationMode.Resize);

            var document = GetDocument();
            if (document != null)
            {
                IgnoreGroupingChanged(document);
                SelectionChanged(document);
            }

            await base.ToolSelected();
        }

        #region Selection changed
        private void SelectionChanged(MapDocument document)
        {
            if (document == null) return;

            foreach (var widget in Children.OfType<Widget>()) widget.SelectionChanged();
            UpdateBoxBasedOnSelection(document);

            if (!document.Selection.IsEmpty)
            {
                var box = document.Selection.GetSelectionBoundingBox();
                _selectionBox.SetRotationOrigin(box.Center);
            }
            else
                TransformationModeChanged(SelectionBoxDraggableState.TransformationMode.Resize);
        }

        /// <summary>
        /// Updates the box based on the currently selected objects.
        /// </summary>
        private void UpdateBoxBasedOnSelection(MapDocument document)
        {
            if (document.Selection.IsEmpty)
            {
                _selectionBox.State.Action = BoxAction.Idle;
                if (_emptyBox.State.Action == BoxAction.Drawn) _emptyBox.State.Action = BoxAction.Idle;
            }
            else
            {
                _emptyBox.State.Action = BoxAction.Idle;

                var box = document.Selection.GetSelectionBoundingBox();
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
            return GetDocument()?.Map.Data.GetOne<SelectionOptions>()?.IgnoreGrouping == true;
        }

        private void IgnoreGroupingChanged(MapDocument document)
        {
            var igVal = _lastIgnoreGroupingValue = IgnoreGrouping();

            var selected = document.Selection.ToList();
            var select = new List<IMapObject>();
            var deselect = new List<IMapObject>();

            if (igVal)
            {
                deselect.AddRange(selected.Where(x => x.Hierarchy.HasChildren));
            }
            else
            {
                var parents = selected.Select(x => x.FindTopmostParent(y => y is Group || y is Primitives.MapObjects.Entity) ?? x).Distinct();
                foreach (var p in parents)
                {
                    var children = p.FindAll();
                    var leaves = children.Where(x => !x.Hierarchy.HasChildren);
                    if (leaves.All(selected.Contains)) select.AddRange(children.Where(x => !selected.Contains(x)));
                    else deselect.AddRange(children.Where(selected.Contains));
                }
            }

            if (select.Any() || deselect.Any())
            {
                var transaction = new Transaction(new Select(select), new Deselect(deselect));
                MapDocumentOperation.Perform(document, transaction);
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
        private static IEnumerable<IMapObject> NormaliseSelection(IEnumerable<IMapObject> objects, bool ignoreGrouping)
        {
            return ignoreGrouping
                       ? objects.Where(x => !x.Hierarchy.HasChildren)
                       : objects.Select(x => x.FindTopmostParent(y => y is Group || y is Primitives.MapObjects.Entity) ?? x).Distinct().SelectMany(x => x.FindAll());
        }

        /// <summary>
        /// Deselect (first) a list of objects and then select (second) another list.
        /// </summary>
        /// <param name="document">The current document</param>
        /// <param name="objectsToDeselect">The objects to deselect</param>
        /// <param name="objectsToSelect">The objects to select</param>
        /// <param name="deselectAll">If true, this will ignore the objectToDeselect parameter and just deselect everything</param>
        /// <param name="ignoreGrouping">If true, object groups will be ignored</param>
        private void SetSelected(MapDocument document, IEnumerable<IMapObject> objectsToDeselect, IEnumerable<IMapObject> objectsToSelect, bool deselectAll, bool ignoreGrouping)
        {
            if (objectsToDeselect == null) objectsToDeselect = new IMapObject[0];
            if (objectsToSelect == null) objectsToSelect = new IMapObject[0];

            if (deselectAll)
            {
                objectsToDeselect = document.Selection.ToList();
            }

            // Normalise selections
            objectsToDeselect = NormaliseSelection(objectsToDeselect.Where(x => x != null), ignoreGrouping).ToList();
            objectsToSelect = NormaliseSelection(objectsToSelect.Where(x => x != null), ignoreGrouping).ToList();

            // Don't bother deselecting the objects we're about to select
            objectsToDeselect = objectsToDeselect.Where(x => !objectsToSelect.Contains(x));

            // Perform selections
            var deselected = objectsToDeselect.ToList();
            var selected = objectsToSelect.ToList();

            var transaction = new Transaction(new Select(selected), new Deselect(deselected));
            MapDocumentOperation.Perform(document, transaction);
        }

        #endregion

        #region 3D interaction

        protected override void MouseDoubleClick(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (document.Selection.IsEmpty) return;
            Oy.Publish("Context:Add", new ContextInfo("BspEditor:ObjectProperties"));
        }

        /// <summary>
        /// When the mouse is pressed in the 3D view, we want to select the clicked object.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="viewport">The viewport that was clicked</param>
        /// <param name="camera"></param>
        /// <param name="e">The click event</param>
        protected override void MouseDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            // First, get the ray that is cast from the clicked point along the viewport frustrum
            var (rayStart, rayEnd) = camera.CastRayFromScreen(new Vector3(e.X, e.Y, 0));
            var ray = new Line(rayStart, rayEnd);

            // Grab all the elements that intersect with the ray
            IntersectingObjectsFor3DSelection = document.Map.Root.GetIntersectionsForVisibleObjects(ray)
                .Select(x => x.Object)
                .ToList();

            // By default, select the closest object
            ChosenItemFor3DSelection = IntersectingObjectsFor3DSelection.FirstOrDefault();

            // If Ctrl is down and the object is already selected, we should deselect it instead.
            var list = new[] { ChosenItemFor3DSelection };
            var desel = ChosenItemFor3DSelection != null && KeyboardState.Ctrl && ChosenItemFor3DSelection.IsSelected;
            SetSelected(document, desel ? list : null, desel ? null : list, !KeyboardState.Ctrl, IgnoreGrouping());
        }

        protected override void MouseUp(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            IntersectingObjectsFor3DSelection = null;
            ChosenItemFor3DSelection = null;
            viewport.ReleaseInputLock(this);
        }

        protected override void MouseWheel(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            // If we're not in 3D cycle mode, carry on
            if (IntersectingObjectsFor3DSelection == null || ChosenItemFor3DSelection == null)
            {
                return;
            }

            e.Handled = true;

            var desel = new List<IMapObject>();
            var sel = new List<IMapObject>();

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

            SetSelected(document, desel, sel, false, IgnoreGrouping());
        }
        
        // /// <summary>
        // /// The select tool captures the mouse wheel when the mouse is down in the 3D viewport
        // /// </summary>
        // /// <returns>True if the select tool is capturing wheel events</returns>
        // public override bool IsCapturingMouseWheel()
        // {
        //     return IntersectingObjectsFor3DSelection != null
        //            && IntersectingObjectsFor3DSelection.Any()
        //            && ChosenItemFor3DSelection != null;
        // }

        #endregion

        #region 2D interaction

        protected override void OnDraggableClicked(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position, IDraggable draggable)
        {
            var ctrl = KeyboardState.Ctrl;
            if (draggable == _emptyBox || ctrl)
            {
                var desel = new List<IMapObject>();
                var sel = new List<IMapObject>();
                var seltest = SelectionTest(document, camera, e);
                if (seltest != null)
                {
                    if (!ctrl || !seltest.IsSelected) sel.Add(seltest);
                    else desel.Add(seltest);
                }
                SetSelected(document, desel, sel, !ctrl, IgnoreGrouping());
            }
            else if (_selectionBox.State.Action == BoxAction.Drawn 
                && draggable is ResizeTransformHandle res 
                && res.Handle == ResizeHandle.Center 
                && SelectionClickCycles)
            {
                _selectionBox.Cycle();
            }
        }

        protected override void OnDraggableDragStarted(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position, IDraggable draggable)
        {
            var ctrl = KeyboardState.Ctrl;

            if (draggable == _emptyBox && !ctrl && !document.Selection.IsEmpty)
            {
                SetSelected(document, null, null, true, IgnoreGrouping());
            }

            // If all selected items have an origin, snap to the closest one
            if (draggable is ResizeTransformHandle res && res.Handle == ResizeHandle.Center && !document.Selection.IsEmpty)
            {
                var origins = document.Selection.Select(x => x.Data.GetOne<Origin>()).ToList();
                if (origins.Any(x => x == null)) return;
                var closest = origins.Select(x => camera.Flatten(x.Location)).OrderBy(x => (x - position).LengthSquared()).First();
                res.SetMoveOrigin(closest);
            }
        }

        protected override void OnDraggableDragMoved(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 previousPosition, Vector3 position, IDraggable draggable)
        {
            base.OnDraggableDragMoved(document, viewport, camera, e, previousPosition, position, draggable);
            if (_selectionBox.State.Action == BoxAction.Resizing && draggable is ITransformationHandle)
            {
                var tform = _selectionBox.GetTransformationMatrix(viewport, camera, document);
                if (tform.HasValue)
                {
                    Engine.Interface.SetSelectiveTransform(tform.Value);

                    var box = new Box(_selectionBox.State.OrigStart, _selectionBox.State.OrigEnd);
                    var trans = tform.Value;
                    box = box.Transform(trans);

                    var label = "";
                    if (box != null && !box.IsEmpty()) label = box.Width.ToString("0") + " x " + box.Length.ToString("0") + " x " + box.Height.ToString("0");
                    Oy.Publish("MapDocument:ToolStatus:UpdateText", label);
                }
            }
        }

        protected override void OnDraggableDragEnded(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position, IDraggable draggable)
        {
            var task = Task.CompletedTask;
            if (_selectionBox.State.Action == BoxAction.Resizing && draggable is ITransformationHandle)
            {
                // Execute the transform on the selection
                var tform = _selectionBox.GetTransformationMatrix(viewport, camera, document);
                if (tform.HasValue)
                {
                    var ttType = _selectionBox.GetTextureTransformationType(document);
                    var createClone = KeyboardState.Shift && draggable is ResizeTransformHandle handle && handle.Handle == ResizeHandle.Center;
                    task = ExecuteTransform(document, tform.Value, createClone, ttType);
                }
            }

            task.ContinueWith(_ => Engine.Interface.SetSelectiveTransform(Matrix4x4.Identity));

            base.OnDraggableDragEnded(document, viewport, camera, e, position, draggable);
        }

        private void EmptyBoxChanged(object sender, EventArgs e)
        {
            if (_emptyBox.State.Action != BoxAction.Idle && _selectionBox.State.Action != BoxAction.Idle)
            {
                _selectionBox.State.Action = BoxAction.Idle;
            }
        }

        private void SelectionBoxChanged(object sender, EventArgs e)
        {

        }

        private bool LineAndCenterIntersectFilter(IMapObject obj, Box box)
        {
            return CenterHandleIntersectFilter(obj, box) || LineIntersectFilter(obj, box);
        }

        private bool CenterHandleIntersectFilter(IMapObject obj, Box box)
        {
            return obj.BoundingBox != null && box.Vector3IsInside(obj.BoundingBox.Center);
        }

        private bool LineIntersectFilter(IMapObject obj, Box box)
        {
            return obj.GetPolygons().Any(p => p.GetLines().Any(box.IntersectsWith));
        }

        private IEnumerable<IMapObject> GetLineIntersections(MapDocument document, Box box, Func<IMapObject, Box, bool> filter)
        {
            return document.Map.Root.Collect(
                x => x is Root || (x.BoundingBox != null && x.BoundingBox.IntersectsWith(box)),
                x => x.Hierarchy.Parent != null && !x.Hierarchy.HasChildren && filter(x, box)
            );
        }

        private IMapObject SelectionTest(MapDocument document, OrthographicCamera camera, ViewportEvent e)
        {
            // Create a box to represent the click, with a tolerance level
            var unused = camera.GetUnusedCoordinate(new Vector3(100000, 100000, 100000));
            var tolerance = 4 / camera.Zoom; // Selection tolerance of four pixels
            var used = camera.Expand(new Vector3(tolerance, tolerance, 0));
            var add = used + unused;
            var click = camera.ScreenToWorld(e.X, e.Y);
            var box = new Box(click - add, click + add);
            
            // Get the first element that intersects with the box, selecting or deselecting as needed
            Func<IMapObject, Box, bool> filter;

            if (OnlySelectByCenterHandles) filter = CenterHandleIntersectFilter;
            else if (!SelectByCenterHandles) filter = LineIntersectFilter;
            else filter = LineAndCenterIntersectFilter;

            return GetLineIntersections(document, box, filter).FirstOrDefault();
        }

        protected override void KeyDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.KeyCode == Keys.Enter) Confirm(document);
            else if (e.KeyCode == Keys.Escape) Cancel(document);

            var nudge = GetNudgeValue(e.KeyCode);
            if (nudge != null && (_selectionBox.State.Action == BoxAction.Drawn) && !document.Selection.IsEmpty)
            {
                var translate = camera.Expand(nudge.Value);
                var transformation = Matrix4x4.CreateTranslation(translate.X, translate.Y, translate.Z);
                var matrix = transformation;
                ExecuteTransform(document, matrix, KeyboardState.Shift, TextureTransformationType.Uniform);
                SelectionChanged(document);
            }
        }

        #endregion

        #region Box confirm/cancel

        protected override void KeyDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (e.KeyCode == Keys.Enter) Confirm(document);
            else if (e.KeyCode == Keys.Escape) Cancel(document);

            base.KeyDown(document, viewport, camera, e);
        }

        private IEnumerable<IMapObject> GetBoxIntersections(MapDocument document, Box box, Predicate<IMapObject> includeFilter)
        {
            return document.Map.Root.Collect(
                x => x is Root || (x.BoundingBox != null && x.BoundingBox.IntersectsWith(box)),
                x => x.Hierarchy.Parent != null && !x.Hierarchy.HasChildren && includeFilter(x)
            );
        }

        /// <summary>
        /// Once a box is confirmed, we select all element intersecting with the box (contained within if shift is down).
        /// </summary>
        private void Confirm(MapDocument document)
        {
            // Only confirm the box if the empty box is drawn
            if (_selectionBox.State.Action != BoxAction.Idle || _emptyBox.State.Action != BoxAction.Drawn) return;

            var boundingbox = _emptyBox.State.GetSelectionBox();
            if (boundingbox != null)
            {
                // If the shift key is down, select all brushes that are fully contained by the box
                // If select by handles only is on, select all brushes with centers inside the box
                // Otherwise, select all brushes that intersect with the box
				Predicate<IMapObject> filter;
				if ( KeyboardState.Shift )
					filter = x => x.BoundingBox.ContainedWithin(boundingbox);
				else if ( SelectionBoxOnlySelectsByCenterHandles )
					filter = x => boundingbox.Vector3IsInside(x.BoundingBox.Center);
				else
					filter = x => true;

                var nodes = GetBoxIntersections(document, boundingbox, filter);

                SetSelected(document, null, nodes, false, IgnoreGrouping());
            }

            SelectionChanged(document);
        }

        private void Cancel(MapDocument document)
        {
            if (_selectionBox.State.Action != BoxAction.Idle && !document.Selection.IsEmpty)
            {
                SetSelected(document, null, null, true, IgnoreGrouping());
            }
            _selectionBox.State.Action = _emptyBox.State.Action = BoxAction.Idle;
            SelectionChanged(document);
        }

        #endregion

        #region Transform stuff

        /// <summary>
        /// Runs the transform on all the currently selected objects
        /// </summary>
        /// <param name="document">The current document</param>
        /// <param name="transform">The transformation to apply</param>
        /// <param name="clone">True to create a clone before transforming the original.</param>
        /// <param name="textureTransformationType"></param>
        private Task ExecuteTransform(MapDocument document, Matrix4x4 transform, bool clone, TextureTransformationType textureTransformationType)
        {
            var parents = document.Selection.GetSelectedParents().ToList();
            var transaction = new Transaction();
            if (clone)
            {
                // We're creating copies, so clone and transform the objects before attaching them.
                var copies = parents
                    .Select(x => x.Copy(document.Map.NumberGenerator))
                    .OfType<IMapObject>()
                    .Select(mo =>
                    {
                        // Transform the clone (and textures)
                        mo.Transform(transform);
                        if (textureTransformationType == TextureTransformationType.Uniform)
                        {
                            foreach (var s in mo.FindAll().OfType<Solid>().SelectMany(x => x.Faces).Select(x => x.Texture))
                            {
                                s.TransformUniform(transform);
                            }
                        }
                        else if (textureTransformationType == TextureTransformationType.Scale)
                        {
                            foreach (var t in mo.FindAll().SelectMany(x => x.Data.OfType<ITextured>()))
                            {
                                t.Texture.TransformScale(transform);
                            }
                        }

                        // Check if we need to clear the visgroups
                        if (!KeepVisgroupsWhenCloning)
                        {
                            mo.Data.Remove(x => x is VisgroupID);
                        }

                        return mo;
                    });

                // Deselect the originals, we want to move the selection to the new nodes
                transaction.Add(new Deselect(document.Selection));

                // Attach the objects
                transaction.Add(new Attach(document.Map.Root.ID, copies));
            }
            else
            {
                // Transform the objects
                transaction.Add(new Transform(transform, parents));

                // Perform texture transforms if required
                if (textureTransformationType == TextureTransformationType.Uniform)
                {
                    transaction.Add(new TransformTexturesUniform(transform, parents.SelectMany(p => p.FindAll())));
                }
                else if (textureTransformationType == TextureTransformationType.Scale)
                {
                    transaction.Add(new TransformTexturesScale(transform, parents.SelectMany(p => p.FindAll())));
                }
            }
            return MapDocumentOperation.Perform(document, transaction);
        }

        #endregion
    }
}