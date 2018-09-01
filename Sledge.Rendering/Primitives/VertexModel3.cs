using System.Numerics;

namespace Sledge.Rendering.Primitives
{
    public struct VertexModel3
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;
        public uint Bone;

        public const int SizeInBytes = (3 + 3 + 2 + 1) * 4;
    }
}