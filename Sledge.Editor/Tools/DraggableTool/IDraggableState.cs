using System.Collections.Generic;

namespace Sledge.Editor.Tools.DraggableTool
{
    public interface IDraggableState : IDraggable
    {
        IEnumerable<IDraggable> GetDraggables();
    }
}