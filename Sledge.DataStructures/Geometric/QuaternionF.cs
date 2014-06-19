using System;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.Geometric
{
    /// <summary>
    /// Represents a QuaternionF. Shamelessly taken in its entirety from OpenTK's QuaternionF structure. http://www.opentk.com/
    /// </summary>
    [Serializable]
    public class QuaternionF : ISerializable
    {
        public static QuaternionF Identity
        {
            get { return new QuaternionF(0, 0, 0, 1); }
        }

        public CoordinateF Vector { get; private set; }
        public float Scalar { get; private set; }

        public float X { get { return Vector.X; } }
        public float Y { get { return Vector.Y; } }
        public float Z { get { return Vector.Z; } }
        public float W { get { return Scalar; } }

        public QuaternionF(CoordinateF vector, float scalar)
        {
            Vector = vector;
            Scalar = scalar;
        }

        public QuaternionF(float x, float y, float z, float w)
        {
            Vector = new CoordinateF(x, y, z);
            Scalar = w;
        }

        public float Dot(QuaternionF c)
        {
            return Vector.Dot(c.Vector) + Scalar * c.Scalar;
        }

        protected QuaternionF(SerializationInfo info, StreamingContext context)
        {
            Vector = (CoordinateF) info.GetValue("Vector", typeof (CoordinateF));
            Scalar = info.GetSingle("Scalar");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Vector", Vector);
            info.AddValue("Scalar", Scalar);
        }

        public float Magnitude()
        {
            return (float) Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2) + Math.Pow(W, 2));
        }

        public QuaternionF Normalise()
        {
            var len = Magnitude();
            return Math.Abs(len - 0) < 0.0001 ? new QuaternionF(0, 0, 0, 0) : new QuaternionF(X / len, Y / len, Z / len, W / len);
        }

        public QuaternionF Conjugate()
        {
            return new QuaternionF(-X, -Y, -Z, W);
        }

        public QuaternionF Inverse()
        {
            var lengthSq = (float) (Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2) + Math.Pow(W, 2));
            if (Math.Abs(lengthSq - 0) > 0.0001)
            {
                var i = 1f / lengthSq;
                return new QuaternionF(Vector * -i, W * i);
            }
            return this;
        }

        public QuaternionF Clone()
        {
            return new QuaternionF(X, Y, Z, W);
        }

        public Tuple<CoordinateF, float> GetAxisAngle()
        {
            var q = W > 1 ? Normalise() : this;
            var angle = 2f * (float) Math.Acos(q.W);
            var denom = (float) Math.Sqrt(1 - q.W * q.W);
            var coord = denom <= 0.0001 ? CoordinateF.UnitX : q.Vector / denom;
            return Tuple.Create(coord, angle);
        }

        public CoordinateF GetEulerAngles(bool homogenous = true)
        {
            // http://willperone.net/Code/quaternion.php
            var sqw = W*W;
            var sqx = X*X;
            var sqy = Y*Y;
            var sqz = Z*Z;

            return homogenous
                       ? new CoordinateF(
                             (float) Math.Atan2(2 * (X * Y + Z * W), sqx - sqy - sqz + sqw),
                             (float) Math.Asin(-2 * (X * Z - Y * W)),
                             (float) Math.Atan2(2 * (Y * Z + X * W), -sqx - sqy + sqz + sqw))
                       : new CoordinateF(
                             (float) Math.Atan2(2 * (Z * Y + X * W), 1 - 2 * (sqx + sqy)),
                             (float) Math.Asin(-2 * (X * Z - Y * W)),
                             (float) Math.Atan2(2 * (X * Y + Z * W), 1 - 2 * (sqy + sqz)));
        }

        public MatrixF GetMatrix()
        {
            // http://local.wasp.uwa.edu.au/~pbourke/miscellaneous/QuaternionFs/
            var xx = X * X;
            var yy = Y * Y;
            var zz = Z * Z;
            return new MatrixF(
                           1 - 2 * yy - 2 * zz,
                           2 * X * Y + 2 * W * Z,
                           2 * X * Z - 2 * W * Y,
                           0,

                           2 * X * Y - 2 * W * Z,
                           1 - 2 * xx - 2 * zz,
                           2 * Y * Z + 2 * W * X,
                           0,

                           2 * X * Z + 2 * W * Y,
                           2 * Y * Z - 2 * W * X,
                           1 - 2 * xx - 2 * yy,
                           0,

                           0,
                           0,
                           0,
                           1
                       );
        }

        public CoordinateF Rotate(CoordinateF coord)
        {
            // http://content.gpwiki.org/index.php/OpenGL:Tutorials:Using_Quaternions_to_represent_rotation
            var q = new QuaternionF(coord.Normalise(), 0);
            var temp = q * Conjugate();
            return (this * temp).Vector;
        }

        public bool Equals(QuaternionF other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Vector, Vector) && Math.Abs(other.Scalar - Scalar) < 0.0001;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(QuaternionF)) return false;
            return Equals((QuaternionF)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Vector != null ? Vector.GetHashCode() : 0) * 397) ^ Scalar.GetHashCode();
            }
        }

        public static bool operator ==(QuaternionF left, QuaternionF right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(QuaternionF left, QuaternionF right)
        {
            return !Equals(left, right);
        }

        public static QuaternionF operator +(QuaternionF c1, QuaternionF c2)
        {
            return new QuaternionF(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z, c1.W + c2.W);
        }

        public static QuaternionF operator -(QuaternionF c1, QuaternionF c2)
        {
            return new QuaternionF(c1.X - c2.X, c1.Y - c2.Y, c1.Z - c2.Z, c1.W - c2.W);
        }

        public static QuaternionF operator -(QuaternionF c1)
        {
            return new QuaternionF(-c1.X, -c1.Y, -c1.Z, -c1.W);
        }

        public static QuaternionF operator /(QuaternionF c, float f)
        {
            return Math.Abs(f - 0) < 0.0001 ? new QuaternionF(0, 0, 0, 0) : new QuaternionF(c.X / f, c.Y / f, c.Z / f, c.W / f);
        }

        public static QuaternionF operator *(QuaternionF c, float f)
        {
            return new QuaternionF(c.X * f, c.Y * f, c.Z * f, c.W * f);
        }

        public static QuaternionF operator *(float f, QuaternionF c)
        {
            return c * f;
        }

        public static QuaternionF operator *(QuaternionF left, QuaternionF right)
        {
            return new QuaternionF(
                right.W * left.Vector + left.W * right.Vector + left.Vector.Cross(right.Vector),
                left.W * right.W - left.Vector.Dot(right.Vector));
        }

        public static QuaternionF operator /(QuaternionF left, QuaternionF right)
        {
            return left * right.Inverse();
        }

        public static QuaternionF EulerAngles(CoordinateF angles)
        {
            // http://www.euclideanspace.com/maths/geometry/rotations/conversions/eulerToQuaternionF/index.htm
            angles = angles / 2;
            var sy = (float) Math.Sin(angles.Z);
            var sp = (float) Math.Sin(angles.Y);
            var sr = (float) Math.Sin(angles.X);
            var cy = (float) Math.Cos(angles.Z);
            var cp = (float) Math.Cos(angles.Y);
            var cr = (float) Math.Cos(angles.X);
            return new QuaternionF(sr * cp * cy - cr * sp * sy,
                                  cr * sp * cy + sr * cp * sy,
                                  cr * cp * sy - sr * sp * cy,
                                  cr * cp * cy + sr * sp * sy);

        }

        public static QuaternionF AxisAngle(CoordinateF axis, float angle)
        {
            return Math.Abs(axis.VectorMagnitude()) < 0.0001
                       ? Identity
                       : new QuaternionF(axis.Normalise() * (float)Math.Sin(angle / 2), (float)Math.Cos(angle / 2)).Normalise();
        }

        public static QuaternionF Lerp(QuaternionF start, QuaternionF end, float blend)
        {
            // Clone to avoid modifying the parameters
            var q1 = start.Clone();
            var q2 = end.Clone();

            // if either input is zero, return the other.
            if (Math.Abs(q1.Magnitude() - 0) < 0.0001) return Math.Abs(q2.Magnitude() - 0) < 0.0001 ? Identity : q2;
            if (Math.Abs(q2.Magnitude() - 0) < 0.0001) return q1;

            var blendA = 1 - blend;
            var blendB = blend;

            var result = new QuaternionF(blendA * q1.Vector + blendB * q2.Vector, blendA * q1.W + blendB * q2.W);
            return result.Magnitude() > 0 ? result.Normalise() : Identity;
        }

        public static QuaternionF Slerp(QuaternionF start, QuaternionF end, float blend)
        {
            // Clone to avoid modifying the parameters
            var q1 = start.Clone();
            var q2 = end.Clone();

            // if either input is zero, return the other.
            if (Math.Abs(q1.Magnitude() - 0) < 0.0001) return Math.Abs(q2.Magnitude() - 0) < 0.0001 ? Identity : q2;
            if (Math.Abs(q2.Magnitude() - 0) < 0.0001) return q1;

            var cosHalfAngle = q1.Dot(q2);

            if (cosHalfAngle >= 1 || cosHalfAngle <= -1) return q1;

            if (cosHalfAngle < 0)
            {
                q2.Vector = -q2.Vector;
                q2.Scalar = -q2.Scalar;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;
            if (cosHalfAngle < 0.99)
            {
                // do proper slerp for big angles
                var halfAngle = (float) Math.Acos(cosHalfAngle);
                var sinHalfAngle = (float)Math.Sin(halfAngle);
                var oneOverSinHalfAngle = 1 / sinHalfAngle;
                blendA = (float) Math.Sin(halfAngle * (1 - blend)) * oneOverSinHalfAngle;
                blendB = (float) Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1 - blend;
                blendB = blend;
            }

            var result = new QuaternionF(blendA * q1.Vector + blendB * q2.Vector, blendA * q1.W + blendB * q2.W);
            return result.Magnitude() > 0 ? result.Normalise() : Identity;
        }

        public override string ToString()
        {
            return Vector + " " + Scalar;
        }
    }
}
