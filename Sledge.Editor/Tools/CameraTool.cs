using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Properties;
using Sledge.Editor.UI;
using Sledge.Extensions;
using Sledge.Graphics;
using Sledge.Settings;
using Sledge.UI;
using Tao.OpenGl;
using Camera = Sledge.DataStructures.MapObjects.Camera;

namespace Sledge.Editor.Tools
{
    public class CameraTool : BaseTool
    {
        private enum State
        {
            None,
            MovingPosition,
            MovingLook
        }

        private State _state;
        private DataStructures.MapObjects.Camera _stateCamera;

        public override void ToolSelected()
        {
            _state = State.None;
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Rotate;
        }

        public override string GetName()
        {
            return "Camera Tool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Camera;
        }

        private Tuple<Coordinate, Coordinate> GetViewportCamera()
        {
            var cam = ViewportManager.Viewports.OfType<Viewport3D>().Select(x => x.Camera).FirstOrDefault();
            if (cam == null) return null;

            var pos = new Coordinate((decimal)cam.Location.X, (decimal)cam.Location.Y, (decimal)cam.Location.Z);
            var look = new Coordinate((decimal)cam.LookAt.X, (decimal)cam.LookAt.Y, (decimal)cam.LookAt.Z);

            var dir = (look - pos).Normalise()*20;
            return Tuple.Create(pos, pos + dir);
        }

        private void SetViewportCamera(Coordinate position, Coordinate look)
        {
            var cam = ViewportManager.Viewports.OfType<Viewport3D>().Select(x => x.Camera).FirstOrDefault();
            if (cam == null) return;

            look = (look - position).Normalise() + position;
            cam.Location = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
            cam.LookAt = new Vector3((float) look.X, (float) look.Y, (float) look.Z);
        }

        private State GetStateAtPoint(int x, int y, Viewport2D viewport, out DataStructures.MapObjects.Camera activeCamera)
        {
            var d = 5 / viewport.Zoom;

            foreach (var cam in GetCameras())
            {
                var p = viewport.ScreenToWorld(x, y);
                var pos = viewport.Flatten(cam.EyePosition);
                var look = viewport.Flatten(cam.LookPosition);
                activeCamera = cam;
                if (p.X >= pos.X - d && p.X <= pos.X + d && p.Y >= pos.Y - d && p.Y <= pos.Y + d) return State.MovingPosition;
                if (p.X >= look.X - d && p.X <= look.X + d && p.Y >= look.Y - d && p.Y <= look.Y + d) return State.MovingLook;
            }

            activeCamera = null;
            return State.None;
        }

        private IEnumerable<Camera> GetCameras()
        {
            if (!Document.Map.Cameras.Any())
            {
                var c = GetViewportCamera();
                Document.Map.Cameras.Add(new Camera { EyePosition = c.Item1, LookPosition = c.Item2});
            }
            foreach (var camera in Document.Map.Cameras)
            {
                var dir = camera.LookPosition - camera.EyePosition;
                camera.LookPosition = camera.EyePosition + dir.Normalise() * Math.Max(Document.Map.GridSpacing * 1.5m, dir.VectorMagnitude());
                yield return camera;
            }
        }

