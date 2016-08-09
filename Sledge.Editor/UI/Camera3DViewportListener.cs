using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using Sledge.Common.Easings;
using Sledge.Common.Mediator;
using Sledge.Editor.Documents;
using Sledge.Editor.Rendering;
using Sledge.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Settings;
using Sledge.Editor.Tools;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.UI
{
    public class Camera3DViewportListener : IViewportEventListener, IMediatorListener
    {
        public MapViewport Viewport { get; set; }

        private int LastKnownX { get; set; }
        private int LastKnownY { get; set; }
        private bool PositionKnown { get; set; }
        private bool FreeLook { get; set; }
        private bool FreeLookToggle { get; set; }
        private bool CursorVisible { get; set; }
        private Rectangle CursorClip { get; set; }
        private bool Focus { get; set; }
        private PerspectiveCamera Camera { get { return Viewport.Viewport.Camera as PerspectiveCamera; }}
        private long _downMillis;
        private long _lastMillis;
        private Easing _easing;

        public Camera3DViewportListener(MapViewport vp)
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

        private readonly List<Keys> _downKeys;

        public void UpdateFrame(Frame frame)
        {
            var currMillis = _lastMillis;
            _lastMillis = frame.Milliseconds;

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

            var seconds = (frame.Milliseconds - currMillis) / 1000f;
            var units = Sledge.Settings.View.ForwardSpeed * seconds;

            var down = KeyboardState.IsAnyKeyDown(Keys.W, Keys.A, Keys.S, Keys.D);
            if (!down) _downMillis = 0;
            else if (_downMillis == 0) _downMillis = currMillis;

            if (Sledge.Settings.View.TimeToTopSpeed > 0)
            {
                var downFor = (frame.Milliseconds - _downMillis) / Sledge.Settings.View.TimeToTopSpeed;
                if (downFor >= 0 && downFor < 1) units *= (float) _easing.Evaluate(downFor);
            }

            var move = units;
            var tilt = 2f;

            // These keys are used for hotkeys, don't want the 3D view to move about when trying to use hotkeys.
            var ignore = KeyboardState.IsAnyKeyDown(Keys.ShiftKey, Keys.ControlKey, Keys.Alt);
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
            return Viewport != null && Viewport.Is3D;
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
                
                if (ToolManager.ActiveTool is CameraTool)
                {
                    FreeLook = left || right;
                }
                else
                {
                    var space = KeyboardState.IsKeyDown(Keys.Space);
                    var req = Sledge.Settings.View.Camera3DPanRequiresMouseClick;
                    FreeLook = space && (!req || left || right);
                }
            }

            if (FreeLook && CursorVisible)
            {
                CursorClip = Cursor.Clip;
                Cursor.Clip = Viewport.Control.RectangleToScreen(new Rectangle(0, 0, Viewport.Width, Viewport.Height));
                Viewport.Control.Capture = true;
                CursorVisible = false;
                Cursor.Hide();
                Viewport.AquireInputLock(this);
                AddSceneObjects();
            }
            else if (!FreeLook && !CursorVisible)
            {
                Cursor.Clip = CursorClip;
                CursorClip = Rectangle.Empty;
                Viewport.Control.Capture = false;
                CursorVisible = true;
                Cursor.Show();
                Viewport.ReleaseInputLock(this);
                ClearSceneObjects();
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

            var left = Control.MouseButtons.HasFlag(MouseButtons.Left);
            var right = Control.MouseButtons.HasFlag(MouseButtons.Right);
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
                var fovdiv = (Viewport.Width / 60f) / 2.5f;
                Camera.Pan(dx / fovdiv);
                Camera.Tilt(dy / fovdiv);
            }

            LastKnownX = Viewport.Width/2;
            LastKnownY = Viewport.Height/2;
            Cursor.Position = Viewport.Control.PointToScreen(new Point(LastKnownX, LastKnownY));
        }

        public void MouseWheel(ViewportEvent e)
        {
            if (!Viewport.IsUnlocked(this) || e.Delta == 0) return;
            if (!Focus || (ToolManager.ActiveTool != null && ToolManager.ActiveTool.IsCapturingMouseWheel())) return;
            Camera.Advance((e.Delta / Math.Abs(e.Delta)) * (float) Sledge.Settings.View.MouseWheelMoveDistance);
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
                    Viewport.Control.Capture = false;
                    CursorVisible = true;
                    Cursor.Show();
                    Viewport.ReleaseInputLock(this);
                    ClearSceneObjects();
                }
                PositionKnown = false;
                Focus = false;
            }
        }

        public void ZoomChanged(ViewportEvent e)
        {

        }

        public void PositionChanged(ViewportEvent e)
        {

        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }

        private void ClearSceneObjects()
        {
            if (DocumentManager.CurrentDocument == null) return;
            DocumentManager.CurrentDocument.SceneManager.ClearTemporaryObjects(this);
        }

        private void AddSceneObjects()
        {
            if (DocumentManager.CurrentDocument == null) return;

            var line1 = new LineElement(PositionType.Screen, Color.White, new List<Position>
            {
                new Position(new Vector3(0.5f, 0.5f, 0)) { Normalised = true, Offset = new Vector3(-5, 0, 0) },
                new Position(new Vector3(0.5f, 0.5f, 0)) { Normalised = true, Offset = new Vector3(+5, 0, 0) },
            })
            {
                Viewport = Viewport.Viewport,
                Smooth = false
            };

            var line2 = new LineElement(PositionType.Screen, Color.White, new List<Position>
            {
                new Position(new Vector3(0.5f, 0.5f, 0)) { Normalised = true, Offset = new Vector3(0, -5, 0) },
                new Position(new Vector3(0.5f, 0.5f, 0)) { Normalised = true, Offset = new Vector3(0, +5, 0) },
            })
            {
                Viewport = Viewport.Viewport,
                Smooth = false
            };

            DocumentManager.CurrentDocument.SceneManager.AddTemporaryObject(this, line1);
            DocumentManager.CurrentDocument.SceneManager.AddTemporaryObject(this, line2);
        }
    }
}
