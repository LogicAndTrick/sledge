using System;
using Sledge.Rendering.Pipelines;

namespace Sledge.Rendering.Renderables
{
    public class BufferGroup
    {
        public PipelineType Pipeline { get; }
        public string Binding { get; }
        public uint Offset { get; }
        public uint Count { get; }

        public BufferGroup(PipelineType pipeline, uint offset, uint count)
        {
            Pipeline = pipeline;
            Binding = String.Empty;
            Offset = offset;
            Count = count;
        }

        public BufferGroup(PipelineType pipeline, string binding, uint offset, uint count)
        {
            Pipeline = pipeline;
            Binding = binding;
            Offset = offset;
            Count = count;
        }
    }
}