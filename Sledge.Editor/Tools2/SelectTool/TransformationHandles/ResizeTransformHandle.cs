using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Documents;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools2.DraggableTool;
using Sledge.Rendering.Cameras;

namespace Sledge.Editor.Tools2.SelectTool.TransformationHandles
{
    public class ResizeTransformHandle : BoxResizeHandle, ITransformationHandle
    {
        public string Name { get { return Handle == ResizeHandle.Center ? "Move" : "Resize"; } }

        public ResizeTransformHandle(BoxDraggableState state, ResizeHandle handle)
            : base(state, handle)
        {
        }

        protected override Box GetRectangle(MapViewport viewport, OrthographicCamera camera)
        {
            if (Handle == ResizeHandle.Center)
            {
                var start = viewport.Flatten(BoxState.Start);
                var end = viewport.Flatten(BoxState.End);
                return new Box(start, end);
            }
            return base.GetRectangle(viewport, camera);
        }

        protected override Coordinate GetResizeOrigin(MapViewport viewport, Coordinate position)
        {
            if (Handle == ResizeHandle.Center)
            {
                var st = viewport.Flatten(BoxState.Start);
                var ed = viewport.Flatten(BoxState.End);
                var points = new[] {st, ed, new Coordinate(st.X, ed.Y, 0), new Coordinate(ed.X, st.Y, 0)};
                return points.OrderBy(x => (position - x).LengthSquared()).First();
            }
            return base.GetResizeOrigin(viewport, position);
        }

        public override void Render(MapViewport viewport, OrthographicCamera camera)
        {
            if (Handle == ResizeHandle.Center)
            {

                if (HighlightedViewport != viewport) return;

                var box = GetRectangle(viewport, camera);

                GL.Begin(PrimitiveType.Quads);
                GL.Color4(State.FillColour);
                Coord(box.Start.X, box.Start.Y, 0);
                Coord(box.End.X, box.Start.Y, 0);
                Coord(box.End.X, box.End.Y, 0);
                Coord(box.Start.X, box.End.Y, 0);
                GL.End();

                if (Handle == ResizeHandle.Center && SnappedMoveOrigin != null)
                {
                    const int size = 6;
                    var dist = size / (decimal)viewport.Zoom;

                    var origin = SnappedMoveOrigin;
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color4(Color.Yellow);
                    Coord(origin.X - dist, origin.Y + dist, 0);
                    Coord(origin.X + dist, origin.Y - dist, 0);
                    Coord(origin.X + dist, origin.Y + dist, 0);
                    Coord(origin.X - dist, origin.Y - dist, 0);
                    GL.End();
                }
            }
            else
            {
                base.Render(viewport, camera);
            }
        }

        public Matrix4? GetTransformationMatrix(MapViewport viewport, OrthographicCamera camera, BoxState state, Document doc)
        {
            Matrix4 resizeMatrix;
            if (Handle == ResizeHandle.Center)
            {
                var movement = state.Start - state.OrigStart;
                resizeMatrix = Matrix4.CreateTranslation((float)movement.X, (float)movement.Y, (float)movement.Z);
            }
            else
            {
                var resize = (state.OrigStart - state.Start) +
                             (state.End - state.OrigEnd);
                resize = resize.ComponentDivide(state.OrigEnd - state.OrigStart);
                resize += new Coordinate(1, 1, 1);
                var offset = -GetOriginForTransform(viewport, camera, state);
                var trans = Matrix4.CreateTranslation((float)offset.X, (float)offset.Y, (float)offset.Z);
                var scale = Matrix4.Mult(trans, Matrix4.CreateScale((float)resize.X, (float)resize.Y, (float)resize.Z));
                resizeMatrix = Matrix4.Mult(scale, Matrix4.Invert(trans));
            }
            return resizeMatrix;
        }

        private Coordinate GetOriginForTransform(MapViewport viewport, OrthographicCamera camera, BoxState state)
        {
            decimal x = 0;
            decimal y = 0;
            var cstart = viewport.Flatten(state.OrigStart);
            var cend = viewport.Flatten(state.OrigEnd);
            switch (Handle)
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
            switch (Handle)
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