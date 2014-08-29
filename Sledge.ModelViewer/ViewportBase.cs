using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Gdk;
using Gtk;
using OpenTK;
using OpenTK.Graphics;
using Sledge.DataStructures.Geometric;
using Box = Sledge.DataStructures.Geometric.Box;
using ClearBufferMask = OpenTK.Graphics.OpenGL.ClearBufferMask;
using GL = OpenTK.Graphics.OpenGL.GL;
using Point = System.Drawing.Point;

namespace Sledge.UI
{
    public class ViewportBase : GLWidget
    {
        //public RenderContext RenderContext { get; set; }
        protected Timer UpdateTimer { get; set; }
        private Stopwatch _stopwatch;
        public List<IViewportEventListener> Listeners { get; set; }
        public bool IsFocused { get; private set; }
        private int UnfocusedUpdateCounter { get; set; }

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

        public delegate void RenderExceptionEventHandler(object sender, Exception exception);
        public event RenderExceptionEventHandler RenderException;
        protected void OnRenderException(Exception ex)
        {
            if (RenderException != null)
            {
                var st = new StackTrace();
                var frames = st.GetFrames() ?? new StackFrame[0];
                var msg = "Rendering exception: " + ex.Message;
                foreach (var frame in frames)
                {
                    var method = frame.GetMethod();
                    msg += "\r\n    " + method.ReflectedType.FullName + "." + method.Name;
                }
                RenderException(this, new Exception(msg, ex));
            }
        }

        public delegate void ListenerExceptionEventHandler(object sender, Exception exception);
        public event ListenerExceptionEventHandler ListenerException;
        protected void OnListenerException(Exception ex)
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

        private IMContext imContext;

        private class ViewportSynchronizingObject : System.ComponentModel.ISynchronizeInvoke
        {

            public IAsyncResult BeginInvoke(Delegate method, object[] args)
            {
                Application.Invoke(delegate {
                    method.DynamicInvoke(args);
                });
                return null;
            }

            public object EndInvoke(IAsyncResult result)
            {
                return null;
            }

            public object Invoke(Delegate method, object[] args)
            {
                return method.DynamicInvoke(args);
            }

            public bool InvokeRequired { get { return true; } }
        }

        public ViewportBase() : base(new GraphicsMode(GraphicsMode.Default.ColorFormat, 24))
        {
            //RenderContext = new RenderContext();
            Listeners = new List<IViewportEventListener>();
            _stopwatch = new Stopwatch();
            UpdateTimer = new Timer { Interval = 1 };
            UpdateTimer.SynchronizingObject = new ViewportSynchronizingObject();
            UpdateTimer.Elapsed += (sender, e) => UpdateFrame();

            CanFocus = true;

            imContext = new IMContextSimple();
            imContext.Commit += OnKeyInputContextCommit;
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var dis in Listeners.OfType<IDisposable>())
                {
                    try
                    {
                        dis.Dispose();
                    }
                    catch
                    {
                        // Don't care
                    }
                }
                Listeners.Clear();
            
                //RenderContext.Dispose();
                UpdateTimer.Dispose();
                _stopwatch.Stop();

