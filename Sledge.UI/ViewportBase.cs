using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Graphics;
using System.Windows.Forms;
using Sledge.Graphics.Helpers;
using KeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;

namespace Sledge.UI
{
    public class ViewportBase : GLControl
    {
        public RenderContext RenderContext { get; set; }
        protected Timer UpdateTimer { get; set; }
        public List<IViewportEventListener> Listeners { get; set; }
        public bool IsFocused { get; private set; }
        private int UnfocusedUpdateCounter { get; set; }

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

        protected ViewportBase()
        {
            RenderContext = new RenderContext();
            Listeners = new List<IViewportEventListener>();
        }

        protected ViewportBase(RenderContext context)
        {
            RenderContext = context;
            Listeners = new List<IViewportEventListener>();
        }

        protected override void Dispose(bool disposing)
        {
            Listeners.OfType<IDisposable>().ToList().ForEach(x => x.Dispose());
            Listeners.Clear();
            
            RenderContext.Dispose();
            UpdateTimer.Dispose();
            base.Dispose(disposing);
        }

        public void ClearViewport()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public virtual void SetViewport()
        {
            Viewport.Switch(0, 0, Width, Height);
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

        protected override void OnResize(EventArgs e)
        {
            MakeCurrent();
            SetViewport();
            base.OnResize(e);
        }

        public void Run()
        {
            MakeCurrent();
            UpdateTimer = new Timer { Interval = 1 };
            UpdateTimer.Tick += (sender, e) => UpdateFrame();
            UpdateTimer.Start();
        }

        public void UpdateNextFrame()
        {
            UnfocusedUpdateCounter = -1;
        }

        protected void UpdateFrame()
        {
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
                if (!Context.IsCurrent)
                {
                    MakeCurrent();
                }
            }
            catch (Exception ex)
            {
                OnRenderException(ex);
            }

            ListenerDo(x => x.UpdateFrame());

            LoadIdentity();
            UpdateAfterLoadIdentity();

            UpdateBeforeSetViewport();
            SetViewport();

            UpdateBeforeClearViewport();
            ClearViewport();

            try
            {
                UpdateBeforeRender();
                RenderContext.Render(this);
                UpdateAfterRender();
            }
            catch(Exception ex)
            {
                OnRenderException(ex);
            }

            if (IsDisposed) return;

            SwapBuffers();
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

        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e),  (l, v) => l.MouseWheel(v));
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
            IsFocused = true;
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseEnter(v));
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            IsFocused = false;
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseLeave(v));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseMove(v));
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseUp(v));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseDown(v));
        }

        protected override bool IsInputKey(Keys keyData)
        {
            // http://www.opentk.com/node/1192
            // Force all keys to be passed to the regular key events
            return true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.KeyDown(v));
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.KeyPress(v));
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.KeyUp(v));
        }
    }
}
