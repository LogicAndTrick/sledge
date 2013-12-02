using System;
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

        private Tuple<Coordinate, Coordinate> GetCamera()
        {
            var cam = ViewportManager.Viewports.OfType<Viewport3D>().Select(x => x.Camera).FirstOrDefault();
            if (cam == null) return null;

            var pos = new Coordinate((decimal)cam.Location.X, (decimal)cam.Location.Y, (decimal)cam.Location.Z);
            var look = new Coordinate((decimal)cam.LookAt.X, (decimal)cam.LookAt.Y, (decimal)cam.LookAt.Z);
            return Tuple.Create(pos, look);
        }

        private void SetCamera(Coordinate position, Coordinate look)
        {
            var cam = ViewportManager.Viewports.OfType<Viewport3D>().Select(x => x.Camera).FirstOrDefault();
            if (cam == null) return;

            look = (look - position).Normalise() + position;
            cam.Location = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
            cam.LookAt = new Vector3((float) look.X, (float) look.Y, (float) look.Z);
        }

        private State GetStateAtPoint(int x, int y, Viewport2D viewport)
        {
            var cam = GetCamera();
            if (cam == null) return State.None;

            var p = viewport.ScreenToWorld(x, y);
            var pos = viewport.Flatten(cam.Item1);
            var look = pos + (viewport.Flatten(cam.Item2) - pos).Normalise() * 50 / viewport.Zoom;

            var d = 5 / viewport.Zoom;

            if (p.X >= pos.X - d && p.X <= pos.X + d && p.Y >= pos.Y - d && p.Y <= pos.Y + d) return State.MovingPosition;
            //todo... if (p.X >= look.X - d && p.X <= look.X + d && p.Y >= look.Y - d && p.Y <= look.Y + d) return State.MovingLook;

            return State.None;
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
            _state = GetStateAtPoint(e.X, vp.Height - e.Y, vp);
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

            var p = vp.Expand(vp.ScreenToWorld(e.X, vp.Height - e.Y));
            var cursor = Cursors.Default;
            var cam = GetCamera();

            switch (_state)
            {
                case State.None:
                    var st = GetStateAtPoint(e.X, vp.Height - e.Y, vp);
                    if (st != State.None) cursor = Cursors.SizeAll;
                    break;
                case State.MovingPosition:
                    var pos = vp.GetUnusedCoordinate(cam.Item1) + p;
                    SetCamera(pos, (cam.Item2 - cam.Item1) + pos);
                    break;
                /* //TODO this is harder than it looks....
                 * case State.MovingLook:
                    var h = cam.Item2 - cam.Item1;
                    var a = vp.ZeroUnusedCoordinate(h);
                    var angle = DMath.Acos(a.VectorMagnitude() / h.VectorMagnitude());
                    //var dir = (a - h).Normalise();
                    var dir = vp.GetUnusedCoordinate(h).Normalise();
                    var dist = p - vp.ZeroUnusedCoordinate(cam.Item1);
                    //h = dir * (a.VectorMagnitude() * DMath.Tan(angle));
                    Debug.WriteLine((a - h).Normalise() + " vs " + vp.GetUnusedCoordinate(h).Normalise());
                    Debug.WriteLine(dir + " " + dist + " " + dir * dist.VectorMagnitude() + " " + DMath.Tan(angle));
                    Debug.WriteLine(p + dir * dist.VectorMagnitude() * DMath.Tan(angle));
                    SetCamera(cam.Item1, p + dir * dist.VectorMagnitude() * DMath.Tan(angle));
                    break;
                 */ 
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

            var cam = ViewportManager.Viewports.OfType<Viewport3D>().Select(x => x.Camera).FirstOrDefault();
            if (cam == null) return;

            var z = (double)vp.Zoom;
            var camera = GetCamera();
            var pos = camera.Item1;
            var look = camera.Item2;
            var p1 = vp.Flatten(pos);
            var p2 = p1 + (vp.Flatten(look) - p1).Normalise() * 50 / vp.Zoom;

            var multiplier = 4 / vp.Zoom;
            var dir = (p2 - p1).Normalise();
            var cp = new Coordinate(-dir.Y, dir.X, 0).Normalise();
            
            GL.Enable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

            // Draw lines between points and point outlines
            GL.Begin(BeginMode.Lines);

            GL.Color3(Color.Red);
            GL.Vertex2(p1.DX, p1.DY);
            GL.Vertex2(p2.DX, p2.DY);
            GL.Vertex2(p2.DX, p2.DY);
            GL.Vertex2(p1.DX, p1.DY);

            GL.Color3(Color.Black);
            GLX.Circle(new Vector2d(p1.DX, p1.DY), 4, z);
            Coord(p2 + dir * 1.5m * multiplier);
            Coord(p2 - (dir + cp) * multiplier);
            Coord(p2 - (dir + cp) * multiplier);
            Coord(p2 - (dir - cp) * multiplier);
            Coord(p2 - (dir - cp) * multiplier);
            Coord(p2 + dir * 1.5m * multiplier);

            GL.End();


            GL.Enable(EnableCap.PolygonSmooth);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);

            // Position circle
            GL.Begin(BeginMode.Polygon);
            GL.Color3(Color.Cyan);
            GLX.Circle(new Vector2d(p1.DX, p1.DY), 4, z, loop: true);
            GL.End();

            // Direction Triangle
            GL.Begin(BeginMode.Triangles);
            GL.Color3(Color.LawnGreen);
            Coord(p2 + dir * 1.5m * multiplier);
            Coord(p2 - (dir + cp) * multiplier);
            Coord(p2 - (dir - cp) * multiplier);
            GL.End();

            GL.Disable(EnableCap.PolygonSmooth);
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
