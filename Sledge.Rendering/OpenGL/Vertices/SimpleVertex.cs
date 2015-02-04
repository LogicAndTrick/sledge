using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sledge.Rendering.OpenGL.Arrays;

namespace Sledge.Rendering.OpenGL.Vertices
{
    public struct SimpleVertex
    {
        public Vector3 Position;

        [ArrayIndex(VertexAttribPointerType.UnsignedByte, 4, true)]
        public int Color;
    }
}
