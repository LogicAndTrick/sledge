using System.Collections.Generic;
using System.Drawing;
using Sledge.EditorNew.Properties;
using Sledge.Settings;

namespace Sledge.EditorNew.Tools
{
    public class DummyBoxTool : BaseBoxTool
    {
        public override IEnumerable<string> GetContexts()
        {
            yield return "Dummy Box Tool";
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Camera;
        }

        public override string GetName()
        {
            return "Dummy Box Tool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return null;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            return HotkeyInterceptResult.Continue;
        }

        protected override Color BoxColour
        {
            get { return Color.Red; }
        }

        protected override Color FillColour
        {
            get { return Color.Purple; }
        }
    }
}