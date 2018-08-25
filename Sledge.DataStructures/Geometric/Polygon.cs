using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.Geometric
{
    /// <summary>
    /// Represents a coplanar, directed polygon with at least 3 vertices.
    /// </summary>
    [Serializable]
    public class Polygon : ISerializable
    {
        public List<Vector3> Vertices { get; set; }

        /// <summary>
        /// Creates a polygon from a list of points
        /// </summary>
        /// <param name="vertices">The vertices of the polygon</param>
        public Polygon(IEnumerable<Vector3> vertices)
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
        public Polygon(Plane plane, float radius = 3000f)
        {
            // Get aligned up and right axes to the plane
            var direction = plane.GetClosestAxisToNormal();
            var tempV = direction == Vector3.UnitZ ? -Vector3.UnitY : -Vector3.UnitZ;
            var up = tempV.Cross(plane.Normal).Normalise();
            var right = plane.Normal.Cross(up).Normalise();

            Vertices = new List<Vector3>
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
            Vertices = ((Vector3[]) info.GetValue("Vertices", typeof (Vector3[]))).ToList();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Vertices", Vertices.ToArray());
        }

        public Polygon Clone()
        {
            return new Polygon(new List<Vector3>(Vertices));
        }

        public void Unclone(Polygon polygon)
        {
            Vertices = new List<Vector3>(polygon.Vertices);
        }

        public Plane GetPlane()
        {
            return new Plane(Vertices[0], Vertices[1], Vertices[2]);
        }

        /// <summary>
        /// Returns the origin of this polygon.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetOrigin()
        {
            return Vertices.Aggregate(Vector3.Zero, (x, y) => x + y) / Vertices.Count;
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
            if (Vertices.Count < 3) return false;
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

        public bool IsConvex(float epsilon = 0.001f)
        {
            if (Vertices.Count < 3) return false;

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
        public void Expand(float radius)
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
            return ClassifyAgainstPlane(p, out _, out _, out _, out _);
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
        /// Splits this polygon by a clipping plane, returning the back and front planes.
        /// The original polygon is not modified.
        /// </summary>
        /// <param name="clip">The clipping plane</param>
        /// <param name="back">The back polygon</param>
        /// <param name="front">The front polygon</param>
        /// <returns>True if the split was successful</returns>
        public bool Split(Plane clip, out Polygon back, out Polygon front)
        {
            return Split(clip, out back, out front, out _, out _);
        }

        private struct Vector3d
        {
            public decimal X;
            public decimal Y;
            public decimal Z;

            public Vector3d(decimal x, decimal y, decimal z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public static Vector3d operator +(Vector3d left, Vector3d right)
            {
                return new Vector3d(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
            }

            public static Vector3d operator *(Vector3d left, decimal right)
            {
                return new Vector3d(left.X * right, left.Y * right, left.Z * right);
            }

            public override string ToString()
            {
                return $"<{X}, {Y}, {Z}>";
            }
        }

        private struct Vector4d
        {
            public decimal X;
            public decimal Y;
            public decimal Z;
            public decimal W;

            public Vector4d(decimal x, decimal y, decimal z, decimal w)
            {
                X = x;
                Y = y;
                Z = z;
                W = w;
            }
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
            const decimal epsilon = (decimal) NumericsExtensions.Epsilon;
            
            var plane = new Vector4d((decimal) clip.A, (decimal) clip.B, (decimal) clip.C, (decimal) clip.D);
            var verts = Vertices.Select(x => new Vector3d((decimal) x.X, (decimal) x.Y, (decimal) x.Z)).ToList();
            var distances = verts.Select(x => plane.X * x.X + plane.Y * x.Y + plane.Z * x.Z + plane.W).ToList();
            
            int cb = 0, cf = 0;
            for (var i = 0; i < distances.Count; i++)
            {
                if (distances[i] < -epsilon) cb++;
                else if (distances[i] > epsilon) cf++;
                else distances[i] = 0;
            }

            Console.WriteLine(string.Join(" ; ", distances));

            // Check non-spanning cases
            if (cb == 0 && cf == 0)
            {
                // Co-planar
                back = front = coplanarBack = coplanarFront = null;
                if (GetPlane().Normal.Dot(clip.Normal) > 0) coplanarFront = this;
                else coplanarBack = this;
                return false;
            }
            else if (cb == 0)
            {
                // All vertices in front
                back = coplanarBack = coplanarFront = null;
                front = this;
                return false;
            }
            else if (cf == 0)
            {
                // All vertices behind
                front = coplanarBack = coplanarFront = null;
                back = this;
                return false;
            }

            //var count = Vertices.Count;
            //
            var classify = ClassifyAgainstPlane(clip, out var classifications, out _, out _, out _);
            
            //// If the polygon doesn't span the plane, return false.
            //if (classify != PlaneClassification.Spanning)
            //{
            //    back = front = null;
            //    coplanarBack = coplanarFront = null;
            //    if (classify == PlaneClassification.Back) back = this;
            //    else if (classify == PlaneClassification.Front) front = this;
            //    else if (GetPlane().Normal.Dot(clip.Normal) > 0) coplanarFront = this;
            //    else coplanarBack = this;
            //    return false;
            //}
            //

            // Get the new front and back vertices
            var backVerts = new List<Vector3d>();
            var frontVerts = new List<Vector3d>();

            for (var i = 0; i < verts.Count; i++)
            {
                var j = (i + 1) % verts.Count;

                Vector3d s = verts[i], e = verts[j];
                decimal sd = distances[i], ed = distances[j];

                if (sd <= 0) backVerts.Add(s);
                if (sd >= 0) frontVerts.Add(s);

                if (sd <= 0 != ed <= 0)
                {
                    var t = sd / (sd - ed);
                    var intersect = s * (1 - t) + e * t;

                    backVerts.Add(intersect);
                    frontVerts.Add(intersect);
                }
            }


            //var prev = 0;
            //
            //for (var i = 0; i <= count; i++)
            //{
            //    var idx = i % count;
            //    var end = Vertices[idx];
            //    var cls = classifications[idx];
            //
            //    // Check plane crossing
            //    if (i > 0 && cls != 0 && prev != 0 && prev != cls)
            //    {
            //        // This line end point has crossed the plane
            //        // Add the line intersect to the 
            //        var start = Vertices[i - 1];
            //        var line = new Line(start, end);
            //        var isect = clip.GetIntersectionPoint(line, true);
            //        if (isect == null) throw new Exception("Expected intersection, got null.");
            //        frontVerts.Add(isect.Value);
            //        backVerts.Add(isect.Value);
            //    }
            //
            //    // Add original points
            //    if (i < Vertices.Count)
            //    {
            //        // OnPlane points get put in both polygons, doesn't generate split
            //        if (cls >= 0) frontVerts.Add(end);
            //        if (cls <= 0) backVerts.Add(end);
            //    }
            //
            //    prev = cls;
            //}

            back = new Polygon(backVerts.Select(x => new Vector3((float) x.X, (float) x.Y, (float) x.Z)));
            front = new Polygon(frontVerts.Select(x => new Vector3((float)x.X, (float)x.Y, (float)x.Z)));
            coplanarBack = coplanarFront = null;

            return true;
        }

        public void Flip()
        {
            Vertices.Reverse();
        }

        public Vector3? GetIntersectionPoint(Line line, bool ignoreDirection = false)
        {
            if (Vertices.Count < 3) return null;

            var plane = GetPlane();
            var isect = plane.GetIntersectionPoint(line, ignoreDirection);
            if (isect == null) return null;

            var intersect = isect.Value;

            var vectors = Vertices;

            // http://paulbourke.net/geometry/insidepoly/

            // The angle sum will be 2 * PI if the point is inside the face
            double sum = 0;
            for (var i = 0; i < vectors.Count; i++)
            {
                var i1 = i;
                var i2 = (i + 1) % vectors.Count;

                // Translate the vertices so that the intersect point is on the origin
                var v1 = vectors[i1] - intersect;
                var v2 = vectors[i2] - intersect;

                var m1 = v1.Length();
                var m2 = v2.Length();
                var nom = m1 * m2;
                if (nom < 0.001f)
                {
                    // intersection is at a vertex
                    return intersect;
                }
                sum += Math.Acos(v1.Dot(v2) / nom);
            }

            var delta = Math.Abs(sum - Math.PI * 2);
            return (delta < 0.001d) ? intersect : (Vector3?) null;
        }
    }
}
