using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.Geometric
{
    /// <summary>
    /// Defines a plane in the form Ax + By + Cz + D = 0
    /// </summary>
    [Serializable]
    public class Plane : ISerializable
    {
        public Vector3 Normal { get; }
        public float DistanceFromOrigin { get; }
        public float A { get; }
        public float B { get; }
        public float C { get; }
        public float D { get; }
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
        
        public Plane(Vector3 norm, float distanceFromOrigin)
        {
            Normal = norm.Normalise();
            DistanceFromOrigin = distanceFromOrigin;
            PointOnPlane = Normal * DistanceFromOrigin;

            A = Normal.X;
            B = Normal.Y;
            C = Normal.Z;
            D = -DistanceFromOrigin;
        }

        protected Plane(SerializationInfo info, StreamingContext context) : this((Vector3)info.GetValue("Normal", typeof(Vector3)), info.GetSingle("DistanceFromOrigin"))
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Normal", Normal);
            info.AddValue("DistanceFromOrigin", DistanceFromOrigin);
        }

        ///  <summary>Finds if the given point is above, below, or on the plane.</summary>
        ///  <param name="co">The Vector3 to test</param>
        /// <param name="epsilon">Tolerance value</param>
        /// <returns>
        ///  value == -1 if Vector3 is below the plane<br />
        ///  value == 1 if Vector3 is above the plane<br />
        ///  value == 0 if Vector3 is on the plane.
        /// </returns>
        public int OnPlane(Vector3 co, float epsilon = 0.5f)
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
        /// <param name="line">The line to intersect with</param>
        /// <param name="ignoreDirection">Set to true to ignore the direction
        /// of the plane and line when intersecting. Defaults to false.</param>
        /// <param name="ignoreSegment">Set to true to ignore the start and
        /// end points of the line in the intersection. Defaults to false.</param>
        /// <returns>The point of intersection, or null if the line does not intersect</returns>
        public Vector3? GetIntersectionPoint(Line line, bool ignoreDirection = false, bool ignoreSegment = false)
        {
            // http://softsurfer.com/Archive/algorithm_0104/algorithm_0104B.htm#Line%20Intersections
            // http://paulbourke.net/geometry/planeline/

            var dir = line.End - line.Start;
            var denominator = -Normal.Dot(dir);
            var numerator = Normal.Dot(line.Start - Normal * DistanceFromOrigin);
            if (Math.Abs(denominator) < 0.00001f || (!ignoreDirection && denominator < 0)) return null;
            var u = numerator / denominator;
            if (!ignoreSegment && (u < 0 || u > 1)) return null;
            return line.Start + u * dir;
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

        public float EvalAtPoint(Vector3 co)
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
            if (denom < 0.00001f) return null; // No intersection, planes must be parallel

            var numer = (-p1.D * c1) + (-p2.D * c2) + (-p3.D * c3);
            return numer / denom;
        }

        public bool EquivalentTo(Plane other, float delta = 0.0001f)
        {
            return Normal.EquivalentTo(other.Normal, delta)
                   && Math.Abs(DistanceFromOrigin - other.DistanceFromOrigin) < delta;
        }

        private bool Equals(Plane other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Normal, Normal) && Math.Abs(other.DistanceFromOrigin - DistanceFromOrigin) < 0.0001f;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Plane)) return false;
            return Equals((Plane)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Normal.GetHashCode() * 397) ^ DistanceFromOrigin.GetHashCode();
            }
        }

        public static bool operator ==(Plane left, Plane right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Plane left, Plane right)
        {
            return !Equals(left, right);
        }

        public Precision.Plane ToPrecisionPlane()
        {
            return new Precision.Plane(Normal.ToPrecisionVector3(), DistanceFromOrigin);
        }
    }
}
