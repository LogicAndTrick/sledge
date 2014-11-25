using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.UI.Viewports;

namespace Sledge.EditorNew.Tools.DraggableTool
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
            Usage = ToolUsage.Both;
        }

        #region Virtual events
        protected virtual void OnDraggableClicked(IViewport2D viewport, ViewportEvent e, Coordinate position, IDraggable draggable)
        {

        }

        protected virtual void OnDraggableDragStarted(IViewport2D viewport, ViewportEvent e, Coordinate position, IDraggable draggable)
        {

        }

        protected virtual void OnDraggableDragMoved(IViewport2D viewport, ViewportEvent e, Coordinate previousPosition, Coordinate position, IDraggable draggable)
        {

        }

        protected virtual void OnDraggableDragEnded(IViewport2D viewport, ViewportEvent e, Coordinate position, IDraggable draggable)
        {

        }
        #endregion

        protected override void MouseClick(IViewport2D viewport, ViewportEvent e)
        {
            if (e.Dragging || e.Button != MouseButton.Left) return;
            if (CurrentDraggable == null) return;
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            OnDraggableClicked(viewport, e, point, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.Click(viewport, e, point);
        }

        protected override void MouseMove(IViewport2D viewport, ViewportEvent e)
        {
            if (e.Dragging || e.Button == MouseButton.Left) return;
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            IDraggable drag = null;
            foreach (var state in States)
            {
                var drags = state.GetDraggables(viewport).ToList();
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

        protected override void DragStart(IViewport2D viewport, ViewportEvent e)
        {
            if (e.Button != MouseButton.Left) return;
            if (CurrentDraggable == null) return;
            _lastDragPoint = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            OnDraggableDragStarted(viewport, e, _lastDragPoint, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.StartDrag(viewport, e, _lastDragPoint);
            _lastDragMoveEvent = e;
        }

        protected override void DragMove(IViewport2D viewport, ViewportEvent e)
        {
            if (e.Button != MouseButton.Left) return;
            if (CurrentDraggable == null) return;
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            OnDraggableDragMoved(viewport, e, _lastDragPoint, point, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.Drag(viewport, e, _lastDragPoint, point);
            _lastDragPoint = point;
            _lastDragMoveEvent = e;
        }

        protected override void DragEnd(IViewport2D viewport, ViewportEvent e)
        {
            if (e.Button != MouseButton.Left) return;
            if (CurrentDraggable == null) return;
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            OnDraggableDragEnded(viewport, e, point, CurrentDraggable);
            if (!e.Handled) CurrentDraggable.EndDrag(viewport, e, point);
            _lastDragMoveEvent = null;
            _lastDragPoint = null;
        }

        public override void PositionChanged(IViewport2D viewport, ViewportEvent e)
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
                OnDraggableDragMoved(viewport, ev, _lastDragPoint, point, CurrentDraggable);
                if (!ev.Handled) CurrentDraggable.Drag(viewport, ev, _lastDragPoint, point);
                _lastDragPoint = point;
            }
            base.PositionChanged(viewport, e);
        }

        protected override void Render(IViewport2D viewport)
        {
            var foundActive = false;
            foreach (var state in States)
            {
                foreach (var draggable in state.GetDraggables(viewport))
                {
                    if (draggable == CurrentDraggable) foundActive = true;
                    else draggable.Render(viewport);
                }
                if (state == CurrentDraggable) foundActive = true;
                else state.Render(viewport);
            }
            if (CurrentDraggable != null && foundActive) CurrentDraggable.Render(viewport);
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