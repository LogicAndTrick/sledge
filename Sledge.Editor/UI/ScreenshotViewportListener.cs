using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Sledge.UI;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Sledge.Editor.UI
{
    public class ScreenshotViewportListener : IViewportEventListener
    {
        public ViewportBase Viewport { get; set; }
        public Image Screenshot { get; private set; }

        public ScreenshotViewportListener(ViewportBase viewport)
        {
            Viewport = viewport;
        }

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

        public void PostRender()
        {
            var bmp = new Bitmap(Viewport.ClientSize.Width, Viewport.ClientSize.Height);
            var data = bmp.LockBits(Viewport.ClientRectangle, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, Viewport.ClientSize.Width, Viewport.ClientSize.Height, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Screenshot = bmp;
        }
    }
}