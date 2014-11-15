using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Gui.Components;

namespace Sledge.EditorNew.Tools.DraggableTool
{
    public class BoxResizeHandle : IDraggable
    {
        protected BoxDraggableState State { get; set; }
        protected ResizeHandle Handle { get; set; }
        protected IViewport2D HighlightedViewport { get; set; }

        protected BoxState BoxState { get { return State.State; } }

        public BoxResizeHandle(BoxDraggableState state, ResizeHandle handle)
        {
            State = state;
            Handle = handle;
            HighlightedViewport = null;
        }

        protected virtual Box GetRectangle(IViewport2D viewport)
        {
            var start = viewport.Flatten(BoxState.Start);
            var end = viewport.Flatten(BoxState.End);
            var mid = (start + end) / 2;
            const int radius = 4;
            const int distance = radius + 4;
            Coordinate center = null;
            switch (Handle)
            {
                case ResizeHandle.TopLeft:
                    center = new Coordinate(start.X - distance, end.Y + distance, 0);
                    break;
                case ResizeHandle.Top:
                    center = new Coordinate(mid.X, end.Y + distance, 0);
                    break;
                case ResizeHandle.TopRight:
                    center = end + new Coordinate(distance, distance, 0);
                    break;
                case ResizeHandle.Left:
                    center = new Coordinate(start.X - distance, mid.Y, 0);
                    break;
                case ResizeHandle.Center:
                    center = mid;
                    break;
                case ResizeHandle.Right:
                    center = new Coordinate(end.X + distance, mid.Y, 0);
                    break;
                case ResizeHandle.BottomLeft:
                    center = start - new Coordinate(distance, distance, 0);
                    break;
                case ResizeHandle.Bottom:
                    center = new Coordinate(mid.X, start.Y - distance, 0);
                    break;
                case ResizeHandle.BottomRight:
                    center = new Coordinate(end.X + distance, start.Y - distance, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var offset = Coordinate.One * radius;
            return new Box(center - offset, center + offset);
        }

        public void Highlight(IViewport2D viewport)
        {
            HighlightedViewport = viewport;
            Cursor.SetCursor(viewport, Handle.GetCursorType());
        }

        public void Unhighlight(IViewport2D viewport)
        {
            HighlightedViewport = null;
            Cursor.SetCursor(viewport, CursorType.Default);
        }

        public bool CanDrag(IViewport2D viewport, ViewportEvent e)
        {
            var box = GetRectangle(viewport);
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            return box.CoordinateIsInside(point);
        }

        public void StartDrag(IViewport2D viewport, ViewportEvent e)
        {
            BoxState.Action = BoxAction.Resizing;
        }

        public void Drag(IViewport2D viewport, ViewportEvent e)
        {
            BoxState.Resize(Handle, viewport, e.DeltaX, -e.DeltaY);
        }

        public void EndDrag(IViewport2D viewport, ViewportEvent e)
        {
            BoxState.FixBounds();
            BoxState.Action = BoxAction.Drawn;
        }

        protected static void Coord(decimal x, decimal y, decimal z)
        {
            GL.Vertex3((double) x, (double) y, (double) z);
        }

        public virtual void Render(IViewport2D viewport)
        {
            var box = GetRectangle(viewport);

            GL.Begin(PrimitiveType.Quads);
            GL.Color4(HighlightedViewport == viewport ? Color.Aqua : Color.White);
            Coord(box.Start.X, box.Start.Y, 0);
            Coord(box.End.X, box.Start.Y, 0);
            Coord(box.End.X, box.End.Y, 0);
            Coord(box.Start.X, box.End.Y, 0);
            GL.End();

            GL.Begin(PrimitiveType.LineLoop);
            GL.Color4(Color.Black);
            Coord(box.Start.X, box.Start.Y, 0);
            Coord(box.End.X, box.Start.Y, 0);
            Coord(box.End.X, box.End.Y, 0);
            Coord(box.Start.X, box.End.Y, 0);
            GL.End();
        }
    }
}