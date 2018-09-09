using System;
using System.Diagnostics;
using System.Windows.Forms;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
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

        public ICamera Camera
        {
            get => _camera;
            set
            {
                _camera = value;
                _camera.Width = Width;
                _camera.Height = Height;
            }
        }

        public Control Control => this;
        public ViewportOverlay Overlay { get; }
        public bool IsFocused => _isFocused;

        private bool _isFocused;
        private int _unfocusedCounter = 0;
        private ICamera _camera;

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

            Overlay = new ViewportOverlay(this);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            // Force all keys to be passed to the regular key events
            return true;
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

        protected override void OnMouseEnter(EventArgs e)
        {
            _isFocused = true;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isFocused = false;
            base.OnMouseLeave(e);
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
            if (!_isFocused)
            {
                _unfocusedCounter++;

                // Update every 10th frame
                if (_unfocusedCounter % 10 != 0)
                {
                    return false;
                }
            }

            _unfocusedCounter = 0;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Overlay.Dispose();
                Swapchain.Dispose();
            }
        }
    }
}
