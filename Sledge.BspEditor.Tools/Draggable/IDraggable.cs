using System;
using System.Collections.Generic;
using System.Numerics;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Rendering.Cameras;

namespace Sledge.BspEditor.Tools.Draggable
{
    public interface IDraggable
    {
        event EventHandler DragStarted;
        event EventHandler DragMoved;
        event EventHandler DragEnded;
        void MouseDown(MapViewport viewport, ViewportEvent e, Vector3 position);
        void MouseUp(MapViewport viewport, ViewportEvent e, Vector3 position);
        void Click(MapViewport viewport, ViewportEvent e, Vector3 position);
        bool CanDrag(MapViewport viewport, ViewportEvent e, Vector3 position);
        void Highlight(MapViewport viewport);
        void Unhighlight(MapViewport viewport);
        void StartDrag(MapViewport viewport, ViewportEvent e, Vector3 position);
        void Drag(MapViewport viewport, ViewportEvent e, Vector3 lastPosition, Vector3 position);
        void EndDrag(MapViewport viewport, ViewportEvent e, Vector3 position);
        IEnumerable<SceneObject> GetSceneObjects();
        IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera);
        IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera);
    }
}