using System;
using System.Collections.Generic;
using OpenTK.Input;
using Sledge.Common.Easings;
using Sledge.Common.Mediator;
using Sledge.Editor;
using Sledge.Graphics;
using Sledge.Settings;
using Sledge.UI;
using OpenTK.Graphics.OpenGL;
using Sledge.Editor.Tools;
using Sledge.Graphics.Helpers;
using Color = System.Drawing.Color;
using Keyboard = OpenTK.Input.Keyboard;
using Rectangle = System.Drawing.Rectangle;

namespace Sledge.ModelViewer
{
    public class Camera3DViewportListener : IViewportEventListener, IMediatorListener
    {
        public ViewportBase Viewport { get; set; }

        private int LastKnownX { get; set; }
        private int LastKnownY { get; set; }
        private bool PositionKnown { get; set; }
        private bool FreeLook { get; set; }
        private bool FreeLookToggle { get; set; }
        private bool CursorVisible { get; set; }
        private Rectangle CursorClip { get; set; }
        private bool Focus { get; set; }
        private Camera Camera { get; set; }
        private long _downMillis;
        private long _lastMillis;
        private Easing _easing;

        public Camera3DViewportListener(Viewport vp)
        {
            LastKnownX = 0;
            LastKnownY = 0;
            PositionKnown = false;
            FreeLook = false;
            FreeLookToggle = false;
            CursorVisible = true;
            Focus = false;
            Viewport = vp;
            Camera = vp.Camera;
            _downKeys = new List<OpenTK.Input.Key>();
            _downMillis = _lastMillis = 0;
            _easing = Easing.FromType(EasingType.Sinusoidal, EasingDirection.Out);
            Mediator.Subscribe(EditorMediator.ToolSelected, this);
        }

        private void ToolSelected(HotkeyTool tool)
        {
            if (FreeLook || FreeLookToggle)
            {
                FreeLook = FreeLookToggle = false;
                SetFreeLook();
            }
        }

        private readonly List<OpenTK.Input.Key> _downKeys;

        public void UpdateFrame(FrameInfo frame)
        {
            var currMillis = _lastMillis;
            _lastMillis = frame.Milliseconds;

            if (currMillis == 0) return;
            if (!Focus || !Viewport.IsUnlocked(this) || !Viewport.IsFocused)
            {
                if (FreeLook || FreeLookToggle)
                {
                    FreeLook = FreeLookToggle = false;
                    SetFreeLook();
                }
                return;
            }

            var seconds = (frame.Milliseconds - currMillis) / 1000m;
            var units = Sledge.Settings.View.ForwardSpeed * seconds;

            var kbState = Keyboard.GetState();
            var down = kbState.IsKeyDown(OpenTK.Input.Key.W)
                       || kbState.IsKeyDown(OpenTK.Input.Key.W)
                       || kbState.IsKeyDown(OpenTK.Input.Key.W)
                       || kbState.IsKeyDown(OpenTK.Input.Key.W);

            if (!down) _downMillis = 0;
            else if (_downMillis == 0) _downMillis = currMillis;

            if (Sledge.Settings.View.TimeToTopSpeed > 0)
            {
                var downFor = (frame.Milliseconds - _downMillis) / Sledge.Settings.View.TimeToTopSpeed;
                if (downFor >= 0 && downFor < 1) units *= _easing.Evaluate(downFor);
            }

            var move = units;
            var tilt = 2m;

            // These keys are used for hotkeys, don't want the 3D view to move about when trying to use hotkeys.
            var ignore = kbState.IsKeyDown(OpenTK.Input.Key.ShiftLeft)
                         || kbState.IsKeyDown(OpenTK.Input.Key.ControlLeft)
                         || kbState.IsKeyDown(OpenTK.Input.Key.AltLeft);
            IfKey(kbState, OpenTK.Input.Key.W, () => Camera.Advance(move), ignore);
            IfKey(kbState, OpenTK.Input.Key.S, () => Camera.Advance(-move), ignore);
            IfKey(kbState, OpenTK.Input.Key.A, () => Camera.Strafe(-move), ignore);
            IfKey(kbState, OpenTK.Input.Key.D, () => Camera.Strafe(move), ignore);
            IfKey(kbState, OpenTK.Input.Key.Q, () => Camera.AscendAbsolute(move), ignore);
            IfKey(kbState, OpenTK.Input.Key.E, () => Camera.AscendAbsolute(-move), ignore);

            // Arrow keys are not really used for hotkeys all that much, so we allow shift+arrows to match Hammer's keys
            var shiftDown = kbState.IsKeyDown(OpenTK.Input.Key.ShiftLeft);
            var otherDown = kbState.IsKeyDown(OpenTK.Input.Key.ControlLeft) || kbState.IsKeyDown(OpenTK.Input.Key.AltLeft);

            IfKey(kbState, OpenTK.Input.Key.Right, () => { if (shiftDown) Camera.Strafe(move); else Camera.Pan(-tilt); }, otherDown);
            IfKey(kbState, OpenTK.Input.Key.Left, () => { if (shiftDown) Camera.Strafe(-move); else Camera.Pan(tilt); }, otherDown);
            IfKey(kbState, OpenTK.Input.Key.Up, () => { if (shiftDown) Camera.Ascend(move); else Camera.Tilt(-tilt); }, otherDown);
            IfKey(kbState, OpenTK.Input.Key.Down, () => { if (shiftDown) Camera.Ascend(-move); else Camera.Tilt(tilt); }, otherDown);
        }

