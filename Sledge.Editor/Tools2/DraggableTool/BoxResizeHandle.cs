using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Tools2.DraggableTool
{
    public class BoxResizeHandle : IDraggable
    {
        protected BoxDraggableState State { get; set; }
        public ResizeHandle Handle { get; protected set; }
        protected MapViewport HighlightedViewport { get; set; }

        protected BoxState BoxState { get { return State.State; } }

        public BoxResizeHandle(BoxDraggableState state, ResizeHandle handle)
        {
            State = state;
            Handle = handle;
            HighlightedViewport = null;
        }

        protected virtual Box GetRectangle(MapViewport viewport, OrthographicCamera camera)
        {
            var start = viewport.Flatten(BoxState.Start);
            var end = viewport.Flatten(BoxState.End);
            var mid = (start + end) / 2;
            var radius = 4 / (decimal)viewport.Zoom;
            var distance = 6 / (decimal)viewport.Zoom;
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

        protected Position GetPosition(MapViewport viewport, OrthographicCamera camera)
        {
            const int distance = 6;
            var start = camera.Flatten(BoxState.Start.ToVector3());
            var end = camera.Flatten(BoxState.End.ToVector3());
            var mid = (start + end) / 2;
            Vector3 center;
            Vector3 offset;
            switch (Handle)
            {
                case ResizeHandle.TopLeft:
                    center = new Vector3(start.X, end.Y, 0);
                    offset = new Vector3(-distance, distance, 0);
                    break;
                case ResizeHandle.Top:
                    center = new Vector3(mid.X, end.Y, 0);
                    offset = new Vector3(0, distance, 0);
                    break;
                case ResizeHandle.TopRight:
                    center = new Vector3(end.X, end.Y, 0);
                    offset = new Vector3(distance, distance, 0);
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
                    offset = new Vector3(-distance, -distance, 0);
                    break;
                case ResizeHandle.Bottom:
                    center = new Vector3(mid.X, start.Y, 0);
                    offset = new Vector3(0, -distance, 0);
                    break;
                case ResizeHandle.BottomRight:
                    center = new Vector3(end.X, start.Y, 0);
                    offset = new Vector3(distance, -distance, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new Position(PositionType.World, camera.Expand(center)) { Offset = offset };
        }

        protected virtual void SetCursorForHandle(MapViewport viewport, ResizeHandle handle)
        {
            viewport.Control.Cursor = handle.GetCursorType();
        }

        public void Highlight(MapViewport viewport, OrthographicCamera camera)
        {
            HighlightedViewport = viewport;
            SetCursorForHandle(viewport, Handle);
        }

        public void Unhighlight(MapViewport viewport, OrthographicCamera camera)
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

        public virtual void Click(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position)
        {

        }

        public virtual bool CanDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position)
        {
            const int padding = 2;
            var box = GetRectangle(viewport, camera);
            var c = position;
            return c.X >= box.Start.X - padding && c.Y >= box.Start.Y - padding && c.Z >= box.Start.Z - padding
                   && c.X <= box.End.X + padding && c.Y <= box.End.Y + padding && c.Z <= box.End.Z + padding;
        }

        public virtual void StartDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position)
        {
            BoxState.Action = BoxAction.Resizing;
            BoxState.OrigStart = BoxState.Start;
            BoxState.OrigEnd = BoxState.End;
            MoveOrigin = GetResizeOrigin(viewport, position);
            SnappedMoveOrigin = MoveOrigin;
        }

        public virtual void Drag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate lastPosition, Coordinate position)
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

        public virtual void EndDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position)
        {
            BoxState.FixBounds();
            BoxState.Action = BoxAction.Drawn;
            MoveOrigin = SnappedMoveOrigin = null;
        }

        protected static void Coord(decimal x, decimal y, decimal z)
        {
            GL.Vertex3((double) x, (double) y, (double) z);
        }

        public virtual IEnumerable<SceneObject> GetSceneObjects()
        {
            yield break;
        }

        public virtual IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            yield break;
        }

        public virtual IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            var pos = GetPosition(viewport, camera);
            yield return new HandleElement(HandleElement.HandleType.Square, pos, 4);
        }

        public virtual void Render(MapViewport viewport, OrthographicCamera camera)
        {
            var box = GetRectangle(viewport, camera);

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