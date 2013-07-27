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

        public void KeyUp(ViewportEvent e)
        {
            if (e.KeyCode == Keys.Space)
            {
                Viewport.Cursor = Cursors.Default;
                e.Handled = true;
            }
        }

        public void KeyDown(ViewportEvent e)
        {
            if (e.KeyCode == Keys.Space)
            {
                Viewport.Cursor = Cursors.SizeAll;
                e.Handled = true;
            }
        }

        public void KeyPress(ViewportEvent e)
        {

        }

        public void MouseMove(ViewportEvent e)
        {
            var lmouse = Control.MouseButtons.HasFlag(MouseButtons.Left);
            var mmouse = Control.MouseButtons.HasFlag(MouseButtons.Middle);
            var space = KeyboardState.IsKeyDown(Keys.Space);
            if (space || mmouse)
            {
                Viewport.Cursor = Cursors.SizeAll;

                if (lmouse || mmouse)
                {
                    var point = new Coordinate(e.X, Viewport2D.Height - e.Y, 0);
                    var difference = _mouseDown - point;
                    Viewport2D.Position += difference / Viewport2D.Zoom;
                    _mouseDown = point;
                    e.Handled = true;
                }
            }

            var pt = Viewport2D.Expand(Viewport2D.ScreenToWorld(new Coordinate(e.X, Viewport2D.Height - e.Y, 0)));
            Mediator.Publish(EditorMediator.MouseCoordinatesChanged, pt);
        }

        public void MouseWheel(ViewportEvent e)
        {
            var before = Viewport2D.ScreenToWorld(e.X, Viewport2D.Height - e.Y);
            Viewport2D.Zoom *= (decimal)Math.Pow(1.2, (e.Delta < 0 ? -1 : 1));
            var after = Viewport2D.ScreenToWorld(e.X, Viewport2D.Height - e.Y);
            Viewport2D.Position -= (after - before);

            Mediator.Publish(EditorMediator.ViewZoomChanged, Viewport2D.Zoom);
        }

        public void MouseUp(ViewportEvent e)
        {
            if ((KeyboardState.IsKeyDown(Keys.Space) && e.Button == MouseButtons.Left) || e.Button == MouseButtons.Middle) e.Handled = true;
            if (e.Button == MouseButtons.Middle) Viewport.Cursor = Cursors.Default;
            _mouseDown = null;
        }

        private Coordinate _mouseDown;

        public void MouseDown(ViewportEvent e)
        {
            if ((KeyboardState.IsKeyDown(Keys.Space) && e.Button == MouseButtons.Left) || e.Button == MouseButtons.Middle) e.Handled = true;
            if (e.Button == MouseButtons.Middle) Viewport.Cursor = Cursors.SizeAll;
            _mouseDown = new Coordinate(e.X, Viewport2D.Height - e.Y, 0);
        }

        public void MouseEnter(ViewportEvent e)
        {
            if (KeyboardState.IsKeyDown(Keys.Space))
            {
                Viewport.Cursor = Cursors.SizeAll;
            }
            Mediator.Publish(EditorMediator.ViewFocused);
            Mediator.Publish(EditorMediator.ViewZoomChanged, Viewport2D.Zoom);
        }

        public void MouseLeave(ViewportEvent e)
        {
            Viewport.Cursor = Cursors.Default;
            Mediator.Publish(EditorMediator.ViewUnfocused);
        }

        private const decimal ScrollStart = 1;
        private const decimal ScrollIncrement = 0.025m;
        private const int ScrollMaximum = 200;
        private const int ScrollPadding = 40;

        public void UpdateFrame()
        {
            if (Viewport2D.IsFocused && _mouseDown != null && Control.MouseButtons.HasFlag(MouseButtons.Left) && !KeyboardState.IsKeyDown(Keys.Space))
            {
                var pt = Viewport2D.PointToClient(Control.MousePosition);
                if (pt.X < ScrollPadding)
                {
                    var mx = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, ScrollPadding - pt.X);
                    mx = mx * mx + ScrollStart;
                    Viewport2D.Position.X -= mx / Viewport2D.Zoom;
                }
                else if (pt.X > Viewport2D.Width - ScrollPadding)
                {
                    var mx = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, pt.X - (Viewport2D.Width - ScrollPadding));
                    mx = mx * mx + ScrollStart;
                    Viewport2D.Position.X += mx / Viewport2D.Zoom;
                }
                if (pt.Y < ScrollPadding)
                {
                    var my = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, ScrollPadding - pt.Y);
                    my = my * my + ScrollStart;
                    Viewport2D.Position.Y += my / Viewport2D.Zoom;
                }
                else if (pt.Y > Viewport2D.Height - ScrollPadding)
                {
                    var my = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, pt.Y - (Viewport2D.Height - ScrollPadding));
                    my = my * my + ScrollStart;
                    Viewport2D.Position.Y -= my / Viewport2D.Zoom;
                }
            }
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
