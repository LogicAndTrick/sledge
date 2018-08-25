using System.Linq;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives
{
    public static class SolidExtensions
    {
        public static bool Split(this Solid solid, UniqueNumberGenerator generator, Plane plane, out Solid back, out Solid front)
        {
            back = front = null;
            var pln = plane.ToPrecisionPlane();
            var poly = solid.ToPolyhedron().ToPrecisionPolyhedron();

            if (!poly.Split(pln, out var backPoly, out var frontPoly))
            {
                if (backPoly != null) back = solid;
                else if (frontPoly != null) front = solid;
                return false;
            }

            front = MakeSolid(generator, solid, frontPoly.ToStandardPolyhedron());
            back = MakeSolid(generator, solid, backPoly.ToStandardPolyhedron());
            return true;
        }

        private static Solid MakeSolid(UniqueNumberGenerator generator, Solid original, Polyhedron poly)
        {
            var solid = new Solid(generator.Next("MapObject")) { IsSelected = original.IsSelected };
            foreach (var p in poly.Polygons)
            {
                // Use the first face if we can't find any with the same plane (it's the clipping plane)
                var originalFace =
                    original.Faces.FirstOrDefault(x => p.ClassifyAgainstPlane(x.Plane) == PlaneClassification.OnPlane)
                    ?? original.Faces.FirstOrDefault();

                var face = new Face(generator.Next("Face"));
                face.Vertices.AddRange(p.Vertices);
                face.Plane = p.Plane;

                if (originalFace != null)
                {
                    face.Texture = originalFace.Texture.Clone();
                }

                solid.Data.Add(face);
            }

            // Add any extra data (visgroups, colour, etc)
            foreach (var data in original.Data.Where(x => !(x is Face)))
            {
                solid.Data.Add((IMapObjectData)data.Clone());
            }
            return solid;
        }

        public static bool IsValid(this Solid solid)
        {
            return solid.ToPolyhedron().IsValid();
        }
    }
}