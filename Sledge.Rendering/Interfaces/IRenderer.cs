using Sledge.Rendering.Scenes;

namespace Sledge.Rendering.Interfaces
{
    public interface IRenderer
    {
        IViewport CreateViewport();
        Scene Scene { get; }
        ITextureStorage Textures { get; }
        IMaterialStorage Materials { get; }
    }
}