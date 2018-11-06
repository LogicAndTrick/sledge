using System;
using System.Drawing;
using System.Globalization;
using System.Numerics;

namespace Sledge.DataStructures.Geometric
{
    public static class NumericsExtensions
    {
        public const float Epsilon = 0.0001f;

        // Vector2
        public static Vector3 ToVector3(this Vector2 self)
        {
            return new Vector3(self, 0);
        }

        // Vector3
        public static bool EquivalentTo(this Vector3 self, Vector3 test, float delta = Epsilon)
        {
            var xd = Math.Abs(self.X - test.X);
            var yd = Math.Abs(self.Y - test.Y);
            var zd = Math.Abs(self.Z - test.Z);
            return xd < delta && yd < delta && zd < delta;
        }

        public static Vector3 Parse(string x, string y, string z, NumberStyles ns, IFormatProvider provider)
        {
            return new Vector3(float.Parse(x, ns, provider), float.Parse(y, ns, provider), float.Parse(z, ns, provider));
        }

        public static Vector3 Normalise(this Vector3 self) => Vector3.Normalize(self);
        public static Vector3 Absolute(this Vector3 self) => Vector3.Abs(self);
        public static float Dot(this Vector3 self, Vector3 other) => Vector3.Dot(self, other);
        public static Vector3 Cross(this Vector3 self, Vector3 other) => Vector3.Cross(self, other);
        public static Vector3 Round(this Vector3 self, int num = 8) => new Vector3((float) Math.Round(self.X, num), (float) Math.Round(self.Y, num), (float) Math.Round(self.Z, num));
        
        public static Vector3 ClosestAxis(this Vector3 self)
        {
            // VHE prioritises the axes in order of X, Y, Z.
            var norm = Vector3.Abs(self);

            if (norm.X >= norm.Y && norm.X >= norm.Z) return Vector3.UnitX;
            if (norm.Y >= norm.Z) return Vector3.UnitY;
            return Vector3.UnitZ;
        }

        public static Vector3 Snap(this Vector3 self, float snapTo)
        {
            return new Vector3(
                (float) Math.Round(self.X / snapTo) * snapTo,
                (float) Math.Round(self.Y / snapTo) * snapTo,
                (float) Math.Round(self.Z / snapTo) * snapTo
            );
        }

        public static Precision.Vector3 ToPrecisionVector3(this Vector3 self)
        {
            return new Precision.Vector3(self.X, self.Y, self.Z);
        }

        public static Vector2 ToVector2(this Vector3 self)
        {
            return new Vector2(self.X, self.Y);
        }

        // Vector4
        public static Vector4 ToVector4(this Color self)
        {
            return new Vector4(self.R, self.G, self.B, self.A) / 255f;
        }

        // Color
        public static Color ToColor(this Vector4 self)
        {
            var mul = self * 255;
            return Color.FromArgb((byte) mul.W, (byte) mul.X, (byte) mul.Y, (byte) mul.Z);
        }

        public static Color ToColor(this Vector3 self)
        {
            var mul = self * 255;
            return Color.FromArgb(255, (byte) mul.X, (byte) mul.Y, (byte) mul.Z);
        }

        // Matrix
        public static Vector3 Transform(this Matrix4x4 self, Vector3 vector) => Vector3.Transform(vector, self);
    }
}