using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using OpenTK.Graphics.OpenGL;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Rendering;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Components;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Shell.Input;
using Camera = Sledge.BspEditor.Primitives.MapData.Camera;

namespace Sledge.BspEditor.Tools
{
    [Export(typeof(ITool))]
    [OrderHint("D")]
    public class CameraTool : BaseTool
    {
        private enum State
        {
            None,
            MovingPosition,
            MovingLook
        }

        private State _state;
        private Camera _stateCamera;

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<object>("BspEditor:CameraNext", CameraNext);
            yield return Oy.Subscribe<object>("BspEditor:CameraPrevious", CameraPrevious);
        }

        public override void ToolSelected()
        {
            _state = State.None;
        }

        private async Task CameraNext(object param)
        {
            var cams = GetDocumentCameras();
            if (_state != State.None || cams.Count < 2) return;
            var idx = cams.FindIndex(x => x.IsActive);
            idx = (idx + 1) % cams.Count;
            cams.ForEach(x => x.IsActive = false);
            cams[idx].IsActive = true;
            SetViewportCamera(cams[idx].EyePosition, cams[idx].LookPosition);
        }

        private async Task CameraPrevious(object param)
        {
            var cams = GetDocumentCameras();
            if (_state != State.None || cams.Count < 2) return;
            var idx = cams.FindIndex(x => x.IsActive);
            idx = (idx + cams.Count - 1) % cams.Count;
            cams.ForEach(x => x.IsActive = false);
            cams[idx].IsActive = true;
            SetViewportCamera(cams[idx].EyePosition, cams[idx].LookPosition);
        }

        private void CameraDelete()
        {
            var cams = GetDocumentCameras();
            if (_state != State.None || cams.Count < 2) return;
            var del = cams.FirstOrDefault(x => x.IsActive);
            CameraPrevious(null);
            if (del != GetDocumentCameras().FirstOrDefault(x => x.IsActive)) Document.Map.Data.Remove(del);
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Camera;
        }

        public override string GetName()
        {
            return "Camera Tool";
        }

        private Tuple<Coordinate, Coordinate> GetViewportCamera()
        {
            var cam = MapDocumentControlHost.Instance.GetControls().OfType<ViewportMapDocumentControl>().Select(x => x.Camera).OfType<PerspectiveCamera>().FirstOrDefault();
            if (cam == null) return null;

            var pos = cam.Position.ToCoordinate();
            var look = cam.LookAt.ToCoordinate();

            var dir = (look - pos).Normalise()*20;
            return Tuple.Create(pos, pos + dir);
        }

        private void SetViewportCamera(Coordinate position, Coordinate look)
        {
            var cam = MapDocumentControlHost.Instance.GetControls().OfType<ViewportMapDocumentControl>().Select(x => x.Camera).OfType<PerspectiveCamera>().FirstOrDefault();
            if (cam == null) return;

            look = (look - position).Normalise() + position;
            cam.Position = position.ToVector3();
            cam.LookAt = look.ToVector3();
        }

        private State GetStateAtPoint(int x, int y, MapViewport viewport, out Camera activeCamera)
        {
            var d = 5 / (decimal) viewport.Zoom;

            foreach (var cam in GetCameraList())
            {
                var p = viewport.Flatten(viewport.ProperScreenToWorld(x, y));
                var pos = viewport.Flatten(cam.EyePosition);
                var look = viewport.Flatten(cam.LookPosition);
                activeCamera = cam;
                if (p.X >= pos.X - d && p.X <= pos.X + d && p.Y >= pos.Y - d && p.Y <= pos.Y + d) return State.MovingPosition;
                if (p.X >= look.X - d && p.X <= look.X + d && p.Y >= look.Y - d && p.Y <= look.Y + d) return State.MovingLook;
            }

            activeCamera = null;
            return State.None;
        }

        private List<Camera> GetDocumentCameras()
        {
            return Document.Map.Data.Get<Camera>().ToList();
        }

