using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Rendering.Viewport
{
    public class MapViewport
    {
        public List<IViewportEventListener> Listeners { get; private set; }
        public IViewport Viewport { get; private set; }

        public Control Control => Viewport.Control;
        public int Height => Control.Height;
        public int Width => Control.Width;

        public bool Is2D => Viewport.Camera.Type == CameraType.Orthographic;
        public bool Is3D => Viewport.Camera.Type == CameraType.Perspective;

        #region Input Locking

        private object _inputLock;

        public bool IsUnlocked(object context)
        {
            return _inputLock == null || _inputLock == context;
        }

        public bool AquireInputLock(object context)
        {
            if (_inputLock == null) _inputLock = context;
            return _inputLock == context;
        }

        public bool ReleaseInputLock(object context)
        {
            if (_inputLock == context) _inputLock = null;
            return _inputLock == null;
        }

        #endregion
        
        public MapViewport(IViewport viewport)
        {
            Viewport = viewport;
            Listeners = new List<IViewportEventListener>();

            viewport.Control.MouseWheel += OnMouseWheel;
            viewport.Control.MouseEnter += OnMouseEnter;
            viewport.Control.MouseLeave += OnMouseLeave;
            viewport.Control.MouseMove += OnMouseMove;
            viewport.Control.MouseUp += OnMouseUp;
            viewport.Control.MouseDown += OnMouseDown;
            viewport.Control.MouseDoubleClick += OnMouseDoubleClick;
            viewport.Control.KeyDown += OnKeyDown;
            viewport.Control.KeyUp += OnKeyUp;
            viewport.OnUpdate += OnUpdate;
        }

        #region Listeners

        public delegate void ListenerExceptionEventHandler(object sender, Exception exception);
        public event ListenerExceptionEventHandler ListenerException;

        private void OnListenerException(Exception ex)
        {
            if (ListenerException != null)
            {
                var st = new StackTrace();
                var frames = st.GetFrames() ?? new StackFrame[0];
                var msg = "Listener exception: " + ex.Message;
                foreach (var frame in frames)
                {
                    var method = frame.GetMethod();
                    msg += "\r\n    " + method.ReflectedType.FullName + "." + method.Name;
                }
                ListenerException(this, new Exception(msg, ex));
            }
        }

        private void ListenerDo(Action<IViewportEventListener> action)
        {
            foreach (var listener in Listeners.Where(x => x.IsActive()).OrderBy(x => x.OrderHint))
            {
                try
                {
                    action(listener);
                }
                catch (Exception ex)
                {
                    OnListenerException(ex);
                }
            }
        }

        private void ListenerDoEvent(ViewportEvent e, Action<IViewportEventListener, ViewportEvent> action)
        {
            foreach (var listener in Listeners.Where(x => x.IsActive()).OrderBy(x => x.OrderHint))
            {
                try
                {
                    action(listener, e);
                }
                catch (Exception ex)
                {
                    OnListenerException(ex);
                }
                if (e.Handled)
                {
                    break;
                }
            }
        }


        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseWheel(v));
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            Viewport.Control.Focus();

            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseEnter(v));
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseLeave(v));
            _lastMouseLocationKnown = false;
            _lastMouseLocation = new Point(-1, -1);
        }

        private bool _dragging = false;
        private MouseButtons _dragButton;
        private bool _lastMouseLocationKnown = false;
        private Point _lastMouseLocation = new Point(-1, -1);
        private Point _mouseDownLocation = new Point(-1, -1);

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_lastMouseLocationKnown)
            {
                _lastMouseLocation = new Point(e.X, e.Y);
            }
            var ve = new ViewportEvent(this, e)
            {
                Dragging = _dragging,
                StartX = _mouseDownLocation.X,
                StartY = _mouseDownLocation.Y,
                LastX = _lastMouseLocation.X,
                LastY = _lastMouseLocation.Y,
            };
            if (!_dragging
                && (Math.Abs(_mouseDownLocation.X - e.Location.X) > 1
                    || Math.Abs(_mouseDownLocation.Y - e.Location.Y) > 1)
                && _mouseDownLocation.X >= 0 && _mouseDownLocation.Y >= 0)
            {
                _dragging = ve.Dragging = true;
                ve.Button = _dragButton;
                ListenerDoEvent(ve, (l, v) => l.DragStart(v));
            }
            ListenerDoEvent(ve, (l, v) => l.MouseMove(v));
            if (_dragging)
            {
                ve.Button = _dragButton;
                ListenerDoEvent(ve, (l, v) => l.DragMove(v));
            }
            _lastMouseLocationKnown = true;
            _lastMouseLocation = new Point(e.X, e.Y);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (!_lastMouseLocationKnown)
            {
                _lastMouseLocation = new Point(e.X, e.Y);
            }
            var ve = new ViewportEvent(this, e)
            {
                Dragging = _dragging,
                StartX = _mouseDownLocation.X,
                StartY = _mouseDownLocation.Y,
                LastX = _lastMouseLocation.X,
                LastY = _lastMouseLocation.Y,
            };
            if (_dragging && ve.Button == _dragButton)
            {
                ListenerDoEvent(ve, (l, v) => l.DragEnd(v));
            }
            ListenerDoEvent(ve, (l, v) => l.MouseUp(v));
            if (!_dragging
                && Math.Abs(_mouseDownLocation.X - e.Location.X) <= 1
                && Math.Abs(_mouseDownLocation.Y - e.Location.Y) <= 1)
            {
                // Mouse hasn't moved very much, trigger the click event
                ListenerDoEvent(ve, (l, v) => l.MouseClick(v));
            }
            if (_dragging && ve.Button == _dragButton)
            {
                _dragging = false;
            }
            if (!_dragging)
            {
                _mouseDownLocation = new Point(-1, -1);
            }
            _lastMouseLocationKnown = true;
            _lastMouseLocation = new Point(e.X, e.Y);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (!_lastMouseLocationKnown)
            {
                _lastMouseLocation = new Point(e.X, e.Y);
            }
            if (!_dragging)
            {
                _mouseDownLocation = new Point(e.X, e.Y);
                _dragging = false;
                _dragButton = e.Button;
            }
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseDown(v));
            _lastMouseLocationKnown = true;
            _lastMouseLocation = new Point(e.X, e.Y);
        }

        private void OnMouseDoubleClick(object sender, EventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseDoubleClick(v));
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.KeyDown(v));
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.KeyUp(v));
        }

        private void OnUpdate(object sender, long frame)
        {
            ListenerDo(x => x.UpdateFrame(frame));
        }

        #endregion
    }
}