using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.DataStructures.Transformations;

namespace Sledge.DataStructures.Geometric
{
    /// <summary>
    /// Represents a coplanar, directed polygon with at least 3 vertices.
    /// </summary>
    [Serializable]
    public class Polygon : ISerializable
    {
        public List<Coordinate> Vertices { get; set; }

        /// <summary>
        /// Creates a polygon from a list of points
        /// </summary>
        /// <param name="vertices">The vertices of the polygon</param>
        public Polygon(IEnumerable<Coordinate> vertices)
        {
            Vertices = vertices.ToList();
            Simplify();
        }

        /// <summary>
        /// Creates a polygon from a plane and a radius.
        /// Expands the plane to the radius size to create a large polygon with 4 vertices.
        /// </summary>
        /// <param name="plane">The polygon plane</param>
        /// <param name="radius">The polygon radius</param>
        public Polygon(Plane plane, decimal radius = 1000000m)
        {
            // Get aligned up and right axes to the plane
            var direction = plane.GetClosestAxisToNormal();
            var tempV = direction == Coordinate.UnitZ ? -Coordinate.UnitY : -Coordinate.UnitZ;
            var up = tempV.Cross(plane.Normal).Normalise();
            var right = plane.Normal.Cross(up).Normalise();

            Vertices = new List<Coordinate>
                           {
                               plane.PointOnPlane + right + up, // Top right
                               plane.PointOnPlane - right + up, // Top left
                               plane.PointOnPlane - right - up, // Bottom left
                               plane.PointOnPlane + right - up, // Bottom right
                           };
            Expand(radius);
        }

        protected Polygon(SerializationInfo info, StreamingContext context)
        {
            Vertices = ((Coordinate[]) info.GetValue("Vertices", typeof (Coordinate[]))).ToList();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Vertices", Vertices.ToArray());
        }

        public Polygon Clone()
        {
            return new Polygon(new List<Coordinate>(Vertices));
        }

        public void Unclone(Polygon polygon)
        {
            Vertices = new List<Coordinate>(polygon.Vertices);
        }

        public Plane GetPlane()
        {
            return new Plane(Vertices[0], Vertices[1], Vertices[2]);
        }

        /// <summary>
        /// Returns the origin of this polygon.
        /// </summary>
        /// <returns></returns>
        public Coordinate GetOrigin()
        {
            return Vertices.Aggregate(Coordinate.Zero, (x, y) => x + y) / Vertices.Count;
        }

        /// <summary>
        /// Get the lines representing the edges of this polygon.
        /// </summary>
        /// <returns>A list of lines</returns>
        public IEnumerable<Line> GetLines()
        {
            for (var i = 1; i < Vertices.Count; i++)
            {
                yield return new Line(Vertices[i - 1], Vertices[i]);
            }
        }

        /// <summary>
        /// Checks that all the points in this polygon are valid.
        /// </summary>
        /// <returns>True if the polygon is valid</returns>
        public bool IsValid()
        {
            var plane = GetPlane();
            return Vertices.All(x => plane.OnPlane(x) == 0);
        }

        /// <summary>
        /// Removes any colinear vertices in the polygon
        /// </summary>
        public void Simplify()
        {
            // Remove colinear vertices
            for (var i = 0; i < Vertices.Count - 2; i++)
            {
                var v1 = Vertices[i];
                var v2 = Vertices[i + 2];
                var p = Vertices[i + 1];
                var line = new Line(v1, v2);
                // If the midpoint is on the line, remove it
                if (line.ClosestPoint(p).EquivalentTo(p))
                {
                    Vertices.RemoveAt(i + 1);
                }
            }
        }

        /// <summary>
        /// Transforms all the points in the polygon.
        /// </summary>
        /// <param name="transform">The transformation to perform</param>
        public void Transform(IUnitTransformation transform)
        {
            Vertices = Vertices.Select(transform.Transform).ToList();
        }

        public bool IsConvex(decimal epsilon = 0.001m)
        {
            var plane = GetPlane();
            for (var i = 0; i < Vertices.Count; i++)
            {
                var v1 = Vertices[i];
                var v2 = Vertices[(i + 1) % Vertices.Count];
                var v3 = Vertices[(i + 2) % Vertices.Count];
                var l1 = (v1 - v2).Normalise();
                var l2 = (v3 - v2).Normalise();
                var cross = l1.Cross(l2);
                if (plane.OnPlane(v2 + cross, epsilon) < 0.0001m) return false;
            }
            return true;
        }

