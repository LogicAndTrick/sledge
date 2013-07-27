using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Graphics;
using Sledge.UI;
using OpenTK.Graphics.OpenGL;
using Sledge.Editor.Tools;
using KeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;
using Sledge.Graphics.Helpers;

namespace Sledge.Editor.UI
{
    public class Camera3DViewportListener : IViewportEventListener
    {
        public ViewportBase Viewport { get; set; }

        private int LastKnownX { get; set; }
        private int LastKnownY { get; set; }
        private bool PositionKnown { get; set; }
        private bool FreeLook { get; set; }
        private bool CursorVisible { get; set; }
        private bool Focus { get; set; }
        private Camera Camera { get; set; }

        public Camera3DViewportListener(Viewport3D vp)
        {
            LastKnownX = 0;
            LastKnownY = 0;
            PositionKnown = false;
            FreeLook = false;
            CursorVisible = true;
            Focus = false;
            Viewport = vp;
            Camera = vp.Camera;
        }

        public void UpdateFrame()
        {
            if (!Focus) return;
            var amt = 15m;
            if (KeyboardState.IsKeyDown(Keys.ShiftKey))
            {
                amt *= 2;
            }
            if (KeyboardState.IsKeyDown(Keys.ControlKey))
            {
                amt = 1m;
            }
            if (KeyboardState.IsKeyDown(Keys.W))
            {
                Camera.Advance(amt);
            }

            if (KeyboardState.IsKeyDown(Keys.S))
            {
                Camera.Advance(-amt);
            }

            if (KeyboardState.IsKeyDown(Keys.A))
            {
                Camera.Strafe(-amt);
            }

            if (KeyboardState.IsKeyDown(Keys.D))
            {
                Camera.Strafe(amt);
            }
        }

        public void PreRender()
        {
            //
        }

        public void Render3D()
        {
            //
        }

        public void Render2D()
        {
            if (!Focus || !FreeLook) return;

            TextureHelper.DisableTexturing();
            GL.Begin(BeginMode.Lines);
            GL.Color3(Color.White);
            GL.Vertex2(0, -5);
            GL.Vertex2(0, 5);
            GL.Vertex2(1, -5);
            GL.Vertex2(1, 5);
            GL.Vertex2(-5, 0);
            GL.Vertex2(5, 0);
            GL.Vertex2(-5, 1);
            GL.Vertex2(5, 1);
            GL.End();
            TextureHelper.EnableTexturing();
        }

        public void KeyUp(ViewportEvent e)
        {
            //
        }

        public void KeyDown(ViewportEvent e)
        {
            if (!Focus) return;
            if (e.KeyCode == Keys.Z)
            {
                FreeLook = !FreeLook;
                if (FreeLook && CursorVisible)
                {
                    CursorVisible = false;
                    Cursor.Hide();
                }
                else if (!FreeLook && !CursorVisible)
                {
                    CursorVisible = true;
                    Cursor.Show();
                }
            }
        }

        public void KeyPress(ViewportEvent e)
        {
            // Nothing.
        }

        public void MouseMove(ViewportEvent e)
        {
            if (!Focus) return;
            if (PositionKnown && FreeLook)
            {
                var dx = LastKnownX - e.X;
                var dy = e.Y - LastKnownY;
                if (dx != 0 || dy != 0)
                {
                    // Camera
                    var fovdiv = (Viewport.Width / 60m) / 2.5m;
                    Camera.Pan(dx / fovdiv);
                    Camera.Tilt(dy / fovdiv);
                    LastKnownX = Viewport.Width / 2;
                    LastKnownY = Viewport.Height / 2;
                    Cursor.Position = Viewport.PointToScreen(new Point(LastKnownX, LastKnownY));
                    return;
                }
            }
            LastKnownX = e.X;
            LastKnownY = e.Y;
            PositionKnown = true;
        }

        public void MouseWheel(ViewportEvent e)
        {
            if (!Focus || (ToolManager.ActiveTool != null && ToolManager.ActiveTool.IsCapturingMouseWheel())) return;
            Camera.Advance((e.Delta / Math.Abs(e.Delta)) * 500);
        }

        public void MouseUp(ViewportEvent e)
        {
            // Nothing.
        }

        public void MouseDown(ViewportEvent e)
        {
            // Nothing.
        }

        public void MouseEnter(ViewportEvent e)
        {
            Focus = true;
        }

        public void MouseLeave(ViewportEvent e)
        {
            if (FreeLook)
            {
                FreeLook = false;
                if (!CursorVisible)
                {
                    CursorVisible = true;
                    Cursor.Show();
                }
            }
            PositionKnown = false;
            Focus = false;
        }
    }
}