                imContext.Dispose();
            }
            base.Dispose(disposing);
        }

        public void ClearViewport()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public virtual void SetViewport()
        {
            //Viewport.Switch(0, 0, Width, Height);
        }

        public virtual Matrix4 GetViewportMatrix()
        {
            return Matrix4.Identity;
        }

        public virtual Matrix4 GetCameraMatrix()
        {
            return Matrix4.Identity;
        }

        public virtual Matrix4 GetModelViewMatrix()
        {
            return Matrix4.Identity;
        }

        public virtual void FocusOn(Box box)
        {
            FocusOn(box.Center);
        }

        public virtual void FocusOn(Coordinate coordinate)
        {
            // Virtual
        }

        // Resize event = configure
        protected override bool OnConfigureEvent(EventConfigure evnt)
        {
            if (IsInitialised)
            {
                MakeCurrent();
                SetViewport();
                UnmakeCurrent();
            }
            return base.OnConfigureEvent(evnt);
        }

        protected override void OnInitialized()
        {
            AddEvents((int) (EventMask.AllEventsMask));
            base.OnInitialized();
        }

        protected override bool OnFocusInEvent(EventFocus evnt)
        {
            HasFocus = true;
            return base.OnFocusInEvent(evnt);
        }

        protected override bool OnFocusOutEvent(EventFocus evnt)
        {
            HasFocus = false;
            return base.OnFocusOutEvent(evnt);
        }

        public void Run()
        {
            _stopwatch.Start();
            UpdateTimer.Start();
        }

        public void UpdateNextFrame()
        {
            UnfocusedUpdateCounter = -1;
        }

        public void UpdateNextFrameImmediately()
        {
            UpdateNextFrame();
            UpdateFrame();
        }

        protected override void OnRenderFrame()
        {
            UpdateFrame();
            base.OnRenderFrame();
        }

        protected void UpdateFrame()
        {
            if (!IsInitialised) return;

            if (!IsFocused) // Change this if things start to get choppy
            {
                UnfocusedUpdateCounter++;
                // Update every 10th frame
                if (UnfocusedUpdateCounter % 10 != 0)
                {
                    return;
                }
            }
            UnfocusedUpdateCounter = 0;

            try
            {
                MakeCurrent();
            }
            catch (Exception ex)
            {
                OnRenderException(ex);
            }
        
            var frame = new FrameInfo(_stopwatch.ElapsedMilliseconds);
            ListenerDo(x => x.UpdateFrame(frame));
        
            LoadIdentity();
            UpdateAfterLoadIdentity();
        
            UpdateBeforeSetViewport();
            SetViewport();
        
            UpdateBeforeClearViewport();
            ClearViewport();
        
            try
            {
                UpdateBeforeRender();
                //RenderContext.Render(this);
                UpdateAfterRender();
            }
            catch(Exception ex)
            {
                OnRenderException(ex);
            }
        
            if (IsDisposed) return;

            UpdateBeforeRender();

            SwapBuffers();
            UnmakeCurrent();
        }

        public void LoadIdentity()
        {
            GL.LoadIdentity();
        }

        private void ListenerDo(Action<IViewportEventListener> action)
        {
            foreach (var listener in Listeners)
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
            foreach (var listener in Listeners)
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

        protected virtual void UpdateAfterLoadIdentity()
        {

        }

        protected virtual void UpdateBeforeSetViewport()
        {

        }

        protected virtual void UpdateBeforeClearViewport()
        {
            ListenerDo(x => x.PreRender());
        }

        protected virtual void UpdateBeforeRender()
        {

        }

        protected virtual void UpdateAfterRender()
        {
            ListenerDo(x => x.PostRender());
        }

        protected override bool OnScrollEvent(EventScroll evnt)
        {
            if (IsFocused)
            {
                ListenerDoEvent(new ViewportEvent(this, evnt.X, evnt.Y, evnt.Direction), (l, v) => l.MouseWheel(v));
            }
            return base.OnScrollEvent(evnt);
        }

        protected override bool OnEnterNotifyEvent(EventCrossing evnt)
        {
            GrabFocus();
            IsFocused = true;
            ListenerDoEvent(new ViewportEvent(this, evnt.X, evnt.Y), (l, v) => l.MouseEnter(v));
            return base.OnEnterNotifyEvent(evnt);
        }

        protected override bool OnLeaveNotifyEvent(EventCrossing evnt)
        {
            IsFocused = false;
            ListenerDoEvent(new ViewportEvent(this, evnt.X, evnt.Y), (l, v) => l.MouseLeave(v));
            return base.OnLeaveNotifyEvent(evnt);
        }

        private Point _mouseDownLocation = new Point(-1, -1);

        protected override bool OnMotionNotifyEvent(EventMotion evnt)
        {
            if (IsFocused)
            {
                ListenerDoEvent(new ViewportEvent(this, evnt.X, evnt.Y), (l, v) => l.MouseMove(v));
                if (_mouseDownLocation.X >= 0 && _mouseDownLocation.Y >= 0
                    && Math.Abs(_mouseDownLocation.X - evnt.X) <= 1
                    && Math.Abs(_mouseDownLocation.Y - evnt.Y) <= 1)
                {
                    // Moved outside of the click hot spot
                    _mouseDownLocation = new Point(-1, -1);
                }
            }
            return base.OnMotionNotifyEvent(evnt);
        }

        protected override bool OnButtonReleaseEvent(EventButton evnt)
        {
            if (IsFocused)
            {
                ListenerDoEvent(new ViewportEvent(this, evnt.X, evnt.Y), (l, v) => l.MouseUp(v));
                if (_mouseDownLocation.X >= 0 && _mouseDownLocation.Y >= 0
                    && Math.Abs(_mouseDownLocation.X - evnt.X) <= 1
                    && Math.Abs(_mouseDownLocation.Y - evnt.Y) <= 1)
                {
                    // Mouse hasn't moved very much, trigger the click event
                    ListenerDoEvent(new ViewportEvent(this, evnt.X, evnt.Y, evnt.Button), (l, v) => l.MouseClick(v));
                }
            }
            _mouseDownLocation = new Point(-1, -1);
            return base.OnButtonReleaseEvent(evnt);
        }

        protected override bool OnButtonPressEvent(EventButton evnt)
        {
            if (IsFocused)
            {
                _mouseDownLocation = new Point((int) evnt.X, (int) evnt.Y);
                ListenerDoEvent(new ViewportEvent(this, evnt.X, evnt.Y, evnt.Button), (l, v) => l.MouseDown(v));
                if (evnt.Type == EventType.TwoButtonPress)
                {
                    ListenerDoEvent(new ViewportEvent(this, evnt.X, evnt.Y, evnt.Button), (l, v) => l.MouseDoubleClick(v));
                }
            }
            return base.OnButtonPressEvent(evnt);
        }
        /*
        protected override bool IsInputKey(Keys keyData)
        {
            // http://www.opentk.com/node/1192
            // Force all keys to be passed to the regular key events
            return true;
        }*/

        protected override bool OnKeyPressEvent(EventKey evnt)
        {
            if (IsFocused)
            {
                // imContext.FilterKeypress(evnt); // todo?

                ListenerDoEvent(new ViewportEvent(this, evnt.Key), (l, v) => l.KeyDown(v));
            }
            return base.OnKeyPressEvent(evnt);
        }

        protected override bool OnKeyReleaseEvent(EventKey evnt)
        {
            if (IsFocused)
            {
                ListenerDoEvent(new ViewportEvent(this, evnt.Key), (l, v) => l.KeyUp(v));
            }
            return base.OnKeyReleaseEvent(evnt);
        }

        protected virtual void OnKeyInputContextCommit(object sender, CommitArgs commit)
        {
            if (IsFocused)
            {
                foreach (var c in commit.Str)
                {
                    ListenerDoEvent(new ViewportEvent(this, c), (l, v) => l.KeyPress(v));
                }
            }
        }
    }
}
