using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Rendering.Dynamic
{
    public interface IDynamicRenderable
    {
        void Render(BufferBuilder builder);
    }
}