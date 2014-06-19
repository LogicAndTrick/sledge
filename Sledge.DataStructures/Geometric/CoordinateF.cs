using System;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.Geometric
{
    [Serializable]
    public class CoordinateF : ISerializable
    {
        public readonly static CoordinateF MaxValue = new CoordinateF(float.MaxValue, float.MaxValue, float.MaxValue);
        public readonly static CoordinateF MinValue = new CoordinateF(float.MinValue, float.MinValue, float.MinValue);
        public readonly static CoordinateF Zero = new CoordinateF(0, 0, 0);
        public readonly static CoordinateF One = new CoordinateF(1, 1, 1);
        public readonly static CoordinateF UnitX = new CoordinateF(1, 0, 0);
        public readonly static CoordinateF UnitY = new CoordinateF(0, 1, 0);
        public readonly static CoordinateF UnitZ = new CoordinateF(0, 0, 1);

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        
        public CoordinateF(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        protected CoordinateF(SerializationInfo info, StreamingContext context)
        {
            X = info.GetSingle("X");
            Y = info.GetSingle("Y");
            Z = info.GetSingle("Z");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("Z", Z);
        }

        public bool EquivalentTo(CoordinateF test, float delta = 0.0001f)
        {
            var xd = Math.Abs(X - test.X);
            var yd = Math.Abs(Y - test.Y);
            var zd = Math.Abs(Z - test.Z);
            return (xd < delta) && (yd < delta) && (zd < delta);
        }

        public bool Equals(CoordinateF other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EquivalentTo(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof (CoordinateF) && Equals((CoordinateF) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = X.GetHashCode();
                result = (result*397) ^ Y.GetHashCode();
                result = (result*397) ^ Z.GetHashCode();
                return result;
            }
        }

        public float Dot(CoordinateF c)
        {
            return ((X * c.X) + (Y * c.Y) + (Z * c.Z));
        }

        public CoordinateF Cross(CoordinateF that)
        {
            var xv = (Y * that.Z) - (Z * that.Y);
            var yv = (Z * that.X) - (X * that.Z);
            var zv = (X * that.Y) - (Y * that.X);
            return new CoordinateF(xv, yv, zv);
        }

        public CoordinateF Round(int num = 4)
        {
            return new CoordinateF(
                (float) Math.Round(X, num),
                (float) Math.Round(Y, num),
                (float) Math.Round(Z, num));
        }

        public CoordinateF Snap(float snapTo)
        {
            return new CoordinateF(
                (float) Math.Round(X / snapTo) * snapTo,
                (float) Math.Round(Y / snapTo) * snapTo,
                (float) Math.Round(Z / snapTo) * snapTo);
        }

        public float VectorMagnitude()
        {
            return (float)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
        }

        public CoordinateF Normalise()
        {
            var len = VectorMagnitude();
            return Math.Abs(len - 0) < 0.0001 ? new CoordinateF(0, 0, 0) : new CoordinateF(X / len, Y / len, Z / len);
        }

        public CoordinateF Absolute()
        {
            return new CoordinateF(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
        }

        public static bool operator ==(CoordinateF c1, CoordinateF c2)
        {
            return Equals(c1, null) ? Equals(c2, null) : c1.Equals(c2);
        }

        public static bool operator !=(CoordinateF c1, CoordinateF c2)
        {
            return Equals(c1, null) ? !Equals(c2, null) : !c1.Equals(c2);
        }

        public static CoordinateF operator +(CoordinateF c1, CoordinateF c2)
        {
            return new CoordinateF(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z);
        }

        public static CoordinateF operator -(CoordinateF c1, CoordinateF c2)
        {
            return new CoordinateF(c1.X - c2.X, c1.Y - c2.Y, c1.Z - c2.Z);
        }

        public static CoordinateF operator -(CoordinateF c1)
        {
            return new CoordinateF(-c1.X, -c1.Y, -c1.Z);
        }

        public static CoordinateF operator /(CoordinateF c, float f)
        {
            return Math.Abs(f - 0) < 0.0001f ? new CoordinateF(0, 0, 0) : new CoordinateF(c.X / f, c.Y / f, c.Z / f);
        }

        public static CoordinateF operator *(CoordinateF c, float f)
        {
            return new CoordinateF(c.X * f, c.Y * f, c.Z * f);
        }

        public static CoordinateF operator *(float f, CoordinateF c)
        {
            return c * f;
        }

        public CoordinateF ComponentMultiply(CoordinateF c)
        {
            return new CoordinateF(X * c.X, Y * c.Y, Z * c.Z);
        }

        public CoordinateF ComponentDivide(CoordinateF c)
        {
            if (Math.Abs(c.X - 0) < 0.0001) c.X = 1;
            if (Math.Abs(c.Y - 0) < 0.0001) c.Y = 1;
            if (Math.Abs(c.Z - 0) < 0.0001) c.Z = 1;
            return new CoordinateF(X / c.X, Y / c.Y, Z / c.Z);
        }

        public override string ToString()
        {
            return "(" + X.ToString("0.0000") + " " + Y.ToString("0.0000") + " " + Z.ToString("0.0000") + ")";
        }

        public CoordinateF Clone()
        {
            return new CoordinateF(X, Y, Z);
        }

        public static CoordinateF Parse(string x, string y, string z)
        {
            return new CoordinateF(float.Parse(x), float.Parse(y), float.Parse(z));
        }
    }
}
