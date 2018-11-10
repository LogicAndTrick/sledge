using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
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

        public override Vector3 Origin => (BoxState.Start + BoxState.End) / 2;

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

        public override void Highlight(MapDocument document, MapViewport viewport)
        {
            HighlightedViewport = viewport.Viewport;
            SetCursorForHandle(viewport, Handle);
        }

        public override void Unhighlight(MapDocument document, MapViewport viewport)
        {
            HighlightedViewport = null;
            viewport.Control.Cursor = Cursors.Default;
        }

        protected virtual Vector3 GetResizeOrigin(MapViewport viewport, OrthographicCamera camera, Vector3 position)
        {
            var st = camera.Flatten(BoxState.Start);
            var ed = camera.Flatten(BoxState.End);
            return (st + ed) / 2;
        }

        protected Vector3? MoveOrigin;
        protected Vector3? SnappedMoveOrigin;

        public void SetMoveOrigin(Vector3 origin)
        {
            MoveOrigin = origin;
            SnappedMoveOrigin = origin;
        }

        public override void Click(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
        {

        }

        public override bool CanDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
        {
            const int width = 8;
            var pos = GetWorldPositionAndScreenOffset(viewport.Viewport.Camera);
            var screenPosition = camera.WorldToScreen(pos.Item1) + pos.Item2;
            var diff = Vector3.Abs(e.Location - screenPosition);
            return diff.X < width && diff.Y < width;
        }

        public override void StartDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
        {
            BoxState.OrigStart = BoxState.Start;
            BoxState.OrigEnd = BoxState.End;
            if (!MoveOrigin.HasValue) MoveOrigin = GetResizeOrigin(viewport, camera, position);
            if (!SnappedMoveOrigin.HasValue) SnappedMoveOrigin = MoveOrigin;
            BoxState.Action = BoxAction.Resizing;
            base.StartDrag(document, viewport, camera, e, position);
        }

        public override void Drag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position)
        {
            if (Handle == ResizeHandle.Center)
            {
                var delta = position - lastPosition;
                var newOrigin = MoveOrigin + delta;
                var snapped = State.Tool.SnapIfNeeded(newOrigin.Value);
                BoxState.Move(camera, snapped - SnappedMoveOrigin.Value);
                SnappedMoveOrigin = snapped;
                MoveOrigin = newOrigin;
            }
            else
            {
                var snapped = State.Tool.SnapIfNeeded(position);
                BoxState.Resize(Handle, viewport, camera, snapped);
            }
            base.Drag(document, viewport, camera, e, lastPosition, position);
        }

        public override void EndDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
        {
            BoxState.FixBounds();
            BoxState.Action = BoxAction.Drawn;
            MoveOrigin = SnappedMoveOrigin = null;
            base.EndDrag(document, viewport, camera, e, position);
        }

        public override void Render(MapDocument document, BufferBuilder builder)
        {
            //
        }

        public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
        {
            if (State.State.Action != BoxAction.Drawn) return;

            var (wpos, soff) = GetWorldPositionAndScreenOffset(camera);
            var spos = camera.WorldToScreen(wpos) + soff;

            const int size = 4;
            
            im.AddRectOutlineOpaque(new Vector2(spos.X - size, spos.Y - size), new Vector2(spos.X + size, spos.Y + size), Color.Black, Color.White);
        }

        public override void Render(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
        {
            //
        }
    }
}