using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Tools.Vertex.Tools
{
    public class WrapperDraggableState : IDraggableState
    {
        private readonly Func<IEnumerable<IDraggable>> _getDraggablesFunc;

        public WrapperDraggableState(Func<IEnumerable<IDraggable>> draggablesFunc)
        {
            _getDraggablesFunc = draggablesFunc;
        }

        public IEnumerable<IDraggable> GetDraggables() => _getDraggablesFunc();

        public Vector3 Origin
        {
            get
            {
                var list = GetDraggables().ToList();
                return list.Aggregate(Vector3.Zero, (a, b) => a + b.Origin) / list.Count();
            }
        }

        public Vector3 ZIndex
        {
            get
            {
                var list = GetDraggables().ToList();
                return list.Aggregate(Vector3.Zero, (a, b) => a + b.ZIndex) / list.Count();
            }
        }

        public event EventHandler DragStarted;
        public event EventHandler DragMoved;
        public event EventHandler DragEnded;
        public void MouseDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) { }
        public void MouseUp(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) { }
        public void Click(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) { }
        public bool CanDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) => false;
        public void Highlight(MapDocument document, MapViewport viewport) { }
        public void Unhighlight(MapDocument document, MapViewport viewport) { }
        public void StartDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) { }
        public void Drag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position) { }
        public void EndDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) { }
        public void Render(MapDocument document, BufferBuilder builder) { }
        public void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im) { }
        public void Render(IViewport viewport, PerspectiveCamera camera, I2DRenderer im) { }
    }
}