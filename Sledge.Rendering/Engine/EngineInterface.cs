using System;
using System.ComponentModel.Composition;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using Buffer = Sledge.Rendering.Resources.Buffer;

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

        public void UploadTexture(string name, Func<ITextureDataSource> source)
        {
            Engine.Instance.Context.ResourceLoader.UploadTexture(name, source);
        }

        public IViewport CreateViewport()
        {
            return Engine.Instance.CreateViewport();
        }

        public void Add(IRenderable renderable) => Engine.Instance.Scene.Add(renderable);
        public void Add(IUpdateable updateable) => Engine.Instance.Scene.Add(updateable);
        public void Remove(IRenderable renderable) => Engine.Instance.Scene.Remove(renderable);
        public void Remove(IUpdateable updateable) => Engine.Instance.Scene.Remove(updateable);
    }
}
