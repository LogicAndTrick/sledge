using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.UI.Viewports;

namespace Sledge.EditorNew.Tools.DraggableTool
{
    public interface IDraggable
    {
        bool CanDrag(IViewport2D viewport, ViewportEvent e, Coordinate position);
        void Highlight(IViewport2D viewport);
        void Unhighlight(IViewport2D viewport);
        void StartDrag(IViewport2D viewport, ViewportEvent e, Coordinate position);
        void Drag(IViewport2D viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position);
        void EndDrag(IViewport2D viewport, ViewportEvent e, Coordinate position);
        void Render(IViewport2D viewport);
    }
}