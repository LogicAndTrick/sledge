using System.Collections.Generic;
using System.Linq;
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

        public float Order { get; set; }

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

        public void RenderTransparent(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
        {
            var location = viewport.Camera.Location;

            var allGroups = new List<(BufferBuilder, int, BufferGroup)>();

            var builders = _sceneBuilder.BufferBuilders.ToList();
            foreach (var buffer in builders)
            {
                for (var i = 0; i < buffer.NumBuffers; i++)
                {
                    var groups = buffer.IndirectBufferGroups[i].Where(x => x.Pipeline == pipeline.Type && x.HasTransparency).Where(x => x.Camera == CameraType.Both || x.Camera == viewport.Camera.Type).ToList();

                    foreach (var bufferGroup in groups)
                    {
                        allGroups.Add((buffer, i, bufferGroup));
                    }
                }
            }

            foreach (var grp in allGroups.OrderByDescending(x => (location - x.Item3.Location).LengthSquared()))
            {
                var buffer = grp.Item1;
                var i = grp.Item2;
                var bg = grp.Item3;

                cl.SetVertexBuffer(0, buffer.VertexBuffers[i]);
                cl.SetIndexBuffer(buffer.IndexBuffers[i], IndexFormat.UInt32);
                pipeline.Bind(context, cl, bg.Binding);
                buffer.IndirectBuffers[i].DrawIndexed(cl, bg.Offset * IndSize, bg.Count, 20);
            }
        }

        public void Dispose()
        {
            //
        }
    }
}