using System;
using System.Linq;
using OpenTK.Input;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.Documents;
using Sledge.Extensions;
using Sledge.Gui.Components;
using Sledge.Gui.Structures;
using Sledge.Settings;

namespace Sledge.EditorNew.UI.Viewports
{
    public class Camera2DViewportEventListener : IViewportEventListener
    {
        public IMapViewport Viewport { get; set; }

        public IViewport2D Viewport2D
        {
            get { return (IViewport2D) Viewport; }
        }

        public Camera2DViewportEventListener(IViewport2D viewport)
        {
            Viewport = viewport;
        }

        private bool _dragSpace = false;
        private bool _dragMiddle = false;

        private void SetDragState(bool dragSpace, bool dragMiddle)
        {
            if (dragSpace != _dragSpace || dragMiddle != _dragMiddle)
            {
                if (dragSpace || dragMiddle)
                {
                    Cursor.SetCursor(Viewport, CursorType.SizeAll);
                    Viewport.AquireInputLock(this);
                }
                else
                {
                    Cursor.SetCursor(Viewport, CursorType.Default);
                    Viewport.ReleaseInputLock(this);
                }
            }
            _dragSpace = dragSpace;
            _dragMiddle = dragMiddle;
        }

        public void KeyUp(ViewportEvent e)
        {
            if (e.KeyValue == Key.Space && !View.Camera2DPanRequiresMouseClick)
            {
                SetDragState(false, _dragMiddle);
            }
        }

        public void KeyDown(ViewportEvent e)
        {
            if (e.KeyValue == Key.Space && !View.Camera2DPanRequiresMouseClick)
            {
                SetDragState(true, _dragMiddle);
                e.Handled = true;
            }

            var cdoc = DocumentManager.CurrentDocument as Document;
            var moveAllowed = cdoc != null &&
                              (cdoc.Selection.IsEmpty() || !Select.ArrowKeysNudgeSelection);
            if (moveAllowed)
            {
                var shift = new Coordinate(0, 0, 0);

                switch (e.KeyValue)
                {
                    case Key.Left:
                        shift.X = -Viewport.Width / Viewport2D.Zoom / 4;
                        break;
                    case Key.Right:
                        shift.X = Viewport.Width / Viewport2D.Zoom / 4;
                        break;
                    case Key.Up:
                        shift.Y = Viewport.Height / Viewport2D.Zoom / 4;
                        break;
                    case Key.Down:
                        shift.Y = -Viewport.Height / Viewport2D.Zoom / 4;
                        break;
                }

                Viewport2D.Position += shift;
            }

            var str = e.KeyValue.ToString();
            if (str.StartsWith("Keypad") || str.StartsWith("Number"))
            {
                var last = str.Last();
                if (Char.IsDigit(last))
                {
                    var press = (int)Char.GetNumericValue(last);
                    if (press >= 0 && press <= 9)
                    {
                        if (press == 0) press = 10;
                        var num = Math.Max(press - 6, 6 - press);
                        var pow = (decimal)Math.Pow(2, num);
                        var zoom = press < 6 ? 1 / pow : pow;
                        Viewport2D.Zoom = zoom;
                        Mediator.Publish(EditorMediator.ViewZoomChanged, Viewport2D.Zoom);
                    }
                }
            }
        }

        public void MouseWheel(ViewportEvent e)
        {
            var before = Viewport2D.ScreenToWorld(e.X, Viewport2D.Height - e.Y);
            Viewport2D.Zoom *= DMath.Pow(View.ScrollWheelZoomMultiplier, (e.Delta < 0 ? -1 : 1));
            var after = Viewport2D.ScreenToWorld(e.X, Viewport2D.Height - e.Y);
            Viewport2D.Position -= (after - before);

            Mediator.Publish(EditorMediator.ViewZoomChanged, Viewport2D.Zoom);
            if (Input.Ctrl)
            {
                Mediator.Publish(EditorMediator.SetZoomValue, Viewport2D.Zoom);
            }
        }

