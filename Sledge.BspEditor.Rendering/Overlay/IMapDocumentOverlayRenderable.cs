using Sledge.BspEditor.Documents;
using Sledge.Rendering.Overlay;

namespace Sledge.BspEditor.Rendering.Overlay
{
    public interface IMapDocumentOverlayRenderable : IOverlayRenderable
    {
        void SetActiveDocument(MapDocument doc);
    }
}