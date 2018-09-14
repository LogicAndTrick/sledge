using System;
using System.Numerics;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Renderables;

namespace Sledge.Rendering.Resources
{
    public class BufferGroup : ILocation
    {
        public PipelineType Pipeline { get; }
        public CameraType Camera { get; }
        public bool HasTransparency { get; }
        public Vector3 Location { get; }
        public string Binding { get; }
        public uint Offset { get; }
        public uint Count { get; }

        public BufferGroup(PipelineType pipeline, CameraType camera, uint offset, uint count)
            : this(pipeline, camera, false, Vector3.Zero, string.Empty, offset, count)
        {
        }

        public BufferGroup(PipelineType pipeline, CameraType camera, string binding, uint offset, uint count)
            : this(pipeline, camera, false, Vector3.Zero, binding, offset, count)
        {
        }

        public BufferGroup(PipelineType pipeline, CameraType camera, Vector3 location, uint offset, uint count)
            : this(pipeline, camera, location, string.Empty, offset, count)
        {
        }

        public BufferGroup(PipelineType pipeline, CameraType camera, Vector3 location, string binding, uint offset, uint count)
            : this(pipeline, camera, true, location, binding, offset, count)
        {
        }

        public BufferGroup(PipelineType pipeline, CameraType camera, bool hasTransparency, Vector3 location,
            string binding, uint offset, uint count)
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