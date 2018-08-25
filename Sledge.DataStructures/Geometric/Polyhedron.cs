using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.Geometric
{
    /// <summary>
    /// Represents a convex polyhedron with at least 4 sides.
    /// </summary>
    [Serializable]
    public class Polyhedron : ISerializable
    {
        public IReadOnlyList<Polygon> Polygons { get; }

        /// <summary>
        /// Gets the origin of this polyhedron.
        /// </summary>
        public Vector3 Origin => Polygons.Aggregate(Vector3.Zero, (x, y) => x + y.Origin) / Polygons.Count;

        /// <summary>
        /// Creates a polyhedron from a list of polygons which are assumed to be valid.
        /// </summary>
        public Polyhedron(IEnumerable<Polygon> polygons)
        {
            Polygons = polygons.ToList();
        }

        /// <summary>
        /// Creates a polyhedron by intersecting a set of at least 4 planes.
        /// This constructor uses high precision operations for plane intersections.
        /// </summary>
        public Polyhedron(IEnumerable<Plane> planes)
        {
            Polygons = new Precision.Polyhedron(planes.Select(x => x.ToPrecisionPlane()))
                .Polygons
                .Select(x => x.ToStandardPolygon())
                .ToList();
        }

        protected Polyhedron(SerializationInfo info, StreamingContext context)
        {
            Polygons = ((Polygon[])info.GetValue("Polygons", typeof(Polygon[]))).ToList();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Polygons", Polygons.ToArray());
        }

        /// <summary>
        /// Get the lines representing the edges of this polyhedron.
        /// </summary>
        /// <returns>A list of lines</returns>
        public IEnumerable<Line> GetLines()
        {
            return Polygons.SelectMany(x => x.GetLines()).Distinct();
        }

        /// <summary>
        /// Checks that all the points in this polyhedron are valid.
        /// </summary>
        /// <returns>True if the polyhedron is valid</returns>
        public bool IsValid()
        {
            const float epsilon = 0.5f;
            return !GetCoplanarPolygons().Any()
                   && !GetBackwardsPolygons(epsilon).Any()
                   && Polygons.All(x => x.IsConvex() && x.IsValid());
        }

        /// <summary>
        /// Returns the intersection point closest to the start of the line.
        /// </summary>
        /// <param name="line">The intersection line</param>
        /// <returns>The closest intersecting point, or null if the line doesn't intersect.</returns>
        public Vector3? GetIntersectionPoint(Line line)
        {
            return Polygons.Select(x => x.GetIntersectionPoint(line))
                .Where(x => x != null)
                .OrderBy(x => (x.Value - line.Start).Length())
                .FirstOrDefault();
        }

        public IEnumerable<Polygon> GetCoplanarPolygons()
        {
            return Polygons.Where(f1 => Polygons.Where(f2 => f2 != f1).Any(f2 => f2.Plane== f1.Plane));
        }

        public IEnumerable<Polygon> GetBackwardsPolygons(float epsilon = 0.001f)
        {
            var origin = Origin;
            return Polygons.Where(x => x.Plane.OnPlane(origin, epsilon) > 0);
        }

        public Precision.Polyhedron ToPrecisionPolyhedron()
        {
            return new Precision.Polyhedron(Polygons.Select(x => x.ToPrecisionPolygon()));
        }
    }
}
