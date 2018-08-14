using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Rendering.Engine
{
    public class Engine : IDisposable
    {
        public static Engine Instance { get; } = new Engine();

        public GraphicsDevice Device { get; }
        public Thread RenderThread { get; }
        public Scene Scene { get; }
        internal RenderContext Context { get; }

        private readonly GraphicsDeviceOptions _options;
        private readonly Stopwatch _timer;
        private readonly CancellationTokenSource _token;
        private readonly object _lock = new object();
        private readonly List<IViewport> _renderTargets;
        private readonly List<IPipeline> _pipelines;
        private readonly CommandList _commandList;

        private Engine()
        {
            _options = new GraphicsDeviceOptions
            {
                HasMainSwapchain = false,
                ResourceBindingModel = ResourceBindingModel.Improved,
                SwapchainDepthFormat = PixelFormat.R32_Float
            };

            Device = GraphicsDevice.CreateD3D11(_options);
            Scene = new Scene();

            _commandList = Device.ResourceFactory.CreateCommandList();

            _timer = new Stopwatch();
            _token = new CancellationTokenSource();

            _renderTargets = new List<IViewport>();
            _pipelines = new List<IPipeline>();
            Context = new RenderContext(Device);
            Scene.Add(Context);

            RenderThread = new Thread(Loop);

            AddPipeline(new WireframeGenericPipeline());
            AddPipeline(new FlatColourGenericPipeline());
            AddPipeline(new TexturedGenericPipeline());

            Application.ApplicationExit += Shutdown;
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
                    if (diff < 16)
                    {
                        Thread.Sleep(1);
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

                foreach (var rt in _renderTargets)
                {
                    rt.Update(frame);
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
            _commandList.ClearColorTarget(0, RgbaFloat.Black);

            foreach (var pipeline in _pipelines.OrderBy(x => x.Order))
            {
                var renderables = Scene.GetRenderables(pipeline, renderTarget);
                pipeline.Render(Context, renderTarget, _commandList, renderables);
            }
            
            _commandList.End();

            Device.SubmitCommands(_commandList);
            Device.SwapBuffers(renderTarget.Swapchain);
        }

        // Viewports

        public IViewport CreateViewport()
        {
            lock (_lock)
            {
                var control = new Viewports.Viewport(Device, _options);
                control.Disposed += DestroyViewport;

                if (!_renderTargets.Any()) Start();
                _renderTargets.Add(control);

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

                t.Control.Disposed -= DestroyViewport;
                t.Dispose();
            }
        }
    }
}
