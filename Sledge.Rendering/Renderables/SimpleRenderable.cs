using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Rendering.Renderables
{
    public class SimpleRenderable : IRenderable
    {
        private readonly Buffer _buffer;
        private readonly HashSet<PipelineType> _pipelines;

        public int IndexOffset { get; set; }
        public int IndexCount { get; set; }

        public bool PerspectiveOnly { get; set; }
        public bool OrthographicOnly { get; set; }

        public SimpleRenderable(Buffer buffer, PipelineType pipeline, int indexOffset, int indexCount) : this(buffer, new [] { pipeline}, indexOffset, indexCount)
        {
        }

        public SimpleRenderable(Buffer buffer, IEnumerable<PipelineType> pipelines, int indexOffset, int indexCount)
        {
            _buffer = buffer;
            _pipelines = pipelines.ToHashSet();
            IndexOffset = indexOffset;
            IndexCount = indexCount;
        }

        public bool ShouldRender(IPipeline pipeline, IViewport viewport)
        {
            return _pipelines.Contains(pipeline.Type)
                   && (!OrthographicOnly || false) // todo
                   && (!PerspectiveOnly || viewport.Camera is PerspectiveCamera);
        }

        public void Render(IPipeline pipeline, IViewport viewport, CommandList cl)
        {
            cl.SetVertexBuffer(0, _buffer.VertexBuffer);
            cl.SetIndexBuffer(_buffer.IndexBuffer, IndexFormat.UInt32);
            Draw(pipeline, viewport, cl);
        }

        protected virtual void Draw(IPipeline pipeline, IViewport viewport, CommandList cl)
        {
            cl.DrawIndexed((uint)IndexCount, 1, (uint)IndexOffset, 0, 0);
        }

        public void Dispose()
        {
            // 
        }
    }
}