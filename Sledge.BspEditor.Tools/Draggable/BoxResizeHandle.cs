using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Tools.Draggable
{
    public class BoxResizeHandle : BaseDraggable
    {
        public BoxDraggableState State { get; protected set; }
        public ResizeHandle Handle { get; protected set; }
        protected IViewport HighlightedViewport { get; set; }

        protected BoxState BoxState { get { return State.State; } }

        public BoxResizeHandle(BoxDraggableState state, ResizeHandle handle)
        {
            State = state;
            Handle = handle;
            HighlightedViewport = null;
        }

        protected (Vector3, Vector3) GetWorldPositionAndScreenOffset(ICamera camera)
        {
            const int distance = 6;
            var start = camera.Flatten(BoxState.Start);
            var end = camera.Flatten(BoxState.End);
            var mid = (start + end) / 2;
            Vector3 center;
            Vector3 offset;
            switch (Handle)
            {
                case ResizeHandle.TopLeft:
                    center = new Vector3(start.X, end.Y, 0);
                    offset = new Vector3(-distance, -distance, 0);
                    break;
                case ResizeHandle.Top:
                    center = new Vector3(mid.X, end.Y, 0);
                    offset = new Vector3(0, -distance, 0);
                    break;
                case ResizeHandle.TopRight:
                    center = new Vector3(end.X, end.Y, 0);
                    offset = new Vector3(distance, -distance, 0);
                    break;
                case ResizeHandle.Left:
                    center = new Vector3(start.X, mid.Y, 0);
                    offset = new Vector3(-distance, 0, 0);
                    break;
                case ResizeHandle.Center:
                    center = mid;
                    offset = Vector3.Zero;
                    break;
                case ResizeHandle.Right:
                    center = new Vector3(end.X, mid.Y, 0);
                    offset = new Vector3(distance, 0, 0);
                    break;
                case ResizeHandle.BottomLeft:
                    center = new Vector3(start.X, start.Y, 0);
                    offset = new Vector3(-distance, distance, 0);
                    break;
                case ResizeHandle.Bottom:
                    center = new Vector3(mid.X, start.Y, 0);
                    offset = new Vector3(0, distance, 0);
                    break;
                case ResizeHandle.BottomRight:
                    center = new Vector3(end.X, start.Y, 0);
                    offset = new Vector3(distance, distance, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return (camera.Expand(center), offset);
        }

        protected virtual void SetCursorForHandle(MapViewport viewport, ResizeHandle handle)
        {
            viewport.Control.Cursor = handle.GetCursorType();
        }

        public override void Highlight(MapViewport viewport)
        {
            HighlightedViewport = viewport.Viewport;
            SetCursorForHandle(viewport, Handle);
        }

        public override void Unhighlight(MapViewport viewport)
        {
            HighlightedViewport = null;
            viewport.Control.Cursor = Cursors.Default;
        }

        protected virtual Vector3 GetResizeOrigin(MapViewport viewport, Vector3 position)
        {
            var st = viewport.Flatten(BoxState.Start);
            var ed = viewport.Flatten(BoxState.End);
            return (st + ed) / 2;
        }

        protected Vector3? MoveOrigin;
        protected Vector3? SnappedMoveOrigin;

        public override void Click(MapViewport viewport, ViewportEvent e, Vector3 position)
        {

        }

        public override bool CanDrag(MapViewport viewport, ViewportEvent e, Vector3 position)
        {
            const int width = 8;
            var pos = GetWorldPositionAndScreenOffset(viewport.Viewport.Camera);
            var screenPosition = viewport.WorldToScreen(pos.Item1) + pos.Item2;
            var diff = Vector3.Abs(e.Location - screenPosition);
            return diff.X < width && diff.Y < width;
        }

        public override void StartDrag(MapViewport viewport, ViewportEvent e, Vector3 position)
        {
            BoxState.OrigStart = BoxState.Start;
            BoxState.OrigEnd = BoxState.End;
            MoveOrigin = GetResizeOrigin(viewport, position);
            SnappedMoveOrigin = MoveOrigin;
            BoxState.Action = BoxAction.Resizing;
            base.StartDrag(viewport, e, position);
        }

        public override void Drag(MapViewport viewport, ViewportEvent e, Vector3 lastPosition, Vector3 position)
        {
            if (Handle == ResizeHandle.Center)
            {
                var delta = position - lastPosition;
                var newOrigin = MoveOrigin + delta;
                var snapped = State.Tool.SnapIfNeeded(newOrigin.Value);
                BoxState.Move(viewport, snapped - SnappedMoveOrigin.Value);
                SnappedMoveOrigin = snapped;
                MoveOrigin = newOrigin;
            }
            else
            {
                var snapped = State.Tool.SnapIfNeeded(position);
                BoxState.Resize(Handle, viewport, snapped);
            }
            base.Drag(viewport, e, lastPosition, position);
        }

        public override void EndDrag(MapViewport viewport, ViewportEvent e, Vector3 position)
        {
            BoxState.FixBounds();
            BoxState.Action = BoxAction.Drawn;
            MoveOrigin = SnappedMoveOrigin = null;
            base.EndDrag(viewport, e, position);
        }

        public override void Render(BufferBuilder builder)
        {
            //
        }

        public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics)
        {
            if (State.State.Action != BoxAction.Drawn) return;

            var (wpos, soff) = GetWorldPositionAndScreenOffset(camera);
            var spos = camera.WorldToScreen(wpos) + soff;

            const int size = 8;
            var rect = new Rectangle((int) spos.X - size / 2, (int) spos.Y - size / 2, size, size);

            graphics.FillRectangle(Brushes.White, rect);
            graphics.DrawRectangle(Pens.Black, rect);
        }

        public override void Render(IViewport viewport, PerspectiveCamera camera, Graphics graphics)
        {
            //
        }
    }
}