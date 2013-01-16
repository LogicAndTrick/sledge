using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
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

            if (!Context.IsCurrent)
            {
                MakeCurrent();
            }

            Listeners.ForEach(l => l.UpdateFrame());

            LoadIdentity();
            UpdateAfterLoadIdentity();

            UpdateBeforeSetViewport();
            SetViewport();

            UpdateBeforeClearViewport();
            ClearViewport();

            UpdateBeforeRender();
            RenderContext.Render(this);
            UpdateAfterRender();

            SwapBuffers();
        }

        public void LoadIdentity()
        {
            GL.LoadIdentity();
        }

        protected virtual void UpdateAfterLoadIdentity()
        {

        }

        protected virtual void UpdateBeforeSetViewport()
        {

        }

        protected virtual void UpdateBeforeClearViewport()
        {
            Listeners.ForEach(x => x.PreRender());
        }

        protected virtual void UpdateBeforeRender()
        {

        }

        protected virtual void UpdateAfterRender()
        {

        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Listeners.ForEach(l => l.MouseWheel(e));
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
            IsFocused = true;
            Listeners.ForEach(l => l.MouseEnter(e));
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Listeners.ForEach(l => l.MouseLeave(e));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Listeners.ForEach(l => l.MouseMove(e));
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            Listeners.ForEach(l => l.MouseUp(e));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Listeners.ForEach(l => l.MouseDown(e));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Listeners.ForEach(l => l.KeyDown(e));
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            Listeners.ForEach(l => l.KeyPress(e));
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            Listeners.ForEach(l => l.KeyUp(e));
        }
    }
}
