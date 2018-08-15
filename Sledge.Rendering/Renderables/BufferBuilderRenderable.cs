using System.Linq;
using System.Runtime.CompilerServices;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Rendering.Renderables
{
    public class BufferBuilderRenderable : IRenderable
    {
        private static readonly uint IndSize = (uint)Unsafe.SizeOf<IndirectDrawIndexedArguments>();

        private readonly BufferBuilder _buffer;

        public BufferBuilderRenderable(BufferBuilder buffer)
        {
            _buffer = buffer;
        }

        public bool ShouldRender(IPipeline pipeline, IViewport viewport)
        {
            return true;
        }

        public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
        {
            for (var i = 0; i < _buffer.NumBuffers; i++)
            {
                var groups = _buffer.IndirectBufferGroups[i].Where(x => x.Pipeline == pipeline.Type).ToList();
                if (!groups.Any()) continue;

                cl.SetVertexBuffer(0, _buffer.VertexBuffers[i]);
                cl.SetIndexBuffer(_buffer.IndexBuffers[i], IndexFormat.UInt32);
                foreach (var bg in groups)
                {
                    pipeline.Bind(context, cl, bg.Binding);
                    cl.DrawIndexedIndirect(_buffer.IndirectBuffers[i], bg.Offset * IndSize, bg.Count, 20);
                }
            }
        }

        public void Dispose()
        {
            _buffer.Dispose();
        }
    }
}