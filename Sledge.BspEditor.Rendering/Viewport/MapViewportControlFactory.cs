using System.ComponentModel.Composition;
using Sledge.BspEditor.Components;
using Sledge.Rendering.Cameras;

namespace Sledge.BspEditor.Rendering.Viewport
{
    [Export(typeof(IMapDocumentControlFactory))]
    public class MapViewportControlFactory : IMapDocumentControlFactory
    {
        public string Type => "MapViewport";

        public IMapDocumentControl Create()
        {
            return new ViewportMapDocumentControl();
        }
    }
}