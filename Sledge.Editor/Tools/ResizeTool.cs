using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Properties;
using Sledge.UI;

namespace Sledge.Editor.Tools
{
    /// <summary>
    /// Allows the selected objects to be scaled and translated
    /// </summary>
    class ResizeTool : TransformationTool
    {
        public override Image GetIcon()
        {
            return Resources.Tool_Translate;
        }

        public override string GetName()
        {
            return "Resize Tool";
        }

        protected override bool RenderCircleHandles
        {
            get { return false; }
        }

        protected override bool AllowCenterHandle
        {
            get { return true; }
        }

        protected override bool FilterHandle(ResizeHandle handle)
        {
            return true;
        }

        protected override string GetTransformName()
        {
            return "Resize";
        }

        protected override Matrix4d? GetTransformationMatrix(Viewport2D viewport, MouseEventArgs e)
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
    }
}
