using System.Collections.Generic;

namespace Sledge.BspEditor.Tools.Draggable
{
    public interface IDraggableState : IDraggable
    {
        IEnumerable<IDraggable> GetDraggables();
    }
}