using System.Collections.Generic;

namespace Sledge.Editor.Tools2.DraggableTool
{
    public interface IDraggableState : IDraggable
    {
        IEnumerable<IDraggable> GetDraggables();
    }
}