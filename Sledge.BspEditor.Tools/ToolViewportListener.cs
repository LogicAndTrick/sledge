using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Common.Shell.Components;

namespace Sledge.BspEditor.Tools
{
    public class ToolViewportListener : IViewportEventListener
    {
        public string OrderHint => "D";

        private WeakReference<BaseTool> _activeTool = new WeakReference<BaseTool>(null);

        private BaseTool ActiveTool => _activeTool.TryGetTarget(out var t) ? t : null;

        public MapViewport Viewport { get; set; }

        public ToolViewportListener(MapViewport viewport)
        {
            Viewport = viewport;

            Oy.Subscribe<ITool>("Tool:Activated", ToolActivated);
        }

        private Task ToolActivated(ITool tool)
        {
            _activeTool = new WeakReference<BaseTool>(tool as BaseTool);
            return Task.CompletedTask;
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

        public void UpdateFrame(long frame)
        {
            if (!ShouldRelayEvent(ActiveTool)) return;
            ActiveTool.UpdateFrame(Viewport, frame);
        }

        public bool Filter(string hotkey, int keys)
        {
            if (!ShouldRelayEvent(ActiveTool)) return false;
            return ActiveTool.FilterHotkey(Viewport, hotkey, (Keys) keys);
        }

        public virtual void Dispose()
        {
            // 
        }
    }
}
