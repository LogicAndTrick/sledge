using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Rendering.Pipelines
{
    public class OverlayPipeline : IPipeline
    {
        public PipelineType Type => PipelineType.Overlay;
        public float Order => 9999;

        private Shader _vertex;
        private Shader _fragment;
        private Pipeline _pipeline;
        private DeviceBuffer _projectionBuffer;
        private ResourceSet _projectionResourceSet;

        public void Create(RenderContext context)
        {
            (_vertex, _fragment) = context.ResourceLoader.LoadShaders(Type.ToString());

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

        public void Render(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables)
        {
            cl.SetPipeline(_pipeline);
            cl.SetGraphicsResourceSet(0, _projectionResourceSet);

            foreach (var r in renderables)
            {
                r.Render(context, this, target, cl);
            }
        }

        public void RenderTransparent(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables)
        {
            cl.SetPipeline(_pipeline);
            cl.SetGraphicsResourceSet(0, _projectionResourceSet);

            foreach (var r in renderables)
            {
                r.RenderTransparent(context, this, target, cl);
            }
        }

        public void Bind(RenderContext context, CommandList cl, string binding)
        {
            //
        }

        public void Dispose()
        {
            _projectionResourceSet?.Dispose();
            _projectionBuffer?.Dispose();
            _pipeline?.Dispose();
            _vertex?.Dispose();
            _fragment?.Dispose();
        }
    }
}