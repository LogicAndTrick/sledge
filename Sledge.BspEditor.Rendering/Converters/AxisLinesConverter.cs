using System.ComponentModel.Composition;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class AxisLinesConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.OverrideLow;

        public bool ShouldStopProcessing(MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Root;
        }
        
        public Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj, ResourceCollector resourceCollector)
        {
            var points = new[]
            {
                // X axis - red
                new VertexStandard { Position = Vector3.Zero, Colour = Vector4.UnitX + Vector4.UnitW },
                new VertexStandard { Position = Vector3.UnitX * 100, Colour = Vector4.UnitX + Vector4.UnitW },

                // Y axis - green
                new VertexStandard { Position = Vector3.Zero, Colour = Vector4.UnitY + Vector4.UnitW },
                new VertexStandard { Position = Vector3.UnitY * 100, Colour = Vector4.UnitY + Vector4.UnitW },

                // Z axis - blue
                new VertexStandard { Position = Vector3.Zero, Colour = Vector4.UnitZ + Vector4.UnitW },
                new VertexStandard { Position = Vector3.UnitZ * 100, Colour = Vector4.UnitZ + Vector4.UnitW },
            };

            var indices = new uint[] { 0, 1, 2, 3, 4, 5 };

            builder.Append(points, indices, new []
            {
                new BufferGroup(PipelineType.Wireframe, CameraType.Perspective, 0, (uint) indices.Length)
            });

            return Task.FromResult(0);
        }
    }
}