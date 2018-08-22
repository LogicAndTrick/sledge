using System.Numerics;

namespace Sledge.Rendering.Primitives
{
    public struct UniformProjection
    {
        public Matrix4x4 Selective;
        public Matrix4x4 Model;
        public Matrix4x4 View;
        public Matrix4x4 Projection;

        public const int SizeInBytes = 4 * 4 * 4 * 4; // 4 * 4x4 matrix, 4 bytes each value
    }
}