using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenTK;
using Sledge.BspEditor.Rendering;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.BspEditor.Tools.Draggable
{
    public class BoxResizeHandle : BaseDraggable
    {
        public BoxDraggableState State { get; protected set; }
        public ResizeHandle Handle { get; protected set; }
        protected MapViewport HighlightedViewport { get; set; }

        protected BoxState BoxState { get { return State.State; } }

        public BoxResizeHandle(BoxDraggableState state, ResizeHandle handle)
        {
            State = state;
            Handle = handle;
            HighlightedViewport = null;
        }

        private Position GetPosition(MapViewport viewport)
        {
            const int distance = 6;
            var camera = viewport.Viewport.Camera;
            var start = camera.Flatten(BoxState.Start.ToVector3());
            var end = camera.Flatten(BoxState.End.ToVector3());
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
            return new Position(camera.Expand(center)) { Offset = offset };
        }

        protected virtual void SetCursorForHandle(MapViewport viewport, ResizeHandle handle)
        {
            viewport.Control.Cursor = handle.GetCursorType();
        }

        public override void Highlight(MapViewport viewport)
        {
            HighlightedViewport = viewport;
            SetCursorForHandle(viewport, Handle);
        }

        public override void Unhighlight(MapViewport viewport)
        {
            HighlightedViewport = null;
            viewport.Control.Cursor = Cursors.Default;
        }

        protected virtual Coordinate GetResizeOrigin(MapViewport viewport, Coordinate position)
        {
            var st = viewport.Flatten(BoxState.Start);
            var ed = viewport.Flatten(BoxState.End);
            return (st + ed) / 2;
        }

        protected Coordinate MoveOrigin;
        protected Coordinate SnappedMoveOrigin;

        public override void Click(MapViewport viewport, ViewportEvent e, Coordinate position)
        {

        }

        public override bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            const int width = 8;
            var pos = GetPosition(viewport);
            var screenPosition = viewport.ProperWorldToScreen(pos.Location.ToCoordinate()) + pos.Offset.ToCoordinate();
            var diff = (e.Location - screenPosition).Absolute();
            return diff.X < width && diff.Y < width;
        }

        public override void StartDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            BoxState.OrigStart = BoxState.Start;
            BoxState.OrigEnd = BoxState.End;
            MoveOrigin = GetResizeOrigin(viewport, position);
            SnappedMoveOrigin = MoveOrigin;
            BoxState.Action = BoxAction.Resizing;
            base.StartDrag(viewport, e, position);
        }

        public override void Drag(MapViewport viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
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
            base.Drag(viewport, e, lastPosition, position);
        }

        public override void EndDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            BoxState.FixBounds();
            BoxState.Action = BoxAction.Drawn;
            MoveOrigin = SnappedMoveOrigin = null;
            base.EndDrag(viewport, e, position);
        }

        public override IEnumerable<SceneObject> GetSceneObjects()
        {
            yield break;
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            yield break;
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            if (State.State.Action != BoxAction.Drawn) yield break;

            var pos = GetPosition(viewport);
            yield return new HandleElement(PositionType.World, HandleElement.HandleType.Square, pos, 4);
        }
    }
}