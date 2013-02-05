using Sledge.Editor.Tools;
using Sledge.Graphics.Renderables;
using Sledge.UI;

namespace Sledge.Editor.Rendering
{
    class ToolRenderable : IRenderable
    {
        public void Render(object sender)
        {
            if (ToolManager.ActiveTool == null) return;

            if ((ToolManager.ActiveTool.Usage == BaseTool.ToolUsage.Both && sender is ViewportBase)
                || (ToolManager.ActiveTool.Usage == BaseTool.ToolUsage.View2D && sender is Viewport2D)
                || (ToolManager.ActiveTool.Usage == BaseTool.ToolUsage.View3D && sender is Viewport3D))
            {
                var vp = sender as ViewportBase;
                ToolManager.ActiveTool.Render(vp);
            }
        }
    }
}
