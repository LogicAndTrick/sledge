using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using Sledge.Editor.Properties;
using Sledge.Extensions;
using Sledge.Settings;
using Sledge.UI;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Editing;
using Sledge.DataStructures.Transformations;
using OpenTK.Graphics.OpenGL;
using Sledge.Editor.Rendering;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.Tools
{
    /// <summary>
    /// The SelectTool is designed to take over the majority of the BaseBoxTool logic
    /// once a selection has been made, so there's a bit of code duplication between the two.
    /// </summary>
    public class NotGreatSelectTool : BaseBoxTool
    {
        protected enum SelectBoxAction
        {
            Default,
            Selection,
            ReadyToResize,
            DownToResize,
            Resizing,
            ReadyToRotate,
            DownToRotate,
            Rotating,
            ReadyToShear,
            DownToShear,
            Shearing
        }

        private SelectBoxAction SelectAction { get; set; }

        private MapObject ChosenItemFor3DSelection { get; set; }
        private List<MapObject> IntersectingObjectsFor3DSelection { get; set; }

        public NotGreatSelectTool()
        {
            Usage = ToolUsage.Both;
            SelectAction = SelectBoxAction.Default;
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Select;
        }

        public override string GetName()
        {
            return "Select Tool";
        }

        public override void ToolSelected()
        {
            Document.UpdateSelectLists();
            UpdateBoxBasedOnSelection();
        }

        protected override Color BoxColour
        {
            get { return Color.Yellow; }
        }

        protected override Color FillColour
        {
            get { return Color.FromArgb(128, Color.Gray); }
        }

        // Mouse Down
        protected override void MouseDown3D(Viewport3D viewport, MouseEventArgs e)
        {
            // Search for hits
            var ray = viewport.CastRayFromScreen(e.X, e.Y);
            var hits = Document.Map.WorldSpawn.GetAllNodesIntersectingWith(ray);
            IntersectingObjectsFor3DSelection = hits
                .Select(x => new { Item = x, Intersection = x.GetIntersectionPoint(ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .Select(x => x.Item)
                .ToList();
            ChosenItemFor3DSelection = IntersectingObjectsFor3DSelection.FirstOrDefault();
            if (!KeyboardState.Ctrl)
            {
                Selection.Clear();
            }

            if (ChosenItemFor3DSelection != null)
            {
                if (ChosenItemFor3DSelection.IsSelected) Selection.Deselect(ChosenItemFor3DSelection);
                else Selection.Select(ChosenItemFor3DSelection);
            }

            Document.UpdateSelectLists();
            UpdateBoxBasedOnSelection();

            var empty = Selection.IsEmpty();
            State.Action = empty ? BoxAction.ReadyToDraw : BoxAction.Drawn;
            SelectAction = empty ? SelectBoxAction.Default : SelectBoxAction.Selection;
            State.Action = empty ? BoxAction.ReadyToDraw : BoxAction.Drawn;
            State.ActiveViewport = null;
        }

        public override void MouseWheel(ViewportBase viewport, MouseEventArgs e)
        {
            if (!(viewport is Viewport3D)
                || IntersectingObjectsFor3DSelection == null
                || ChosenItemFor3DSelection == null)
            {
                return;
            }

            if (ChosenItemFor3DSelection.IsSelected) Selection.Deselect(ChosenItemFor3DSelection);
            else Selection.Select(ChosenItemFor3DSelection);

            var index = IntersectingObjectsFor3DSelection.IndexOf(ChosenItemFor3DSelection);
            if (index < 0) return;

            var dir = e.Delta / Math.Abs(e.Delta);
            index = (index + dir) % IntersectingObjectsFor3DSelection.Count;
            if (index < 0) index += IntersectingObjectsFor3DSelection.Count;

            ChosenItemFor3DSelection = IntersectingObjectsFor3DSelection[index];

            if (ChosenItemFor3DSelection.IsSelected) Selection.Deselect(ChosenItemFor3DSelection);
            else Selection.Select(ChosenItemFor3DSelection);

            Document.UpdateSelectLists();
            UpdateBoxBasedOnSelection();

            var empty = Selection.IsEmpty();
            State.Action = empty ? BoxAction.ReadyToDraw : BoxAction.Drawn;
            SelectAction = empty ? SelectBoxAction.Default : SelectBoxAction.Selection;
            State.Action = empty ? BoxAction.ReadyToDraw : BoxAction.Drawn;
            State.ActiveViewport = null;
        }

        public override bool IsCapturingMouseWheel()
        {
            return IntersectingObjectsFor3DSelection != null
                   && IntersectingObjectsFor3DSelection.Any()
                   && ChosenItemFor3DSelection != null;
        }

        protected override void LeftMouseDownToDraw(Viewport2D viewport, MouseEventArgs e)
        {
            // If we've clicked outside a selection box and not holding down control, clear the selection
            if (SelectAction != SelectBoxAction.Default && !Selection.IsEmpty() && !KeyboardState.Ctrl)
            {
                Selection.Clear();
                Document.UpdateSelectLists();
            }

            SelectAction = SelectBoxAction.Default;
            base.LeftMouseDownToDraw(viewport, e);
        }

        protected override void LeftMouseDownToResize(Viewport2D viewport, MouseEventArgs e)
        {
            // If control is down, we are drawing a new box, even if we have a resize handle highlighted.
            if (KeyboardState.Ctrl)
            {
                LeftMouseDownToDraw(viewport, e);
                return;
            }

            base.LeftMouseDownToResize(viewport, e);

            // If we're not in selection mode, all control goes to the base
            if (SelectAction == SelectBoxAction.Default) return;

            // Set the appropriate action based on the resize handle
            switch (SelectAction)
            {
                case SelectBoxAction.ReadyToResize:
                    SelectAction = SelectBoxAction.DownToResize;
                    break;
                case SelectBoxAction.ReadyToRotate:
                    SelectAction = SelectBoxAction.DownToRotate;
                    break;
                case SelectBoxAction.ReadyToShear:
                    SelectAction = SelectBoxAction.DownToShear;
                    break;
            }
        }

        // Mouse Up
        protected override void MouseUp3D(Viewport3D viewport, MouseEventArgs e)
        {
            IntersectingObjectsFor3DSelection = null;
            ChosenItemFor3DSelection = null;
        }

        protected override void LeftMouseUpResizing(Viewport2D viewport, MouseEventArgs e)
        {
            // Give control to the base if we aren't in a selection
            if (SelectAction == SelectBoxAction.Default)
            {
                base.LeftMouseUpResizing(viewport, e);
            }
            else
            {
                // Execute the selection transform
                Matrix4d? transformation = null;
                switch (SelectAction)
                {
                    case SelectBoxAction.Resizing:
                        transformation = GetResizeMatrix(viewport, e);
                        break;
                    case SelectBoxAction.Rotating:
                        transformation = GetRotationMatrix(viewport, e);
                        break;
                    case SelectBoxAction.Shearing:
                        transformation = GetShearMatrix(viewport, e);
                        break;
                }
                if (transformation.HasValue)
                {
                    ExecuteTransform(CreateMatrixMultTransformation(transformation.Value));
                }
                Document.EndSelectionTransform();
                SelectAction = SelectBoxAction.Selection;
                State.ActiveViewport = null;
                State.Action = BoxAction.Drawn;
                UpdateBoxBasedOnSelection();
            }
        }

        protected override void LeftMouseClick(Viewport2D viewport, MouseEventArgs e)
        {
            // Do a click selection
            var unused = viewport.GetUnusedCoordinate(new Coordinate(100000, 100000, 100000));
            var tolerance = 4 / viewport.Zoom;
            var used = viewport.Expand(new Coordinate(tolerance, tolerance, 0));
            var add = used + unused;
            var click = viewport.Expand(viewport.ScreenToWorld(e.X, viewport.Height - e.Y));
            var box = new Box(click - add, click + add);
            var seltest = Document.Map.WorldSpawn.GetAllNodesIntersecting2DLineTest(box).FirstOrDefault();
            if (seltest != null)
            {
                if (seltest.IsSelected) Selection.Deselect(seltest);
                else Selection.Select(seltest);
                Document.UpdateSelectLists();
            }
            base.LeftMouseClick(viewport, e);
            SelectAction = SelectBoxAction.Default;

            if (Selection.IsEmpty()) return;
            State.Action = BoxAction.Drawn;
            SelectAction = SelectBoxAction.Selection;
            UpdateBoxBasedOnSelection();
        }

        #region Transformation/Matrices
        protected IUnitTransformation CreateMatrixMultTransformation(Matrix4d mat)
        {
            var dmat = new[]
                           {
                               (decimal) mat.M11, (decimal) mat.M21, (decimal) mat.M31, (decimal) mat.M41,
                               (decimal) mat.M12, (decimal) mat.M22, (decimal) mat.M32, (decimal) mat.M42,
                               (decimal) mat.M13, (decimal) mat.M23, (decimal) mat.M33, (decimal) mat.M43,
                               (decimal) mat.M14, (decimal) mat.M24, (decimal) mat.M34, (decimal) mat.M44,
                           };
            return new UnitMatrixMult(dmat);
        }

        protected void ExecuteTransform(IUnitTransformation transform)
        {
            foreach (var o in Selection.GetSelectedObjects().Where(o => o.Parent == null || !o.Parent.IsSelected))
            {
                o.Transform(transform);
            }
        }

        protected Coordinate GetOriginForTransform(Viewport2D viewport)
        {
            decimal x = 0;
            decimal y = 0;
            var cstart = viewport.Flatten(State.PreTransformBoxStart);
            var cend = viewport.Flatten(State.PreTransformBoxEnd);
            switch (State.Handle)
            {
                case ResizeHandle.TopLeft:
                case ResizeHandle.Top:
                case ResizeHandle.TopRight:
                case ResizeHandle.Left:
                case ResizeHandle.Right:
                    y = cstart.Y;
                    break;
                case ResizeHandle.BottomLeft:
                case ResizeHandle.Bottom:
                case ResizeHandle.BottomRight:
                    y = cend.Y;
                    break;
            }
            switch (State.Handle)
            {
                case ResizeHandle.Top:
                case ResizeHandle.TopRight:
                case ResizeHandle.Right:
                case ResizeHandle.BottomRight:
                case ResizeHandle.Bottom:
                    x = cstart.X;
                    break;
                case ResizeHandle.TopLeft:
                case ResizeHandle.Left:
                case ResizeHandle.BottomLeft:
                    x = cend.X;
                    break;
            }
            return viewport.Expand(new Coordinate(x, y, 0));
        }

        protected Matrix4d GetResizeMatrix(Viewport2D viewport, MouseEventArgs e)
        {
            var coords = GetBoxCoordinatesForSelectionResize(viewport, e);
            State.BoxStart = coords.Item1;
            State.BoxEnd = coords.Item2;
            Matrix4d resizeMatrix;
            if (State.Handle == ResizeHandle.Center)
            {
                var movement = State.BoxStart - State.PreTransformBoxStart;
                resizeMatrix = Matrix4d.CreateTranslation((float)movement.X, (float)movement.Y, (float)movement.Z);
            }
            else
            {
                var resize = (State.PreTransformBoxStart - State.BoxStart) +
                             (State.BoxEnd - State.PreTransformBoxEnd);
                resize = resize.ComponentDivide(State.PreTransformBoxEnd - State.PreTransformBoxStart);
                resize += new Coordinate(1, 1, 1);
                var offset = -GetOriginForTransform(viewport);
                var trans = Matrix4d.CreateTranslation((float)offset.X, (float)offset.Y, (float)offset.Z);
                var scale = Matrix4d.Mult(trans, Matrix4d.Scale((float)resize.X, (float)resize.Y, (float)resize.Z));
                resizeMatrix = Matrix4d.Mult(scale, Matrix4d.Invert(trans));
            }
            return resizeMatrix;
        }

        protected Matrix4d GetRotationMatrix(Viewport2D viewport, MouseEventArgs e)
        {
            var origin = viewport.ZeroUnusedCoordinate((State.PreTransformBoxStart + State.PreTransformBoxEnd) / 2);
            var forigin = viewport.Flatten(origin);

            var origv = (State.MoveStart - forigin).Normalise();
            var newv = (viewport.ScreenToWorld(e.X, viewport.Height - e.Y) - forigin).Normalise();

            var angle = DMath.Acos(Math.Max(-1, Math.Min(1, origv.Dot(newv))));
            if ((origv.Cross(newv).Z < 0)) angle = 2 * DMath.PI - angle;

            var shf = KeyboardState.Shift;
            var def = Select.RotationStyle;
            var snap = (def == RotationStyle.SnapOnShift && shf) || (def == RotationStyle.SnapOffShift && !shf);
            if (snap)
            {
                var deg = angle * (180 / DMath.PI);
                var rnd = Math.Round(deg / 15) * 15;
                angle = rnd * (DMath.PI / 180);
            }

            Matrix4d rotm;
            if (viewport.Direction == Viewport2D.ViewDirection.Top) rotm = Matrix4d.CreateRotationZ((float)angle);
            else if (viewport.Direction == Viewport2D.ViewDirection.Front) rotm = Matrix4d.CreateRotationX((float)angle);
            else rotm = Matrix4d.CreateRotationY((float)-angle); // The Y axis rotation goes in the reverse direction for whatever reason

            var mov = Matrix4d.CreateTranslation((float)-origin.X, (float)-origin.Y, (float)-origin.Z);
            var rot = Matrix4d.Mult(mov, rotm);
            return Matrix4d.Mult(rot, Matrix4d.Invert(mov));
        }

        protected Matrix4d GetShearMatrix(Viewport2D viewport, MouseEventArgs e)
        {
            var shearUpDown = State.Handle == ResizeHandle.Left || State.Handle == ResizeHandle.Right;
            var shearTopRight = State.Handle == ResizeHandle.Top || State.Handle == ResizeHandle.Right;

            var nsmd = viewport.ScreenToWorld(e.X, viewport.Height - e.Y) - State.MoveStart;
            var mouseDiff = SnapIfNeeded(nsmd);
            if (!KeyboardState.Alt && KeyboardState.Shift)
            {
                mouseDiff = nsmd.Snap(Document.GridSpacing / 2);
            }
            
            var relative = viewport.Flatten(State.PreTransformBoxEnd - State.PreTransformBoxStart);
            var shearOrigin = (shearTopRight) ? State.PreTransformBoxStart : State.PreTransformBoxEnd;

            var shearAmount = new Coordinate(mouseDiff.X / relative.Y, mouseDiff.Y / relative.X, 0);
            if (!shearTopRight) shearAmount *= -1;

            var shearMatrix = Matrix4d.Identity;
            var sax = (float)shearAmount.X;
            var say = (float)shearAmount.Y;

            switch (viewport.Direction)
            {
                case Viewport2D.ViewDirection.Top:
                    if (shearUpDown) shearMatrix.M12 = say;
                    else shearMatrix.M21 = sax;
                    break;
                case Viewport2D.ViewDirection.Front:
                    if (shearUpDown) shearMatrix.M23 = say;
                    else shearMatrix.M32 = sax;
                    break;
                case Viewport2D.ViewDirection.Side:
                    if (shearUpDown) shearMatrix.M13 = say;
                    else shearMatrix.M31 = sax;
                    break;
            }


            var stran = Matrix4d.CreateTranslation((float)-shearOrigin.X, (float)-shearOrigin.Y, (float)-shearOrigin.Z);
            var shear = Matrix4d.Mult(stran, shearMatrix);
            return Matrix4d.Mult(shear, Matrix4d.Invert(stran));
        }
        #endregion

        private Matrix4d _transform;

        // Mouse Move
        protected override void MouseMove3D(Viewport3D viewport, MouseEventArgs e)
        {
            base.MouseMove3D(viewport, e);
        }

        protected override void MouseDraggingToResize(Viewport2D viewport, MouseEventArgs e)
        {
            if (SelectAction == SelectBoxAction.Default)
            {
                base.MouseDraggingToResize(viewport, e);
                return;
            }
            State.Action = BoxAction.Resizing;
            Matrix4d? transform = null;
            var prev = SelectAction;
            switch (SelectAction)
            {
                case SelectBoxAction.DownToResize:
                case SelectBoxAction.Resizing:
                    SelectAction = SelectBoxAction.Resizing;
                    transform = GetResizeMatrix(viewport, e);
                    break;
                case SelectBoxAction.DownToRotate:
                case SelectBoxAction.Rotating:
                    SelectAction = SelectBoxAction.Rotating;
                    transform = GetRotationMatrix(viewport, e);
                    break;
                case SelectBoxAction.DownToShear:
                case SelectBoxAction.Shearing:
                    SelectAction = SelectBoxAction.Shearing;
                    transform = GetShearMatrix(viewport, e);
                    break;
            }
            if (SelectAction != prev)
            {
                // This is the first drag event on a resize
                Document.StartSelectionTransform();
            }
            if (transform.HasValue)
            {
                _transform = transform.Value;
                Document.SetSelectListTransform(transform.Value);
            }
        }

        protected override void MouseHoverWhenDrawn(Viewport2D viewport, MouseEventArgs e)
        {
            var padding = 7 / (double) viewport.Zoom;

            base.MouseHoverWhenDrawn(viewport, e);

            // Leave control to the base if we are not in selection mode
            if (SelectAction == SelectBoxAction.Default || Selection.IsEmpty())
            {
                SelectAction = SelectBoxAction.Default;
                return;
            }

            SelectAction = SelectBoxAction.Selection;

            var now = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            var start = viewport.Flatten(State.BoxStart);
            var end = viewport.Flatten(State.BoxEnd);

            var handles = GetResizeAndSkewHandleCenters(start, end, (double) viewport.Zoom);
            for (var i = 0; i < handles.Length; i++)
            {
                var h = handles[i];
                if (now.DX < h.X - padding || now.DX > h.X + padding || now.DY < h.Y - padding || now.DY > h.Y + padding) continue;

                var rz = (ResizeHandle)i;
                if (rz == ResizeHandle.Center) continue;

                var square = rz != ResizeHandle.BottomLeft && rz != ResizeHandle.BottomRight && rz != ResizeHandle.TopLeft && rz != ResizeHandle.TopRight;
                State.Handle = rz;
                State.ActiveViewport = viewport;
                State.Action = BoxAction.ReadyToResize;

                if (square)
                {
                    SelectAction = SelectBoxAction.ReadyToShear;
                    viewport.Cursor = (rz == ResizeHandle.Top || rz == ResizeHandle.Bottom) ? Cursors.SizeWE : Cursors.SizeNS;
                }
                else
                {
                    SelectAction = SelectBoxAction.ReadyToRotate;
                    viewport.Cursor = SledgeCursors.RotateCursor;
                }
                return;
            }

            if (State.Action == BoxAction.ReadyToResize)
            {
                SelectAction = SelectBoxAction.ReadyToResize;
            }
        }

        protected Tuple<Coordinate, Coordinate> GetBoxCoordinatesForSelectionResize(Viewport2D viewport, MouseEventArgs e)
        {
            if (State.Action != BoxAction.Resizing) return Tuple.Create(State.BoxStart, State.BoxEnd);
            var now = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            var cstart = viewport.Flatten(State.BoxStart);
            var cend = viewport.Flatten(State.BoxEnd);
            switch (State.Handle)
            {
                case ResizeHandle.TopLeft:
                    cstart.X = Math.Min(now.X, cend.X - 1);
                    cend.Y = Math.Max(now.Y, cstart.Y + 1);
                    break;
                case ResizeHandle.Top:
                    cend.Y = Math.Max(now.Y, cstart.Y + 1);
                    break;
                case ResizeHandle.TopRight:
                    cend.X = Math.Max(now.X, cstart.X + 1);
                    cend.Y = Math.Max(now.Y, cstart.Y + 1);
                    break;
                case ResizeHandle.Left:
                    cstart.X = Math.Min(now.X, cend.X - 1);
                    break;
                case ResizeHandle.Center:
                    var cdiff = cend - cstart;
                    cstart = viewport.Flatten(State.PreTransformBoxStart) + now - State.MoveStart;
                    cend = cstart + cdiff;
                    break;
                case ResizeHandle.Right:
                    cend.X = Math.Max(now.X, cstart.X + 1);
                    break;
                case ResizeHandle.BottomLeft:
                    cstart.X = Math.Min(now.X, cend.X - 1);
                    cstart.Y = Math.Min(now.Y, cend.Y - 1);
                    break;
                case ResizeHandle.Bottom:
                    cstart.Y = Math.Min(now.Y, cend.Y - 1);
                    break;
                case ResizeHandle.BottomRight:
                    cend.X = Math.Max(now.X, cstart.X + 1);
                    cstart.Y = Math.Min(now.Y, cend.Y - 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            cstart = viewport.Expand(cstart) + viewport.GetUnusedCoordinate(State.BoxStart);
            cend = viewport.Expand(cend) + viewport.GetUnusedCoordinate(State.BoxEnd);
            return Tuple.Create(SnapIfNeeded(cstart), SnapIfNeeded(cend));
        }

        protected virtual void UpdateBoxBasedOnSelection()
        {
            decimal x1 = Decimal.MaxValue, y1 = Decimal.MaxValue, z1 = Decimal.MaxValue;
            decimal x2 = Decimal.MinValue, y2 = Decimal.MinValue, z2 = Decimal.MinValue;
            foreach (var c in Selection.GetSelectedObjects())
            {
                var min = c.BoundingBox.Start;
                var max = c.BoundingBox.End;

                x1 = Math.Min(x1, min.X);
                y1 = Math.Min(y1, min.Y);
                z1 = Math.Min(z1, min.Z);

                x2 = Math.Max(x2, max.X);
                y2 = Math.Max(y2, max.Y);
                z2 = Math.Max(z2, max.Z);
            }
            State.BoxStart = new Coordinate(x1, y1, z1);
            State.BoxEnd = new Coordinate(x2, y2, z2);

            if (Selection.IsEmpty())
            {
                State.Action = BoxAction.ReadyToDraw;
                SelectAction = SelectBoxAction.Default;
                State.Action = BoxAction.ReadyToDraw;
            }
        }

        public override void BoxDrawnConfirm(ViewportBase viewport)
        {
            if (SelectAction != SelectBoxAction.Default) return;

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

            var boundingbox = new Box(start, end);
            var nodes = KeyboardState.Shift
                            ? Document.Map.WorldSpawn.GetAllNodesContainedWithin(boundingbox)
                            : Document.Map.WorldSpawn.GetAllNodesIntersectingWith(boundingbox);
            Selection.Select(nodes);
            Document.UpdateSelectLists();
            base.BoxDrawnConfirm(viewport);

            if (!nodes.Any()) return;
            State.Action = BoxAction.Drawn;
            SelectAction = SelectBoxAction.Selection;
            UpdateBoxBasedOnSelection();
        }

        public override void BoxDrawnCancel(ViewportBase viewport)
        {
            var transforming = SelectAction == SelectBoxAction.Resizing
                               || SelectAction == SelectBoxAction.Rotating
                               || SelectAction == SelectBoxAction.Shearing;
            if (transforming || !Selection.IsEmpty())
            {
                if (transforming) Document.EndSelectionTransform();
                if (State.ActiveViewport != null) State.ActiveViewport.Cursor = Cursors.Default;
                SelectAction = SelectBoxAction.Selection;
                State.ActiveViewport = null;
                State.Action = BoxAction.Drawn;
                UpdateBoxBasedOnSelection();
            }
            else
            {
                base.BoxDrawnCancel(viewport);
            }
        }

        protected override bool ShouldDrawBox()
        {
            return SelectAction != SelectBoxAction.Default
                   || base.ShouldDrawBox();
        }

        protected override Color GetRenderBoxColour()
        {
            if (SelectAction != SelectBoxAction.Default)
            {
                return Color.Blue;
            }
            return base.GetRenderBoxColour();
        }

        protected bool ShouldRenderRotateHandles(Viewport2D viewport)
        {
            return SelectAction != SelectBoxAction.Default
                   && SelectAction != SelectBoxAction.Resizing
                   && SelectAction != SelectBoxAction.Rotating
                   && SelectAction != SelectBoxAction.Shearing;
        }

        protected override bool ShouldRenderResizeBox(Viewport2D viewport)
        {
            return base.ShouldRenderResizeBox(viewport)
                && SelectAction != SelectBoxAction.ReadyToShear
                && SelectAction != SelectBoxAction.ReadyToRotate
                && SelectAction != SelectBoxAction.DownToShear
                && SelectAction != SelectBoxAction.DownToRotate
                && SelectAction != SelectBoxAction.Shearing
                && SelectAction != SelectBoxAction.Rotating;
        }

        private static Vector2d[] GetResizeAndSkewHandleCenters(Coordinate start, Coordinate end, double zoom, double offset = 7)
        {
            /*
            TopLeft,    Top,     TopRight,
            Left,       Center,  Right,
            BottomLeft, Bottom,  BottomRight
            */
            var half = (end - start) / 2;
            var dist = offset / zoom;
            return new[]
                       {
                           new Vector2d(start.DX - dist, end.DY + dist),         // Top Left
                           new Vector2d(start.DX + half.DX, end.DY + dist),      // Top
                           new Vector2d(end.DX + dist, end.DY + dist),           // Top Right
                           new Vector2d(start.DX - dist, start.DY + half.DY),    // Left
                           new Vector2d(start.DX + half.DX, start.DY + half.DY), // Center
                           new Vector2d(end.DX + dist, start.DY + half.DY),      // Right
                           new Vector2d(start.DX - dist, start.DY - dist),       // Bottom Left
                           new Vector2d(start.DX + half.DX, start.DY - dist),    // Bottom
                           new Vector2d(end.DX + dist, start.DY - dist),         // Bottom Right
                       };
        }

        protected void RenderRotateHandles(Viewport2D viewport, Coordinate start, Coordinate end)
        {
            var z = (double) viewport.Zoom;
            var centers = GetResizeAndSkewHandleCenters(start, end, z);

            GL.Color3(Color.White);
            for (var i = 0; i < centers.Length; i++)
            {
                var rz = (ResizeHandle) i;
                if (rz == ResizeHandle.Center) continue;
                var square = rz != ResizeHandle.BottomLeft && rz != ResizeHandle.BottomRight && rz != ResizeHandle.TopLeft && rz != ResizeHandle.TopRight;

                var v = centers[i];
                GL.Begin(BeginMode.Polygon);
                if (square) GLX.Square(v, 4, z, true);
                else GLX.Circle(v, 4, z, loop: true);
                GL.End();
            }

            GL.Color3(Color.Black);
            GL.Enable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.Begin(BeginMode.Lines);
            for (var i = 0; i < centers.Length; i++)
            {
                var rz = (ResizeHandle)i;
                if (rz == ResizeHandle.Center) continue;
                var square = rz != ResizeHandle.BottomLeft && rz != ResizeHandle.BottomRight && rz != ResizeHandle.TopLeft && rz != ResizeHandle.TopRight;
                if (square) continue;

                var v = centers[i];
                GLX.Circle(v, 4, z);
            }
            GL.End();
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Fastest);
            GL.Disable(EnableCap.LineSmooth);

            GL.Begin(BeginMode.Lines);
            for (var i = 0; i < centers.Length; i++)
            {
                var rz = (ResizeHandle)i;
                if (rz == ResizeHandle.Center) continue;
                var square = rz != ResizeHandle.BottomLeft && rz != ResizeHandle.BottomRight && rz != ResizeHandle.TopLeft && rz != ResizeHandle.TopRight;
                if (!square) continue;

                var v = centers[i];
                GLX.Square(v, 4, z);
            }
            GL.End();
        }

        protected override void Render2D(Viewport2D viewport)
        {
            if (State.Action == BoxAction.ReadyToDraw || State.Action == BoxAction.DownToDraw) return;
            //base.Render2D(viewport);
            var start = viewport.Flatten(State.BoxStart);
            var end = viewport.Flatten(State.BoxEnd);

            Matrix4d mat;
            GL.GetDouble(GetPName.ProjectionMatrix, out mat);
            var tform =  (SelectAction == SelectBoxAction.Resizing
                           || SelectAction == SelectBoxAction.Rotating
                           || SelectAction == SelectBoxAction.Shearing);
            if (viewport == State.ActiveViewport && tform && SelectAction != SelectBoxAction.Resizing)
            {
                start = viewport.Flatten(State.PreTransformBoxStart);
                end = viewport.Flatten(State.PreTransformBoxEnd);

                var dir = DisplayListGroup.GetMatrixFor(viewport.Direction);
                var inv = Matrix4d.Invert(dir);
                GL.MultMatrix(ref dir);
                GL.MultMatrix(ref _transform);
                GL.MultMatrix(ref inv);
            }

            if (ShouldDrawBox() && (viewport == State.ActiveViewport || !tform))
            {
                RenderBox(viewport, start, end);
            }
            if (ShouldRenderResizeBox(viewport))
            {
                RenderResizeBox(viewport, start, end);
            }
            GL.LoadMatrix(ref mat);

            if (ShouldRenderRotateHandles(viewport))
            {
                RenderRotateHandles(viewport, start, end);
            }
        }

        protected override void Render3D(Viewport3D viewport)
        {
            base.Render3D(viewport);
        }
    }
}
