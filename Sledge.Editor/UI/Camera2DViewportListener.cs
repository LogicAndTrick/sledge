using System;
using System.Windows.Forms;
using Sledge.UI;

namespace Sledge.Editor.UI
{
    public class Camera2DViewportListener : IViewportEventListener
    {
        public ViewportBase Viewport
        {
            get { return Viewport2D; }
            set { Viewport2D = (Viewport2D) value; }
        }

        public Viewport2D Viewport2D { get; set; }

        public Camera2DViewportListener(Viewport2D viewport)
        {
            Viewport = viewport;
            Viewport2D = viewport;
        }

        public void KeyUp(KeyEventArgs e)
        {
            
        }

        public void KeyDown(KeyEventArgs e)
        {
            
        }

        public void KeyPress(KeyPressEventArgs e)
        {
            
        }

        public void MouseMove(MouseEventArgs e)
        {
            
        }

        public void MouseWheel(MouseEventArgs e)
        {
            var before = Viewport2D.ScreenToWorld(e.X, Viewport2D.Height - e.Y);
            Viewport2D.Zoom *= (decimal)Math.Pow(1.2, (e.Delta < 0 ? -1 : 1));
            var after = Viewport2D.ScreenToWorld(e.X, Viewport2D.Height - e.Y);
            Viewport2D.Position -= (after - before);
        }

        public void MouseUp(MouseEventArgs e)
        {
            
        }

        public void MouseDown(MouseEventArgs e)
        {

        }

        public void MouseEnter(EventArgs e)
        {
            
        }

        public void MouseLeave(EventArgs e)
        {
            
        }

        public void UpdateFrame()
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
    }
}
