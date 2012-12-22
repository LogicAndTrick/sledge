using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Editing;
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
        private MapObject ChosenItemFor3DSelection { get; set; }
        private List<MapObject> IntersectingObjectsFor3DSelection { get; set; }

        public SelectTool()
        {
            Usage = ToolUsage.Both;
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
            Document.UpdateSelectLists();
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

            // Clear other selected objects if ctrl isn't down
            if (!KeyboardState.Ctrl)
            {
                Selection.Clear();
            }

            // Select (or deselect if applicable) the clicked object
            if (ChosenItemFor3DSelection != null)
            {
                if (ChosenItemFor3DSelection.IsSelected) Selection.Deselect(ChosenItemFor3DSelection);
                else Selection.Select(ChosenItemFor3DSelection);
            }

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

            // Select (or deselect) the current element
            if (ChosenItemFor3DSelection.IsSelected) Selection.Deselect(ChosenItemFor3DSelection);
            else Selection.Select(ChosenItemFor3DSelection);

            // Get the index of the current element
            var index = IntersectingObjectsFor3DSelection.IndexOf(ChosenItemFor3DSelection);
            if (index < 0) return;

            // Move the index in the mouse wheel direction, cycling if needed
            var dir = e.Delta / Math.Abs(e.Delta);
            index = (index + dir) % IntersectingObjectsFor3DSelection.Count;
            if (index < 0) index += IntersectingObjectsFor3DSelection.Count;

            ChosenItemFor3DSelection = IntersectingObjectsFor3DSelection[index];

            // Select (or deselect) the new current element
            if (ChosenItemFor3DSelection.IsSelected) Selection.Deselect(ChosenItemFor3DSelection);
            else Selection.Select(ChosenItemFor3DSelection);

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
            if (!Selection.IsEmpty() && !KeyboardState.Ctrl)
            {
                Selection.Clear();
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
                if (seltest.IsSelected) Selection.Deselect(seltest);
                else Selection.Select(seltest);
                Document.UpdateSelectLists();
            }

            base.LeftMouseClick(viewport, e);
        }

        /// <summary>
        /// Once a box is confirmed, we select all element intersecting with the box (contained within if shift is down).
        /// </summary>
        /// <param name="viewport">The viewport that the box was confirmed in</param>
        public override void BoxDrawnConfirm(ViewportBase viewport)
        {
            // If one of the dimensions has a depth value of 0, extend it out into infinite space
            // If two or more dimensions have depth 0, do nothing.

            var sameX = State.BoxStart.X == State.BoxEnd.X;
            var sameY = State.BoxStart.Y == State.BoxEnd.Y;
            var sameZ = State.BoxStart.Z == State.BoxEnd.Z;
            var start = State.BoxStart.Clone();
            var end = State.BoxEnd.Clone();
            var invalid = false;

            if (sameX)
            {
                if (sameY || sameZ) invalid = true;
                start.X = Decimal.MinValue;
                end.X = Decimal.MaxValue;
            }

            if (sameY)
            {
                if (sameZ) invalid = true;
                start.Y = Decimal.MinValue;
                end.Y = Decimal.MaxValue;
            }

            if (sameZ)
            {
                start.Z = Decimal.MinValue;
                end.Z = Decimal.MaxValue;
            }

            if (invalid)
            {
                base.BoxDrawnConfirm(viewport);
                return;
            }

            // If the shift key is down, select all brushes that are fully contained by the box
            // Otherwise, select all brushes that intersect with the box

            var boundingbox = new Box(start, end);
            var nodes = KeyboardState.Shift
                            ? Document.Map.WorldSpawn.GetAllNodesContainedWithin(boundingbox).ToList()
                            : Document.Map.WorldSpawn.GetAllNodesIntersectingWith(boundingbox).ToList();
            Selection.Select(nodes);
            Document.UpdateSelectLists();
            base.BoxDrawnConfirm(viewport);
        }
    }
}
