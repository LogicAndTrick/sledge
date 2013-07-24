using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            ListenerDo(l => l.MouseWheel(e));
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
            IsFocused = true;
            ListenerDo(l => l.MouseEnter(e));
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            ListenerDo(l => l.MouseLeave(e));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            ListenerDo(l => l.MouseMove(e));
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            ListenerDo(l => l.MouseUp(e));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            ListenerDo(l => l.MouseDown(e));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            ListenerDo(l => l.KeyDown(e));
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            ListenerDo(l => l.KeyPress(e));
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            ListenerDo(l => l.KeyUp(e));
        }
    }
}
