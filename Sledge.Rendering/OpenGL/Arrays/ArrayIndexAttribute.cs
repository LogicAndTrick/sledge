using System;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Rendering.OpenGL.Arrays
{
    public class ArrayIndexAttribute : Attribute
    {
        public VertexAttribPointerType Type { get; set; }
        public int Length { get; set; }
        public bool Normalised { get; set; }

        public ArrayIndexAttribute(VertexAttribPointerType type, int length = 1, bool normalised = false)
        {
            Type = type;
            Length = length;
            Normalised = normalised;
        }
    }
}