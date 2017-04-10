using Sledge.BspEditor.Rendering.Viewport;

namespace Sledge.BspEditor.Tools.State
{
    public class StateEvent
    {
        public MapViewport Viewport { get; set; }
        public ViewportEvent ViewportEvent { get; set; }
        public StateAction Action { get; set; }

        public StateEvent(MapViewport viewport, ViewportEvent viewportEvent, StateAction action)
        {
            Viewport = viewport;
            ViewportEvent = viewportEvent;
            Action = action;
        }
    }
}