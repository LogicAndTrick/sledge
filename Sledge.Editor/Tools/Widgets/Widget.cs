using System.Drawing;
using OpenTK;
using Sledge.Editor.Rendering;
using Sledge.Settings;

namespace Sledge.Editor.Tools.Widgets
{
    public abstract class Widget : BaseTool
    {
        protected MapViewport _activeViewport;

        public delegate void TransformEventHandler(object sender, Matrix4? transformation);
        public event TransformEventHandler Transforming;
        public event TransformEventHandler Transformed;

        protected void OnTransforming(Matrix4? transformation)
        {
            if (Transforming != null) Transforming(this, transformation);
        }

        protected void OnTransformed(Matrix4? transformation)
        {
            if (Transformed != null) Transformed(this, transformation);
        }

        public override Image GetIcon() { return null; }
        public override string GetName() { return "Widget"; }
        public override HotkeyTool? GetHotkeyToolType() { return null; }
        public override string GetContextualHelp() { return ""; }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters) { return HotkeyInterceptResult.Continue; }

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