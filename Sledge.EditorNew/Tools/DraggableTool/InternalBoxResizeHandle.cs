using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.UI.Viewports;

namespace Sledge.EditorNew.Tools.DraggableTool
{
    public class InternalBoxResizeHandle : BoxResizeHandle
    {
        public InternalBoxResizeHandle(BoxDraggableState state, ResizeHandle handle) : base(state, handle)
        {
        }

        protected override Box GetRectangle(IViewport2D viewport)
        {
            var start = viewport.Flatten(BoxState.Start);
            var end = viewport.Flatten(BoxState.End);
            var box = new Box(start, end);
            var wid = Math.Min(box.Width / 10, 20 / viewport.Zoom);
            var len = Math.Min(box.Length / 10, 20 / viewport.Zoom);
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
            throw new NotImplementedException();
        }

        public override void Render(IViewport2D viewport)
        {
            if (HighlightedViewport != viewport) return;

            var box = GetRectangle(viewport);

            GL.Begin(PrimitiveType.Quads);
            GL.Color4(Color.FromArgb(64, Color.Purple));
            Coord(box.Start.X, box.Start.Y, 0);
            Coord(box.End.X, box.Start.Y, 0);
            Coord(box.End.X, box.End.Y, 0);
            Coord(box.Start.X, box.End.Y, 0);
            GL.End();
        }
    }
}