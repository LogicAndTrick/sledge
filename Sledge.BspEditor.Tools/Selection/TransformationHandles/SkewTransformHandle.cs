using System.Numerics;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Grid;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using KeyboardState = Sledge.Shell.Input.KeyboardState;

namespace Sledge.BspEditor.Tools.Selection.TransformationHandles
{
    public class SkewTransformHandle : BoxResizeHandle, ITransformationHandle
    {
        private Vector3? _skewStart;
        private Vector3? _skewEnd;

        public string Name => "Skew";

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

        public override void StartDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera,
            ViewportEvent e,
            Vector3 position)
        {
            _skewStart = _skewEnd = position;
            base.StartDrag(document, viewport, camera, e, position);
        }

        public override void Drag(MapDocument document, MapViewport viewport, OrthographicCamera camera,
            ViewportEvent e,
            Vector3 lastPosition, Vector3 position)
        {
            _skewEnd = position;
        }

        public override void EndDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera,
            ViewportEvent e, Vector3 position)
        {
            _skewStart = _skewEnd = null;
            base.EndDrag(document, viewport, camera, e, position);
        }

        public Matrix4x4? GetTransformationMatrix(MapViewport viewport, OrthographicCamera camera, BoxState state, MapDocument doc)
        {
            var shearUpDown = Handle == ResizeHandle.Left || Handle == ResizeHandle.Right;
            var shearTopRight = Handle == ResizeHandle.Top || Handle == ResizeHandle.Right;

            if (!_skewStart.HasValue || !_skewEnd.HasValue) return null;

            var nsmd = _skewEnd.Value - _skewStart.Value;
            var mouseDiff = State.Tool.SnapIfNeeded(nsmd);
            if (KeyboardState.Shift && !KeyboardState.Alt)
            {
                // todo post-beta: this is hard-coded to only work on the square grid
                var gridData = doc.Map.Data.GetOne<GridData>();
                if (gridData?.Grid is SquareGrid sg && gridData?.SnapToGrid == true)
                {
                    mouseDiff = nsmd.Snap(sg.Step / 2);
                }
            }

            var relative = camera.Flatten(state.OrigEnd - state.OrigStart);
            var shearOrigin = (shearTopRight) ? state.OrigStart : state.OrigEnd;

            var shearAmount = new Vector3(mouseDiff.X / relative.Y, mouseDiff.Y / relative.X, 0);
            if (!shearTopRight) shearAmount *= -1;

            var shearMatrix = Matrix4x4.Identity;
            var sax = shearAmount.X;
            var say = shearAmount.Y;

            switch (camera.ViewType)
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

            var stran = Matrix4x4.CreateTranslation(-shearOrigin.X, -shearOrigin.Y, -shearOrigin.Z);
            var shear = Matrix4x4.Multiply(stran, shearMatrix);
            var inv = Matrix4x4.Invert(stran, out var i) ? i : Matrix4x4.Identity;
            return Matrix4x4.Multiply(shear, inv);
        }

        public TextureTransformationType GetTextureTransformationType(MapDocument doc)
        {
            // Never transform textures on skew
            return TextureTransformationType.None;
        }
    }
}