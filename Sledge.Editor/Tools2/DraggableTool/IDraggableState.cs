using System.Collections.Generic;
using Sledge.Editor.Rendering;
using Sledge.Rendering.Cameras;

namespace Sledge.Editor.Tools2.DraggableTool
{
    public interface IDraggableState : IDraggable
    {
        IEnumerable<IDraggable> GetDraggables(MapViewport viewport, OrthographicCamera camera);
    }
}