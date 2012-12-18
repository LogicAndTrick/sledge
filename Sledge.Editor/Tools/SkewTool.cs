using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Properties;
using Sledge.UI;

namespace Sledge.Editor.Tools
{
    /// <summary>
    /// Allows the selected objects to be skewed
    /// </summary>
    class SkewTool : TransformationTool
    {
        public override Image GetIcon()
        {
            return Resources.Tool_Skew;
        }

        public override string GetName()
        {
            return "Skew Tool";
        }

        protected override bool RenderCircleHandles
        {
            get { return false; }
        }

        protected override bool AllowCenterHandle
        {
            get { return false; }
        }

        protected override bool FilterHandle(ResizeHandle handle)
        {
            return handle == ResizeHandle.Bottom
                   || handle == ResizeHandle.Left
                   || handle == ResizeHandle.Top
                   || handle == ResizeHandle.Right;
        }

        protected override Cursor CursorForHandle(ResizeHandle handle)
        {
            return (handle == ResizeHandle.Top || handle == ResizeHandle.Bottom) ? Cursors.SizeWE : Cursors.SizeNS;
        }

        protected override Matrix4d? GetTransformationMatrix(Viewport2D viewport, MouseEventArgs e)
        {
            var shearUpDown = State.Handle == ResizeHandle.Left || State.Handle == ResizeHandle.Right;
            var shearTopRight = State.Handle == ResizeHandle.Top || State.Handle == ResizeHandle.Right;

            var nsmd = viewport.ScreenToWorld(e.X, viewport.Height - e.Y) - State.MoveStart;
            var mouseDiff = SnapIfNeeded(nsmd);
            if (!KeyboardState.Alt && KeyboardState.Shift)
            {
                mouseDiff = nsmd.Snap(Document.GridSpacing / 2);
            }

            var relative = viewport.Flatten(State.PreTransformBoxEnd - State.PreTransformBoxStart);
            var shearOrigin = (shearTopRight) ? State.PreTransformBoxStart : State.PreTransformBoxEnd;

            var shearAmount = new Coordinate(mouseDiff.X / relative.Y, mouseDiff.Y / relative.X, 0);
            if (!shearTopRight) shearAmount *= -1;

            var shearMatrix = Matrix4d.Identity;
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


            var stran = Matrix4d.CreateTranslation((float)-shearOrigin.X, (float)-shearOrigin.Y, (float)-shearOrigin.Z);
            var shear = Matrix4d.Mult(stran, shearMatrix);
            return Matrix4d.Mult(shear, Matrix4d.Invert(stran));
        }
    }
}
