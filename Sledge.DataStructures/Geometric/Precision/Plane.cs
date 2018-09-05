using System;

namespace Sledge.DataStructures.Geometric.Precision
{
    /// <summary>
    /// Defines a plane in the form Ax + By + Cz + D = 0. Uses high-precision value types.
    /// </summary>
    public struct Plane
    {
        public Vector3 Normal { get; }
        public double DistanceFromOrigin { get; }
        public double A { get; }
        public double B { get; }
        public double C { get; }
        public double D { get; }
        public Vector3 PointOnPlane { get; }
        
        public Plane(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var ab = p2 - p1;
            var ac = p3 - p1;

            Normal = ac.Cross(ab).Normalise();
            DistanceFromOrigin = Normal.Dot(p1);
            PointOnPlane = p1;

            A = Normal.X;
            B = Normal.Y;
            C = Normal.Z;
            D = -DistanceFromOrigin;
        }

        public Plane(Vector3 norm, Vector3 pointOnPlane)
        {
            Normal = norm.Normalise();
            DistanceFromOrigin = Normal.Dot(pointOnPlane);
            PointOnPlane = pointOnPlane;

            A = Normal.X;
            B = Normal.Y;
            C = Normal.Z;
            D = -DistanceFromOrigin;
        }
        
        public Plane(Vector3 norm, double distanceFromOrigin)
        {
            Normal = norm.Normalise();
            DistanceFromOrigin = distanceFromOrigin;
            PointOnPlane = Normal * DistanceFromOrigin;

            A = Normal.X;
            B = Normal.Y;
            C = Normal.Z;
            D = -DistanceFromOrigin;
        }

        ///  <summary>Finds if the given point is above, below, or on the plane.</summary>
        ///  <param name="co">The Vector3 to test</param>
        /// <param name="epsilon">Tolerance value</param>
        /// <returns>
        ///  value == -1 if Vector3 is below the plane<br />
        ///  value == 1 if Vector3 is above the plane<br />
        ///  value == 0 if Vector3 is on the plane.
        /// </returns>
        public int OnPlane(Vector3 co, double epsilon = 0.0001d)
        {
            //eval (s = Ax + By + Cz + D) at point (x,y,z)
            //if s > 0 then point is "above" the plane (same side as normal)
            //if s < 0 then it lies on the opposite side
            //if s = 0 then the point (x,y,z) lies on the plane
            var res = EvalAtPoint(co);
            if (Math.Abs(res) < epsilon) return 0;
            if (res < 0) return -1;
            return 1;
        }

        /// <summary>
        /// Gets the point that the line intersects with this plane.
        /// </summary>
        /// <param name="start">The start of the line to intersect with</param>
        /// <param name="end">The end of the line to intersect with</param>
        /// <param name="ignoreDirection">Set to true to ignore the direction
        /// of the plane and line when intersecting. Defaults to false.</param>
        /// <param name="ignoreSegment">Set to true to ignore the start and
        /// end points of the line in the intersection. Defaults to false.</param>
        /// <returns>The point of intersection, or null if the line does not intersect</returns>
        public Vector3? GetIntersectionPoint(Vector3 start, Vector3 end, bool ignoreDirection = false, bool ignoreSegment = false)
        {
            // http://softsurfer.com/Archive/algorithm_0104/algorithm_0104B.htm#Line%20Intersections
            // http://paulbourke.net/geometry/planeline/

            var dir = end - start;
            var denominator = -Normal.Dot(dir);
            var numerator = Normal.Dot(start - Normal * DistanceFromOrigin);
            if (Math.Abs(denominator) < 0.00001d || (!ignoreDirection && denominator < 0)) return null;
            var u = numerator / denominator;
            if (!ignoreSegment && (u < 0 || u > 1)) return null;
            return start + u * dir;
        }

        /// <summary>
        /// Project a point into the space of this plane. I.e. Get the point closest
        /// to the provided point that is on this plane.
        /// </summary>
        /// <param name="point">The point to project</param>
        /// <returns>The point projected onto this plane</returns>
        public Vector3 Project(Vector3 point)
        {
            // http://www.gamedev.net/topic/262196-projecting-vector-onto-a-plane/
            // Projected = Point - ((Point - PointOnPlane) . Normal) * Normal
            return point - ((point - PointOnPlane).Dot(Normal)) * Normal;
        }

        public double EvalAtPoint(Vector3 co)
        {
            return A * co.X + B * co.Y + C * co.Z + D;
        }

        /// <summary>
        /// Gets the axis closest to the normal of this plane
        /// </summary>
        /// <returns>Vector3.UnitX, Vector3.UnitY, or Vector3.UnitZ depending on the plane's normal</returns>
        public Vector3 GetClosestAxisToNormal()
        {
            // VHE prioritises the axes in order of X, Y, Z.
            var norm = Normal.Absolute();

            if (norm.X >= norm.Y && norm.X >= norm.Z) return Vector3.UnitX;
            if (norm.Y >= norm.Z) return Vector3.UnitY;
            return Vector3.UnitZ;
        }
        
        public Plane Clone()
        {
            return new Plane(Normal, DistanceFromOrigin);
        }

        /// <summary>
        /// Intersects three planes and gets the point of their intersection.
        /// </summary>
        /// <returns>The point that the planes intersect at, or null if they do not intersect at a point.</returns>
        public static Vector3? Intersect(Plane p1, Plane p2, Plane p3)
        {
            // http://paulbourke.net/geometry/3planes/

            var c1 = p2.Normal.Cross(p3.Normal);
            var c2 = p3.Normal.Cross(p1.Normal);
            var c3 = p1.Normal.Cross(p2.Normal);

            var denom = p1.Normal.Dot(c1);
            if (denom < 0.00001d) return null; // No intersection, planes must be parallel

            var numer = (-p1.D * c1) + (-p2.D * c2) + (-p3.D * c3);
            return numer / denom;
        }

        public bool EquivalentTo(Plane other, double delta = 0.0001d)
        {
            return Normal.EquivalentTo(other.Normal, delta)
                   && Math.Abs(DistanceFromOrigin - other.DistanceFromOrigin) < delta;
        }

        public Geometric.Plane ToStandardPlane()
        {
            return new Geometric.Plane(Normal.ToStandardVector3(), (float) Math.Round(DistanceFromOrigin, 2));
        }
    }
}
