using System;
using System.Windows.Forms;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Documents;
using Sledge.UI;

namespace Sledge.Editor.Tools.TransformationTools
{
    /// <summary>
    /// Allows the selected objects to be scaled and translated
    /// </summary>
    class ResizeTool : TransformationTool
    {

        public override bool RenderCircleHandles
        {
            get { return false; }
        }

        public override bool FilterHandle(BaseBoxTool.ResizeHandle handle)
        {
            return true;
        }

        public override string GetTransformName()
        {
            return "Resize";
        }

        public override Cursor CursorForHandle(BaseBoxTool.ResizeHandle handle)
        {
            return null;
        }

        public override Matrix4? GetTransformationMatrix(Viewport2D viewport, ViewportEvent e, BaseBoxTool.BoxState state, Document doc)
        {
            var coords = GetBoxCoordinatesForSelectionResize(viewport, e, state, doc);
            state.BoxStart = coords.Item1;
            state.BoxEnd = coords.Item2;
            Matrix4 resizeMatrix;
            if (state.Handle == BaseBoxTool.ResizeHandle.Center)
            {
                var movement = state.BoxStart - state.PreTransformBoxStart;
                resizeMatrix = Matrix4.CreateTranslation((float)movement.X, (float)movement.Y, (float)movement.Z);
            }
            else
            {
                var resize = (state.PreTransformBoxStart - state.BoxStart) +
                             (state.BoxEnd - state.PreTransformBoxEnd);
                resize = resize.ComponentDivide(state.PreTransformBoxEnd - state.PreTransformBoxStart);
                resize += new Coordinate(1, 1, 1);
                var offset = -GetOriginForTransform(viewport, state);
                var trans = Matrix4.CreateTranslation((float)offset.X, (float)offset.Y, (float)offset.Z);
                var scale = Matrix4.Mult(trans, Matrix4.Scale((float)resize.X, (float)resize.Y, (float)resize.Z));
                resizeMatrix = Matrix4.Mult(scale, Matrix4.Invert(trans));
            }
            return resizeMatrix;
        }

        private Tuple<Coordinate, Coordinate> GetBoxCoordinatesForSelectionResize(Viewport2D viewport, ViewportEvent e, BaseBoxTool.BoxState state, Document document)
        {
            if (state.Action != BaseBoxTool.BoxAction.Resizing) return Tuple.Create(state.BoxStart, state.BoxEnd);
            var now = SnapIfNeeded(viewport.ScreenToWorld(e.X, viewport.Height - e.Y), document);
            var cstart = viewport.Flatten(state.BoxStart);
            var cend = viewport.Flatten(state.BoxEnd);
            switch (state.Handle)
            {
                case BaseBoxTool.ResizeHandle.TopLeft:
                    cstart.X = Math.Min(now.X, cend.X - 1);
                    cend.Y = Math.Max(now.Y, cstart.Y + 1);
                    break;
                case BaseBoxTool.ResizeHandle.Top:
                    cend.Y = Math.Max(now.Y, cstart.Y + 1);
                    break;
                case BaseBoxTool.ResizeHandle.TopRight:
                    cend.X = Math.Max(now.X, cstart.X + 1);
                    cend.Y = Math.Max(now.Y, cstart.Y + 1);
                    break;
                case BaseBoxTool.ResizeHandle.Left:
                    cstart.X = Math.Min(now.X, cend.X - 1);
                    break;
                case BaseBoxTool.ResizeHandle.Center:
                    var cdiff = cend - cstart;
                    cstart = viewport.Flatten(state.PreTransformBoxStart) + now - SnapIfNeeded(state.MoveStart, document);
                    cend = cstart + cdiff;
                    break;
                case BaseBoxTool.ResizeHandle.Right:
                    cend.X = Math.Max(now.X, cstart.X + 1);
                    break;
                case BaseBoxTool.ResizeHandle.BottomLeft:
                    cstart.X = Math.Min(now.X, cend.X - 1);
                    cstart.Y = Math.Min(now.Y, cend.Y - 1);
                    break;
                case BaseBoxTool.ResizeHandle.Bottom:
                    cstart.Y = Math.Min(now.Y, cend.Y - 1);
                    break;
                case BaseBoxTool.ResizeHandle.BottomRight:
                    cend.X = Math.Max(now.X, cstart.X + 1);
                    cstart.Y = Math.Min(now.Y, cend.Y - 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return SnapBoxCoordinatesIfNeeded(viewport, state, document, cstart, cend);
        }

        private Tuple<Coordinate, Coordinate> SnapBoxCoordinatesIfNeeded(Viewport2D viewport, BaseBoxTool.BoxState state, Document document, Coordinate start, Coordinate end)
        {
            if (state.Action == BaseBoxTool.BoxAction.Resizing && state.Handle == BaseBoxTool.ResizeHandle.Center)
            {
                // Pick the corner to snap
                var ms = state.MoveStart;
                var pts = viewport.Flatten(state.PreTransformBoxStart);
                var pte = viewport.Flatten(state.PreTransformBoxEnd);
                var ss = SnapIfNeeded(start, document);
                var se = SnapIfNeeded(end, document);
                var middle = (pts + pte) / 2;
                var delta = ss - start;
                if (ms.Y > middle.Y)
                {
                    // Top
                    delta.Y = se.Y - end.Y;
                }
                if (ms.X > middle.X)
                {
                    // Right
                    delta.X = se.X - end.X;
                }
                start += delta;
                end += delta;
            }

            var cstart = viewport.Expand(start) + viewport.GetUnusedCoordinate(state.BoxStart);
            var cend = viewport.Expand(end) + viewport.GetUnusedCoordinate(state.BoxEnd);
            return Tuple.Create(cstart, cend);
        }

        private static Coordinate GetOriginForTransform(Viewport2D viewport, BaseBoxTool.BoxState state)
        {
            decimal x = 0;
            decimal y = 0;
            var cstart = viewport.Flatten(state.PreTransformBoxStart);
            var cend = viewport.Flatten(state.PreTransformBoxEnd);
            switch (state.Handle)
            {
                case BaseBoxTool.ResizeHandle.TopLeft:
                case BaseBoxTool.ResizeHandle.Top:
                case BaseBoxTool.ResizeHandle.TopRight:
                case BaseBoxTool.ResizeHandle.Left:
                case BaseBoxTool.ResizeHandle.Right:
                    y = cstart.Y;
                    break;
                case BaseBoxTool.ResizeHandle.BottomLeft:
                case BaseBoxTool.ResizeHandle.Bottom:
                case BaseBoxTool.ResizeHandle.BottomRight:
                    y = cend.Y;
                    break;
            }
            switch (state.Handle)
            {
                case BaseBoxTool.ResizeHandle.Top:
                case BaseBoxTool.ResizeHandle.TopRight:
                case BaseBoxTool.ResizeHandle.Right:
                case BaseBoxTool.ResizeHandle.BottomRight:
                case BaseBoxTool.ResizeHandle.Bottom:
                    x = cstart.X;
                    break;
                case BaseBoxTool.ResizeHandle.TopLeft:
                case BaseBoxTool.ResizeHandle.Left:
                case BaseBoxTool.ResizeHandle.BottomLeft:
                    x = cend.X;
                    break;
            }
            return viewport.Expand(new Coordinate(x, y, 0));
        }
    }
}
