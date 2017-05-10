using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Sledge.BspEditor.Rendering.Viewport
{
    [Export(typeof(IViewportEventListenerFactory))]
    public class CameraViewportEventListenerFactory : IViewportEventListenerFactory
    {
        public IEnumerable<IViewportEventListener> Create(MapViewport viewport)
        {
            yield return new PerspectiveCameraNavigationViewportListener(viewport);
            yield return new OrthographicCameraViewportListener(viewport);
        }
    }
}