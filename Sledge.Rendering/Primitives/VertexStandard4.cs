using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Rendering.Primitives
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexStandard4
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 Colour;
        public Vector2 Texture;

        public const int SizeInBytes = (3 + 3 + 4 + 2) * 4;
    }
}
