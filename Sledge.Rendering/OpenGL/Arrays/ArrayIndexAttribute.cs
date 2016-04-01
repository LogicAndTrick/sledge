using System;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class ArrayIndexAttribute : Attribute
    {
        public VertexAttribPointerType Type { get; private set; }
        public int Length { get; private set; }
        public bool Normalised { get; private set; }

        public ArrayIndexAttribute(VertexAttribPointerType type, int length = 1, bool normalised = false)
        {
            Type = type;
            Length = length;
            Normalised = normalised;
        }
    }
}