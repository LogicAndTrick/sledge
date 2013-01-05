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

        private static readonly Matrix4d TopMatrix = Matrix4d.Identity;
        private static readonly Matrix4d FrontMatrix = new Matrix4d(Vector4d.UnitZ, Vector4d.UnitX, Vector4d.UnitY, Vector4d.UnitW);
        private static readonly Matrix4d SideMatrix = new Matrix4d(Vector4d.UnitX, Vector4d.UnitZ, Vector4d.UnitY, Vector4d.UnitW);

        public static Matrix4d GetMatrixFor(Viewport2D.ViewDirection dir)
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

        public static DisplayListGroup Create2D(Viewport2D.ViewDirection viewDirection)
        {
            var dlg = new DisplayListGroup { Is3D = false, ViewDirection = viewDirection };
            dlg.Init();
            return dlg;
        }

        public static DisplayListGroup Create3D()
        {
            var dlg = new DisplayListGroup { Is3D = true };
            dlg.Init();
            return dlg;
        }

        #endregion

        private bool Is3D { get; set; }
        private Viewport2D.ViewDirection ViewDirection { get; set; }

        private TransformedDisplayListRenderable DeselectedObjects { get; set; }
        private TransformedDisplayListRenderable SelectedObjects { get; set; }
        private TransformedDisplayListRenderable SelectedObjectWireframes { get; set; }
        private TransformedDisplayListRenderable SelectedObjectTints { get; set; }

        private void Init()
        {
            var mat = GetMatrix();
            DeselectedObjects = new TransformedDisplayListRenderable(Is3D ? MapDisplayLists.DeselectedObjects3DName : MapDisplayLists.DeselectedObjects2DName, mat);
            SelectedObjects = new TransformedDisplayListRenderable(Is3D ? MapDisplayLists.SelectedObjects3DName : MapDisplayLists.SelectedObjects2DName, mat);
            SelectedObjectWireframes = new TransformedDisplayListRenderable(MapDisplayLists.SelectedObjectWireframesName, mat);
            SelectedObjectTints = new TransformedDisplayListRenderable(MapDisplayLists.SelectedObjectTintsName, mat);
            SelectedObjectWireframes.Colour = Is3D ? Color.Yellow : Color.Red;
        }

        private Matrix4d GetMatrix()
        {
            return Is3D ? Matrix4d.Identity : GetMatrixFor(ViewDirection);
        }

        public void Register()
        {
            if (Is3D)
            {
                ViewportManager.AddContext3D(DeselectedObjects);
                ViewportManager.AddContext3D(SelectedObjects);
                ViewportManager.AddContext3D(SelectedObjectWireframes);
                ViewportManager.AddContext3D(SelectedObjectTints);
            }
            else
            {
                ViewportManager.AddContext2D(DeselectedObjects, ViewDirection);
                ViewportManager.AddContext2D(SelectedObjects, ViewDirection);
                ViewportManager.AddContext2D(SelectedObjectWireframes, ViewDirection);
            }
        }

        public void Unregister()
        {
            if (Is3D)
            {
                ViewportManager.RemoveContext3D(DeselectedObjects);
                ViewportManager.RemoveContext3D(SelectedObjects);
                ViewportManager.RemoveContext3D(SelectedObjectWireframes);
                ViewportManager.RemoveContext3D(SelectedObjectTints);
            }
            else
            {
                ViewportManager.RemoveContext2D(DeselectedObjects, ViewDirection);
                ViewportManager.RemoveContext2D(SelectedObjects, ViewDirection);
                ViewportManager.RemoveContext2D(SelectedObjectWireframes, ViewDirection);
            }
        }

        public void SetSelectListTransform(Matrix4d matrix)
        {
            SelectedObjects.Matrix = Matrix4d.Mult(matrix, GetMatrix());
        }

        public void SetTintSelectListEnabled(bool enabled)
        {
            SelectedObjectTints.Enabled = enabled;
        }
    }
}
