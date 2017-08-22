using System;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Common.Shell.Components;
using Sledge.Rendering;

namespace Sledge.BspEditor.Tools
{
    public class ToolViewportListener : IViewportEventListener
    {
        private WeakReference<BaseTool> _activeTool = new WeakReference<BaseTool>(null);

        private BaseTool ActiveTool => _activeTool.TryGetTarget(out BaseTool t) ? t : null;

        public MapViewport Viewport { get; set; }

        public ToolViewportListener(MapViewport viewport)
        {
            Viewport = viewport;

            Oy.Subscribe<ITool>("Tool:Activated", ToolActivated);
        }

        private async Task ToolActivated(ITool tool)
        {
            _activeTool = new WeakReference<BaseTool>(tool as BaseTool);
        }

        private bool ShouldRelayEvent(BaseTool tool)
        {
            if (tool == null) return false;
            var usage = tool.Usage;
            return usage == BaseTool.ToolUsage.Both
                   || (usage == BaseTool.ToolUsage.View2D && Viewport.Is2D)
                   || (usage == BaseTool.ToolUsage.View3D && Viewport.Is3D);
        }

        public bool IsActive()
        {
            return ShouldRelayEvent(ActiveTool);
        }

        public void KeyUp(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.KeyUp(Viewport, e);
        }

        public void KeyDown(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.KeyDown(Viewport, e);
        }

        public void KeyPress(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.KeyPress(Viewport, e);
        }

        public void MouseMove(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.MouseMove(Viewport, e);
        }

        public void MouseWheel(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.MouseWheel(Viewport, e);
        }

        public void MouseUp(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.MouseUp(Viewport, e);
        }

        public void MouseDown(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.MouseDown(Viewport, e);
        }

        public void MouseClick(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.MouseClick(Viewport, e);
        }

        public void MouseDoubleClick(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.MouseDoubleClick(Viewport, e);
        }

        public void DragStart(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.DragStart(Viewport, e);
        }

        public void DragMove(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.DragMove(Viewport, e);
        }

        public void DragEnd(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.DragEnd(Viewport, e);
        }

        public void MouseEnter(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.MouseEnter(Viewport, e);
        }

        public void MouseLeave(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.MouseLeave(Viewport, e);
        }

        public void ZoomChanged(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.ZoomChanged(Viewport, e);
        }

        public void PositionChanged(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.PositionChanged(Viewport, e);
        }

        public void UpdateFrame(Frame frame)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.UpdateFrame(Viewport, frame);
        }
    }
}
