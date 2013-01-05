using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Editing;
using Sledge.Editor.History;
using Sledge.Editor.Properties;
using Sledge.UI;

namespace Sledge.Editor.Tools
{
    /// <summary>
    /// The select tool is used to select objects in several different ways:
    /// 1. Single click in the 2D view will perform edge-detection selection
    /// 2. Single click in the 3D view allows ray-casting selection (with mouse wheel cycling)
    /// 3. Drawing a box in the 2D view and confirming it will select everything in the box
    /// </summary>
    class SelectTool : BaseBoxTool
    {
        private Box _selectionBoundingBox;
        private SelectToolContextMenu _contextMenu;
        private MapObject ChosenItemFor3DSelection { get; set; }
        private List<MapObject> IntersectingObjectsFor3DSelection { get; set; }

        public SelectTool()
        {
            Usage = ToolUsage.Both;
            _contextMenu = new SelectToolContextMenu(this);
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Select;
        }

        public override string GetName()
        {
            return "Select Tool";
        }

        protected override Color BoxColour
        {
            get { return Color.Yellow; }
        }

        protected override Color FillColour
        {
            get { return Color.FromArgb(128, Color.Gray); }
        }

        public override void ToolSelected()
        {
            Mediator.Subscribe(EditorMediator.SelectionChanged, this);
            SelectionChanged();
            Document.UpdateSelectLists();
        }

        public override void ToolDeselected()
        {
            _selectionBoundingBox = null;
        }

        private void SelectionChanged()
        {
            _selectionBoundingBox = Document == null || Document.Selection.IsEmpty()
                                        ? null
                                        : new Box(Document.Selection.GetSelectedObjects().Select(x => x.BoundingBox));
        }

        /// <summary>
        /// If ignoreGrouping is disabled, this will convert the list of objects into their topmost group or entity.
        /// </summary>
        /// <param name="objects">The object list to normalise</param>
        /// <param name="ignoreGrouping">True if grouping is being ignored</param>
        /// <returns>The normalised list of objects</returns>
        private static IEnumerable<MapObject> NormaliseSelection(IEnumerable<MapObject> objects, bool ignoreGrouping)
        {
            //TODO should selection flatten?
            return ignoreGrouping
                       ? objects
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
            }

            // Normalise selections
            objectsToDeselect = NormaliseSelection(objectsToDeselect.Where(x => x != null), ignoreGrouping);
            objectsToSelect = NormaliseSelection(objectsToSelect.Where(x => x != null), ignoreGrouping);

            // Don't bother deselecting the objects we're about to select
            objectsToDeselect = objectsToDeselect.Where(x => !objectsToSelect.Contains(x));

            // Perform selections
            var deselected = objectsToDeselect.ToList();
            var selected = objectsToSelect.ToList();

            deselected.ForEach(Document.Selection.Deselect);
            selected.ForEach(Document.Selection.Select);

            // Log history
            var hd = new HistorySelect("Deselected objects", deselected, true);
            var hs = new HistorySelect("Selected objects", selected, true);
            var ic = new HistoryItemCollection("Selection changed", new[] {hd, hs});
            Document.History.AddHistoryItem(ic);
        }

