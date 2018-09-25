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
using Texture = Sledge.Rendering.Resources.Texture;

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

        /// <summary>
        /// Upload a texture to the engine.
        /// A texture is a resource that is internally managed by the engine.
        /// Call <see cref="DestroyResource"/> to destroy a texture.
        /// If the texture already exists it will not be created a second time.
        /// </summary>
        /// <param name="name">The texture name</param>
        /// <param name="width">The texture width</param>
        /// <param name="height">The texture height</param>
        /// <param name="data">The texture data, row first, in RGBA format.</param>
        /// <param name="sampleType">The sample type for this texture</param>
        /// <returns>The created texture resource</returns>
        public Texture UploadTexture(string name, int width, int height, byte[] data, TextureSampleType sampleType)
        {
            return Engine.Instance.Context.ResourceLoader.UploadTexture(name, width, height, data, sampleType);
        }

        /// <summary>
        /// Create a resource. This will create all the resources required for this class to render.
        /// </summary>
        /// <param name="resource">The resource to create</param>
        public void CreateResource(IResource resource)
        {
            resource.CreateResources(this, Engine.Instance.Context);
        }

        /// <summary>
        /// Destroy a resource. This will not dispose the object, however it will destroy all engine resources owned by the resource.
        /// </summary>
        /// <param name="resource">The resource to destroy</param>
        public void DestroyResource(IResource resource)
        {
            resource.DestroyResources();
            switch (resource)
            {
                case Texture t:
                    Engine.Instance.Context.ResourceLoader.DestroyTexture(t);
                    break;
            }
        }

        /// <summary>
        /// Create a new viewport for the engine.
        /// </summary>
        /// <returns>The viewport</returns>
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
