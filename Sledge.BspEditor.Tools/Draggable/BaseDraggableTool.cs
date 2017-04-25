using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.BspEditor.Tools.Draggable
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
        protected virtual void OnDraggableMouseDown(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position, IDraggable draggable)
        {

        }
        protected virtual void OnDraggableMouseUp(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position, IDraggable draggable)
        {

        }
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
            var point = viewport.ProperScreenToWorld(e.X, e.Y);
            point = viewport.Flatten(point);
            OnDraggableClicked(viewport, camera, e, point, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.Click(viewport, e, point);
            Invalidate();
        }

        protected override void MouseDown(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (CurrentDraggable == null) return;
            var point = viewport.ProperScreenToWorld(e.X, e.Y);
            point = viewport.Flatten(point);
            OnDraggableMouseDown(viewport, camera, e, point, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.MouseDown(viewport, e, point);
            Invalidate();
        }

        protected override void MouseUp(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (CurrentDraggable == null) return;
            var point = viewport.ProperScreenToWorld(e.X, e.Y);
            point = viewport.Flatten(point);
            OnDraggableMouseUp(viewport, camera, e, point, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.MouseUp(viewport, e, point);
            Invalidate();
        }

        protected override void MouseMove(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Dragging || e.Button == MouseButtons.Left) return;
            var point = viewport.ProperScreenToWorld(e.X, e.Y);
            point = viewport.Flatten(point);
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
                Invalidate();
            }
        }

        protected override void DragStart(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (CurrentDraggable == null) return;
            _lastDragPoint = viewport.ProperScreenToWorld(e.X, e.Y);
            _lastDragPoint = viewport.Flatten(_lastDragPoint);
            OnDraggableDragStarted(viewport, camera, e, _lastDragPoint, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.StartDrag(viewport, e, _lastDragPoint);
            _lastDragMoveEvent = e;
            Invalidate();
        }

        protected override void DragMove(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (CurrentDraggable == null) return;
            var point = viewport.ProperScreenToWorld(e.X, e.Y);
            point = viewport.Flatten(point);
            OnDraggableDragMoving(viewport, camera, e, _lastDragPoint, point, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.Drag(viewport, e, _lastDragPoint, point);
            if (!e.Handled) OnDraggableDragMoved(viewport, camera, e, _lastDragPoint, point, CurrentDraggable);
            _lastDragPoint = point;
            _lastDragMoveEvent = e;
            Invalidate();
        }

        protected override void DragEnd(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (CurrentDraggable == null) return;
            var point = viewport.ProperScreenToWorld(e.X, e.Y);
            point = viewport.Flatten(point);
            OnDraggableDragEnded(viewport, camera, e, point, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.EndDrag(viewport, e, point);
            _lastDragMoveEvent = null;
            _lastDragPoint = null;
            Invalidate();
        }

        public override void PositionChanged(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (viewport.Is2D && _lastDragMoveEvent != null && CurrentDraggable != null && _lastDragMoveEvent.Sender == viewport)
            {
                var point = viewport.ProperScreenToWorld(_lastDragMoveEvent.X, _lastDragMoveEvent.Y);
                point = viewport.Flatten(point);
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
                Invalidate();
            }
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
            return CollectObjects(x => x.GetSceneObjects()).Union(base.GetSceneObjects());
        }

        protected override IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            return CollectObjects(x => x.GetViewportElements(viewport, camera)).Union(base.GetViewportElements(viewport, camera));
        }

        protected override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            return CollectObjects(x => x.GetViewportElements(viewport, camera)).Union(base.GetViewportElements(viewport, camera));
        }
    }
}