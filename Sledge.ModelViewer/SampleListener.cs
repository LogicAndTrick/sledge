using System.Diagnostics;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Sledge.UI
{
    public class SampleListener : IViewportEventListener
    {
        public ViewportBase Viewport { get; set; }
        public void KeyUp(ViewportEvent e)
        {
            
        }

        public void KeyDown(ViewportEvent e)
        {

        }

        public void KeyPress(ViewportEvent e)
        {

        }

        public void MouseMove(ViewportEvent e)
        {

        }

        public void MouseWheel(ViewportEvent e)
        {

        }

        public void MouseUp(ViewportEvent e)
        {

        }

        public void MouseDown(ViewportEvent e)
        {

        }

        public void MouseClick(ViewportEvent e)
        {
            Debug.WriteLine("CLICK! " + e.Button);
        }

        public void MouseDoubleClick(ViewportEvent e)
        {

        }

        public void MouseEnter(ViewportEvent e)
        {

        }

        public void MouseLeave(ViewportEvent e)
        {

        }

        public void UpdateFrame(FrameInfo frame)
        {

        }

        public void PreRender()
        {
        }

        public void Render3D()
        {

        }

        public void Render2D()
        {

        }

        Color c = Color.Red;
        public void PostRender()
        {
            c = c == Color.Red ? Color.Blue : Color.Red;
            GL.Color3(c);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex2(-1, 1);
            GL.Vertex2(1, 1);
            GL.Vertex2(1, -1);
            GL.End();
        }
    }
}