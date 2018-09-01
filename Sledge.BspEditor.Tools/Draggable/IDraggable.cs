using System;
using System.Numerics;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Tools.Draggable
{
    public interface IDraggable : IOverlayRenderable
    {
        Vector3 Origin { get; }
        event EventHandler DragStarted;
        event EventHandler DragMoved;
        event EventHandler DragEnded;
        void MouseDown(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position);
        void MouseUp(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position);
        void Click(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position);
        bool CanDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position);
        void Highlight(MapViewport viewport);
        void Unhighlight(MapViewport viewport);
        void StartDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position);
        void Drag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position);
        void EndDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position);
        void Render(BufferBuilder builder);
    }
}