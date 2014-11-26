using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sledge.Common.Easings;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.Tools;
using Sledge.Graphics;
using Sledge.Graphics.Helpers;
using Sledge.Gui.Components;
using Sledge.Gui.Structures;
using Sledge.Settings;

namespace Sledge.EditorNew.UI.Viewports
{
    public class Camera3DViewportListener : IViewportEventListener, IMediatorListener
    {
        public IMapViewport Viewport { get; set; }

        private bool Focus { get; set; }
        private Camera Camera { get; set; }

        private bool _spaceDown;
        private bool _leftMouseDown;
        private bool _freeLookToggled;
        private bool _freeLooking;
        private Coordinate _centerPoint;

        private long _downMillis;
        private long _lastMillis;
        private readonly Easing _easing;

        public Camera3DViewportListener(IViewport3D vp)
        {
            Focus = false;
            Viewport = vp;
            Camera = vp.Camera;
            _downKeys = new List<Key>();
            _downMillis = _lastMillis = 0;
            _easing = Easing.FromType(EasingType.Sinusoidal, EasingDirection.Out);
            Mediator.Subscribe(EditorMediator.ToolSelected, this);
        }

        private void ToolSelected(HotkeyTool tool)
        {
            SetState(false, false, false);
        }

        private void SetState(bool spaceDown, bool leftMouseDown, bool freeLookToggled)
        {
            var fLooking = (spaceDown && !View.Camera3DPanRequiresMouseClick) ||
                            (spaceDown && leftMouseDown) ||
                            _freeLookToggled;
            if (fLooking && !_freeLooking)
            {
                Cursor.HideCursor(Viewport);
                Viewport.AquireInputLock(this);
            }
            else if (!fLooking && _freeLooking)
            {
                Cursor.ShowCursor(Viewport);
                Viewport.ReleaseInputLock(this);
            }
            _freeLooking = fLooking;

            _spaceDown = spaceDown;
            _leftMouseDown = leftMouseDown;
            _freeLookToggled = freeLookToggled;
            _centerPoint = null;
        }

        private readonly List<Key> _downKeys;

        public void UpdateFrame(Frame frame)
        {
            var currMillis = _lastMillis;
            _lastMillis = frame.Milliseconds;

            if (currMillis == 0) return;
            if (!Focus || !Viewport.IsUnlocked(this) || !Viewport.Focused)
            {
                SetState(false, false, false);
                return;
            }

            var seconds = (frame.Milliseconds - currMillis) / 1000m;
            var units = View.ForwardSpeed * seconds;

            var down = Input.IsAnyKeyDown(Key.W, Key.A, Key.S, Key.D);
            if (!down) _downMillis = 0;
            else if (_downMillis == 0) _downMillis = currMillis;

            if (View.TimeToTopSpeed > 0)
            {
                var downFor = (frame.Milliseconds - _downMillis) / View.TimeToTopSpeed;
                if (downFor >= 0 && downFor < 1) units *= _easing.Evaluate(downFor);
            }

            var move = units;
            var tilt = 2m;

            var state = Keyboard.GetState();
            var mstate = Mouse.GetCursorState();

            var cpoint = _centerPoint;
            SetState(state.IsKeyDown(Key.Space), mstate.IsButtonDown(MouseButton.Left), _freeLookToggled);
            _centerPoint = cpoint;

            if (_freeLooking && _centerPoint != null)
            {
                var diff = _centerPoint - new Coordinate(mstate.X, mstate.Y, 0);

                if (diff.X != 0 || diff.Y != 0)
                {
                    MouseMoved((int) diff.X, (int) -diff.Y);
                }
            }

            // Arrow keys are not really used for hotkeys all that much, so we allow shift+arrows to match Hammer's keys
            var shiftDown = state.IsKeyDown(Key.ShiftLeft) || state.IsKeyDown(Key.ShiftRight);
            var otherDown = state.IsKeyDown(Key.ControlLeft) || state.IsKeyDown(Key.ControlRight) ||
                            state.IsKeyDown(Key.AltLeft) || state.IsKeyDown(Key.AltRight);

            // These keys are used for hotkeys, don't want the 3D view to move about when trying to use hotkeys.
            var ignore = shiftDown || otherDown;

            IfKey(state, Key.W, () => Camera.Advance(move), ignore);
            IfKey(state, Key.S, () => Camera.Advance(-move), ignore);
            IfKey(state, Key.A, () => Camera.Strafe(-move), ignore);
            IfKey(state, Key.D, () => Camera.Strafe(move), ignore);
            IfKey(state, Key.Q, () => Camera.AscendAbsolute(move), ignore);
            IfKey(state, Key.E, () => Camera.AscendAbsolute(-move), ignore);
            
            IfKey(state, Key.Right, () => { if (shiftDown) Camera.Strafe(move); else Camera.Pan(-tilt); }, otherDown);
            IfKey(state, Key.Left, () => { if (shiftDown) Camera.Strafe(-move); else Camera.Pan(tilt); }, otherDown);
            IfKey(state, Key.Up, () => { if (shiftDown) Camera.Ascend(move); else Camera.Tilt(-tilt); }, otherDown);
            IfKey(state, Key.Down, () => { if (shiftDown) Camera.Ascend(-move); else Camera.Tilt(tilt); }, otherDown);
        }

