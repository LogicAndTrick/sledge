using Sledge.EditorNew.UI.Viewports;

namespace Sledge.EditorNew.Tools.DraggableTool
{
    public interface IDraggable
    {
        bool CanDrag(IViewport2D viewport, ViewportEvent e);
        void Highlight(IViewport2D viewport);
        void Unhighlight(IViewport2D viewport);
        void StartDrag(IViewport2D viewport, ViewportEvent e);
        void Drag(IViewport2D viewport, ViewportEvent e);
        void EndDrag(IViewport2D viewport, ViewportEvent e);
        void Render(IViewport2D viewport);
    }
}