using Sledge.BspEditor.Documents;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Rendering.Dynamic
{
    public interface IMapObjectDynamicRenderable
    {
        void Render(MapDocument document, BufferBuilder builder);
    }
}