using System;
using System.ComponentModel.Composition;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Converters;

namespace Sledge.BspEditor.Rendering.Scene
{
    [Export(typeof(ISceneObjectProviderFactory))]
    public class MapDocumentSceneObjectProviderFactory : ISceneObjectProviderFactory
    {
        [Import] private Lazy<MapObjectConverter> _converter;

        public ISceneObjectProvider MakeProvider(MapDocument document)
        {
            return new MapDocumentSceneObjectProvider(document, _converter.Value);
        }
    }
}