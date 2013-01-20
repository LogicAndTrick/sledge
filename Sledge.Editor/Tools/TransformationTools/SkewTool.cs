using System.Windows.Forms;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Documents;
using Sledge.UI;

namespace Sledge.Editor.Tools.TransformationTools
{
    /// <summary>
    /// Allows the selected objects to be skewed
    /// </summary>
    class SkewTool : TransformationTool
    {
        public override bool RenderCircleHandles
        {
            get { return false; }
        }

        public override bool FilterHandle(BaseBoxTool.ResizeHandle handle)
        {
            return handle == BaseBoxTool.ResizeHandle.Bottom
                   || handle == BaseBoxTool.ResizeHandle.Left
                   || handle == BaseBoxTool.ResizeHandle.Top
                   || handle == BaseBoxTool.ResizeHandle.Right;
        }

        public override string GetTransformName()
        {
            return "Skew";
        }

        public override Cursor CursorForHandle(BaseBoxTool.ResizeHandle handle)
        {
            return (handle == BaseBoxTool.ResizeHandle.Top || handle == BaseBoxTool.ResizeHandle.Bottom)
                       ? Cursors.SizeWE
                       : Cursors.SizeNS;
        }

        public override Matrix4? GetTransformationMatrix(Viewport2D viewport, MouseEventArgs e, BaseBoxTool.BoxState state, Document doc)
        {
            var shearUpDown = state.Handle == BaseBoxTool.ResizeHandle.Left || state.Handle == BaseBoxTool.ResizeHandle.Right;
            var shearTopRight = state.Handle == BaseBoxTool.ResizeHandle.Top || state.Handle == BaseBoxTool.ResizeHandle.Right;

            var nsmd = viewport.ScreenToWorld(e.X, viewport.Height - e.Y) - state.MoveStart;
            var mouseDiff = SnapIfNeeded(nsmd, doc);
            if (!KeyboardState.Alt && KeyboardState.Shift)
            {
                mouseDiff = nsmd.Snap(doc.GridSpacing / 2);
            }

            var relative = viewport.Flatten(state.PreTransformBoxEnd - state.PreTransformBoxStart);
            var shearOrigin = (shearTopRight) ? state.PreTransformBoxStart : state.PreTransformBoxEnd;

            var shearAmount = new Coordinate(mouseDiff.X / relative.Y, mouseDiff.Y / relative.X, 0);
            if (!shearTopRight) shearAmount *= -1;

            var shearMatrix = Matrix4.Identity;
            var sax = (float)shearAmount.X;
            var say = (float)shearAmount.Y;

            switch (viewport.Direction)
            {
                case Viewport2D.ViewDirection.Top:
                    if (shearUpDown) shearMatrix.M12 = say;
                    else shearMatrix.M21 = sax;
                    break;
                case Viewport2D.ViewDirection.Front:
                    if (shearUpDown) shearMatrix.M23 = say;
                    else shearMatrix.M32 = sax;
                    break;
                case Viewport2D.ViewDirection.Side:
                    if (shearUpDown) shearMatrix.M13 = say;
                    else shearMatrix.M31 = sax;
                    break;
            }


            var stran = Matrix4.CreateTranslation((float)-shearOrigin.X, (float)-shearOrigin.Y, (float)-shearOrigin.Z);
            var shear = Matrix4.Mult(stran, shearMatrix);
            return Matrix4.Mult(shear, Matrix4.Invert(stran));
        }
    }
}
