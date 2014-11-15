using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Gui.Structures;

namespace Sledge.EditorNew.Tools.DraggableTool
{
    public abstract class BaseDraggableTool : BaseTool
    {
        public Stack<IDraggableState> States { get; set; }

        private IDraggable _currentDraggable;

        public override void MouseMove(IMapViewport viewport, ViewportEvent e)
        {
            if (!viewport.Is2D || e.Dragging) return;
            var vp = (IViewport2D)viewport;
            IDraggable drag = null;
            foreach (var state in States)
            {
                var drags = state.GetDraggables(vp).ToList();
                drags.Add(state);
                foreach (var draggable in drags)
                {
                    if (draggable.CanDrag(vp, e))
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
            _currentDraggable.StartDrag(vp, e);
        }

        public override void DragMove(IMapViewport viewport, ViewportEvent e)
        {
            if (!viewport.Is2D || e.Button != MouseButton.Left) return;
            var vp = (IViewport2D)viewport;
            if (_currentDraggable == null) return;
            _currentDraggable.Drag(vp, e);
        }

        public override void DragEnd(IMapViewport viewport, ViewportEvent e)
        {
            if (!viewport.Is2D || e.Button != MouseButton.Left) return;
            var vp = (IViewport2D)viewport;
            if (_currentDraggable == null) return;
            _currentDraggable.EndDrag(vp, e);
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

        public override void MouseClick(IMapViewport viewport, ViewportEvent e)
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