        /// <summary>
        /// Expands this plane's points outwards from the origin by a radius value.
        /// </summary>
        /// <param name="radius">The distance the points will be from the origin after expanding</param>
        public void Expand(decimal radius)
        {
            // 1. Center the polygon at the world origin
            // 2. Normalise all the vertices
            // 3. Multiply them by the radius
            // 4. Move the polygon back to the original origin
            var origin = GetOrigin();
            Vertices = Vertices.Select(x => (x - origin).Normalise() * radius + origin).ToList();
        }

        /// <summary>
        /// Determines if this polygon is behind, in front, or spanning a plane.
        /// </summary>
        /// <param name="p">The plane to test against</param>
        /// <returns>A PlaneClassification value.</returns>
        public PlaneClassification ClassifyAgainstPlane(Plane p)
        {
            int front, back, onplane;
            int[] classifications;
            return ClassifyAgainstPlane(p, out classifications, out front, out back, out onplane);
        }

        /// <summary>
        /// Determines if this polygon is behind, in front, or spanning a plane. Returns calculated data.
        /// </summary>
        /// <param name="p">The plane to test against</param>
        /// <param name="classifications">The OnPlane classification for each vertex</param>
        /// <param name="front">The number of vertices in front of the plane</param>
        /// <param name="back">The number of vertices behind the plane</param>
        /// <param name="onplane">The number of vertices on the plane</param>
        /// <returns>A PlaneClassification value.</returns>
        private PlaneClassification ClassifyAgainstPlane(Plane p, out int[] classifications, out int front, out int back, out int onplane)
        {
            var count = Vertices.Count;
            front = 0;
            back = 0;
            onplane = 0;
            classifications = new int[count];

            for (var i = 0; i < Vertices.Count; i++)
            {
                var test = p.OnPlane(Vertices[i]);

                // Vertices on the plane are both in front and behind the plane in this context
                if (test <= 0) back++;
                if (test >= 0) front++;
                if (test == 0) onplane++;

                classifications[i] = test;
            }

            if (onplane == count) return PlaneClassification.OnPlane;
            if (front == count) return PlaneClassification.Front;
            if (back == count) return PlaneClassification.Back;
            return PlaneClassification.Spanning;
        }

        /// <summary>
        /// Splits this polygon by a clipping plane, discarding the front side.
        /// The original polygon is modified to be the back side of the split.
        /// If the front side doesn't exist then the polygon won't be modified.
        /// </summary>
        /// <param name="clip">The clipping plane</param>
        public void Split(Plane clip)
        {
            // Ideally we would just be able to use Split(..., out back) and Unclone(back) but that takes twice as long.
            // So we copy the whole algorithm and do it inline instead

            int frontCount, backCount, onPlaneCount, count = Vertices.Count;
            int[] classifications;

            var classify = ClassifyAgainstPlane(clip, out classifications, out frontCount, out backCount, out onPlaneCount);

            // If the polygon doesn't span the plane don't do anything
            if (classify != PlaneClassification.Spanning)
            {
                return;
            }

            // Get the new back vertices
            var backVerts = new List<Coordinate>();

            var prev = 0;

            for (var i = 0; i <= count; i++)
            {
                var idx = i % count;
                var end = Vertices[idx];
                var cls = classifications[idx];

                // Check plane crossing
                if (i > 0 && cls != 0 && prev != 0 && prev != cls)
                {
                    // This line end point has crossed the plane
                    // Add the line intersect to the 
                    var start = Vertices[i - 1];
                    var line = new Line(start, end);
                    var isect = clip.GetIntersectionPoint(line, true);
                    if (isect == null) throw new Exception("Expected intersection, got null.");
                    backVerts.Add(isect);
                }

                // Add original points
                if (i < Vertices.Count)
                {
                    if (cls <= 0) backVerts.Add(end);
                }

                prev = cls;
            }

            // Swap out the vertices
            Vertices = backVerts;
        }

