using System.Windows.Forms;
using OpenTK;
using OpenTK.Input;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using KeyboardState = Sledge.Shell.Input.KeyboardState;

namespace Sledge.BspEditor.Tools.Selection.TransformationHandles
{
    public class SkewTransformHandle : BoxResizeHandle, ITransformationHandle
    {
        private Coordinate _skewStart;
        private Coordinate _skewEnd;

        public string Name { get { return "Skew"; } }

        public SkewTransformHandle(BoxDraggableState state, ResizeHandle handle)
            : base(state, handle)
        {
        }

        protected override void SetCursorForHandle(MapViewport viewport, ResizeHandle handle)
        {
            var ct = handle.GetCursorType();
            switch (handle)
            {
                case ResizeHandle.Top:
                case ResizeHandle.Bottom:
                    ct = Cursors.SizeWE;
                    break;
                case ResizeHandle.Left:
                case ResizeHandle.Right:
                    ct = Cursors.SizeNS;
                    break;
            }
            viewport.Control.Cursor = ct;
        }

        public override void StartDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            _skewStart = _skewEnd = position;
            base.StartDrag(viewport, e, position);
        }

        public override void Drag(MapViewport viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {
            _skewEnd = position;
        }

        public override void EndDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            _skewStart = _skewEnd = null;
            base.EndDrag(viewport, e, position);
        }

        public Matrix4? GetTransformationMatrix(MapViewport viewport, OrthographicCamera camera, BoxState state, MapDocument doc)
        {
            var shearUpDown = Handle == ResizeHandle.Left || Handle == ResizeHandle.Right;
            var shearTopRight = Handle == ResizeHandle.Top || Handle == ResizeHandle.Right;

            var nsmd = _skewEnd - _skewStart;
            var mouseDiff = nsmd; // doc.Snap(nsmd, doc.Map.GridSpacing);
            if (KeyboardState.Shift)
            {
                // !todo selection (snapping)
                // mouseDiff = doc.Snap(nsmd, doc.Map.GridSpacing / 2);
            }

            var relative = viewport.Flatten(state.OrigEnd - state.OrigStart);
            var shearOrigin = (shearTopRight) ? state.OrigStart : state.OrigEnd;

            var shearAmount = new Coordinate(mouseDiff.X / relative.Y, mouseDiff.Y / relative.X, 0);
            if (!shearTopRight) shearAmount *= -1;

            var shearMatrix = Matrix4.Identity;
            var sax = (float)shearAmount.X;
            var say = (float)shearAmount.Y;

            switch (viewport.Direction)
            {
                case OrthographicCamera.OrthographicType.Top:
                    if (shearUpDown) shearMatrix.M12 = say;
                    else shearMatrix.M21 = sax;
                    break;
                case OrthographicCamera.OrthographicType.Front:
                    if (shearUpDown) shearMatrix.M23 = say;
                    else shearMatrix.M32 = sax;
                    break;
                case OrthographicCamera.OrthographicType.Side:
                    if (shearUpDown) shearMatrix.M13 = say;
                    else shearMatrix.M31 = sax;
                    break;
            }

            var stran = Matrix4.CreateTranslation((float)-shearOrigin.X, (float)-shearOrigin.Y, (float)-shearOrigin.Z);
            var shear = Matrix4.Mult(stran, shearMatrix);
            return Matrix4.Mult(shear, Matrix4.Invert(stran));
        }

        public TextureTransformationType GetTextureTransformationType(MapDocument doc)
        {
            // Never transform textures on skew
            return TextureTransformationType.None;
        }
    }
}