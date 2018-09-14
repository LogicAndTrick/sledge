using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.ChangeHandlers;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.DataStructures.Geometric;
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

        public Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj, ResourceCollector resourceCollector)
        {
            return ConvertBox(builder, obj, obj.BoundingBox);
        }

        internal static Task ConvertBox(BufferBuilder builder, IMapObject obj, Box box)
        {
            // It's always a box, these numbers are known
            const uint numVertices = 4 * 6;

            // Pack the indices like this [ solid1 ... solidn ] [ wireframe1 ... wireframe n ]
            const uint numSolidIndices = 36;
            const uint numWireframeIndices = numVertices * 2;

            var points = new VertexStandard[numVertices];
            var indices = new uint[numSolidIndices + numWireframeIndices];

            var c = obj.IsSelected ? Color.Red : obj.Data.GetOne<ObjectColor>()?.Color ?? Color.Magenta;
            var colour = new Vector4(c.R, c.G, c.B, c.A) / 255f;
            
            var flags = obj.IsSelected ? VertexFlags.SelectiveTransformed : VertexFlags.None;

            var vi = 0u;
            var si = 0u;
            var wi = numSolidIndices;
            foreach (var face in box.GetBoxFaces())
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
                        Tint = Vector4.One,
                        Flags = flags | VertexFlags.FlatColour
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

            var origin = obj.Data.GetOne<Origin>()?.Location ?? box.Center;

            var groups = new List<BufferGroup>();

            if (!obj.Data.OfType<IContentsReplaced>().Any(x => x.ContentsReplaced))
            {
                groups.Add(new BufferGroup(PipelineType.TexturedOpaque, CameraType.Perspective, 0, numSolidIndices));
            }
            
            groups.Add(new BufferGroup(PipelineType.Wireframe, obj.IsSelected ? CameraType.Both : CameraType.Orthographic, numSolidIndices, numWireframeIndices));

            builder.Append(points, indices, groups);
            
            // Also push the untransformed wireframe when selected
            if (obj.IsSelected)
            {
                for (var i = 0; i < points.Length; i++) points[i].Flags = VertexFlags.None;
                var untransformedIndices = indices.Skip((int)numSolidIndices);
                builder.Append(points, untransformedIndices, new[]
                {
                    new BufferGroup(PipelineType.Wireframe, CameraType.Both, 0, numWireframeIndices)
                });
            }

            return Task.FromResult(0);
        }
    }
}