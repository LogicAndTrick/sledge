using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Rendering.Engine
{
    public class Engine : IDisposable
    {
        internal static Engine Instance { get; } = new Engine();
        public static EngineInterface Interface { get; } = new EngineInterface();

        public GraphicsDevice Device { get; }
        public Thread RenderThread { get; private set; }
        public Scene Scene { get; }
        internal RenderContext Context { get; }

        private CancellationTokenSource _token;

        private readonly GraphicsDeviceOptions _options;
        private readonly Stopwatch _timer;
        private readonly object _lock = new object();
        private readonly List<IViewport> _renderTargets;
        private readonly List<IPipeline> _pipelines;
        private readonly CommandList _commandList;

        private RgbaFloat _clearColourPerspective;
        private RgbaFloat _clearColourOrthographic;

        private Engine()
        {
            _options = new GraphicsDeviceOptions
            {
                HasMainSwapchain = false,
                ResourceBindingModel = ResourceBindingModel.Improved,
                SwapchainDepthFormat = PixelFormat.R32_Float,
            };

            Device = GraphicsDevice.CreateD3D11(_options);
            Scene = new Scene();

            _commandList = Device.ResourceFactory.CreateCommandList();

            SetClearColour(CameraType.Both, RgbaFloat.Black);

            _timer = new Stopwatch();
            _token = new CancellationTokenSource();

            _renderTargets = new List<IViewport>();
            _pipelines = new List<IPipeline>();
            Context = new RenderContext(Device);
            Scene.Add(Context);
            Scene.Add(new FpsMonitor());

            RenderThread = new Thread(Loop);

            AddPipeline(new WireframeGenericPipeline());
            AddPipeline(new FlatColourGenericPipeline());
            AddPipeline(new TexturedGenericPipeline());
            AddPipeline(new TexturedBillboardPipeline());
            AddPipeline(new OverlayPipeline());

            Application.ApplicationExit += Shutdown;
        }

        internal void SetClearColour(CameraType type, RgbaFloat colour)
        {
            if (type == CameraType.Both) _clearColourOrthographic = _clearColourPerspective = colour;
            else if (type == CameraType.Orthographic) _clearColourOrthographic = colour;
            else _clearColourPerspective = colour;
        }

        public void AddPipeline(IPipeline pipeline)
        {
            pipeline.Create(Context);
            _pipelines.Add(pipeline);
        }

        public void Dispose()
        {
            _pipelines.ForEach(x => x.Dispose());
            _pipelines.Clear();

            _renderTargets.ForEach(x => x.Dispose());
            _renderTargets.Clear();

            Device.Dispose();
            _token.Dispose();
        }

        private void Shutdown(object sender, EventArgs e)
        {
            Dispose();
            Application.ApplicationExit -= Shutdown;
        }

        // Render loop

        private void Start()
        {
            _timer.Start();
            RenderThread.Start(_token.Token);
        }

        private void Stop()
        {
            _token.Cancel();
            _timer.Stop();

            RenderThread = new Thread(Loop);
            _token = new CancellationTokenSource();
        }


        private int _paused = 0;
        private readonly ManualResetEvent _pauseThreadEvent = new ManualResetEvent(false);

        public IDisposable Pause()
        {
            _paused++;
            if (_timer.IsRunning) _pauseThreadEvent.WaitOne();
            return new PauseImpl(() =>
            {
                _paused--;
                _pauseThreadEvent.Reset();
            });
        }
        private class PauseImpl : IDisposable
        {
            private readonly Action _disposeAction;
            public PauseImpl(Action disposeFunc) => _disposeAction = disposeFunc;
            public void Dispose() => _disposeAction();
        }

        private void Loop(object o)
        {
            var token = (CancellationToken) o;
            try
            {
                var lastFrame = _timer.ElapsedMilliseconds;
                while (!token.IsCancellationRequested)
                {
                    var frame = _timer.ElapsedMilliseconds;
                    var diff = (frame - lastFrame);
                    if (diff < 16 || _paused > 0)
                    {
                        if (_paused > 0) _pauseThreadEvent.Set();
                        Thread.Sleep(2);
                        continue;
                    }

                    lastFrame = frame;
                    Render(frame);
                    Device.WaitForIdle();
                }
            }
            catch (ThreadInterruptedException)
            {
                // exit
            }
            catch (ThreadAbortException)
            {
                // exit
            }
        }

        private void Render(long frame)
        {
            lock (_lock)
            {
                Scene.Update(frame);
                var overlays = Scene.GetOverlayRenderables();

                foreach (var rt in _renderTargets)
                {
                    rt.Update(frame);
                    rt.Overlay.Build(Context, overlays);
                    if (rt.ShouldRender(frame))
                    {
                        Render(rt);
                    }
                }
            }
        }

        private void Render(IViewport renderTarget)
        {
            _commandList.Begin();
            _commandList.SetFramebuffer(renderTarget.Swapchain.Framebuffer);
            _commandList.ClearDepthStencil(1);

            var cc = renderTarget.Camera.Type == CameraType.Perspective
                ? _clearColourPerspective
                : _clearColourOrthographic;
            _commandList.ClearColorTarget(0, cc);

            var renderables = _pipelines.ToDictionary(x => x, x => Scene.GetRenderables(x, renderTarget).OrderBy(r => r.Order).ToList());

            foreach (var pipeline in _pipelines.OrderBy(x => x.Order))
            {
                if (!renderables.ContainsKey(pipeline)) continue;
                pipeline.Render(Context, renderTarget, _commandList, renderables[pipeline]);
            }

            foreach (var pipeline in _pipelines.OrderBy(x => x.Order))
            {
                if (!renderables.ContainsKey(pipeline)) continue;
                pipeline.RenderTransparent(Context, renderTarget, _commandList, renderables[pipeline]);
            }
            
            _commandList.End();

            Device.SubmitCommands(_commandList);
            Device.SwapBuffers(renderTarget.Swapchain);
        }

        // Viewports

        public event EventHandler<IViewport> ViewportCreated;
        public event EventHandler<IViewport> ViewportDestroyed;

        public IViewport CreateViewport()
        {
            lock (_lock)
            {
                var control = new Viewports.Viewport(Device, _options);
                control.Disposed += DestroyViewport;

                if (!_renderTargets.Any()) Start();
                _renderTargets.Add(control);

                Scene.Add(control.Overlay);
                ViewportCreated?.Invoke(this, control);

                return control;
            }
        }

        private void DestroyViewport(object viewport, EventArgs e)
        {
            if (!(viewport is IViewport t)) return;

            lock (_lock)
            {
                _renderTargets.Remove(t);
                Device.WaitForIdle();

                if (!_renderTargets.Any()) Stop();

                ViewportDestroyed?.Invoke(this, t);
                Scene.Remove(t.Overlay);

                t.Control.Disposed -= DestroyViewport;
                t.Dispose();
            }
        }
    }
}