        private void IfKey(KeyboardState state, Key key, Action action, bool ignoreKeyboard)
        {
            if (!state.IsKeyDown(key))
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

        private void MouseMoved(int dx, int dy)
        {

            var left = Input.IsButtonDown(MouseButton.Left);
            var right = Input.IsButtonDown(MouseButton.Right);
            var updown = !left && right;
            var forwardback = left && right;

            if (View.InvertX) dx = -dx;
            if (View.InvertY) dy = -dy;

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

            Mouse.SetPosition((int) _centerPoint.X, (int) _centerPoint.Y);
        }

        public void Render2D()
        {
            if (!Focus || !_freeLooking) return;

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

        public void KeyDown(ViewportEvent e)
        {
            if (!Focus || !Viewport.IsUnlocked(this)) return;
            if (e.KeyValue == Key.Z && !e.Alt && !e.Control && !e.Shift)
            {
                SetState(_spaceDown, _leftMouseDown, !_freeLookToggled);
                e.Handled = true;
            }
            if (e.KeyValue == Key.Space)
            {
                SetState(true, _leftMouseDown, _freeLookToggled);
                e.Handled = true;
            }
        }

        public void MouseDown(ViewportEvent e)
        {
            if (e.Button == MouseButton.Left)
            {
                SetState(_spaceDown, true, _freeLookToggled);
            }
        }

        public void MouseMove(ViewportEvent e)
        {
            if (_centerPoint == null)
            {
                var state = Mouse.GetCursorState();
                var pos = new Coordinate(state.X, state.Y, 0);
                var relPos = new Coordinate(e.X, e.Y, 0);
                var zeroPoint = pos - relPos;
                _centerPoint = zeroPoint + new Coordinate(Viewport.Width / 2m, Viewport.Height / 2m, 0);
            }
        }

        public void MouseWheel(ViewportEvent e)
        {
            if (!Viewport.IsUnlocked(this) || e.Delta == 0) return;
            if (!Focus || (ToolManager.ActiveTool != null && ToolManager.ActiveTool.IsCapturingMouseWheel())) return;
            Camera.Advance((e.Delta / Math.Abs(e.Delta)) * View.MouseWheelMoveDistance);
        }

        public void MouseEnter(ViewportEvent e)
        {
            Focus = true;
            SetState(false, false, false);
        }

        public void MouseLeave(ViewportEvent e)
        {
            SetState(false, false, false);
        }

        public void MouseUp(ViewportEvent e)
        {

        }

        public void MouseClick(ViewportEvent e)
        {

        }

        public void MouseDoubleClick(ViewportEvent e)
        {

        }

        public void KeyUp(ViewportEvent e)
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

        public void PreRender()
        {
            //
        }

        public void Render3D()
        {
            //
        }

        public void PostRender()
        {
            // Not used
        }

        public void PositionChanged(ViewportEvent e)
        {

        }

        public void ZoomChanged(ViewportEvent e)
        {

        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }
    }
}