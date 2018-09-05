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

            front = MakeSolid(generator, solid, frontPoly);
            back = MakeSolid(generator, solid, backPoly);
            return true;
        }

        private static Solid MakeSolid(UniqueNumberGenerator generator, Solid original, DataStructures.Geometric.Precision.Polyhedron poly)
        {
            var originalFacePlanes = original.Faces.Select(x => new
            {
                Face = x,
                Plane = x.Plane.ToPrecisionPlane()
            }).ToList();

            var solid = new Solid(generator.Next("MapObject")) { IsSelected = original.IsSelected };
            foreach (var p in poly.Polygons)
            {
                // Use the first face if we can't find any with the same plane (it's the clipping plane)
                var originalFace = originalFacePlanes
                                       .Where(x => p.ClassifyAgainstPlane(x.Plane) == PlaneClassification.OnPlane)
                                       .Select(x => x.Face)
                                       .FirstOrDefault() ?? original.Faces.FirstOrDefault();

                var face = new Face(generator.Next("Face"));
                face.Vertices.AddRange(p.Vertices.Select(x => x.ToStandardVector3()));
                face.Plane = p.Plane.ToStandardPlane();

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