using System.Collections.Generic;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Rendering;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;

namespace Sledge.Editor.Tools2.DraggableTool
{
    public interface IDraggable
    {
        void Click(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position);
        bool CanDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position);
        void Highlight(MapViewport viewport, OrthographicCamera camera);
        void Unhighlight(MapViewport viewport, OrthographicCamera camera);
        void StartDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position);
        void Drag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate lastPosition, Coordinate position);
        void EndDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Coordinate position);
        IEnumerable<SceneObject> GetSceneObjects();
    }
}