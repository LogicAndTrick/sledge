using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Rendering.OpenGL.Arrays;

namespace Sledge.Rendering.OpenGL.Vertices
{
    public struct SimpleVertex
    {
        public Vector3 Position;

        public Vector3 Normal;

        //[ArrayIndex(VertexAttribPointerType.UnsignedShort, 2, true)]
        //public int Texture;
        public Vector2 Texture;

        [ArrayIndex(VertexAttribPointerType.UnsignedByte, 4, true)]
        public int Color;

        [ArrayIndex(VertexAttribIntegerType.UnsignedInt)]
        public VertexFlags Flags;
    }
}
