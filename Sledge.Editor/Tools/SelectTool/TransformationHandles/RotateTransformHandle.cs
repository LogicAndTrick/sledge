using System;
using System.Collections.Generic;
using OpenTK;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Documents;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Editor.UI;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Settings;

namespace Sledge.Editor.Tools.SelectTool.TransformationHandles
{
    public class RotateTransformHandle : BoxResizeHandle, ITransformationHandle
    {
        private readonly RotationOrigin _origin;
        private Coordinate _rotateStart;
        private Coordinate _rotateEnd;

        public string Name { get { return "Rotate"; } }

        public RotateTransformHandle(BoxDraggableState state, ResizeHandle handle, RotationOrigin origin) : base(state, handle)
        {
            _origin = origin;
        }

        public override void StartDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            _rotateStart = _rotateEnd = position;
            base.StartDrag(viewport, e, position);
        }

        public override void Drag(MapViewport viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {
            _rotateEnd = position;
        }

        public override void EndDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            _rotateStart = _rotateEnd = null;
            base.EndDrag(viewport, e, position);
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            foreach (var e in base.GetViewportElements(viewport, camera))
            {
                var handle = e as HandleElement;
                if (handle != null) handle.Type = HandleElement.HandleType.Circle;
                yield return e;
            }
        }

        public Matrix4? GetTransformationMatrix(MapViewport viewport, OrthographicCamera camera, BoxState state, Document doc)
        {
            var origin = viewport.ZeroUnusedCoordinate((state.OrigStart + state.OrigEnd) / 2);
            if (_origin != null) origin = _origin.Position;

            var forigin = viewport.Flatten(origin);

            var origv = (_rotateStart - forigin).Normalise();
            var newv = (_rotateEnd - forigin).Normalise();

            var angle = DMath.Acos(Math.Max(-1, Math.Min(1, origv.Dot(newv))));
            if ((origv.Cross(newv).Z < 0)) angle = 2 * DMath.PI - angle;

            var shf = KeyboardState.Shift;
            var def = Select.RotationStyle;
            var snap = (def == RotationStyle.SnapOnShift && shf) || (def == RotationStyle.SnapOffShift && !shf);
            if (snap)
            {
                var deg = angle * (180 / DMath.PI);
                var rnd = Math.Round(deg / 15) * 15;
                angle = rnd * (DMath.PI / 180);
            }

            Matrix4 rotm;
            if (viewport.Direction == OrthographicCamera.OrthographicType.Top) rotm = Matrix4.CreateRotationZ((float)angle);
            else if (viewport.Direction == OrthographicCamera.OrthographicType.Front) rotm = Matrix4.CreateRotationX((float)angle);
            else rotm = Matrix4.CreateRotationY((float)-angle); // The Y axis rotation goes in the reverse direction for whatever reason

            var mov = Matrix4.CreateTranslation((float)-origin.X, (float)-origin.Y, (float)-origin.Z);
            var rot = Matrix4.Mult(mov, rotm);
            return Matrix4.Mult(rot, Matrix4.Invert(mov));
        }
    }
}