        public void MouseUp(ViewportEvent e)
        {
            if (e.Button == MouseButton.Middle)
            {
                SetDragState(_dragSpace, false);
            }
            if (e.Button == MouseButton.Left && View.Camera2DPanRequiresMouseClick)
            {
                SetDragState(false, _dragMiddle);
            }
            if (e.Button == MouseButton.Left) _leftMouseDown = false;
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
            var mid = e.Button == MouseButton.Middle;
            var space = _dragSpace;
            if (e.Button == MouseButton.Left && View.Camera2DPanRequiresMouseClick)
            {
                var state = Keyboard.GetState();
                space = state.IsKeyDown(Key.Space);
            }
            SetDragState(space, mid);

            if (e.Button == MouseButton.Left) _leftMouseDown = true;
        }

        public void MouseMove(ViewportEvent e)
        {
            if (_dragMiddle || _dragSpace)
            {
                Viewport2D.Position -= new Coordinate(e.DeltaX, -e.DeltaY, 0) / Viewport2D.Zoom;
                e.Handled = true;
            }

            var pt = Viewport2D.Expand(Viewport2D.ScreenToWorld(new Coordinate(e.X, Viewport2D.Height - e.Y, 0)));
            Mediator.Publish(EditorMediator.MouseCoordinatesChanged, pt);

            _mousePosition = new Coordinate(e.X, e.Y, 0);
        }

        public void DragMove(ViewportEvent e)
        {

        }

        public void DragEnd(ViewportEvent e)
        {
            if (e.Button == MouseButton.Middle)
            {
                SetDragState(_dragSpace, false);
            }
        }

        public void MouseEnter(ViewportEvent e)
        {
            Mediator.Publish(EditorMediator.ViewFocused);
            Mediator.Publish(EditorMediator.ViewZoomChanged, Viewport2D.Zoom);
        }

        public void MouseLeave(ViewportEvent e)
        {
            SetDragState(false, false);
            Mediator.Publish(EditorMediator.ViewUnfocused);
        }

        public void ZoomChanged(ViewportEvent e)
        {
            var doc = Documents.DocumentManager.CurrentDocument as Document;
            if (doc != null)
            {
                doc.Renderer.UpdateGrid(doc.Map.GridSpacing, doc.Map.Show2DGrid, doc.Map.Show3DGrid, false);
            }
        }

        public void PositionChanged(ViewportEvent e)
        {

        }

        private const decimal ScrollStart = 1;
        private const decimal ScrollIncrement = 0.025m;
        private const int ScrollMaximum = 200;
        private const int ScrollPadding = 40;
        private bool _leftMouseDown;
        private Coordinate _mousePosition;

        public void UpdateFrame(Frame frame)
        {
            if (Viewport2D.Focused && _leftMouseDown && !_dragMiddle && !_dragSpace)
            {
                var pos = Viewport2D.Position.Clone();
                var pt = _mousePosition;
                if (pt.X < ScrollPadding)
                {
                    var mx = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, ScrollPadding - pt.X);
                    mx = mx * mx + ScrollStart;
                    pos.X -= mx / Viewport2D.Zoom;
                }
                else if (pt.X > Viewport2D.Width - ScrollPadding)
                {
                    var mx = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, pt.X - (Viewport2D.Width - ScrollPadding));
                    mx = mx * mx + ScrollStart;
                    pos.X += mx / Viewport2D.Zoom;
                }
                if (pt.Y < ScrollPadding)
                {
                    var my = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, ScrollPadding - pt.Y);
                    my = my * my + ScrollStart;
                    pos.Y += my / Viewport2D.Zoom;
                }
                else if (pt.Y > Viewport2D.Height - ScrollPadding)
                {
                    var my = ScrollStart + ScrollIncrement * Math.Min(ScrollMaximum, pt.Y - (Viewport2D.Height - ScrollPadding));
                    my = my * my + ScrollStart;
                    pos.Y -= my / Viewport2D.Zoom;
                }
                if (Viewport2D.Position != pos)
                {
                    Viewport2D.Position = pos;
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
            
        }
    }
}