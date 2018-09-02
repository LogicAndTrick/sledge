using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.Rendering.Cameras;
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
        public void MouseDown(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) { }
        public void MouseUp(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) { }
        public void Click(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) { }
        public bool CanDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) => false;
        public void Highlight(MapViewport viewport) { }
        public void Unhighlight(MapViewport viewport) { }
        public void StartDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) { }
        public void Drag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition,
            Vector3 position) { }
        public void EndDrag(MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position) { }
        public void Render(BufferBuilder builder) { }
        public void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics) { }
        public void Render(IViewport viewport, PerspectiveCamera camera, Graphics graphics) { }
    }
}