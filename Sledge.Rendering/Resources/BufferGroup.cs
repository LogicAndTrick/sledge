using System;
using System.Numerics;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;

namespace Sledge.Rendering.Resources
{
    public class BufferGroup
    {
        public PipelineType Pipeline { get; }
        public CameraType Camera { get; }
        public bool HasTransparency { get; }
        public Vector3 Location { get; }
        public string Binding { get; }
        public uint Offset { get; }
        public uint Count { get; }

        public BufferGroup(PipelineType pipeline, CameraType camera, bool hasTransparency, Vector3 location, uint offset, uint count)
        {
            Pipeline = pipeline;
            Camera = camera;
            HasTransparency = hasTransparency;
            Location = location;
            Binding = String.Empty;
            Offset = offset;
            Count = count;
        }

        public BufferGroup(PipelineType pipeline, CameraType camera, bool hasTransparency, Vector3 location, string binding, uint offset, uint count)
        {
            Pipeline = pipeline;
            Camera = camera;
            HasTransparency = hasTransparency;
            Location = location;
            Binding = binding;
            Offset = offset;
            Count = count;
        }
    }
}