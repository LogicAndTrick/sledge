using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Sledge.Rendering.Primitives
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexStandard
    {
        /// <summary>The position of the vertex</summary>
        public Vector3 Position;

        /// <summary>The normal of the vertex</summary>
        public Vector3 Normal;

        /// <summary>The colour of the vertex when untextured or in wireframe</summary>
        public Vector4 Colour;

        /// <summary>The texture coordinates of the vertex</summary>
        public Vector2 Texture;

        /// <summary>The colour modifier of the vertex when solid</summary>
        public Vector4 Tint;

        /// <summary>Bitflags for the vertex</summary>
        public VertexFlags Flags;

        /// <summary>The size of this structure in bytes</summary>
        public const int SizeInBytes = (3 + 3 + 4 + 2 + 1 * 4) * 4;
    }

    [Flags]
    public enum VertexFlags : uint
    {
        None = 0,
        SelectiveTransformed = 1 << 0,
    }
}
