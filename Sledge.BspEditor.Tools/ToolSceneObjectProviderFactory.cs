using System.ComponentModel.Composition;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Scene;

namespace Sledge.BspEditor.Tools
{
    [Export(typeof(ISceneObjectProviderFactory))]
    public class ToolSceneObjectProviderFactory : ISceneObjectProviderFactory
    {
        public ISceneObjectProvider MakeProvider(MapDocument document)
        {
            return new ToolSceneObjectProvider();
        }
    }
}