using System;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.Documents;
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

        protected virtual Coordinate GetResizeOrigin(IViewport2D viewport, Coordinate position)
        {
            var st = viewport.Flatten(BoxState.Start);
            var ed = viewport.Flatten(BoxState.End);
            return (st + ed) / 2;
        }

        protected Coordinate MoveOrigin;
        protected Coordinate SnappedMoveOrigin;

        public virtual void Click(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {

        }

        public bool CanDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {
            var box = GetRectangle(viewport);
            return box.CoordinateIsInside(position);
        }

        public void StartDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {
            BoxState.Action = BoxAction.Resizing;
            MoveOrigin = GetResizeOrigin(viewport, position);
            SnappedMoveOrigin = MoveOrigin;
        }

        public void Drag(IViewport2D viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {
            if (Handle == ResizeHandle.Center)
            {
                var delta = position - lastPosition;
                var newOrigin = MoveOrigin + delta;
                var snapped = State.Tool.SnapIfNeeded(newOrigin);
                BoxState.Move(viewport, snapped - SnappedMoveOrigin);
                SnappedMoveOrigin = snapped;
                MoveOrigin = newOrigin;
            }
            else
            {
                var snapped = State.Tool.SnapIfNeeded(position);
                BoxState.Resize(Handle, viewport, snapped);
            }
        }

        public void EndDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {
            BoxState.FixBounds();
            BoxState.Action = BoxAction.Drawn;
            MoveOrigin = SnappedMoveOrigin = null;
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