using System;
using System.Numerics;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Tools.Draggable
{
    public interface IDraggable : IOverlayRenderable
    {
        Vector3 Origin { get; }
        Vector3 ZIndex { get; }
        event EventHandler DragStarted;
        event EventHandler DragMoved;
        event EventHandler DragEnded;
        void MouseDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position);
        void MouseUp(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position);
        void Click(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position);
        bool CanDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position);
        void Highlight(MapDocument document, MapViewport viewport);
        void Unhighlight(MapDocument document, MapViewport viewport);
        void StartDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position);
        void Drag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position);
        void EndDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position);
        void Render(MapDocument document, BufferBuilder builder);
    }
}