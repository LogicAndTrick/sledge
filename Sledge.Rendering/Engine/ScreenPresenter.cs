using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Viewports;
using Veldrid;
using Buffer = Sledge.Rendering.Resources.Buffer;
using Viewport = Sledge.Rendering.Viewports.Viewport;

namespace Sledge.Rendering.Engine
{
    public class ScreenPresenter
    {
        private readonly Viewport _viewport;

        private int _width;
        private int _height;

        private Texture _texture;
        private TextureView _view;
        private ResourceSet _set;
        private Buffer _buffer;

        private Shader _vertex;
        private Shader _fragment;
        private Pipeline _pipeline;
        private DeviceBuffer _projectionBuffer;
        private ResourceSet _projectionResourceSet;

        public ScreenPresenter(IViewport viewport)
        {
            _viewport = (Viewport) viewport;
            _width = -1;
            _height = -1;

            _buffer = Engine.Interface.CreateBuffer();
            _buffer.Update(new[]
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

            var context = Engine.Instance.Context;

            (_vertex, _fragment) = context.ResourceLoader.LoadShaders("Overlay");

            var pDesc = new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleAlphaBlend,
                DepthStencilState = DepthStencilStateDescription.Disabled,
                RasterizerState = RasterizerStateDescription.Default,
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                ResourceLayouts = new[] { context.ResourceLoader.ProjectionLayout, context.ResourceLoader.TextureLayout },
                ShaderSet = new ShaderSetDescription(new[] { context.ResourceLoader.VertexStandardLayoutDescription }, new[] { _vertex, _fragment }),
                Outputs = new OutputDescription
                {
                    ColorAttachments = new[] { new OutputAttachmentDescription(PixelFormat.B8_G8_R8_A8_UNorm) },
                    DepthAttachment = new OutputAttachmentDescription(PixelFormat.R32_Float),
                    SampleCount = TextureSampleCount.Count1
                }
            };

            _pipeline = context.Device.ResourceFactory.CreateGraphicsPipeline(ref pDesc);

            _projectionBuffer = context.Device.ResourceFactory.CreateBuffer(
                new BufferDescription((uint)Unsafe.SizeOf<UniformProjection>(), BufferUsage.UniformBuffer)
            );

            context.Device.UpdateBuffer(_projectionBuffer, 0, new UniformProjection
            {
                Selective = context.SelectiveTransform,
                Model = Matrix4x4.Identity,
                View = Matrix4x4.Identity,
                Projection = Matrix4x4.CreateOrthographicOffCenter(0, 1, 1, 0, -1, 1)
            });

            _projectionResourceSet = context.Device.ResourceFactory.CreateResourceSet(
                new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _projectionBuffer)
            );
        }

        public void Resize(RenderContext context)
        {
            var vpw = Math.Max(1, _viewport.Width);
            var vph = Math.Max(1, _viewport.Height);
            if (_texture != null && vpw == _width && vph == _height) return;

            _width = vpw;
            _height = vph;

            _set?.Dispose();
            _view?.Dispose();
            _texture?.Dispose();

            _texture = context.Device.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
                (uint)_width, (uint)_height, 1, 1,
                Veldrid.PixelFormat.B8_G8_R8_A8_UNorm,
                TextureUsage.Sampled
            ));
            _view = context.Device.ResourceFactory.CreateTextureView(_texture);
            _set = context.Device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                context.ResourceLoader.TextureLayout, _view, context.ResourceLoader.OverlaySampler
            ));
        }

        public void Present(CommandList cl)
        {
            cl.SetFramebuffer(_viewport.Swapchain.Framebuffer);
            cl.SetFullViewports();
            cl.SetFullScissorRects();

            cl.SetPipeline(_pipeline);
            cl.SetGraphicsResourceSet(0, _projectionResourceSet);

            cl.ResolveTexture(_viewport.ColourTexture, _texture);
            cl.SetGraphicsResourceSet(1, _set);

            _buffer.Bind(cl, 0);
            cl.DrawIndexed(6, 1, 0, 0, 0);
        }
    }
}
