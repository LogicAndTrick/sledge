using System;
using System.Diagnostics;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.OpenGL
{
    public class OpenGLViewport : GLControl, IViewport
    {
        private Timer UpdateTimer { get; set; }
        private readonly Stopwatch _stopwatch;
        public bool IsFocused { get; private set; }
        private int UnfocusedUpdateCounter { get; set; }
        public Camera Camera { get; set; }

        private static int _viewportCounter;
        private string _vpHandle;

        public string ViewportHandle
        {
            get { return _vpHandle ?? (_vpHandle = "OpenGLViewport_" + _viewportCounter++); }
        }

        #region Events

        public Control Control { get { return this; } }
        public new event FrameEventHandler Update;
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

        public OpenGLViewport() : base(new GraphicsMode(GraphicsMode.Default.ColorFormat, 24))
        {
            _stopwatch = new Stopwatch();
            UpdateTimer = new Timer { Interval = 1 };
            UpdateTimer.Tick += (sender, e) => UpdateFrame();
            Camera = new OrthographicCamera(OrthographicCamera.OrthographicType.Top);
        }

        public void Run()
        {
            Toolkit.Init();
            MakeCurrent();
            _stopwatch.Start();
            UpdateTimer.Start();
        }

        public void UpdateNextFrame()
        {
            UnfocusedUpdateCounter = -1;
        }

        #region Overrides
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UpdateTimer.Dispose();
                _stopwatch.Stop();
            }
            base.Dispose(disposing);
        }

        protected override void OnResize(EventArgs e)
        {
            MakeCurrent();
            SetViewport();
            base.OnResize(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
            IsFocused = true;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            IsFocused = false;
            base.OnMouseLeave(e);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            // http://www.opentk.com/node/1192
            // Force all keys to be passed to the regular key events
            return true;
        }
        #endregion

        #region Update loop
        private void UpdateFrame()
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

        private void SetViewport()
        {
            GL.Viewport(0, 0, Width, Height);
            GL.Scissor(0, 0, Width, Height);
        }
        #endregion
    }
}