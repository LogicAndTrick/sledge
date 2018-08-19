using System.Numerics;

namespace Sledge.BspEditor.Primitives
{
    public interface ITransformable
    {
        /// <summary>
        /// Transforms members of this object, and all child elements.
        /// </summary>
        /// <param name="matrix">The transformation matrix</param>
        void Transform(Matrix4x4 matrix);
    }
}
