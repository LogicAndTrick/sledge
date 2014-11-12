using Sledge.EditorNew.Tools;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Graphics.Renderables;

namespace Sledge.EditorNew.Rendering
{
    class ToolRenderable : IRenderable
    {
        public void Render(object sender)
        {
            if (ToolManager.ActiveTool == null) return;

            if ((ToolManager.ActiveTool.Usage == BaseTool.ToolUsage.Both && sender is IMapViewport)
                || (ToolManager.ActiveTool.Usage == BaseTool.ToolUsage.View2D && sender is IViewport2D)
                || (ToolManager.ActiveTool.Usage == BaseTool.ToolUsage.View3D && sender is IViewport3D))
            {
                var vp = sender as IMapViewport;
                ToolManager.ActiveTool.Render(vp);
            }
        }
    }
}
