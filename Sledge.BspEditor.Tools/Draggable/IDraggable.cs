using System;
using System.Collections.Generic;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.BspEditor.Tools.Draggable
{
    public interface IDraggable
    {
        event EventHandler DragStarted;
        event EventHandler DragMoved;
        event EventHandler DragEnded;
        void MouseDown(MapViewport viewport, ViewportEvent e, Coordinate position);
        void MouseUp(MapViewport viewport, ViewportEvent e, Coordinate position);
        void Click(MapViewport viewport, ViewportEvent e, Coordinate position);
        bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position);
        void Highlight(MapViewport viewport);
        void Unhighlight(MapViewport viewport);
        void StartDrag(MapViewport viewport, ViewportEvent e, Coordinate position);
        void Drag(MapViewport viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position);
        void EndDrag(MapViewport viewport, ViewportEvent e, Coordinate position);
        IEnumerable<SceneObject> GetSceneObjects();
        IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera);
        IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera);
    }
}