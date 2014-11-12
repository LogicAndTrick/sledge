using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Sledge.EditorNew.Properties;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Gui.Structures;
using Sledge.Settings;

namespace Sledge.EditorNew.Tools
{
    public class DummyTool : BaseTool
    {
        public override IEnumerable<string> GetContexts()
        {
            yield return "DummyTool";
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Select;
        }

        public override string GetName()
        {
            return "Dummy";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return null;
        }

        public override void MouseEnter(IMapViewport viewport, ViewportEvent e)
        {
        }

        public override void MouseLeave(IMapViewport viewport, ViewportEvent e)
        {
        }

        public override void MouseDown(IMapViewport viewport, ViewportEvent e)
        {
        }

        public override void MouseClick(IMapViewport viewport, ViewportEvent e)
        {
        }

        public override void MouseDoubleClick(IMapViewport viewport, ViewportEvent e)
        {
        }

        public override void MouseUp(IMapViewport viewport, ViewportEvent e)
        {
        }

        public override void MouseWheel(IMapViewport viewport, ViewportEvent e)
        {
        }

        public override void MouseMove(IMapViewport viewport, ViewportEvent e)
        {
        }

        public override void KeyPress(IMapViewport viewport, ViewportEvent e)
        {
        }

        public override void KeyDown(IMapViewport viewport, ViewportEvent e)
        {
        }

        public override void KeyUp(IMapViewport viewport, ViewportEvent e)
        {

        }

        public override void UpdateFrame(IMapViewport viewport, Frame frame)
        {

        }

        public override void Render(IMapViewport viewport)
        {

        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            return HotkeyInterceptResult.Continue;
        }
    }
}
