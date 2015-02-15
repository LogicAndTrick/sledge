using System;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Rendering;
using Sledge.Extensions;
using Sledge.Editor.Documents;
using Sledge.Rendering;

namespace Sledge.Editor.UI
{
    public class Camera2DViewportListener : IViewportEventListener, IMediatorListener
    {
        public MapViewport Viewport
        {
            get { return MapViewport; }
            set { MapViewport = (MapViewport) value; }
        }

        public MapViewport MapViewport { get; set; }

        public Camera2DViewportListener(MapViewport viewport)
        {
            Viewport = viewport;
            MapViewport = viewport;
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
                if (!Sledge.Settings.View.Camera2DPanRequiresMouseClick)
                {
                    Viewport.Control.Capture = true;
                    var p = e.Sender.Control.PointToClient(Cursor.Position);
                    _mouseDown = new Coordinate(p.X, MapViewport.Height - p.Y, 0);
                }
                e.Handled = true;
            }

            var moveAllowed = DocumentManager.CurrentDocument != null &&
                              (DocumentManager.CurrentDocument.Selection.IsEmpty()
                               || !Sledge.Settings.Select.ArrowKeysNudgeSelection);
            if (moveAllowed)
            {
                var shift = new Coordinate(0, 0, 0);

                switch (e.KeyCode)
                {
                    case Keys.Left:
                        shift.X = (decimal) (-Viewport.Width / MapViewport.Zoom / 4);
                        break;
                    case Keys.Right:
                        shift.X = (decimal) (Viewport.Width / MapViewport.Zoom / 4);
                        break;
                    case Keys.Up:
                        shift.Y = (decimal) (Viewport.Height / MapViewport.Zoom / 4);
                        break;
                    case Keys.Down:
                        shift.Y = (decimal) (-Viewport.Height / MapViewport.Zoom / 4);
                        break;
                }

                //MapViewport.Position += shift; todo
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
                        // todo MapViewport.Zoom = zoom;
                        Mediator.Publish(EditorMediator.ViewZoomChanged, MapViewport.Zoom);
                    }
                }
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
                Viewport.Control.Cursor = Cursors.SizeAll;
                if (lmouse || mmouse || !Sledge.Settings.View.Camera2DPanRequiresMouseClick)
                {
                    var point = new Coordinate(e.X, MapViewport.Height - e.Y, 0);
                    if (_mouseDown != null)
                    {
                        var difference = _mouseDown - point;
                        // todo MapViewport.Position += difference / MapViewport.Zoom;
                    }
                    _mouseDown = point;
                    e.Handled = true;
                }
            }

            var pt = MapViewport.Expand(MapViewport.ScreenToWorld(new Coordinate(e.X, MapViewport.Height - e.Y, 0)));
            Mediator.Publish(EditorMediator.MouseCoordinatesChanged, pt);
        }

        public void MouseWheel(ViewportEvent e)
        {
            var before = MapViewport.ScreenToWorld(e.X, MapViewport.Height - e.Y);
            // todo MapViewport.Zoom *= DMath.Pow(Sledge.Settings.View.ScrollWheelZoomMultiplier, (e.Delta < 0 ? -1 : 1));
            var after = MapViewport.ScreenToWorld(e.X, MapViewport.Height - e.Y);
            // todo MapViewport.Position -= (after - before);

            Mediator.Publish(EditorMediator.ViewZoomChanged, MapViewport.Zoom);
            if (KeyboardState.IsKeyDown(Keys.ControlKey))
            {
                Mediator.Publish(EditorMediator.SetZoomValue, MapViewport.Zoom);
            }
        }

        public void MouseUp(ViewportEvent e)
        {
            var space = KeyboardState.IsKeyDown(Keys.Space);
            var req = Sledge.Settings.View.Camera2DPanRequiresMouseClick;
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
            var req = Sledge.Settings.View.Camera2DPanRequiresMouseClick;
            if (space && (!req || e.Button == MouseButtons.Left))
            {
                e.Handled = true;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                e.Handled = true;
                Viewport.Control.Cursor = Cursors.SizeAll;
            }
            _mouseDown = new Coordinate(e.X, MapViewport.Height - e.Y, 0);
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
            Mediator.Publish(EditorMediator.ViewFocused);
            Mediator.Publish(EditorMediator.ViewZoomChanged, MapViewport.Zoom);
        }

        public void MouseLeave(ViewportEvent e)
        {
            Viewport.Control.Cursor = Cursors.Default;
            Mediator.Publish(EditorMediator.ViewUnfocused);
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
            if (MapViewport.Viewport.IsFocused && _mouseDown != null && Control.MouseButtons.HasFlag(MouseButtons.Left) && !KeyboardState.IsKeyDown(Keys.Space))
            {
                var pt = MapViewport.Control.PointToClient(Control.MousePosition);
                if (pt.X < ScrollPadding)
                {
                    var mx = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, ScrollPadding - pt.X);
                    mx = mx * mx + ScrollStart;
                    // todo MapViewport.Position.X -= mx / MapViewport.Zoom;
                }
                else if (pt.X > MapViewport.Width - ScrollPadding)
                {
                    var mx = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, pt.X - (MapViewport.Width - ScrollPadding));
                    mx = mx * mx + ScrollStart;
                    // todo MapViewport.Position.X += mx / MapViewport.Zoom;
                }
                if (pt.Y < ScrollPadding)
                {
                    var my = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, ScrollPadding - pt.Y);
                    my = my * my + ScrollStart;
                    // todo MapViewport.Position.Y += my / MapViewport.Zoom;
                }
                else if (pt.Y > MapViewport.Height - ScrollPadding)
                {
                    var my = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, pt.Y - (MapViewport.Height - ScrollPadding));
                    my = my * my + ScrollStart;
                    // todo MapViewport.Position.Y -= my / MapViewport.Zoom;
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

        public void PostRender()
        {
            //
        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }
    }
}
