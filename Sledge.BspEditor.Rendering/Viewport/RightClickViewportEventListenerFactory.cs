using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Rendering.Viewport
{
    [Export(typeof(IViewportEventListenerFactory))]
    public class RightClickViewportEventListenerFactory : IViewportEventListenerFactory
    {
        public IEnumerable<IViewportEventListener> Create(MapViewport viewport)
        {
            yield return new RightClickViewportListener(viewport);
        }
    }
}