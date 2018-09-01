using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sledge.BspEditor.Components;
using Sledge.Rendering.Engine;

namespace Sledge.BspEditor.Rendering.Viewport
{
    [Export(typeof(IMapDocumentControlFactory))]
    public class MapViewportControlFactory : IMapDocumentControlFactory
    {
        [ImportMany] private IEnumerable<Lazy<IViewportEventListenerFactory>> _viewportEventListenerFactories;
        [Import] private Lazy<EngineInterface> _engine;

        public string Type => "MapViewport";

        public IMapDocumentControl Create()
        {
            return new ViewportMapDocumentControl(_engine.Value, _viewportEventListenerFactories.Select(x => x.Value));
        }

        public bool IsType(IMapDocumentControl control)
        {
            return control is ViewportMapDocumentControl;
        }
    }
}