        public override void MouseEnter(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseLeave(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseDown(ViewportBase viewport, ViewportEvent e)
        {
            var vp = viewport as Viewport2D;
            if (vp == null) return;
            _state = GetStateAtPoint(e.X, vp.Height - e.Y, vp, out _stateCamera);
            if (_stateCamera != null)
            {
                SetViewportCamera(_stateCamera.EyePosition, _stateCamera.LookPosition);
                Document.Map.ActiveCamera = _stateCamera;
            }
        }

        public override void MouseUp(ViewportBase viewport, ViewportEvent e)
        {
            _state = State.None;
        }

        public override void MouseWheel(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseMove(ViewportBase viewport, ViewportEvent e)
        {
            var vp = viewport as Viewport2D;
            if (vp == null) return;

            var p = SnapIfNeeded(vp.Expand(vp.ScreenToWorld(e.X, vp.Height - e.Y)));
            var cursor = Cursors.Default;

            switch (_state)
            {
                case State.None:
                    var st = GetStateAtPoint(e.X, vp.Height - e.Y, vp, out _stateCamera);
                    if (st != State.None) cursor = Cursors.SizeAll;
                    break;
                case State.MovingPosition:
                    if (_stateCamera == null) break;
                    _stateCamera.EyePosition = vp.GetUnusedCoordinate(_stateCamera.EyePosition) + p;
                    SetViewportCamera(_stateCamera.EyePosition, _stateCamera.LookPosition);
                    break;
                case State.MovingLook:
                    if (_stateCamera == null) break;
                    _stateCamera.LookPosition = vp.GetUnusedCoordinate(_stateCamera.LookPosition) + p;
                    SetViewportCamera(_stateCamera.EyePosition, _stateCamera.LookPosition);
                    break;
            }
            vp.Cursor = cursor;
        }

        public override void KeyPress(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void KeyDown(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void KeyUp(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void UpdateFrame(ViewportBase viewport)
        {
            //
        }

        public override void Render(ViewportBase viewport)
        {
            var vp = viewport as Viewport2D;
            if (vp == null) return;

            var cams = GetCameras().ToList();
            if (!cams.Any()) return;

            var z = (double)vp.Zoom;

            GL.Enable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

            // Draw lines between points and point outlines
            GL.Begin(BeginMode.Lines);

            foreach (var camera in cams)
            {
                var pos = camera.EyePosition;
                var look = camera.LookPosition;
                var p1 = vp.Flatten(pos);
                var p2 = vp.Flatten(look);

                var multiplier = 4/vp.Zoom;
                var dir = (p2 - p1).Normalise();
                var cp = new Coordinate(-dir.Y, dir.X, 0).Normalise();

                GL.Color3(Color.Red);
                GL.Vertex2(p1.DX, p1.DY);
                GL.Vertex2(p2.DX, p2.DY);
                GL.Vertex2(p2.DX, p2.DY);
                GL.Vertex2(p1.DX, p1.DY);
            }

            GL.End();

            GL.Enable(EnableCap.PolygonSmooth);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);

            foreach (var camera in cams)
            {
                var pos = camera.EyePosition;
                var p1 = vp.Flatten(pos);

                // Position circle
                GL.Begin(BeginMode.Polygon);
                GL.Color3(Color.Cyan);
                GLX.Circle(new Vector2d(p1.DX, p1.DY), 4, z, loop: true);
                GL.End();
            }
            foreach (var camera in cams)
            {
                var pos = camera.EyePosition;
                var look = camera.LookPosition;
                var p1 = vp.Flatten(pos);
                var p2 = vp.Flatten(look);

                var multiplier = 4 / vp.Zoom;
                var dir = (p2 - p1).Normalise();
                var cp = new Coordinate(-dir.Y, dir.X, 0).Normalise();

                // Direction Triangle
                GL.Begin(BeginMode.Triangles);
                GL.Color3(Color.LawnGreen);
                Coord(p2 + dir*1.5m*multiplier);
                Coord(p2 - (dir + cp)*multiplier);
                Coord(p2 - (dir - cp)*multiplier);
                GL.End();
            }

            GL.Disable(EnableCap.PolygonSmooth);

            GL.Begin(BeginMode.Lines);

            foreach (var camera in cams)
            {
                var pos = camera.EyePosition;
                var look = camera.LookPosition;
                var p1 = vp.Flatten(pos);
                var p2 = vp.Flatten(look);

                var multiplier = 4 / vp.Zoom;
                var dir = (p2 - p1).Normalise();
                var cp = new Coordinate(-dir.Y, dir.X, 0).Normalise();

                GL.Color3(Color.Black);
                GLX.Circle(new Vector2d(p1.DX, p1.DY), 4, z);
                Coord(p2 + dir * 1.5m * multiplier);
                Coord(p2 - (dir + cp) * multiplier);
                Coord(p2 - (dir + cp) * multiplier);
                Coord(p2 - (dir - cp) * multiplier);
                Coord(p2 - (dir - cp) * multiplier);
                Coord(p2 + dir * 1.5m * multiplier);
            }

            GL.End();

            GL.Disable(EnableCap.LineSmooth);
        }

        protected static void Coord(Coordinate c)
        {
            GL.Vertex3(c.DX, c.DY, c.DZ);
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage)
        {
            return HotkeyInterceptResult.Continue;
        }
    }
}