        private List<Camera> GetCameraList()
        {
            var c = GetViewportCamera();
            if (!Document.Map.Data.Get<Camera>().Any())
            {
                Document.Map.Data.Add(new Camera {EyePosition = c.Item1, LookPosition = c.Item2});
            }
            var active = Document.Map.Data.Get<Camera>().FirstOrDefault(x => x.IsActive);
            if (active == null)
            {
                active = Document.Map.Data.GetOne<Camera>();
                active.IsActive = true;
            }
            var len = active.Length;
            active.EyePosition = c.Item1;
            active.LookPosition = c.Item1 + (c.Item2 - c.Item1).Normalise() * len;

            var gs = Document.Map.Data.GetOne<GridData>()?.Grid?.Spacing ?? 64;
            var cameras = new List<Camera>();
            foreach (var camera in Document.Map.Data.Get<Camera>())
            {
                var dir = camera.LookPosition - camera.EyePosition;
                camera.LookPosition = camera.EyePosition + dir.Normalise() * Math.Max(gs * 1.5m, dir.VectorMagnitude());
                cameras.Add(camera);
            }
            return cameras;
        }

        protected override void MouseDown(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            var vp = viewport;
            if (vp == null) return;

            var gs = Document.Map.Data.GetOne<GridData>()?.Grid?.Spacing ?? 64;

            _state = GetStateAtPoint(e.X, e.Y, vp, out _stateCamera);
            if (_state == State.None && KeyboardState.Shift)
            {
                var p = SnapIfNeeded(vp.ProperScreenToWorld(e.X, e.Y));
                _stateCamera = new Camera { EyePosition = p, LookPosition = p + Coordinate.UnitX * 1.5m * gs };
                Document.Map.Data.Add(_stateCamera);
                _state = State.MovingLook;
            }
            if (_stateCamera != null)
            {
                SetViewportCamera(_stateCamera.EyePosition, _stateCamera.LookPosition);
                GetDocumentCameras().ForEach(x => x.IsActive = false);
                _stateCamera.IsActive = true;
            }
        }

        protected override void MouseUp(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            _state = State.None;
        }

        protected override void MouseMove(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            var vp = viewport;
            if (vp == null) return;

            var p = SnapIfNeeded(vp.ProperScreenToWorld(e.X, e.Y));
            var cursor = Cursors.Default;

            switch (_state)
            {
                case State.None:
                    var st = GetStateAtPoint(e.X, e.Y, vp, out _stateCamera);
                    if (st != State.None) cursor = Cursors.SizeAll;
                    break;
                case State.MovingPosition:
                    if (_stateCamera == null) break;
                    var newEye = vp.GetUnusedCoordinate(_stateCamera.EyePosition) + p;
                    if (KeyboardState.Ctrl) _stateCamera.LookPosition += (newEye - _stateCamera.EyePosition);
                    _stateCamera.EyePosition = newEye;
                    SetViewportCamera(_stateCamera.EyePosition, _stateCamera.LookPosition);
                    break;
                case State.MovingLook:
                    if (_stateCamera == null) break;
                    var newLook = vp.GetUnusedCoordinate(_stateCamera.LookPosition) + p;
                    if (KeyboardState.Ctrl) _stateCamera.EyePosition += (newLook - _stateCamera.LookPosition);
                    _stateCamera.LookPosition = newLook;
                    SetViewportCamera(_stateCamera.EyePosition, _stateCamera.LookPosition);
                    break;
            }
            vp.Control.Cursor = cursor;
        }

        protected override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            var list = base.GetViewportElements(viewport, camera).ToList();

            foreach (var cam in GetCameraList())
            {
                var p1 = cam.EyePosition.ToVector3();
                var p2 = cam.LookPosition.ToVector3();

                var lineColor = cam.IsActive ? Color.Red : Color.Cyan;
                var handleColor = cam.IsActive ? Color.DarkOrange : Color.LawnGreen;

                list.Add(new LineElement(PositionType.World, lineColor, new List<Position> { new Position(p1), new Position(p2) }));
                list.Add(new HandleElement(PositionType.World, HandleElement.HandleType.Circle, new Position(p1), 4) { Color = handleColor, LineColor = Color.Black });
                // todo: triangle arrow?
            }

            return list;
        }
    }
}
