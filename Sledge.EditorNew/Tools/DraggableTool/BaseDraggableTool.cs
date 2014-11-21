using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Gui.Structures;

namespace Sledge.EditorNew.Tools.DraggableTool
{
    public abstract class BaseDraggableTool : BaseTool
    {
        public List<IDraggableState> States { get; set; }

        private IDraggable _currentDraggable;
        private ViewportEvent _lastDragMoveEvent = null;
        private Coordinate _lastDragPoint = null;

        protected BaseDraggableTool()
        {
            States = new List<IDraggableState>();
        }

        public override void MouseClick(IMapViewport viewport, ViewportEvent e)
        {
            if (!viewport.Is2D || e.Dragging || e.Button != MouseButton.Left) return;
            var vp = (IViewport2D)viewport;
            if (_currentDraggable == null) return;
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            _currentDraggable.Click(vp, e, point);
        }

        public override void MouseMove(IMapViewport viewport, ViewportEvent e)
        {
            if (!viewport.Is2D || e.Dragging || e.Button == MouseButton.Left) return;
            var vp = (IViewport2D)viewport;
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            IDraggable drag = null;
            foreach (var state in States)
            {
                var drags = state.GetDraggables(vp).ToList();
                drags.Add(state);
                foreach (var draggable in drags)
                {
                    if (draggable.CanDrag(vp, e, point))
                    {
                        drag = draggable;
                        break;
                    }
                }
                if (drag != null) break;
            }
            if (drag != _currentDraggable)
            {
                if (_currentDraggable != null) _currentDraggable.Unhighlight(vp);
                _currentDraggable = drag;
                if (_currentDraggable != null) _currentDraggable.Highlight(vp);
            }
        }
        
        public override void DragStart(IMapViewport viewport, ViewportEvent e)
        {
            if (!viewport.Is2D || e.Button != MouseButton.Left) return;
            var vp = (IViewport2D)viewport;
            if (_currentDraggable == null) return;
            _lastDragPoint = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            _currentDraggable.StartDrag(vp, e, _lastDragPoint);
            _lastDragMoveEvent = e;
        }

        public override void DragMove(IMapViewport viewport, ViewportEvent e)
        {
            if (!viewport.Is2D || e.Button != MouseButton.Left) return;
            var vp = (IViewport2D)viewport;
            if (_currentDraggable == null) return;
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            _currentDraggable.Drag(vp, e, _lastDragPoint, point);
            _lastDragPoint = point;
            _lastDragMoveEvent = e;
        }

        public override void DragEnd(IMapViewport viewport, ViewportEvent e)
        {
            if (!viewport.Is2D || e.Button != MouseButton.Left) return;
            var vp = (IViewport2D)viewport;
            if (_currentDraggable == null) return;
            var point = viewport.ScreenToWorld(e.X, viewport.Height - e.Y);
            _currentDraggable.EndDrag(vp, e, point);
            _lastDragMoveEvent = null;
            _lastDragPoint = null;
        }

        public override void PositionChanged(IMapViewport viewport, ViewportEvent e)
        {
            if (viewport.Is2D && _lastDragMoveEvent != null && _currentDraggable != null && _lastDragMoveEvent.Sender == viewport)
            {
                var vp = (IViewport2D) viewport;
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
                _currentDraggable.Drag(vp, ev, _lastDragPoint, point);
                _lastDragPoint = point;
            }
            base.PositionChanged(viewport, e);
        }

        public override void Render(IMapViewport viewport)
        {
            if (!viewport.Is2D) return;
            var vp = (IViewport2D) viewport;
            var foundActive = false;
            foreach (var state in States)
            {
                foreach (var draggable in state.GetDraggables(vp))
                {
                    if (draggable == _currentDraggable) foundActive = true;
                    else draggable.Render(vp);
                }
                if (state == _currentDraggable) foundActive = true;
                else state.Render(vp);
            }
            if (_currentDraggable != null && foundActive) _currentDraggable.Render(vp);
        }

        #region Unused (for now)
        public override void MouseDown(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void MouseUp(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void MouseEnter(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void MouseLeave(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void MouseDoubleClick(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void MouseWheel(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void KeyPress(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void KeyDown(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void KeyUp(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void UpdateFrame(IMapViewport viewport, Frame frame)
        {

        }

        public override void PreRender(IMapViewport viewport)
        {

        }
        #endregion
    }
}