using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sledge.BspEditor.Components;
using Sledge.Common.Translations;
using Sledge.Rendering.Engine;

namespace Sledge.BspEditor.Rendering.Viewport
{
    [Export(typeof(IMapDocumentControlFactory))]
    [AutoTranslate]
    public class MapViewportControlFactory : IMapDocumentControlFactory
    {
        private readonly IEnumerable<Lazy<IViewportEventListenerFactory>> _viewportEventListenerFactories;
        private readonly Lazy<EngineInterface> _engine;

        public string Perspective { get; set; }
        public string OrthographicTop { get; set; }
        public string OrthographicFront { get; set; }
        public string OrthographicSide { get; set; }

        public string Type => "MapViewport";

        [ImportingConstructor]
        public MapViewportControlFactory(
            [ImportMany] IEnumerable<Lazy<IViewportEventListenerFactory>> viewportEventListenerFactories,
            [Import] Lazy<EngineInterface> engine
        )
        {
            _viewportEventListenerFactories = viewportEventListenerFactories;
            _engine = engine;
        }

        public IMapDocumentControl Create()
        {
            return new ViewportMapDocumentControl(_engine.Value, _viewportEventListenerFactories.Select(x => x.Value));
        }

        public bool IsType(IMapDocumentControl control)
        {
            return control is ViewportMapDocumentControl;
        }

        public Dictionary<string, string> GetStyles()
        {
            return new Dictionary<string, string>
            {
                {"PerspectiveCamera/", Perspective},
                {"OrthographicCamera/Top", OrthographicTop},
                {"OrthographicCamera/Front", OrthographicFront},
                {"OrthographicCamera/Side", OrthographicSide}
            };
        }

        public bool IsStyle(IMapDocumentControl control, string style)
        {
            return control is ViewportMapDocumentControl
                   && control.GetSerialisedSettings().StartsWith(style);
        }
    }
}