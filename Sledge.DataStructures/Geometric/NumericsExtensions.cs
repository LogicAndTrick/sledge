using System;
using System.Numerics;

namespace Sledge.DataStructures.Geometric
{
    public static class NumericsExtensions
    {
        public const float Epsilon = 0.0001f;

        // Vector3
        public static bool EquivalentTo(this Vector3 self, Vector3 test, float delta = Epsilon)
        {
            var xd = Math.Abs(self.X - test.X);
            var yd = Math.Abs(self.Y - test.Y);
            var zd = Math.Abs(self.Z - test.Z);
            return xd < delta && yd < delta && zd < delta;
        }

        public static Vector3 Parse(string x, string y, string z) => new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
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

        // Matrix
        public static Vector3 Transform(this Matrix4x4 self, Vector3 vector) => Vector3.Transform(vector, self);
    }
}