using Sledge.BspEditor.Rendering.Resources;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Rendering.Dynamic
{
    public interface IDynamicRenderable
    {
        void Render(BufferBuilder builder, ResourceCollector resourceCollector);
    }
}