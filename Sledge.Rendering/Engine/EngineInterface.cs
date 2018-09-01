using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Numerics;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using Veldrid;
using Buffer = Sledge.Rendering.Resources.Buffer;

namespace Sledge.Rendering.Engine
{
    [Export]
    public class EngineInterface
    {
        public event EventHandler<IViewport> ViewportCreated
        {
            add => Engine.Instance.ViewportCreated += value;
            remove => Engine.Instance.ViewportCreated -= value;
        }

        public event EventHandler<IViewport> ViewportDestroyed
        {
            add => Engine.Instance.ViewportDestroyed += value;
            remove => Engine.Instance.ViewportDestroyed -= value;
        }

        public void SetClearColour(CameraType cameraType, Color colour)
        {
            Engine.Instance.SetClearColour(cameraType, new RgbaFloat(new Vector4(colour.R, colour.G, colour.B, colour.A) / 255));
        }

        public Buffer CreateBuffer()
        {
            return new Buffer(Engine.Instance.Device);
        }

        public BufferBuilder CreateBufferBuilder(BufferSize size)
        {
            return new BufferBuilder(Engine.Instance.Device, size);
        }

        public void UploadTexture(string name, Func<ITextureDataSource> source)
        {
            Engine.Instance.Context.ResourceLoader.UploadTexture(name, source);
        }

        public IViewport CreateViewport()
        {
            return Engine.Instance.CreateViewport();
        }

        public IDisposable Pause()
        {
            return Engine.Instance.Pause();
        }

        public void SetSelectiveTransform(Matrix4x4 matrix)
        {
            Engine.Instance.Context.SelectiveTransform = matrix;
        }

        public void Add(IRenderable renderable) => Engine.Instance.Scene.Add(renderable);
        public void Add(IUpdateable updateable) => Engine.Instance.Scene.Add(updateable);
        public void Add(IOverlayRenderable overlayRenderable) => Engine.Instance.Scene.Add(overlayRenderable);

        public void Remove(IRenderable renderable) => Engine.Instance.Scene.Remove(renderable);
        public void Remove(IUpdateable updateable) => Engine.Instance.Scene.Remove(updateable);
        public void Remove(IOverlayRenderable overlayRenderable) => Engine.Instance.Scene.Remove(overlayRenderable);
    }
}
