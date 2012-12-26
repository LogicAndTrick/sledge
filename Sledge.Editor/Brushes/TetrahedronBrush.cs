using System.Collections.Generic;
using System.Linq;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes.Controls;

namespace Sledge.Editor.Brushes
{
    public class TetrahedronBrush : IBrush
    {
        public string Name
        {
            get { return "Tetrahedron"; }
        }

        public IEnumerable<BrushControl> GetControls()
        {
            return new List<BrushControl>();
        }

        public IEnumerable<MapObject> Create(Box box, ITexture texture)
        {
            var solid = new Solid { Colour = Colour.GetRandomBrushColour() };
            // The higher Z plane will be triangle, with the lower X value getting the two corners
            var c1 = new Coordinate(box.Start.X, box.Start.Y, box.End.Z);
            var c2 = new Coordinate(box.End.X, box.Start.Y, box.End.Z);
            var c3 = new Coordinate(box.Center.X, box.End.Y, box.End.Z);
            var c4 = new Coordinate(box.Center.X, box.Center.Y, box.Start.Z);
            var faces = new[]
                            {
                                new[] { c3, c2, c1 },
                                new[] { c3, c1, c4 },
                                new[] { c2, c3, c4 },
                                new[] { c1, c2, c4 }
                            };
            foreach (var arr in faces)
            {
                var face = new Face
                               {
                                   Parent = solid,
                                   Plane = new Plane(arr[0], arr[1], arr[2]),
                                   Colour = solid.Colour,
                                   Texture = { Texture = texture }
                               };
                face.Vertices.AddRange(arr.Select(x => new Vertex(x, face)));
                face.UpdateBoundingBox();
                face.AlignTextureToFace();
                solid.Faces.Add(face);
            }
            solid.UpdateBoundingBox();
            yield return solid;
        }
    }
}