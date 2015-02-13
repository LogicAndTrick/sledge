using Sledge.Rendering.Scenes;

namespace Sledge.Rendering.Interfaces
{
    public interface IRenderer
    {
        IViewport CreateViewport();

        Scene CreateScene();
        void SetActiveScene(Scene scene);
        void RemoveScene(Scene scene);

        ITextureStorage Textures { get; }
        IMaterialStorage Materials { get; }
    }
}