using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Renderables;

namespace Sledge.Rendering.Engine
{
    [Export]
    public class EngineInterface
    {
        public Buffer CreateBuffer()
        {
            return new Buffer(Engine.Instance.Device);
        }

        public TextureBinding CreateTexture(string name, ITextureDataSource source)
        {
            return Engine.Instance.Context.ResourceLoader.CreateTextureBinding(name, source);
        }
    }
}
