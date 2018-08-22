using System;
using System.Drawing;
using System.Numerics;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Resources;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Tools.Draggable
{
    public abstract class BaseDraggable : IDraggable
    {
        public event EventHandler DragStarted;
        public event EventHandler DragMoved;
        public event EventHandler DragEnded;

        protected virtual void OnDragStarted()
        {
            DragStarted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDragMoved()
        {
            DragMoved?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDragEnded()
        {
            DragEnded?.Invoke(this, EventArgs.Empty);
        }

        public virtual void StartDrag(MapViewport viewport, ViewportEvent e, Vector3 position)
        {
            OnDragStarted();
        }

        public virtual void Drag(MapViewport viewport, ViewportEvent e, Vector3 lastPosition, Vector3 position)
        {
            OnDragMoved();
        }

        public virtual void EndDrag(MapViewport viewport, ViewportEvent e, Vector3 position)
        {
            OnDragEnded();
        }


        public virtual void MouseDown(MapViewport viewport, ViewportEvent e, Vector3 position)
        {

        }

        public virtual void MouseUp(MapViewport viewport, ViewportEvent e, Vector3 position)
        {
            
        }

        public abstract void Click(MapViewport viewport, ViewportEvent e, Vector3 position);
        public abstract bool CanDrag(MapViewport viewport, ViewportEvent e, Vector3 position);
        public abstract void Highlight(MapViewport viewport);
        public abstract void Unhighlight(MapViewport viewport);
        public abstract void Render(BufferBuilder builder);
        public abstract void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics);
        public abstract void Render(IViewport viewport, PerspectiveCamera camera, Graphics graphics);
    }
}