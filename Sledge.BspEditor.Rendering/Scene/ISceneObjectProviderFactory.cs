using Sledge.BspEditor.Documents;

namespace Sledge.BspEditor.Rendering.Scene
{
    public interface ISceneObjectProviderFactory
    {
        ISceneObjectProvider MakeProvider(MapDocument document);
    }
}