using System;

namespace Sledge.Rendering.Primitives
{
    /// <summary>
    /// Various rendering flags for a vertex.
    /// These flags are intended to be applied consistently to an entire polygon.
    /// </summary>
    [Flags]
    public enum VertexFlags : uint
    {
        /// <summary>
        /// Nothing interesting
        /// </summary>
        None = 0,

        /// <summary>
        /// The vertex is transformed by the selective transform matrix
        /// </summary>
        SelectiveTransformed = 1 << 0,

        /// <summary>
        /// The texture sample will be replaced with the vertex colour
        /// </summary>
        FlatColour = 1 << 1,

        /// <summary>
        /// Semi-transparent texture samples are rounded to 0 or 1
        /// </summary>
        AlphaTested = 1 << 2,
    }
}