using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.DataStructures.Transformations;

namespace Sledge.DataStructures.Geometric
{
    /// <summary>
    /// Represents a convex polyhedron with at least 4 sides
    /// </summary>
    [Serializable]
    public class Polyhedron : ISerializable
    {
        public List<Polygon> Polygons { get; set; }

        /// <summary>
        /// Creates a polyhedron from a list of polygons which are assumed to be valid.
        /// </summary>
        public Polyhedron(IEnumerable<Polygon> polygons)
        {
            Polygons = polygons.ToList();
        }

        /// <summary>
        /// Creates a polyhedron by intersecting a set of at least 4 planes.
        /// </summary>
        public Polyhedron(IEnumerable<Plane> planes)
        {
            Polygons = new List<Polygon>();

            var list = planes.ToList();
            for (var i = 0; i < list.Count; i++)
            {
                // Split the polygon by all the other planes
                var poly = new Polygon(list[i]);
                for (var j = 0; j < list.Count; j++)
                {
                    if (i != j) poly.Split(list[j]);
                }
                Polygons.Add(poly);
            }

            // Ensure all the faces point outwards
            var origin = GetOrigin();
            foreach (var face in Polygons)
            {
                if (face.GetPlane().OnPlane(origin) >= 0) face.Flip();
            }
        }

        protected Polyhedron(SerializationInfo info, StreamingContext context)
        {
            Polygons = ((Polygon[])info.GetValue("Polygons", typeof(Polygon[]))).ToList();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Polygons", Polygons.ToArray());
        }

        public Polyhedron Clone()
        {
            return new Polyhedron(Polygons.Select(x => x.Clone()));
        }

        /// <summary>
        /// Returns the origin of this polyhedron.
        /// </summary>
        /// <returns></returns>
        public Coordinate GetOrigin()
        {
            return Polygons.Aggregate(Coordinate.Zero, (x, y) => x + y.GetOrigin()) / Polygons.Count;
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
            const decimal epsilon = 0.5m;
            return !GetCoplanarPolygons().Any()
                   && !GetBackwardsPolygons(epsilon).Any()
                   && Polygons.All(x => x.IsConvex() && x.IsValid());
        }

        /// <summary>
        /// Transforms all the points in the polyhedron.
        /// </summary>
        /// <param name="transform">The transformation to perform</param>
        public void Transform(IUnitTransformation transform)
        {
            Polygons.ForEach(x => x.Transform(transform));

            // Handle flip transforms / negative scales
            var origin = GetOrigin();
            if (Polygons.All(x => x.GetPlane().OnPlane(origin) >= 0))
            {
                // All planes are facing inwards - flip them all
                Polygons.ForEach(x => x.Flip());
            }
        }

        /// <summary>
        /// Returns the intersection point closest to the start of the line.
        /// </summary>
        /// <param name="line">The intersection line</param>
        /// <returns>The closest intersecting point, or null if the line doesn't intersect.</returns>
        public Coordinate GetIntersectionPoint(Line line)
        {
            return Polygons.Select(x => x.GetIntersectionPoint(line))
                .Where(x => x != null)
                .OrderBy(x => (x - line.Start).VectorMagnitude())
                .FirstOrDefault();
        }

        public IEnumerable<Polygon> GetCoplanarPolygons()
        {
            return Polygons.Where(f1 => Polygons.Where(f2 => f2 != f1).Any(f2 => f2.GetPlane() == f1.GetPlane()));
        }

        public IEnumerable<Polygon> GetBackwardsPolygons(decimal epsilon = 0.001m)
        {
            var origin = GetOrigin();
            return Polygons.Where(x => x.GetPlane().OnPlane(origin, epsilon) > 0);
        }

        /// <summary>
        /// Splits this polyhedron into two polyhedron by intersecting against a plane.
        /// </summary>
        /// <param name="plane">The splitting plane</param>
        /// <param name="back">The back side of the polyhedron</param>
        /// <param name="front">The front side of the polyhedron</param>
        /// <returns>True if the plane splits the polyhedron, false if the plane doesn't intersect</returns>
        public bool Split(Plane plane, out Polyhedron back, out Polyhedron front)
        {
            back = front = null;

            // Check that this solid actually spans the plane
            var classify = Polygons.Select(x => x.ClassifyAgainstPlane(plane)).Distinct().ToList();
            if (classify.All(x => x != PlaneClassification.Spanning))
            {
                if (classify.Any(x => x == PlaneClassification.Back)) back = this;
                else if (classify.Any(x => x == PlaneClassification.Front)) front = this;
                return false;
            }

            var backPlanes = new List<Plane> { plane };
            var frontPlanes = new List<Plane> { new Plane(-plane.Normal, -plane.DistanceFromOrigin) };

            foreach (var face in Polygons)
            {
                var classification = face.ClassifyAgainstPlane(plane);
                if (classification != PlaneClassification.Back) frontPlanes.Add(face.GetPlane());
                if (classification != PlaneClassification.Front) backPlanes.Add(face.GetPlane());
            }

            back = new Polyhedron(backPlanes);
            front = new Polyhedron(frontPlanes);
            
            return true;
        }
    }
}
