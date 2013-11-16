using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Properties;
using Sledge.Editor.UI;
using Sledge.Graphics;
using Sledge.Settings;
using Sledge.UI;
using Tao.OpenGl;

namespace Sledge.Editor.Tools
{
    public class CameraTool : BaseTool
    {
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
            //
        }

        public override void MouseUp(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseWheel(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseMove(ViewportBase viewport, ViewportEvent e)
        {
            //
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
            var pos = new Coordinate((decimal)cam.Location.X, (decimal)cam.Location.Y, (decimal)cam.Location.Z);
            var look = new Coordinate((decimal)cam.LookAt.X, (decimal)cam.LookAt.Y, (decimal)cam.LookAt.Z);
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