        /// <summary>
        /// Splits this polygon by a clipping plane, returning the back and front planes.
        /// The original polygon is not modified.
        /// </summary>
        /// <param name="clip">The clipping plane</param>
        /// <param name="back">The back polygon</param>
        /// <param name="front">The front polygon</param>
        /// <returns>True if the split was successful</returns>
        public bool Split(Plane clip, out Polygon back, out Polygon front)
        {
            Polygon cFront, cBack;
            return Split(clip, out back, out front, out cBack, out cFront);
        }

        /// <summary>
        /// Splits this polygon by a clipping plane, returning the back and front planes.
        /// The original polygon is not modified.
        /// </summary>
        /// <param name="clip">The clipping plane</param>
        /// <param name="back">The back polygon</param>
        /// <param name="front">The front polygon</param>
        /// <param name="coplanarBack">If the polygon rests on the plane and points backward, this will not be null</param>
        /// <param name="coplanarFront">If the polygon rests on the plane and points forward, this will not be null</param>
        /// <returns>True if the split was successful</returns>
        public bool Split(Plane clip, out Polygon back, out Polygon front, out Polygon coplanarBack, out Polygon coplanarFront)
        {
            int frontCount, backCount, onPlaneCount, count = Vertices.Count;
            int[] classifications;

            var classify = ClassifyAgainstPlane(clip, out classifications, out frontCount, out backCount, out onPlaneCount);

            // If the polygon doesn't span the plane, return false.
            if (classify != PlaneClassification.Spanning)
            {
                back = front = null;
                coplanarBack = coplanarFront = null;
                if (classify == PlaneClassification.Back) back = this;
                else if (classify == PlaneClassification.Front) front = this;
                else if (GetPlane().Normal.Dot(clip.Normal) > 0) coplanarFront = this;
                else coplanarBack = this;
                return false;
            }

            // Get the new front and back vertices
            var backVerts = new List<Coordinate>();
            var frontVerts = new List<Coordinate>();
            var prev = 0;

            for (var i = 0; i <= count; i++)
            {
                var idx = i % count;
                var end = Vertices[idx];
                var cls = classifications[idx];

                // Check plane crossing
                if (i > 0 && cls != 0 && prev != 0 && prev != cls)
                {
                    // This line end point has crossed the plane
                    // Add the line intersect to the 
                    var start = Vertices[i - 1];
                    var line = new Line(start, end);
                    var isect = clip.GetIntersectionPoint(line, true);
                    if (isect == null) throw new Exception("Expected intersection, got null.");
                    frontVerts.Add(isect);
                    backVerts.Add(isect);
                }

                // Add original points
                if (i < Vertices.Count)
                {
                    // OnPlane points get put in both polygons, doesn't generate split
                    if (cls >= 0) frontVerts.Add(end);
                    if (cls <= 0) backVerts.Add(end);
                }

                prev = cls;
            }

            back = new Polygon(backVerts);
            front = new Polygon(frontVerts);
            coplanarBack = coplanarFront = null;

            return true;
        }

        public void Flip()
        {
            Vertices.Reverse();
        }

        public Coordinate GetIntersectionPoint(Line line, bool ignoreDirection = false)
        {
            var plane = GetPlane();
            var intersect = plane.GetIntersectionPoint(line, ignoreDirection);
            if (intersect == null) return null;

            var coordinates = Vertices;

            // http://paulbourke.net/geometry/insidepoly/

            // The angle sum will be 2 * PI if the point is inside the face
            double sum = 0;
            for (var i = 0; i < coordinates.Count; i++)
            {
                var i1 = i;
                var i2 = (i + 1) % coordinates.Count;

                // Translate the vertices so that the intersect point is on the origin
                var v1 = coordinates[i1] - intersect;
                var v2 = coordinates[i2] - intersect;

                var m1 = v1.VectorMagnitude();
                var m2 = v2.VectorMagnitude();
                var nom = m1 * m2;
                if (nom < 0.001m)
                {
                    // intersection is at a vertex
                    return intersect;
                }
                sum += Math.Acos((double)(v1.Dot(v2) / nom));
            }

            var delta = Math.Abs(sum - Math.PI * 2);
            return (delta < 0.001d) ? intersect : null;
        }
    }
}
