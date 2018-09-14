using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.BspEditor.Rendering.Scene
{
    public class SceneBuilderRenderable : IRenderable
    {
        private static readonly uint IndSize = (uint) Unsafe.SizeOf<IndirectDrawIndexedArguments>();

        private readonly SceneBuilder _sceneBuilder;

        public SceneBuilderRenderable(SceneBuilder sceneBuilder)
        {
            _sceneBuilder = sceneBuilder;
        }

        public bool ShouldRender(IPipeline pipeline, IViewport viewport)
        {
            return true;
        }

        public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
        {
            var builders = _sceneBuilder.BufferBuilders.ToList();
            foreach (var buffer in builders)
            {
                for (var i = 0; i < buffer.NumBuffers; i++)
                {
                    var groups = buffer.IndirectBufferGroups[i].Where(x => x.Pipeline == pipeline.Type && !x.HasTransparency).Where(x => x.Camera == CameraType.Both || x.Camera == viewport.Camera.Type).ToList();
                    if (!groups.Any()) continue;

                    cl.SetVertexBuffer(0, buffer.VertexBuffers[i]);
                    cl.SetIndexBuffer(buffer.IndexBuffers[i], IndexFormat.UInt32);
                    foreach (var bg in groups)
                    {
                        pipeline.Bind(context, cl, bg.Binding);
                        buffer.IndirectBuffers[i].DrawIndexed(cl, bg.Offset * IndSize, bg.Count, 20);
                    }
                }
            }
        }

        public IEnumerable<ILocation> GetLocationObjects(IPipeline pipeline, IViewport viewport)
        {
            foreach (var buffer in _sceneBuilder.BufferBuilders)
            {
                for (var i = 0; i < buffer.NumBuffers; i++)
                {
                    foreach (var group in buffer.IndirectBufferGroups[i])
                    {
                        if (group.Pipeline != pipeline.Type || !group.HasTransparency) continue;
                        if (group.Camera != CameraType.Both && group.Camera != viewport.Camera.Type) continue;
                        yield return new GroupLocation(buffer, i, group);
                    }
                }
            }
        }

        public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl, ILocation locationObject)
        {
            var groupLocation = (GroupLocation) locationObject;

            var buffer = groupLocation.Builder;
            var i = groupLocation.Index;
            var bg = groupLocation.Group;

            cl.SetVertexBuffer(0, buffer.VertexBuffers[i]);
            cl.SetIndexBuffer(buffer.IndexBuffers[i], IndexFormat.UInt32);
            pipeline.Bind(context, cl, bg.Binding);
            buffer.IndirectBuffers[i].DrawIndexed(cl, bg.Offset * IndSize, bg.Count, 20);
        }

        public void Dispose()
        {
            //
        }

        private class GroupLocation : ILocation
        {
            public Vector3 Location => Group.Location;
            public BufferBuilder Builder { get; }
            public int Index { get; }
            public BufferGroup Group { get; }

            public GroupLocation(BufferBuilder builder, int index, BufferGroup group)
            {
                Builder = builder;
                Index = index;
                Group = group;
            }
        }
    }
}