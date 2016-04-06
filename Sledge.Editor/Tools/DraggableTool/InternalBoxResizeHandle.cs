using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Tools.DraggableTool
{
    public class InternalBoxResizeHandle : BoxResizeHandle
    {
        public InternalBoxResizeHandle(BoxDraggableState state, ResizeHandle handle) : base(state, handle)
        {
        }

        private Box GetRectangle(MapViewport viewport)
        {
            var start = viewport.Flatten(BoxState.Start);
            var end = viewport.Flatten(BoxState.End);
            var box = new Box(start, end);
            var wid = Math.Min(box.Width / 10, viewport.PixelsToUnits(20));
            var len = Math.Min(box.Length / 10, viewport.PixelsToUnits(20));
            switch (Handle)
            {
                case ResizeHandle.TopLeft:
                    return new Box(new Coordinate(start.X, end.Y - len, 0), new Coordinate(start.X + wid, end.Y, 0));
                case ResizeHandle.Top:
                    return new Box(new Coordinate(start.X, end.Y - len, 0), end);
                case ResizeHandle.TopRight:
                    return new Box(new Coordinate(end.X - wid, end.Y - len, 0), new Coordinate(end.X, end.Y, 0));
                case ResizeHandle.Left:
                    return new Box(start, new Coordinate(start.X + wid, end.Y, 0));
                case ResizeHandle.Center:
                    return box;
                case ResizeHandle.Right:
                    return new Box(new Coordinate(end.X - wid, start.Y, 0), end);
                case ResizeHandle.BottomLeft:
                    return new Box(new Coordinate(start.X, start.Y, 0), new Coordinate(start.X + wid, start.Y + len, 0));
                case ResizeHandle.Bottom:
                    return new Box(start, new Coordinate(end.X, start.Y + len, 0));
                case ResizeHandle.BottomRight:
                    return new Box(new Coordinate(end.X - wid, start.Y, 0), new Coordinate(end.X, start.Y + len, 0));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            const int padding = 2;
            var box = GetRectangle(viewport);
            var c = position;
            return c.X >= box.Start.X - padding && c.Y >= box.Start.Y - padding && c.Z >= box.Start.Z - padding
                   && c.X <= box.End.X + padding && c.Y <= box.End.Y + padding && c.Z <= box.End.Z + padding;
        
        }

        protected override Coordinate GetResizeOrigin(MapViewport viewport, Coordinate position)
        {
            var st = viewport.Flatten(BoxState.Start);
            var ed = viewport.Flatten(BoxState.End);
            var points = new[] { st, ed, new Coordinate(st.X, ed.Y, 0), new Coordinate(ed.X, st.Y, 0) };
            return points.OrderBy(x => (position - x).LengthSquared()).First();
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            if (HighlightedViewport != viewport) yield break;

            var b = GetRectangle(viewport);
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
                var orig = viewport.Expand(SnappedMoveOrigin).ToVector3();

                yield return new LineElement(PositionType.World, Color.Yellow, new List<Position>
                {
                    new Position(orig) {Offset = new Vector3(-size, size, 0)},
                    new Position(orig) {Offset = new Vector3(size, -size, 0)},
                });
                yield return new LineElement(PositionType.World, Color.Yellow, new List<Position>
                {
                    new Position(orig) {Offset = new Vector3(size, size, 0)},
                    new Position(orig) {Offset = new Vector3(-size, -size, 0)},
                });
            }
        }
    }
}