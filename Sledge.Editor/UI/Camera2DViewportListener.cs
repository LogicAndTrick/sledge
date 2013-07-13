using System;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
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
            var pt = Viewport2D.Expand(Viewport2D.ScreenToWorld(new Coordinate(e.X, Viewport2D.Height - e.Y, 0)));
            Mediator.Publish(EditorMediator.MouseCoordinatesChanged, pt);
        }

        public void MouseWheel(MouseEventArgs e)
        {
            var before = Viewport2D.ScreenToWorld(e.X, Viewport2D.Height - e.Y);
            Viewport2D.Zoom *= (decimal)Math.Pow(1.2, (e.Delta < 0 ? -1 : 1));
            var after = Viewport2D.ScreenToWorld(e.X, Viewport2D.Height - e.Y);
            Viewport2D.Position -= (after - before);

            Mediator.Publish(EditorMediator.ViewZoomChanged, Viewport2D.Zoom);
        }

        public void MouseUp(MouseEventArgs e)
        {
            
        }

        public void MouseDown(MouseEventArgs e)
        {

        }

        public void MouseEnter(EventArgs e)
        {
            Mediator.Publish(EditorMediator.ViewFocused);
            Mediator.Publish(EditorMediator.ViewZoomChanged, Viewport2D.Zoom);
        }

        public void MouseLeave(EventArgs e)
        {
            Mediator.Publish(EditorMediator.ViewUnfocused);
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
