using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hotkeys;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;
using Sledge.Shell.Input;
using Camera = Sledge.BspEditor.Primitives.MapData.Camera;

namespace Sledge.BspEditor.Tools
{
    [Export(typeof(ITool))]
    [OrderHint("D")]
    [DefaultHotkey("Shift+C")]
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

        private readonly Lazy<MapDocumentControlHost> _controlHost;

        [ImportingConstructor]
        public CameraTool(
            [Import] Lazy<MapDocumentControlHost> controlHost
        )
        {
            _controlHost = controlHost;
        }
        
        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<object>("BspEditor:CameraNext", CameraNext);
            yield return Oy.Subscribe<object>("BspEditor:CameraPrevious", CameraPrevious);
        }

        public override Task ToolSelected()
        {
            _state = State.None;
            return Task.CompletedTask;
        }

        private Task CameraNext(object param)
        {
            var document = GetDocument();
            if (document == null) return Task.CompletedTask;

            var cams = GetDocumentCameras(document);

            if (_state == State.None && cams.Count >= 2)
            {
                var idx = cams.FindIndex(x => x.IsActive);
                idx = (idx + 1) % cams.Count;
                cams.ForEach(x => x.IsActive = false);
                cams[idx].IsActive = true;
                SetViewportCamera(cams[idx].EyePosition, cams[idx].LookPosition);
            }

            return Task.CompletedTask;
        }

        private Task CameraPrevious(object param)
        {
            var document = GetDocument();
            if (document == null) return Task.CompletedTask;

            var cams = GetDocumentCameras(document);

            if (_state == State.None && cams.Count >= 2)
            {
                var idx = cams.FindIndex(x => x.IsActive);
                idx = (idx + cams.Count - 1) % cams.Count;
                cams.ForEach(x => x.IsActive = false);
                cams[idx].IsActive = true;
                SetViewportCamera(cams[idx].EyePosition, cams[idx].LookPosition);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        private void CameraDelete()
        {
            var document = GetDocument();
            if (document == null) return;

            var cams = GetDocumentCameras(document);
            if (_state != State.None || cams.Count < 2) return;
            var del = cams.FirstOrDefault(x => x.IsActive);
            CameraPrevious(null);
            if (del != GetDocumentCameras(document).FirstOrDefault(x => x.IsActive)) document.Map.Data.Remove(del);
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Camera;
        }

        public override string GetName()
        {
            return "Camera Tool";
        }

        private Tuple<Vector3, Vector3> GetViewportCamera()
        {
            var cam = _controlHost.Value.GetControls().OfType<ViewportMapDocumentControl>().Select(x => x.Camera).OfType<PerspectiveCamera>().FirstOrDefault();
            if (cam == null) return null;

            var pos = cam.Position;
            var look = pos + cam.Direction;

            var dir = (look - pos).Normalise()*20;
            return Tuple.Create(pos, pos + dir);
        }

        private void SetViewportCamera(Vector3 position, Vector3 look)
        {
            var cam = _controlHost.Value.GetControls().OfType<ViewportMapDocumentControl>().Select(x => x.Camera).OfType<PerspectiveCamera>().FirstOrDefault();
            if (cam == null) return;

            look = (look - position).Normalise() + position;
            cam.Position = position;
            cam.Direction = look - position;
        }

        private State GetStateAtPoint(MapDocument document, int x, int y, OrthographicCamera camera, out Camera activeCamera)
        {
            var d = 5 / camera.Zoom;

            foreach (var cam in GetCameraList(document))
            {
                var p = camera.Flatten(camera.ScreenToWorld(new Vector3(x, y, 0)));
                var pos = camera.Flatten(cam.EyePosition);
                var look = camera.Flatten(cam.LookPosition);
                activeCamera = cam;
                if (p.X >= pos.X - d && p.X <= pos.X + d && p.Y >= pos.Y - d && p.Y <= pos.Y + d) return State.MovingPosition;
                if (p.X >= look.X - d && p.X <= look.X + d && p.Y >= look.Y - d && p.Y <= look.Y + d) return State.MovingLook;
            }

            activeCamera = null;
            return State.None;
        }

        private List<Camera> GetDocumentCameras(MapDocument document)
        {
            return document.Map.Data.Get<Camera>().ToList();
        }

        private List<Camera> GetCameraList(MapDocument document)
        {
            var c = GetViewportCamera();
            if (!document.Map.Data.Get<Camera>().Any())
            {
                document.Map.Data.Add(new Camera {EyePosition = c.Item1, LookPosition = c.Item2});
            }
            var active = document.Map.Data.Get<Camera>().FirstOrDefault(x => x.IsActive);
            if (active == null)
            {
                active = document.Map.Data.GetOne<Camera>() ?? new Camera();
                active.IsActive = true;
            }
            var len = active.Length;
            active.EyePosition = c.Item1;
            active.LookPosition = c.Item1 + (c.Item2 - c.Item1).Normalise() * len;

            var gs = document.Map.Data.GetOne<GridData>()?.Grid?.Spacing ?? 64;
            var cameras = new List<Camera>();
            foreach (var camera in document.Map.Data.Get<Camera>())
            {
                var dir = camera.LookPosition - camera.EyePosition;
                camera.LookPosition = camera.EyePosition + dir.Normalise() * Math.Max(gs * 1.5f, dir.Length());
                cameras.Add(camera);
            }
            return cameras;
        }

        protected override void MouseDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            var vp = viewport;
            if (vp == null) return;

            var gs = document.Map.Data.GetOne<GridData>()?.Grid?.Spacing ?? 64;

            _state = GetStateAtPoint(document, e.X, e.Y, camera, out _stateCamera);
            if (_state == State.None && KeyboardState.Shift)
            {
                var p = SnapIfNeeded(camera.ScreenToWorld(e.X, e.Y));
                _stateCamera = new Camera { EyePosition = p, LookPosition = p + Vector3.UnitX * 1.5f * gs };
                document.Map.Data.Add(_stateCamera);
                _state = State.MovingLook;
            }
            if (_stateCamera != null)
            {
                SetViewportCamera(_stateCamera.EyePosition, _stateCamera.LookPosition);
                GetDocumentCameras(document).ForEach(x => x.IsActive = false);
                _stateCamera.IsActive = true;
            }
        }

        protected override void MouseUp(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            _state = State.None;
        }

        protected override void MouseMove(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            var vp = viewport;
            if (vp == null) return;

            var p = SnapIfNeeded(camera.ScreenToWorld(e.X, e.Y));
            var cursor = Cursors.Default;

            switch (_state)
            {
                case State.None:
                    var st = GetStateAtPoint(document, e.X, e.Y, camera, out _stateCamera);
                    if (st != State.None) cursor = Cursors.SizeAll;
                    break;
                case State.MovingPosition:
                    if (_stateCamera == null) break;
                    var newEye = camera.GetUnusedCoordinate(_stateCamera.EyePosition) + p;
                    if (KeyboardState.Ctrl) _stateCamera.LookPosition += (newEye - _stateCamera.EyePosition);
                    _stateCamera.EyePosition = newEye;
                    SetViewportCamera(_stateCamera.EyePosition, _stateCamera.LookPosition);
                    break;
                case State.MovingLook:
                    if (_stateCamera == null) break;
                    var newLook = camera.GetUnusedCoordinate(_stateCamera.LookPosition) + p;
                    if (KeyboardState.Ctrl) _stateCamera.EyePosition += (newLook - _stateCamera.LookPosition);
                    _stateCamera.LookPosition = newLook;
                    SetViewportCamera(_stateCamera.EyePosition, _stateCamera.LookPosition);
                    break;
            }
            vp.Control.Cursor = cursor;
        }

        protected override void Render(MapDocument document, IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
        {
            base.Render(document, viewport, camera, worldMin, worldMax, im);

            foreach (var cam in GetCameraList(document))
            {
                var p1 = camera.WorldToScreen(cam.EyePosition);
                var p2 = camera.WorldToScreen(cam.LookPosition);
                
                var lineColor = cam.IsActive ? Color.Red : Color.Cyan;
                var handleColor = cam.IsActive ? Color.DarkOrange : Color.LawnGreen;

                im.AddLine(p1.ToVector2(), p2.ToVector2(), lineColor);
                im.AddCircleFilled(p1.ToVector2(), 4, handleColor);
                im.AddCircle(p1.ToVector2(), 4, Color.Black);
                
                // todo post-beta: triangle arrow for cameras in 2D?
            }
        }
    }
}
