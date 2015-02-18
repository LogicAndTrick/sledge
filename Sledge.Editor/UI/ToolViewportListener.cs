using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools;
using Sledge.Rendering;

namespace Sledge.Editor.UI
{
    public class ToolViewportListener : IViewportEventListener
    {
        public MapViewport Viewport { get; set; }

        public ToolViewportListener(MapViewport viewport)
        {
            Viewport = viewport;
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
            return ShouldRelayEvent(ToolManager.ActiveTool);
        }

        public void KeyUp(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.KeyUp(Viewport, e);
        }

        public void KeyDown(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.KeyDown(Viewport, e);
        }

        public void KeyPress(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.KeyPress(Viewport, e);
        }

        public void MouseMove(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.MouseMove(Viewport, e);
        }

        public void MouseWheel(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.MouseWheel(Viewport, e);
        }

        public void MouseUp(ViewportEvent e)
        {
            if (e.Button == MouseButtons.Right && Viewport is MapViewport) Mediator.Publish(EditorMediator.ViewportRightClick, new object[] {Viewport, e});
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.MouseUp(Viewport, e);
        }

        public void MouseDown(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.MouseDown(Viewport, e);
        }

        public void MouseClick(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.MouseClick(Viewport, e);
        }

        public void MouseDoubleClick(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.MouseDoubleClick(Viewport, e);
        }

        public void DragStart(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.DragStart(Viewport, e);
        }

        public void DragMove(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.DragMove(Viewport, e);
        }

        public void DragEnd(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.DragEnd(Viewport, e);
        }

        public void MouseEnter(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.MouseEnter(Viewport, e);
        }

        public void MouseLeave(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.MouseLeave(Viewport, e);
        }

        public void ZoomChanged(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.ZoomChanged(Viewport, e);
        }

        public void PositionChanged(ViewportEvent e)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.PositionChanged(Viewport, e);
        }

        public void UpdateFrame(Frame frame)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.UpdateFrame(Viewport, frame);
        }
    }
}
