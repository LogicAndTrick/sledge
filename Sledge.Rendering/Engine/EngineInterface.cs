using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Renderables;
using Buffer = Sledge.Rendering.Renderables.Buffer;

namespace Sledge.Rendering.Engine
{
    [Export]
    public class EngineInterface
    {
        public Buffer CreateBuffer()
        {
            return new Buffer(Engine.Instance.Device);
        }
        public BufferBuilder CreateBufferBuilder()
        {
            return new BufferBuilder(Engine.Instance.Device);
        }

        public void CreateTexture(string name, Func<ITextureDataSource> source)
        {
            Engine.Instance.Context.ResourceLoader.CreateTexture(name, source);
        }

        public TextureBinding CreateTextureBinding(string name, Func<ITextureDataSource> source)
        {
            return Engine.Instance.Context.ResourceLoader.CreateTextureBinding(name, source);
        }
    }
}
