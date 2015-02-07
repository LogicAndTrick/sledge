using Sledge.Rendering.OpenGL;
using Sledge.Rendering.Scenes;

namespace Sledge.Rendering
{
    public interface IRenderer
    {
        IViewport CreateViewport();
        Scene Scene { get; }
        ITextureStorage Textures { get; }
        IMaterialStorage Materials { get; }
    }
}