        /// <summary>
        /// When the mouse is pressed in the 3D view, we want to select the clicked object.
        /// </summary>
        /// <param name="viewport">The viewport that was clicked</param>
        /// <param name="e">The click event</param>
        protected override void MouseDown3D(Viewport3D viewport, MouseEventArgs e)
        {
            // First, get the ray that is cast from the clicked point along the viewport frustrum
            var ray = viewport.CastRayFromScreen(e.X, e.Y);

            // Grab all the elements that intersect with the ray
            var hits = Document.Map.WorldSpawn.GetAllNodesIntersectingWith(ray);

            // Sort the list of intersecting elements by distance from ray origin
            IntersectingObjectsFor3DSelection = hits
                .Select(x => new { Item = x, Intersection = x.GetIntersectionPoint(ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .Select(x => x.Item)
                .ToList();

            // By default, select the closest object
            ChosenItemFor3DSelection = IntersectingObjectsFor3DSelection.FirstOrDefault();

            // If Ctrl is down and the object is already selected, we should deselect it instead.
            var list = new[] {ChosenItemFor3DSelection};
            var desel = ChosenItemFor3DSelection != null && KeyboardState.Ctrl && ChosenItemFor3DSelection.IsSelected;
            SetSelected(desel ? list : null, desel ? null : list, !KeyboardState.Ctrl, false);

            Document.UpdateSelectLists();
            State.ActiveViewport = null;
        }

        /// <summary>
        /// When the mouse wheel is scrolled while the mouse is down in the 3D view, cycle through the candidate elements.
        /// </summary>
        /// <param name="viewport">The viewport that was scrolled</param>
        /// <param name="e">The scroll event</param>
        public override void MouseWheel(ViewportBase viewport, MouseEventArgs e)
        {
            // If we're not in 3D cycle mode, carry on
            if (!(viewport is Viewport3D)
                || IntersectingObjectsFor3DSelection == null
                || ChosenItemFor3DSelection == null)
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

            SetSelected(desel, sel, false, false);

            Document.UpdateSelectLists();

            State.ActiveViewport = null;
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

        /// <summary>
        /// The select tool will deselect all selected objects if ctrl is not held down when drawing a box.
        /// </summary>
        /// <param name="viewport">The viewport to draw in</param>
        /// <param name="e">The mouse event</param>
        protected override void LeftMouseDownToDraw(Viewport2D viewport, MouseEventArgs e)
        {
            // If we've clicked outside a selection box and not holding down control, clear the selection
            if (!Document.Selection.IsEmpty() && !KeyboardState.Ctrl)
            {
                SetSelected(null, null, true, false);
                Document.UpdateSelectLists();
            }

            base.LeftMouseDownToDraw(viewport, e);
        }

        /// <summary>
        /// Once the mouse is released in the 3D view, the 3D select cycle has finished.
        /// </summary>
        /// <param name="viewport">The 3D viewport</param>
        /// <param name="e">The mouse event</param>
        protected override void MouseUp3D(Viewport3D viewport, MouseEventArgs e)
        {
            IntersectingObjectsFor3DSelection = null;
            ChosenItemFor3DSelection = null;
        }

        /// <summary>
        /// If the mouse is single-clicked in a 3D viewport, select the closest element that is under the cursor
        /// </summary>
        /// <param name="viewport">The 2D viewport</param>
        /// <param name="e">The mouse event</param>
        protected override void LeftMouseClick(Viewport2D viewport, MouseEventArgs e)
        {
            // Create a box to represent the click, with a tolerance level
            var unused = viewport.GetUnusedCoordinate(new Coordinate(100000, 100000, 100000));
            var tolerance = 4 / viewport.Zoom; // Selection tolerance of four pixels
            var used = viewport.Expand(new Coordinate(tolerance, tolerance, 0));
            var add = used + unused;
            var click = viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y));
            var box = new Box(click - add, click + add);

            // Get the first element that intersects with the box, selecting or deselecting as needed
            var seltest = Document.Map.WorldSpawn.GetAllNodesIntersecting2DLineTest(box).FirstOrDefault();
            if (seltest != null)
            {
                var list = new[] { seltest };
                SetSelected(seltest.IsSelected ? list : null, seltest.IsSelected ? null : list, false, false);
                Document.UpdateSelectLists();
            }

            base.LeftMouseClick(viewport, e);
        }

        public override void MouseUp(ViewportBase viewport, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && viewport is Viewport2D && _selectionBoundingBox != null)
            {
                MouseRightClick(e, (Viewport2D)viewport);
            }
            base.MouseUp(viewport, e);
        }

        private void MouseRightClick(MouseEventArgs e, Viewport2D vp)
        {
            var point = vp.ScreenToWorld(e.X, vp.Height - e.Y);
            var start = vp.Flatten(_selectionBoundingBox.Start);
            var end = vp.Flatten(_selectionBoundingBox.End);
            if (point.X >= start.X && point.X <= end.X && point.Y >= start.Y && point.Y <= end.Y)
            {
                // Show context menu
                _contextMenu.Show(vp, e.X, e.Y);
            }
        }

        /// <summary>
        /// Once a box is confirmed, we select all element intersecting with the box (contained within if shift is down).
        /// </summary>
        /// <param name="viewport">The viewport that the box was confirmed in</param>
        public override void BoxDrawnConfirm(ViewportBase viewport)
        {
            Box boundingbox;
            if (GetSelectionBox(out boundingbox))
            {
                // If the shift key is down, select all brushes that are fully contained by the box
                // Otherwise, select all brushes that intersect with the box
                var nodes = KeyboardState.Shift
                                ? Document.Map.WorldSpawn.GetAllNodesContainedWithin(boundingbox).ToList()
                                : Document.Map.WorldSpawn.GetAllNodesIntersectingWith(boundingbox).ToList();
                SetSelected(null, nodes, false, false);
                Document.UpdateSelectLists();
            }
            base.BoxDrawnConfirm(viewport);
        }

        protected override void Render2D(Viewport2D viewport)
        {
            base.Render2D(viewport);

            if (_selectionBoundingBox == null) return;

            var start = viewport.Flatten(_selectionBoundingBox.Start);
            var end = viewport.Flatten(_selectionBoundingBox.End);
            GL.Enable(EnableCap.LineStipple);
            GL.LineStipple(10, 0x5555);
            GL.Begin(BeginMode.LineLoop);
            GL.Color3(Color.Red);
            Coord(start.DX, start.DY, start.DZ);
            Coord(end.DX, start.DY, start.DZ);
            Coord(end.DX, end.DY, start.DZ);
            Coord(start.DX, end.DY, start.DZ);
            GL.End();
            GL.Disable(EnableCap.LineStipple);
        }

        private void Undo()
        {

        }

        private void Redo()
        {

        }

        private void Cut()
        {

        }

        private void Copy()
        {

        }

        private void Delete()
        {

        }

        private void Group()
        {

        }

        private void Ungroup()
        {

        }

        private void ToWorld()
        {

        }

        private void ToEntity()
        {

        }

        private sealed class SelectToolContextMenu : ContextMenuStrip
        {
            private readonly SelectTool _tool;

            public SelectToolContextMenu(SelectTool tool)
            {
                _tool = tool;
                Add("Cut", () => _tool.Cut());
                Add("Copy", () => _tool.Copy());
                Add("Delete", () => _tool.Delete());
                Add("Paste Special", () => { });
                Items.Add(new ToolStripSeparator());
                Add("Undo", () => _tool.Undo());
                Add("Redo", () => _tool.Redo());
                Items.Add(new ToolStripSeparator());
                Add("Carve", () => { });
                Add("Hollow", () => { });
                Items.Add(new ToolStripSeparator());
                Add("Group", () => _tool.Group());
                Add("Ungroup", () => _tool.Ungroup());
                Items.Add(new ToolStripSeparator());
                Add("Move To Entity", () => _tool.ToEntity());
                Add("Move To World", () => _tool.ToWorld());
                Items.Add(new ToolStripSeparator());
                Items.Add(new ToolStripMenuItem("Align", null,
                                                CreateMenuItem("Top", () => { }),
                                                CreateMenuItem("Left", () => { }),
                                                CreateMenuItem("Right", () => { }),
                                                CreateMenuItem("Bottom", () => { })));
                Add("Properties", () => { });
            }

            private void Add(string name, Action onclick)
            {
                Items.Add(CreateMenuItem(name, onclick));
            }

            private static ToolStripItem CreateMenuItem(string name, Action onclick)
            {
                var item = new ToolStripMenuItem(name);
                item.Click += (sender, args) => onclick();
                return item;
            }
        }
    }
}
