using System.ComponentModel.Composition;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class AxisLinesConverter : IMapObjectSceneConverter
    {
        private static readonly object Holder = new object();
        [Import] private EngineInterface _engine;

        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.OverrideLow;

        public bool ShouldStopProcessing(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Root;
        }
        
        public Task Convert(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            //smo.SceneObjects.Add(new Holder(), new Line(Color.FromArgb(255, Color.Red), Vector3.Zero, Vector3.UnitX * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            //smo.SceneObjects.Add(new Holder(), new Line(Color.FromArgb(255, Color.Lime), Vector3.Zero, Vector3.UnitY * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });
            //smo.SceneObjects.Add(new Holder(), new Line(Color.FromArgb(255, Color.Blue), Vector3.Zero, Vector3.UnitZ * 10) { RenderFlags = RenderFlags.Wireframe, CameraFlags = CameraFlags.Perspective });

            var points = new[]
            {
                // X axis - red
                new VertexStandard4 { Position = Vector3.Zero, Colour = Vector4.UnitX + Vector4.UnitW },
                new VertexStandard4 { Position = Vector3.UnitX * 10, Colour = Vector4.UnitX + Vector4.UnitW },

                // Y axis - green
                new VertexStandard4 { Position = Vector3.Zero, Colour = Vector4.UnitY + Vector4.UnitW },
                new VertexStandard4 { Position = Vector3.UnitY * 10, Colour = Vector4.UnitY + Vector4.UnitW },

                // Z axis - blue
                new VertexStandard4 { Position = Vector3.Zero, Colour = Vector4.UnitZ + Vector4.UnitW },
                new VertexStandard4 { Position = Vector3.UnitZ * 10, Colour = Vector4.UnitZ + Vector4.UnitW },
            };

            var indices = new uint[] { 0, 1, 2, 3, 4, 5 };

            var buffer = _engine.CreateBuffer();
            buffer.Update(points, indices);

            smo.Buffers.Add(Holder, buffer);

            var renderable = new SimpleRenderable(buffer, PipelineType.WireframeGeneric, 0, indices.Length) { PerspectiveOnly = true };
            smo.Renderables.Add(Holder, renderable);

            return Task.FromResult(0);
        }

        public Task<bool> Update(SceneMapObject smo, MapDocument document, IMapObject obj)
        {
            return Task.FromResult(true);
        }
    }
}