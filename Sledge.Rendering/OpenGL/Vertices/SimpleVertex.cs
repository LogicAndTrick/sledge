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
        public int MaterialColor;

        [ArrayIndex(VertexAttribPointerType.UnsignedByte, 4, true)]
        public int AccentColor;

        [ArrayIndex(VertexAttribPointerType.UnsignedByte, 4, true)]
        public int PointColor;

        [ArrayIndex(VertexAttribPointerType.UnsignedByte, 4, true)]
        public int TintColor;

        [ArrayIndex(VertexAttribPointerType.UnsignedInt)]
        public VertexFlags Flags;
        
        public float ZIndex;

        public Vector3 Offset;
    }
}
