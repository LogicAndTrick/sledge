using System.ComponentModel.Composition;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Scene;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class DefaultEntityConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.DefaultLowest;

        public bool ShouldStopProcessing(MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Entity && !obj.Hierarchy.HasChildren;
        }

        public Task Convert(SceneBuilder builder, MapDocument document, IMapObject obj)
        {
            var entity = (Entity) obj;

            // It's always a box, these numbers are known
            const uint numVertices = 4 * 6;

            // Pack the indices like this [ solid1 ... solidn ] [ wireframe1 ... wireframe n ]
            const uint numSolidIndices = 36;
            const uint numWireframeIndices = numVertices * 2;

            var points = new VertexStandard[numVertices];
            var indices = new uint[numSolidIndices + numWireframeIndices];

            var c = entity.IsSelected ? Color.Red : entity.Color?.Color ?? Color.Magenta;
            var colour = new Vector4(c.R, c.G, c.B, c.A) / 255f;

            //c = entity.IsSelected ? Color.FromArgb(255, 128, 128) : Color.White;
            c = Color.White;
            var tint = new Vector4(c.R, c.G, c.B, c.A) / 255f;
            
            var flags = entity.IsSelected ? VertexFlags.SelectiveTransformed : VertexFlags.None;

            var vi = 0u;
            var si = 0u;
            var wi = numSolidIndices;
            foreach (var face in entity.BoundingBox.GetBoxFaces())
            {
                var offs = vi;

                var normal = new Plane(face[0], face[1], face[2]).Normal;
                foreach (var v in face)
                {
                    points[vi++] = new VertexStandard
                    {
                        Position = v,
                        Colour = colour,
                        Normal = normal,
                        Texture = Vector2.Zero,
                        Tint = tint,
                        Flags = flags
                    };
                }

                // Triangles - [0 1 2]  ... [0 n-1 n]
                for (uint i = 2; i < 4; i++)
                {
                    indices[si++] = offs;
                    indices[si++] = offs + i - 1;
                    indices[si++] = offs + i;
                }

                // Lines - [0 1] ... [n-1 n] [n 0]
                for (uint i = 0; i < 4; i++)
                {
                    indices[wi++] = offs + i;
                    indices[wi++] = offs + (i == 4 - 1 ? 0 : i + 1);
                }
            }

            var groups = new[]
            {
                new BufferGroup(PipelineType.FlatColourGeneric, CameraType.Perspective, false, entity.Origin, 0, numSolidIndices),
                new BufferGroup(PipelineType.WireframeGeneric, entity.IsSelected ? CameraType.Both : CameraType.Orthographic, false, entity.Origin, numSolidIndices, numWireframeIndices)
            };

            builder.MainBuffer.Append(points, indices, groups);

            return Task.FromResult(0);
        }
    }
}