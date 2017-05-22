using System;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Rendering.Viewport
{
    public class OrthographicCameraViewportListener : IViewportEventListener
    {
        public MapViewport Viewport { get; set; }

        private OrthographicCamera Camera => Viewport.Viewport.Camera as OrthographicCamera;

        public OrthographicCameraViewportListener(MapViewport viewport)
        {
            Viewport = viewport;
        }

        public bool IsActive()
        {
            return Viewport != null && Viewport.Is2D;
        }

        public void KeyUp(ViewportEvent e)
        {
            if (e.KeyCode == Keys.Space)
            {
                Viewport.Control.Cursor = Cursors.Default;
                Viewport.Control.Capture = false;
                e.Handled = true;
            }
        }

        public void KeyDown(ViewportEvent e)
        {
            if (e.KeyCode == Keys.Space)
            {
                Viewport.Control.Cursor = Cursors.SizeAll;
                if (false) //!Sledge.Settings.View.Camera2DPanRequiresMouseClick)
                {
                    Viewport.Control.Capture = true;
                    var p = e.Sender.Control.PointToClient(Cursor.Position);
                    _mouseDown = new Coordinate(p.X, Viewport.Height - p.Y, 0);
                }
                e.Handled = true;
            }

            var moveAllowed = false; //DocumentManager.CurrentDocument != null && (DocumentManager.CurrentDocument.Selection.IsEmpty() || !Sledge.Settings.Select.ArrowKeysNudgeSelection);
            if (moveAllowed)
            {
                var shift = new Coordinate(0, 0, 0);

                switch (e.KeyCode)
                {
                    case Keys.Left:
                        shift.X = (decimal) (-Viewport.Width / Camera.Zoom / 4);
                        break;
                    case Keys.Right:
                        shift.X = (decimal)(Viewport.Width / Camera.Zoom / 4);
                        break;
                    case Keys.Up:
                        shift.Y = (decimal)(Viewport.Height / Camera.Zoom / 4);
                        break;
                    case Keys.Down:
                        shift.Y = (decimal)(-Viewport.Height / Camera.Zoom / 4);
                        break;
                }

                Camera.Position += shift.ToVector3();
            }

            var str = e.KeyCode.ToString();
            if (str.StartsWith("NumPad") || str.StartsWith("D"))
            {
                var last = str.Last();
                if (Char.IsDigit(last))
                {
                    var press = (int) Char.GetNumericValue(last);
                    if (press >= 0 && press <= 9)
                    {
                        if (press == 0) press = 10;
                        var num = Math.Max(press - 6, 6 - press);
                        var pow = (decimal) Math.Pow(2, num);
                        var zoom = press < 6 ? 1 / pow : pow;
                        Camera.Zoom = (float) zoom;
                        // Mediator.Publish(EditorMediator.ViewZoomChanged, Camera.Zoom);
                    }
                }
            }
        }

        public void MouseMove(ViewportEvent e)
        {
            var lmouse = Control.MouseButtons.HasFlag(MouseButtons.Left);
            var mmouse = Control.MouseButtons.HasFlag(MouseButtons.Middle);
            var space = KeyboardState.IsKeyDown(Keys.Space);
            if (space || mmouse)
            {
                Viewport.Control.Cursor = Cursors.SizeAll;
                if (lmouse || mmouse /*|| !Sledge.Settings.View.Camera2DPanRequiresMouseClick*/)
                {
                    var point = new Coordinate(e.X, Viewport.Height - e.Y, 0);
                    if (_mouseDown != null)
                    {
                        var difference = _mouseDown - point;
                        Camera.Position += (difference / (decimal)Viewport.Zoom).ToVector3();
                    }
                    _mouseDown = point;
                    e.Handled = true;
                }
            }

            var pt = Viewport.ProperScreenToWorld(new Coordinate(e.X, e.Y, 0));
            //Mediator.Publish(EditorMediator.MouseCoordinatesChanged, pt);
        }

        public void MouseWheel(ViewportEvent e)
        {
            var before = Viewport.Flatten(Viewport.ProperScreenToWorld(e.X, e.Y));
            Camera.Zoom *= (float) DMath.Pow(/*Sledge.Settings.View.ScrollWheelZoomMultiplier*/ 1.4m, (e.Delta < 0 ? -1 : 1));
            var after = Viewport.Flatten(Viewport.ProperScreenToWorld(e.X, e.Y));
            Camera.Position -= (after - before).ToVector3();

            //Mediator.Publish(EditorMediator.ViewZoomChanged, Camera.Zoom);
            if (KeyboardState.IsKeyDown(Keys.ControlKey))
            {
                //Mediator.Publish(EditorMediator.SetZoomValue, Camera.Zoom);
            }
        }

        public void MouseUp(ViewportEvent e)
        {
            var space = KeyboardState.IsKeyDown(Keys.Space);
            var req = true;// Sledge.Settings.View.Camera2DPanRequiresMouseClick;
            if (space && (!req || e.Button == MouseButtons.Left))
            {
                e.Handled = true;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                e.Handled = true;
                Viewport.Control.Cursor = Cursors.Default;
            }
        }

        private Coordinate _mouseDown;

        public void MouseDown(ViewportEvent e)
        {
            var space = KeyboardState.IsKeyDown(Keys.Space);
            var req = true; // Sledge.Settings.View.Camera2DPanRequiresMouseClick;
            if (space && (!req || e.Button == MouseButtons.Left))
            {
                e.Handled = true;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                e.Handled = true;
                Viewport.Control.Cursor = Cursors.SizeAll;
            }
            _mouseDown = new Coordinate(e.X, Viewport.Height - e.Y, 0);
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
            if (KeyboardState.IsKeyDown(Keys.Space))
            {
                Viewport.Control.Cursor = Cursors.SizeAll;
            }
            //Mediator.Publish(EditorMediator.ViewFocused);
            //Mediator.Publish(EditorMediator.ViewZoomChanged, Camera.Zoom);
        }

        public void MouseLeave(ViewportEvent e)
        {
            Viewport.Control.Cursor = Cursors.Default;
            //Mediator.Publish(EditorMediator.ViewUnfocused);
        }

        public void ZoomChanged(ViewportEvent e)
        {

        }

        public void PositionChanged(ViewportEvent e)
        {

        }

        private const decimal ScrollStart = 1;
        private const decimal ScrollIncrement = 0.025m;
        private const int ScrollMaximum = 200;
        private const int ScrollPadding = 40;

        public void UpdateFrame(Frame frame)
        {
            if (Viewport.Viewport.IsFocused && _mouseDown != null && Control.MouseButtons.HasFlag(MouseButtons.Left) && !KeyboardState.IsKeyDown(Keys.Space))
            {
                var pt = Viewport.Control.PointToClient(Control.MousePosition);
                var pos = Camera.Position.ToCoordinate();
                if (pt.X < ScrollPadding)
                {
                    var mx = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, ScrollPadding - pt.X);
                    mx = mx * mx + ScrollStart;
                    pos.X -= mx / (decimal) Viewport.Zoom;
                }
                else if (pt.X > Viewport.Width - ScrollPadding)
                {
                    var mx = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, pt.X - (Viewport.Width - ScrollPadding));
                    mx = mx * mx + ScrollStart;
                    pos.X += mx / (decimal)Viewport.Zoom;
                }
                if (pt.Y < ScrollPadding)
                {
                    var my = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, ScrollPadding - pt.Y);
                    my = my * my + ScrollStart;
                    pos.Y += my / (decimal)Viewport.Zoom;
                }
                else if (pt.Y > Viewport.Height - ScrollPadding)
                {
                    var my = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, pt.Y - (Viewport.Height - ScrollPadding));
                    my = my * my + ScrollStart;
                    pos.Y -= my / (decimal)Viewport.Zoom;
                }
                Camera.Position = pos.ToVector3();
            }
        }
    }
}
