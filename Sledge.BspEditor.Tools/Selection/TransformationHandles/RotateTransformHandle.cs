using System;
using System.Drawing;
using System.Numerics;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;
using KeyboardState = Sledge.Shell.Input.KeyboardState;

namespace Sledge.BspEditor.Tools.Selection.TransformationHandles
{
    public class RotateTransformHandle : BoxResizeHandle, ITransformationHandle
    {
        private readonly RotationOrigin _origin;
        private Vector3? _rotateStart;
        private Vector3? _rotateEnd;

        public string Name => "Rotate";

        public RotateTransformHandle(BoxDraggableState state, ResizeHandle handle, RotationOrigin origin) : base(state, handle)
        {
            _origin = origin;
        }

        protected override void SetCursorForHandle(MapViewport viewport, ResizeHandle handle)
        {
            var ct = ToolCursors.RotateCursor;
            viewport.Control.Cursor = ct;
        }

        public override void StartDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
        {
            _rotateStart = _rotateEnd = position;
            base.StartDrag(document, viewport, camera, e, position);
        }

        public override void Drag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position)
        {
            _rotateEnd = position;
        }

        public override void EndDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
        {
            _rotateStart = _rotateEnd = null;
            base.EndDrag(document, viewport, camera, e, position);
        }

        public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
        {
            var (wpos, soff) = GetWorldPositionAndScreenOffset(camera);
            var spos = camera.WorldToScreen(wpos) + soff;

            const float radius = 4;

            im.AddCircleFilled(spos.ToVector2(), radius, Color.White);
            im.AddCircle(spos.ToVector2(), radius, Color.Black);
        }

        public Matrix4x4? GetTransformationMatrix(MapViewport viewport, OrthographicCamera camera, BoxState state, MapDocument doc)
        {
            var origin = camera.ZeroUnusedCoordinate((state.OrigStart + state.OrigEnd) / 2);
            if (_origin != null) origin = _origin.Position;

            if (!_rotateStart.HasValue || !_rotateEnd.HasValue) return null;

            var forigin = camera.Flatten(origin);

            var origv = Vector3.Normalize(_rotateStart.Value - forigin);
            var newv =  Vector3.Normalize(_rotateEnd.Value - forigin);

            var angle = Math.Acos(Math.Max(-1, Math.Min(1, origv.Dot(newv))));
            if ((origv.Cross(newv).Z < 0)) angle = 2 * Math.PI - angle;

            // TODO post-beta: configurable rotation snapping
            var roundingDegrees = 15f;
            if (KeyboardState.Alt) roundingDegrees = 1;

            var deg = angle * (180 / Math.PI);
            var rnd = Math.Round(deg / roundingDegrees) * roundingDegrees;
            angle = rnd * (Math.PI / 180);

            Matrix4x4 rotm;
            if (camera.ViewType == OrthographicCamera.OrthographicType.Top) rotm = Matrix4x4.CreateRotationZ((float)angle);
            else if (camera.ViewType == OrthographicCamera.OrthographicType.Front) rotm = Matrix4x4.CreateRotationX((float)angle);
            else rotm = Matrix4x4.CreateRotationY((float)-angle); // The Y axis rotation goes in the reverse direction for whatever reason

            var mov = Matrix4x4.CreateTranslation(-origin.X, -origin.Y, -origin.Z);
            var rot = Matrix4x4.Multiply(mov, rotm);
            var inv = Matrix4x4.Invert(mov, out var i) ? i : Matrix4x4.Identity;
            return Matrix4x4.Multiply(rot, inv);
        }

        public TextureTransformationType GetTextureTransformationType(MapDocument doc)
        {
            var tl = doc.Map.Data.GetOne<TransformationFlags>() ?? new TransformationFlags();
            return !tl.TextureLock ? TextureTransformationType.None : TextureTransformationType.Uniform;
        }
    }
}