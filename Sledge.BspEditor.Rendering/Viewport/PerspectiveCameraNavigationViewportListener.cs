using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common;
using Sledge.Common.Easings;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;
using Sledge.Shell;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Rendering.Viewport
{
    public class PerspectiveCameraNavigationViewportListener : IViewportEventListener, IOverlayRenderable
    {
        public string OrderHint => FreeLook ? "A" : "W";
        public MapViewport Viewport { get; set; }

        private int LastKnownX { get; set; }
        private int LastKnownY { get; set; }
        private bool PositionKnown { get; set; }
        private bool FreeLook { get; set; }
        private bool FreeLookToggle { get; set; }
        private bool CursorVisible { get; set; }
        private Rectangle CursorClip { get; set; }
        private bool Focus { get; set; }
        private PerspectiveCamera Camera => Viewport.Viewport.Camera as PerspectiveCamera;
        private long _downMillis;
        private long _lastMillis;
        private readonly Easing _easing;
        private readonly List<Keys> _downKeys;
        private readonly IContext _context;

        public PerspectiveCameraNavigationViewportListener(MapViewport vp)
        {
            LastKnownX = 0;
            LastKnownY = 0;
            PositionKnown = false;
            FreeLook = false;
            FreeLookToggle = false;
            CursorVisible = true;
            Focus = false;
            Viewport = vp;
            _downKeys = new List<Keys>();
            _downMillis = _lastMillis = 0;
            _easing = Easing.FromType(EasingType.Sinusoidal, EasingDirection.Out);

            _context = Container.Get<IContext>();
            Oy.Subscribe<ITool>("Tool:Activated", ToolSelected);
        }

        private Task ToolSelected(ITool tool)
        {
            if (FreeLook || FreeLookToggle)
            {
                FreeLook = FreeLookToggle = false;
                SetFreeLook();
            }
            return Task.CompletedTask;
        }

        public void UpdateFrame(long frame)
        {
            var currMillis = _lastMillis;
            _lastMillis = frame;

            if (currMillis == 0) return;
            if (!Focus || !Viewport.IsUnlocked(this) || !Viewport.Viewport.IsFocused)
            {
                if (FreeLook || FreeLookToggle)
                {
                    FreeLook = FreeLookToggle = false;
                    SetFreeLook();
                }
                return;
            }

            var seconds = (frame - currMillis) / 1000f;
            var units = CameraNavigationViewportSettings.ForwardSpeed * seconds;

            var down = KeyboardState.IsAnyKeyDown(Keys.W, Keys.A, Keys.S, Keys.D);
            if (!down) _downMillis = 0;
            else if (_downMillis == 0) _downMillis = currMillis;

            if (CameraNavigationViewportSettings.TimeToTopSpeed > 0)
            {
                var downFor = (frame - _downMillis) / (CameraNavigationViewportSettings.TimeToTopSpeed * 1000);
                if (downFor >= 0 && downFor < 1) units *= (float) _easing.Evaluate((double) downFor);
            }

            if (FreeLook)
            {
                if (KeyboardState.Shift) units *= 2;
                if (KeyboardState.Ctrl) units /= 2;
            }

            if (float.IsNaN(units) || float.IsInfinity(units) || units < 0.001f) units = 0;

            var move = units;
            var tilt = 2f;

            // These keys are used for hotkeys, don't want the 3D view to move about when trying to use hotkeys.
            var ignore = !FreeLook && KeyboardState.IsAnyKeyDown(Keys.ShiftKey, Keys.ControlKey, Keys.Alt);
            IfKey(Keys.W, () => Camera.Advance(move), ignore);
            IfKey(Keys.S, () => Camera.Advance(-move), ignore);
            IfKey(Keys.A, () => Camera.Strafe(-move), ignore);
            IfKey(Keys.D, () => Camera.Strafe(move), ignore);
            IfKey(Keys.Q, () => Camera.AscendAbsolute(move), ignore);
            IfKey(Keys.E, () => Camera.AscendAbsolute(-move), ignore);

            // Arrow keys are not really used for hotkeys all that much, so we allow shift+arrows to match Hammer's keys
            var shiftDown = KeyboardState.IsKeyDown(Keys.ShiftKey);
            var otherDown = KeyboardState.IsAnyKeyDown(Keys.ControlKey, Keys.Alt);

            IfKey(Keys.Right, () => { if (shiftDown) Camera.Strafe(move); else Camera.Pan(-tilt); }, otherDown);
            IfKey(Keys.Left, () => { if (shiftDown) Camera.Strafe(-move); else Camera.Pan(tilt); }, otherDown);
            IfKey(Keys.Up, () => { if (shiftDown) Camera.Ascend(move); else Camera.Tilt(-tilt); }, otherDown);
            IfKey(Keys.Down, () => { if (shiftDown) Camera.Ascend(-move); else Camera.Tilt(tilt); }, otherDown);
        }

        private void IfKey(Keys key, Action action, bool ignoreKeyboard)
        {
            if (!KeyboardState.IsKeyDown(key))
            {
                _downKeys.Remove(key);
            }
            else if (ignoreKeyboard)
            {
                if (_downKeys.Contains(key)) action();
            }
            else
            {
                if (!_downKeys.Contains(key)) _downKeys.Add(key);
                action();
            }
        }

        public bool IsActive()
        {
            return Viewport != null && Camera != null;
        }

        public void KeyUp(ViewportEvent e)
        {
            SetFreeLook();
        }

        public void KeyDown(ViewportEvent e)
        {
            if (!Focus || !Viewport.IsUnlocked(this)) return;
            if (e.KeyCode == Keys.Z && !e.Alt && !e.Control && !e.Shift)
            {
                FreeLookToggle = !FreeLookToggle;
                SetFreeLook();
                PositionKnown = false;
            }
            else
            {
                SetFreeLook();
            }
            if (FreeLook)
            {
                e.Handled = true;
            }
        }

        private void SetFreeLook()
        {
            if (!Viewport.IsUnlocked(this)) return;
            FreeLook = false;
            
            if (FreeLookToggle)
            {
                FreeLook = true;
            }
            else
            {
                var left = Control.MouseButtons.HasFlag(MouseButtons.Left);
                var right = Control.MouseButtons.HasFlag(MouseButtons.Right);

                var activeTool = _context.Get<ITool>("ActiveTool");
                if (activeTool != null && activeTool.GetType().Name == "CameraTool")
                {
                    FreeLook = left || right;
                }
                else
                {
                    var space = KeyboardState.IsKeyDown(Keys.Space);
                    var req = CameraNavigationViewportSettings.Camera3DPanRequiresMouseClick;
                    FreeLook = space && (!req || left || right);
                }
            }

            if (FreeLook && CursorVisible)
            {
                CursorClip = Cursor.Clip;
                Cursor.Clip = Viewport.Control.RectangleToScreen(new Rectangle(0, 0, Viewport.Width, Viewport.Height));
                SetCapture(true);
                Viewport.AquireInputLock(this);
            }
            else if (!FreeLook && !CursorVisible)
            {
                Cursor.Clip = CursorClip;
                CursorClip = Rectangle.Empty;
                SetCapture(false);
                Viewport.ReleaseInputLock(this);
            }
        }

        private void SetCapture(bool capture)
        {
            Viewport.Control.InvokeSync(() =>
            {
                Viewport.Control.Capture = capture;
                if (capture && CursorVisible)
                {
                    CursorVisible = false;
                    Cursor.Hide();
                }
                else if (!capture && !CursorVisible)
                {
                    CursorVisible = true;
                    Cursor.Show();
                }
            });
        }

        public void KeyPress(ViewportEvent e)
        {
            if (FreeLook)
            {
                e.Handled = true;
            }
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
                    MouseMoved(e, dx, dy);
                    return;
                }
            }
            LastKnownX = e.X;
            LastKnownY = e.Y;
            PositionKnown = true;
        }

        private void MouseMoved(ViewportEvent e, int dx, int dy)
        {
            if (!FreeLook) return;

            var left = Control.MouseButtons.HasFlag(MouseButtons.Left);
            var right = Control.MouseButtons.HasFlag(MouseButtons.Right);
            var updown = !left && right;
            var forwardback = left && right;

            if (CameraNavigationViewportSettings.InvertX) dx = -dx;
            if (CameraNavigationViewportSettings.InvertY) dy = -dy;

            if (updown)
            {
                Camera.Strafe(-dx);
                Camera.Ascend(-dy);
            }
            else if (forwardback)
            {
                Camera.Strafe(-dx);
                Camera.Advance(-dy);
            }
            else // left mouse or z-toggle
            {
                // adjust is an arbitrary and tunable parameter to get setting
                // values that are sensible and not too high or too low.
                const float adjust = 100.0f;

                // Scale with FOV for consistency
                float fovscale = CameraNavigationViewportSettings.FOV / 60f;
                float scale = fovscale * ( (float)CameraNavigationViewportSettings.Sensitivity / adjust );
                Camera.Pan(dx * scale);
                Camera.Tilt(dy * scale);
            }

            LastKnownX = Viewport.Width/2;
            LastKnownY = Viewport.Height/2;
            Cursor.Position = Viewport.Control.PointToScreen(new Point(LastKnownX, LastKnownY));
        }

        public void MouseWheel(ViewportEvent e)
        {
            if (!Viewport.IsUnlocked(this) || e.Delta == 0) return;
            Camera.Advance((e.Delta / (float) Math.Abs(e.Delta)) * (float) CameraNavigationViewportSettings.MouseWheelMoveDistance);
        }

        public void MouseUp(ViewportEvent e)
        {
            SetFreeLook();
        }

        public void MouseDown(ViewportEvent e)
        {
            if (FreeLook && KeyboardState.IsKeyDown(Keys.Space)) e.Handled = true;
            SetFreeLook();
        }

        public void MouseClick(ViewportEvent e)
        {
            if (FreeLook) e.Handled = true;
        }

        public void MouseDoubleClick(ViewportEvent e)
        {
            if (FreeLook) e.Handled = true;
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
            Focus = true;
        }

        public void MouseLeave(ViewportEvent e)
        {
            if (FreeLook)
            {
                LastKnownX = Viewport.Width/2;
                LastKnownY = Viewport.Height/2;
                Cursor.Position = Viewport.Control.PointToScreen(new Point(LastKnownX, LastKnownY));
            }
            else
            {
                if (!CursorVisible)
                {
                    Cursor.Clip = CursorClip;
                    CursorClip = Rectangle.Empty;
                    SetCapture(false);
                    Viewport.ReleaseInputLock(this);
                }
                PositionKnown = false;
                Focus = false;
            }
        }

        public bool Filter(string hotkey, int keys)
        {
            // If we're freelooking, consume any hotkeys using WASD/QE and ctrl or shift
            if (FreeLook)
            {
                var k = (Keys) keys;
                if (
                    k.HasFlag(Keys.W) || k.HasFlag(Keys.A) ||
                    k.HasFlag(Keys.S) || k.HasFlag(Keys.D) ||
                    k.HasFlag(Keys.Q) || k.HasFlag(Keys.E)
                )
                {
                    return !k.HasFlag(Keys.Alt);
                }
            }
            return false;
        }

        public void Dispose()
        {
            // 
        }

        public void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
        {
            // 
        }

        public void Render(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
        {
            if (CursorVisible) return;
            if (Viewport.Viewport != viewport) return;

            var x = viewport.Width / 2;
            var y = viewport.Height / 2;
            const int size = 3;

            im.AddRectFilled(new Vector2(x - 1, y - size - 1), new Vector2(x + 2, y + size + 2), Color.Black);
            im.AddRectFilled(new Vector2(x - size - 1, y - 1), new Vector2(x + size + 2, y + 2), Color.Black);

            im.AddLine(new Vector2(x, y - size), new Vector2(x, y + size + 1), Color.White, 1, false);
            im.AddLine(new Vector2(x - size, y), new Vector2(x + size + 1, y), Color.White, 1, false);
        }
    }
}
