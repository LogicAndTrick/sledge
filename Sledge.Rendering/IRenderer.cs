using Sledge.Rendering.Scenes;

namespace Sledge.Rendering
{
    public interface IRenderer
    {
        IViewport CreateViewport();
        Scene Scene { get; }
    }
}