using System;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class ArrayIndexAttribute : Attribute
    {
        public bool IsIntegerType { get; private set; }
        public VertexAttribPointerType Type { get; private set; }
        public VertexAttribIntegerType IntegerType { get; private set; }
        public int Length { get; private set; }
        public bool Normalised { get; private set; }

        public ArrayIndexAttribute(VertexAttribPointerType type, int length = 1, bool normalised = false)
        {
            IsIntegerType = false;
            Type = type;
            Length = length;
            Normalised = normalised;
        }

        public ArrayIndexAttribute(VertexAttribIntegerType type, int length = 1)
        {
            IsIntegerType = true;
            IntegerType = type;
            Length = length;
            Normalised = false;
        }
    }
}