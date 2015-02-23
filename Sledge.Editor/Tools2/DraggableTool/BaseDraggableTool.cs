using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Input;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Tools2.DraggableTool
{
    public abstract class BaseDraggableTool : BaseTool
    {
        public List<IDraggableState> States { get; set; }

        public IDraggable CurrentDraggable { get; private set; }
        private ViewportEvent _lastDragMoveEvent = null;
        private Coordinate _lastDragPoint = null;

        protected BaseDraggableTool()
        {
            States = new List<IDraggableState>();
        }

        #region Virtual events
        protected virtual void OnDraggableClicked(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position, IDraggable draggable)
        {

        }

        protected virtual void OnDraggableDragStarted(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position, IDraggable draggable)
        {

        }

        protected virtual void OnDraggableDragMoving(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate previousPosition, Coordinate position, IDraggable draggable)
        {

        }

        protected virtual void OnDraggableDragMoved(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate previousPosition, Coordinate position, IDraggable draggable)
        {

        }

        protected virtual void OnDraggableDragEnded(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position, IDraggable draggable)
        {

        }
        #endregion

        protected override void MouseClick(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Dragging || e.Button != MouseButtons.Left) return;
            if (CurrentDraggable == null) return;
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            OnDraggableClicked(viewport, camera, e, point, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.Click(viewport, e, point);
        }

        protected override void MouseMove(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Dragging || e.Button == MouseButtons.Left) return;
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            IDraggable drag = null;
            foreach (var state in States)
            {
                var drags = state.GetDraggables().ToList();
                drags.Add(state);
                foreach (var draggable in drags)
                {
                    if (draggable.CanDrag(viewport, e, point))
                    {
                        drag = draggable;
                        break;
                    }
                }
                if (drag != null) break;
            }
            if (drag != CurrentDraggable)
            {
                if (CurrentDraggable != null) CurrentDraggable.Unhighlight(viewport);
                CurrentDraggable = drag;
                if (CurrentDraggable != null) CurrentDraggable.Highlight(viewport);
            }
        }

        protected override void DragStart(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (CurrentDraggable == null) return;
            _lastDragPoint = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            OnDraggableDragStarted(viewport, camera, e, _lastDragPoint, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.StartDrag(viewport, e, _lastDragPoint);
            _lastDragMoveEvent = e;
        }

        protected override void DragMove(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (CurrentDraggable == null) return;
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            OnDraggableDragMoving(viewport, camera, e, _lastDragPoint, point, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.Drag(viewport, e, _lastDragPoint, point);
            if (!e.Handled) OnDraggableDragMoved(viewport, camera, e, _lastDragPoint, point, CurrentDraggable);
            _lastDragPoint = point;
            _lastDragMoveEvent = e;
        }

        protected override void DragEnd(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (CurrentDraggable == null) return;
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            OnDraggableDragEnded(viewport, camera, e, point, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.EndDrag(viewport, e, point);
            _lastDragMoveEvent = null;
            _lastDragPoint = null;
        }

        public override void PositionChanged(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (viewport.Is2D && _lastDragMoveEvent != null && CurrentDraggable != null && _lastDragMoveEvent.Sender == viewport)
            {
                var point = viewport.ScreenToWorld(_lastDragMoveEvent.X, viewport.Height - _lastDragMoveEvent.Y);
                var ev = new ViewportEvent(viewport)
                {
                    Dragging = true,
                    Button = _lastDragMoveEvent.Button,
                    StartX = _lastDragMoveEvent.StartX,
                    StartY = _lastDragMoveEvent.StartY
                };
                ev.X = ev.LastX = _lastDragMoveEvent.X;
                ev.Y = ev.LastY = _lastDragMoveEvent.Y;
                
                OnDraggableDragMoving(viewport, camera, ev, _lastDragPoint, point, CurrentDraggable);
                if (!ev.Handled) CurrentDraggable.Drag(viewport, ev, _lastDragPoint, point);
                if (!ev.Handled) OnDraggableDragMoved(viewport, camera, ev, _lastDragPoint, point, CurrentDraggable);
                _lastDragPoint = point;
            }
            base.PositionChanged(viewport, e);
        }

        private IEnumerable<T> CollectObjects<T>(Func<IDraggable, IEnumerable<T>> collector)
        {
            var list = new List<T>();

            var foundActive = false;
            foreach (var state in States)
            {
                foreach (var draggable in state.GetDraggables())
                {
                    if (draggable == CurrentDraggable) foundActive = true;
                    else list.AddRange(collector(draggable));
                }
                if (state == CurrentDraggable) foundActive = true;
                else list.AddRange(collector(state));
            }
            if (CurrentDraggable != null && foundActive) list.AddRange(collector(CurrentDraggable));

            return list;
        }

        protected override IEnumerable<SceneObject> GetSceneObjects()
        {
            return CollectObjects(x => x.GetSceneObjects());
        }

        protected override IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            return CollectObjects(x => x.GetViewportElements(viewport, camera));
        }

        protected override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            return CollectObjects(x => x.GetViewportElements(viewport, camera));
        }

        protected bool GetSelectionBox(BoxState state, out Box boundingbox)
        {
            // If one of the dimensions has a depth value of 0, extend it out into infinite space
            // If two or more dimensions have depth 0, do nothing.

            var sameX = state.Start.X == state.End.X;
            var sameY = state.Start.Y == state.End.Y;
            var sameZ = state.Start.Z == state.End.Z;
            var start = state.Start.Clone();
            var end = state.End.Clone();
            var invalid = false;

            if (sameX)
            {
                if (sameY || sameZ) invalid = true;
                start.X = Decimal.MinValue;
                end.X = Decimal.MaxValue;
            }

            if (sameY)
            {
                if (sameZ) invalid = true;
                start.Y = Decimal.MinValue;
                end.Y = Decimal.MaxValue;
            }

            if (sameZ)
            {
                start.Z = Decimal.MinValue;
                end.Z = Decimal.MaxValue;
            }

            boundingbox = new Box(start, end);
            return !invalid;
        }
    }
}