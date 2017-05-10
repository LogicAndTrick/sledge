using System.Collections.Generic;

namespace Sledge.BspEditor.Rendering.Viewport
{
    public interface IViewportEventListenerFactory
    {
        IEnumerable<IViewportEventListener> Create(MapViewport viewport);
    }
}