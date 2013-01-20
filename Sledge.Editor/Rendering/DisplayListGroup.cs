using System;
using System.Drawing;
using Sledge.Editor.UI;
using Sledge.UI;
using OpenTK;

namespace Sledge.Editor.Rendering
{
    /// <summary>
    /// A display list group holds a number of references to some (shared)
    /// display lists. The selected and deselected object have their own display lists,
    /// in both 2D (wireframe) and 3D (textured). The selected objects also have their own
    /// wireframe in 3D and the red tints in the 3D view.
    /// 
    /// The lists stored in an instance of this class are simply wrappers around these six
    /// global display lists. Each one applies different transformations before drawing the
    /// lists, so that they look correct in their respective views.
    /// </summary>
    public sealed class DisplayListGroup
    {
        #region Static

        private static readonly Matrix4 TopMatrix = Matrix4.Identity;
        private static readonly Matrix4 FrontMatrix = new Matrix4(Vector4.UnitZ, Vector4.UnitX, Vector4.UnitY, Vector4.UnitW);
        private static readonly Matrix4 SideMatrix = new Matrix4(Vector4.UnitX, Vector4.UnitZ, Vector4.UnitY, Vector4.UnitW);

        public static Matrix4 GetMatrixFor(Viewport2D.ViewDirection dir)
        {
            switch (dir)
            {
                case Viewport2D.ViewDirection.Top:
                    return TopMatrix;
                case Viewport2D.ViewDirection.Front:
                    return FrontMatrix;
                case Viewport2D.ViewDirection.Side:
                    return SideMatrix;
                default:
                    throw new ArgumentOutOfRangeException("dir");
            }
        }

        #endregion

    }
}