        private void IfKey(OpenTK.Input.KeyboardState kbState, OpenTK.Input.Key key, Action action, bool ignoreKeyboard)
        {
            if (!kbState.IsKeyDown(key))
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

        public void PreRender()
        {
            //
        }

        public void Render3D()
        {
            //
        }

        public void Render2D()
        {
            if (!Focus || !FreeLook) return;

            TextureHelper.Unbind();
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.White);
            GL.Vertex2(0, -5);
            GL.Vertex2(0, 5);
            GL.Vertex2(1, -5);
            GL.Vertex2(1, 5);
            GL.Vertex2(-5, 0);
            GL.Vertex2(5, 0);
            GL.Vertex2(-5, 1);
            GL.Vertex2(5, 1);
            GL.End();
        }

        public void PostRender()
        {
            // Not used
        }

        public void KeyUp(ViewportEvent e)
        {
            SetFreeLook();
        }

        public void KeyDown(ViewportEvent e)
        {
            if (!Focus || !Viewport.IsUnlocked(this)) return;
            if (e.KeyValue == Key.Z && !e.Alt && !e.Control && !e.Shift)
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
                var mState = OpenTK.Input.Mouse.GetState();
                var kbState = OpenTK.Input.Keyboard.GetState();
                var left = mState.IsButtonDown(MouseButton.Left);
                var right = mState.IsButtonDown(MouseButton.Right);
                
                if (ToolManager.ActiveTool is CameraTool)
                {
                    FreeLook = left || right;
                }
                else
                {
                    var space = kbState.IsKeyDown(OpenTK.Input.Key.Space);
                    var req = Sledge.Settings.View.Camera3DPanRequiresMouseClick;
                    FreeLook = space && (!req || left || right);
                }
            }

            if (FreeLook && CursorVisible)
            {
                //var pix = new Pixmap(null, 1, 1, 1);
                //var col = new Gdk.Color();
                //var cur = new Cursor(pix, pix, col, col, 0, 0);
                //var status = Gdk.Pointer.Grab(Viewport.GdkWindow, false, EventMask.AllEventsMask, Viewport.GdkWindow, null, 0);
                Gtk.Grab.Add(Viewport);

                //CursorClip = Cursor.Clip;
                //Cursor.Clip = Viewport.RectangleToScreen(new Rectangle(0, 0, Viewport.Width, Viewport.Height));
                //Viewport.Capture = true;
                CursorVisible = false;
                //Cursor.Hide();
                Viewport.AquireInputLock(this);
            }
            else if (!FreeLook && !CursorVisible)
            {
                //Gdk.Pointer.Ungrab(0);
                Gtk.Grab.Remove(Viewport);

                //Cursor.Clip = CursorClip;
                //CursorClip = Rectangle.Empty;
                //Viewport.Capture = false;
                CursorVisible = true;
                //Cursor.Show();
                Viewport.ReleaseInputLock(this);
            }
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

            var mState = OpenTK.Input.Mouse.GetState();

            var left = mState.IsButtonDown(MouseButton.Left);
            var right = mState.IsButtonDown(MouseButton.Right);
            var updown = !left && right;
            var forwardback = left && right;

            if (Sledge.Settings.View.InvertX) dx = -dx;
            if (Sledge.Settings.View.InvertY) dy = -dy;

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
                // Camera
                var fovdiv = (Viewport.Width / 60m) / 2.5m;
                Camera.Pan(dx / fovdiv);
                Camera.Tilt(dy / fovdiv);
            }

            int x, y;
            Viewport.GdkWindow.GetOrigin(out x, out y);

            LastKnownX = Viewport.Width / 2;
            LastKnownY = Viewport.Height / 2;
            Mouse.SetPosition(x + LastKnownX - Viewport.RootWindow.FrameExtents.X, y + LastKnownY - Viewport.RootWindow.FrameExtents.Y);
        }

        public void MouseWheel(ViewportEvent e)
        {
            if (!Viewport.IsUnlocked(this) || e.Delta == 0) return;
            if (!Focus || (ToolManager.ActiveTool != null && ToolManager.ActiveTool.IsCapturingMouseWheel())) return;
            Camera.Advance((e.Delta / Math.Abs(e.Delta)) * Sledge.Settings.View.MouseWheelMoveDistance);
        }

        public void MouseUp(ViewportEvent e)
        {
            SetFreeLook();
        }

        public void MouseDown(ViewportEvent e)
        {
            SetFreeLook();
        }

        public void MouseClick(ViewportEvent e)
        {
            
        }

        public void MouseDoubleClick(ViewportEvent e)
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
                int x, y;
                Viewport.GdkWindow.GetOrigin(out x, out y);
                Viewport.Display.WarpPointer(Viewport.Screen, x + LastKnownX, y + LastKnownY);
            }
            else
            {
                if (!CursorVisible)
                {
                    //Gdk.Pointer.Ungrab(0);
                    Gtk.Grab.Remove(Viewport);

                    //Cursor.Clip = CursorClip;
                    //CursorClip = Rectangle.Empty;
                    //Viewport.Capture = false;
                    CursorVisible = true;
                    //Cursor.Show();
                    Viewport.ReleaseInputLock(this);
                }
                PositionKnown = false;
                Focus = false;
            }
        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }
    }
}
