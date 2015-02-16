using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using Sledge.Editor.Rendering;
using Sledge.Rendering;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Sledge.Editor.UI
{
    public class ScreenshotViewportListener : IViewportEventListener
    {
        public MapViewport Viewport { get; set; }
        public Image Screenshot { get; private set; }

        public ScreenshotViewportListener(MapViewport viewport)
        {
            Viewport = viewport;
        }

        public bool IsActive()
        {
            return true;
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

        public void DragStart(ViewportEvent e)
        {
            
        }

        public void DragMove(ViewportEvent e)
        {

        }

        public void DragEnd(ViewportEvent e)
        {

        }

        public void MouseEnter(ViewportEvent e)
        {
            
        }

        public void MouseLeave(ViewportEvent e)
        {
            
        }

        public void ZoomChanged(ViewportEvent e)
        {

        }

        public void PositionChanged(ViewportEvent e)
        {

        }

        public void UpdateFrame(Frame frame)
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
            var bmp = new Bitmap(Viewport.Control.ClientSize.Width, Viewport.Control.ClientSize.Height);
            var data = bmp.LockBits(Viewport.Control.ClientRectangle, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, Viewport.Control.ClientSize.Width, Viewport.Control.ClientSize.Height, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Screenshot = bmp;
        }
    }
}