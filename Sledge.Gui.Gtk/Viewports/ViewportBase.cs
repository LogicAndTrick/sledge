using System;
using System.Diagnostics;
using Gdk;
using GLib;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sledge.Gui.Events;
using Sledge.Gui.Gtk.Controls.Implementations;
using Frame = Sledge.Gui.Structures.Frame;
using FrameEventHandler = Sledge.Gui.Events.FrameEventHandler;
using Timeout = GLib.Timeout;

namespace Sledge.Gui.Gtk.Viewports
{
    public abstract class ViewportBase : GLWidget
    {
        private readonly Stopwatch _stopwatch;
        public bool IsFocused { get; private set; }
        private int UnfocusedUpdateCounter { get; set; }
        private uint _timer;

        #region Events
        public event FrameEventHandler Update;
        public event FrameEventHandler Render;
        public event RenderExceptionEventHandler RenderException;

        private void OnUpdate(Frame frame)
        {
            if (Update != null) Update(this, frame);
        }

        private void OnRender(Frame frame)
        {
            if (Render != null) Render(this, frame);
        }

        private void OnRenderException(Exception ex)
        {
            if (RenderException == null) return;

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
        #endregion

        public ViewportBase() : base(new GraphicsMode(GraphicsMode.Default.ColorFormat, 24))
        {
            _stopwatch = new Stopwatch();
            CanFocus = true;
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Source.Remove(_timer);
                _stopwatch.Stop();
            }
            base.Dispose(disposing);
        }

        public void Run()
        {
            _stopwatch.Start();
            _timer = Timeout.Add(10, delegate
            {
                UpdateFrame();
                return true;
            });
            Idle.Add(IdleHandlerMethod);
        }

        public void UpdateNextFrame()
        {
            UnfocusedUpdateCounter = -1;
        }

        #region Overrides
        protected override void OnInitialized()
        {
            AddEvents((int) (EventMask.AllEventsMask));
            base.OnInitialized();
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
        #endregion

        #region Update loop
        private bool IdleHandlerMethod()
        {
            UpdateFrame();
            return true;
        }

        protected override void OnRenderFrame()
        {
            UpdateFrame();
            base.OnRenderFrame();
        }

        private void UpdateFrame()
        {
            if (!IsInitialised) return;

            if (!IsFocused) // Change this if things start to get choppy
            {
                UnfocusedUpdateCounter++;
                // Update every 10th frame
                if (UnfocusedUpdateCounter % 100 != 0)
                {
                    return;
                }
            }
            UnfocusedUpdateCounter = 0;

            try
            {
                MakeCurrent();

                var frame = new Frame(_stopwatch.ElapsedMilliseconds);
                OnUpdate(frame);

                LoadIdentity();
                SetViewport();
                ClearViewport();
                OnRender(frame);
            }
            catch (Exception ex)
            {
                OnRenderException(ex);
            }

            if (IsDisposed) return;

            SwapBuffers();
        }

        private void LoadIdentity()
        {
            GL.LoadIdentity();
        }

        private void ClearViewport()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        protected virtual void SetViewport()
        {
            GL.Viewport(0, 0, Allocation.Width, Allocation.Height);
            GL.Scissor(0, 0, Allocation.Width, Allocation.Height);
        }
        #endregion
    }
}
