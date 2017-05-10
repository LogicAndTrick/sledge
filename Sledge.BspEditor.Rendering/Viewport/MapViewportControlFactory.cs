using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sledge.BspEditor.Components;
using Sledge.Rendering.Cameras;

namespace Sledge.BspEditor.Rendering.Viewport
{
    [Export(typeof(IMapDocumentControlFactory))]
    public class MapViewportControlFactory : IMapDocumentControlFactory
    {
        [ImportMany] private IEnumerable<Lazy<IViewportEventListenerFactory>> _viewportEventListenerFactories;

        public string Type => "MapViewport";

        public IMapDocumentControl Create()
        {
            return new ViewportMapDocumentControl(_viewportEventListenerFactories.Select(x => x.Value));
        }
    }
}