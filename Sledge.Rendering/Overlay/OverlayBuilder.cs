using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Numerics;
using LogicAndTrick.Oy;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using Veldrid;
using Buffer = Sledge.Rendering.Resources.Buffer;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Sledge.Rendering.Overlay
{
    public class ViewportOverlay : IRenderable
    {
        private readonly IViewport _viewport;

        public float Order => float.MaxValue;

        private Bitmap _bitmap;

        private int _width;
        private int _height;

        private Texture _texture;
        private TextureView _view;
        private ResourceSet _set;
        private Buffer _buffer;

        internal ViewportOverlay(IViewport viewport)
        {
            _viewport = viewport;
            _width = -1;
            _height = -1;

            _buffer = Engine.Engine.Interface.CreateBuffer();
            _buffer.Update(new []
            {
                new VertexStandard{ Position = new Vector3(0, 0, 0), Texture = new Vector2(0, 0) },
                new VertexStandard{ Position = new Vector3(0, 1, 0), Texture = new Vector2(0, 1) },
                new VertexStandard{ Position = new Vector3(1, 1, 0), Texture = new Vector2(1, 1) },
                new VertexStandard{ Position = new Vector3(1, 0, 0), Texture = new Vector2(1, 0) },
            }, new uint[]
            {
                0, 2, 1,
                0, 3, 2
            });
        }

        private void Resize(RenderContext context)
        {
            var vpw = Math.Max(1, _viewport.Width);
            var vph = Math.Max(1, _viewport.Height);
            if (_bitmap != null && vpw == _width && vph == _height) return;

            _width = vpw;
            _height = vph;

            _set?.Dispose();
            _view?.Dispose();
            _texture?.Dispose();
            _bitmap?.Dispose();

            _bitmap = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);
            _texture = context.Device.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
                (uint) _width, (uint) _height, 1, 1,
                Veldrid.PixelFormat.B8_G8_R8_A8_UNorm,
                TextureUsage.Sampled
            ));
            _view = context.Device.ResourceFactory.CreateTextureView(_texture);
            _set = context.Device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                context.ResourceLoader.TextureLayout, _view, context.ResourceLoader.OverlaySampler
            ));
        }

        public void Build(RenderContext context, IEnumerable<IOverlayRenderable> builders)
        {
            Resize(context);

            var min = Vector3.Zero;
            var max = Vector3.Zero;

            var oc = _viewport.Camera as OrthographicCamera;
            var pc = _viewport.Camera as PerspectiveCamera;

            if (oc != null)
            {
                var up = (Vector3.One - oc.Expand(new Vector3(1, 1, 0))) * 1000;
                var tl = oc.ScreenToWorld(Vector3.Zero) + up;
                var br = oc.ScreenToWorld(new Vector3(_width, _height, 0)) - up;
                min = Vector3.Min(tl, br);
                max = Vector3.Max(tl, br);
            }

            using (var g = Graphics.FromImage(_bitmap))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.FillRectangle(Brushes.Transparent, 0, 0, _width, _height);
                g.CompositingMode = CompositingMode.SourceOver;

                foreach (var b in builders)
                {
                    try
                    {
                        if (pc != null) b.Render(_viewport, pc, g);
                        if (oc != null) b.Render(_viewport, oc, min, max, g);
                    }
                    catch (Exception ex)
                    {
                        Oy.Publish("Shell:UnhandledExceptionOnce", ex);
                    }
                }
            }

            var lb = _bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            context.Device.UpdateTexture(_texture, lb.Scan0, (uint) (lb.Stride * lb.Height), 0, 0, 0, _texture.Width, _texture.Height, _texture.Depth, 0, 0);
            _bitmap.UnlockBits(lb);
        }

        public bool ShouldRender(IPipeline pipeline, IViewport viewport)
        {
            return pipeline.Type == PipelineType.Overlay && viewport == _viewport;
        }

        public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
        {
            cl.SetGraphicsResourceSet(1, _set);
            _buffer.Bind(cl, 0);
            cl.DrawIndexed(6, 1, 0, 0, 0);
        }

        public void RenderTransparent(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
        {
            //
        }

        public void Dispose()
        {

        }
    }
}
