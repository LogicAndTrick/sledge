using System.Collections.Generic;
using Sledge.EditorNew.UI.Viewports;

namespace Sledge.EditorNew.Tools.DraggableTool
{
    public interface IDraggableState : IDraggable
    {
        IEnumerable<IDraggable> GetDraggables(IViewport2D viewport);
    }
}