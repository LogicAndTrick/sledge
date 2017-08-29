using System.Drawing;
using OpenTK;
using Sledge.BspEditor.Rendering.Viewport;

namespace Sledge.BspEditor.Tools.Widgets
{
    public abstract class Widget : BaseTool
    {
        protected MapViewport _activeViewport;

        public delegate void TransformEventHandler(Widget sender, Matrix4? transformation);
        public event TransformEventHandler Transforming;
        public event TransformEventHandler Transformed;

        public abstract bool IsUniformTransformation { get; }
        public abstract bool IsScaleTransformation { get; }

        protected void OnTransforming(Matrix4? transformation)
        {
            Transforming?.Invoke(this, transformation);
        }

        protected void OnTransformed(Matrix4? transformation)
        {
            Transformed?.Invoke(this, transformation);
        }

        public override Image GetIcon() { return null; }
        public override string GetName() { return "Widget"; }

        public override void MouseEnter(MapViewport viewport, ViewportEvent e)
        {
            _activeViewport = viewport;
            base.MouseEnter(viewport, e);
        }

        public override void MouseLeave(MapViewport viewport, ViewportEvent e)
        {
            _activeViewport = null;
            base.MouseLeave(viewport, e);
        }

        public abstract void SelectionChanged();
    }
}