using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sledge.Rendering.Cameras;
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

        public float Order { get; set; }

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
                var groups = _buffer.IndirectBufferGroups[i].Where(x => x.Pipeline == pipeline.Type && !x.HasTransparency).Where(x => x.Camera == CameraType.Both || x.Camera == viewport.Camera.Type).ToList();
                if (!groups.Any()) continue;

                cl.SetVertexBuffer(0, _buffer.VertexBuffers[i]);
                cl.SetIndexBuffer(_buffer.IndexBuffers[i], IndexFormat.UInt32);
                foreach (var bg in groups)
                {
                    pipeline.Bind(context, cl, bg.Binding);
                    _buffer.IndirectBuffers[i].DrawIndexed(cl, bg.Offset * IndSize, bg.Count, 20);
                }
            }
        }

        public IEnumerable<ILocation> GetLocationObjects(IPipeline pipeline, IViewport viewport)
        {
            for (var i = 0; i < _buffer.NumBuffers; i++)
            {
                foreach (var group in _buffer.IndirectBufferGroups[i])
                {
                    if (group.Pipeline != pipeline.Type || !group.HasTransparency) continue;
                    if (group.Camera != CameraType.Both && group.Camera != viewport.Camera.Type) continue;
                    yield return new GroupLocation(i, group);
                }
            }
        }

        public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl, ILocation locationObject)
        {
            var groupLocation = (GroupLocation)locationObject;

            var i = groupLocation.Index;
            var bg = groupLocation.Group;

            cl.SetVertexBuffer(0, _buffer.VertexBuffers[i]);
            cl.SetIndexBuffer(_buffer.IndexBuffers[i], IndexFormat.UInt32);
            pipeline.Bind(context, cl, bg.Binding);
            _buffer.IndirectBuffers[i].DrawIndexed(cl, bg.Offset * IndSize, bg.Count, 20);
        }

        public void Dispose()
        {
            _buffer.Dispose();
        }

        private class GroupLocation : ILocation
        {
            public Vector3 Location => Group.Location;
            public int Index { get; }
            public BufferGroup Group { get; }

            public GroupLocation(int index, BufferGroup group)
            {
                Index = index;
                Group = group;
            }
        }
    }
}