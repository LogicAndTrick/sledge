using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Tools.SelectTool.TransformationHandles
{
    public class ResizeTransformHandle : BoxResizeHandle, ITransformationHandle
    {
        public string Name { get { return Handle == ResizeHandle.Center ? "Move" : "Resize"; } }

        public ResizeTransformHandle(BoxDraggableState state, ResizeHandle handle)
            : base(state, handle)
        {
        }

        public override bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            if (Handle == ResizeHandle.Center)
            {
                const int padding = 2;
                var box = new Box(viewport.Flatten(BoxState.Start), viewport.Flatten(BoxState.End));
                var c = position;
                return c.X >= box.Start.X - padding && c.Y >= box.Start.Y - padding && c.Z >= box.Start.Z - padding
                       && c.X <= box.End.X + padding && c.Y <= box.End.Y + padding && c.Z <= box.End.Z + padding;
            }
            return base.CanDrag(viewport, e, position);
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

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            if (Handle == ResizeHandle.Center)
            {
                if (HighlightedViewport != viewport) yield break;

                var b = new Box(viewport.Flatten(BoxState.Start), viewport.Flatten(BoxState.End));
                var st = b.Start.ToVector3();
                var en = b.End.ToVector3();
                var cam = viewport.Viewport.Camera;
                yield return new FaceElement(PositionType.World, Material.Flat(State.FillColour), new[]
                {
                    new PositionVertex(new Position(cam.Expand(new Vector3(st.X, st.Y, 0))), 0, 0),
                    new PositionVertex(new Position(cam.Expand(new Vector3(st.X, en.Y, 0))), 0, 0),
                    new PositionVertex(new Position(cam.Expand(new Vector3(en.X, en.Y, 0))), 0, 0),
                    new PositionVertex(new Position(cam.Expand(new Vector3(en.X, st.Y, 0))), 0, 0)
                }) { ZIndex = -20 };
                if (Handle == ResizeHandle.Center && SnappedMoveOrigin != null)
                {
                    const int size = 6;

                    yield return new LineElement(PositionType.World, Color.Yellow, new List<Position>
                    {
                        new Position(SnappedMoveOrigin.ToVector3()) {Offset = new Vector3(-size, size, 0)},
                        new Position(SnappedMoveOrigin.ToVector3()) {Offset = new Vector3(size, -size, 0)},
                    });
                    yield return new LineElement(PositionType.World, Color.Yellow, new List<Position>
                    {
                        new Position(SnappedMoveOrigin.ToVector3()) {Offset = new Vector3(size, size, 0)},
                        new Position(SnappedMoveOrigin.ToVector3()) {Offset = new Vector3(-size, -size, 0)},
                    });
                }
            }
            else
            {
                foreach (var e in base.GetViewportElements(viewport, camera))
                {
                    yield return e;
                }
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