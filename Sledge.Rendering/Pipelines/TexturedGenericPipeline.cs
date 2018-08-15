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
    public class TexturedGenericPipeline : IPipeline
    {
        public PipelineType Type => PipelineType.TexturedGeneric;
        public float Order => 1;

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
                DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
                RasterizerState = RasterizerStateDescription.Default,
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                ResourceLayouts = new[] { context.ResourceLoader.ProjectionLayout, context.ResourceLoader.TextureLayout },
                ShaderSet = new ShaderSetDescription(new[] { context.ResourceLoader.VertexStandard4LayoutDescription }, new[] { _vertex, _fragment }),
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

            _projectionResourceSet = context.Device.ResourceFactory.CreateResourceSet(
                new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _projectionBuffer)
            );
        }

        public void Render(RenderContext context, IViewport target, CommandList cl, IEnumerable<IRenderable> renderables)
        {
            context.Device.UpdateBuffer(_projectionBuffer, 0, new UniformProjection
            {
                Model = Matrix4x4.Identity,
                View = target.Camera.View,
                Projection = target.Camera.Projection,
            });

            cl.SetPipeline(_pipeline);
            cl.SetGraphicsResourceSet(0, _projectionResourceSet);

            foreach (var r in renderables)
            {
                r.Render(this, target, cl);
            }
        }

        public void Bind(CommandList cl, string binding)
        {
            var tb = Engine.Engine.Instance.Context.ResourceLoader.CreateTextureBinding(binding);
            tb?.BindTo(cl, 1);
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