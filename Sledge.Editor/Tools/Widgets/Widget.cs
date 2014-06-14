using System;
using System.Drawing;
using OpenTK;
using Sledge.Settings;
using Sledge.UI;

namespace Sledge.Editor.Tools.Widgets
{
    public abstract class Widget : BaseTool
    {
        protected ViewportBase _activeViewport;

        private Action<Matrix4?> _transformedCallback = null;
        private Action<Matrix4?> _transformingCallback = null;

        public Action<Matrix4?> OnTransformed
        {
            get
            {
                return _transformedCallback ?? (x => { });
            }
            set
            {
                _transformedCallback = value;
            }
        }

        public Action<Matrix4?> OnTransforming
        {
            get
            {
                return _transformingCallback ?? (x => { });
            }
            set
            {
                _transformingCallback = value;
            }
        }

        public override Image GetIcon() { return null; }
        public override string GetName() { return "Widget"; }
        public override HotkeyTool? GetHotkeyToolType() { return null; }
        public override string GetContextualHelp() { return ""; }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters) { return HotkeyInterceptResult.Continue; }
        public override void KeyUp(ViewportBase viewport, ViewportEvent e) { }
        public override void KeyDown(ViewportBase viewport, ViewportEvent e) { }
        public override void KeyPress(ViewportBase viewport, ViewportEvent e) { }
        public override void MouseClick(ViewportBase viewport, ViewportEvent e) { }
        public override void MouseDoubleClick(ViewportBase viewport, ViewportEvent e) { }
        public override void UpdateFrame(ViewportBase viewport, FrameInfo frame) { }

        public override void MouseEnter(ViewportBase viewport, ViewportEvent e)
        {
            _activeViewport = viewport;
        }

        public override void MouseLeave(ViewportBase viewport, ViewportEvent e)
        {
            _activeViewport = null;
        }

        public abstract void SelectionChanged();
    }
}