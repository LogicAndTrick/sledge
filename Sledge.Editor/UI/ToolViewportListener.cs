using System;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.UI;
using Sledge.Editor.Tools;

namespace Sledge.Editor.UI
{
    public class ToolViewportListener : IViewportEventListener
    {
        public ViewportBase Viewport { get; set; }

        public ToolViewportListener(ViewportBase viewport)
        {
            Viewport = viewport;
        }

        private bool ShouldRelayEvent(BaseTool tool)
        {
            if (tool == null) return false;
            var usage = tool.Usage;
            return usage == BaseTool.ToolUsage.Both
                   || (usage == BaseTool.ToolUsage.View2D && Viewport is Viewport2D)
                   || (usage == BaseTool.ToolUsage.View3D && Viewport is Viewport3D);
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
            if (e.Button == MouseButtons.Right && Viewport is Viewport2D) Mediator.Publish(EditorMediator.ViewportRightClick, new object[] {Viewport, e});
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

        public void UpdateFrame(FrameInfo frame)
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.UpdateFrame(Viewport, frame);
        }

        public void PreRender()
        {
            if (!ShouldRelayEvent(ToolManager.ActiveTool)) return;
            ToolManager.ActiveTool.PreRender(Viewport);
        }

        public void Render3D()
        {
            
        }

        public void Render2D()
        {
            
        }

        public void PostRender()
        {
            
        }
    }
}
