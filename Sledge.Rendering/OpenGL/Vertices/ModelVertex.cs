using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Rendering.OpenGL.Arrays;

namespace Sledge.Rendering.OpenGL.Vertices
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ModelVertex
    {
        public Vector3 Position;

        public Vector3 Normal;

        public Vector2 Texture;

        [ArrayIndex(VertexAttribPointerType.UnsignedByte, 4, true)]
        public int AccentColor;

        [ArrayIndex(VertexAttribPointerType.UnsignedByte, 4, true)]
        public int TintColor;

        [ArrayIndex(VertexAttribIntegerType.UnsignedInt)]
        public VertexFlags Flags;

        [ArrayIndex(VertexAttribIntegerType.Int)]
        public byte WeightingIndex1;
        public float WeightingValue1;

        [ArrayIndex(VertexAttribIntegerType.Int)]
        public byte WeightingIndex2;
        public float WeightingValue2;

        [ArrayIndex(VertexAttribIntegerType.Int)]
        public byte WeightingIndex3;
        public float WeightingValue3;
    }
}