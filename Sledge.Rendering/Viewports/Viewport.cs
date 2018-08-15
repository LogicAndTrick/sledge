using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Veldrid;

namespace Sledge.Rendering.Viewports
{
    public class Viewport : Control, IViewport
    {
        private static int _nextId = 1;
        private static readonly IntPtr HInstance = Process.GetCurrentProcess().Handle;

        private bool _resizeRequired;

        public int ID { get; }
        public Swapchain Swapchain { get; }
        public ICamera Camera { get; set; }
        public Control Control => this;

        public event EventHandler<long> OnUpdate;

        public Viewport(GraphicsDevice graphics, GraphicsDeviceOptions options)
        {
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            DoubleBuffered = false;

            var hWnd = Handle; // Will call CreateHandle internally
            var hInstance = HInstance;

            uint w = (uint)Width, h = (uint)Height;
            if (w <= 0) w = 1;
            if (h <= 0) h = 1;

            ID = _nextId++;
            Camera = new PerspectiveCamera { Width = Width, Height = Height };

            var source = SwapchainSource.CreateWin32(hWnd, hInstance);
            var desc = new SwapchainDescription(source, w, h, options.SwapchainDepthFormat, options.SyncToVerticalBlank);
            Swapchain = graphics.ResourceFactory.CreateSwapchain(desc);
        }

        public void Update(long frame)
        {
            if (_resizeRequired)
            {
                var w = Math.Max(Width, 1);
                var h = Math.Max(Height, 1);
                Swapchain.Resize((uint) w, (uint) h);
                _resizeRequired = false;
            }

            OnUpdate?.Invoke(this, frame);
        }

        protected override void OnResize(EventArgs e)
        {
            _resizeRequired = true;
            Camera.Width = Width;
            Camera.Height = Height;
            base.OnResize(e);
        }

        public bool ShouldRender(long frame)
        {
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Swapchain.Dispose();
            }
        }
    }
}
