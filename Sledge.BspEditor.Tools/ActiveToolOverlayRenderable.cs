using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hooks;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Tools
{
    [Export(typeof(IOverlayRenderable))]
    [Export(typeof(IStartupHook))]
    public class ActiveToolOverlayRenderable : IOverlayRenderable, IStartupHook
    {
        private readonly WeakReference<BaseTool> _activeTool = new WeakReference<BaseTool>(null);
        private BaseTool ActiveTool => _activeTool.TryGetTarget(out var t) ? t : null;

        public Task OnStartup()
        {
            Oy.Subscribe<ITool>("Tool:Activated", ToolActivated);
            return Task.CompletedTask;
        }

        private Task ToolActivated(ITool tool)
        {
            _activeTool.SetTarget(tool as BaseTool);
            return Task.CompletedTask;
        }

        public void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics)
        {
            var at = ActiveTool;
            at?.Render(viewport, camera, worldMin, worldMax, graphics);
        }

        public void Render(IViewport viewport, PerspectiveCamera camera, Graphics graphics)
        {
            var at = ActiveTool;
            at?.Render(viewport, camera, graphics);
        }
    }
}
