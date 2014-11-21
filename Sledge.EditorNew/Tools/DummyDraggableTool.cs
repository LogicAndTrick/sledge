using System.Collections.Generic;
using System.Drawing;
using Sledge.EditorNew.Properties;
using Sledge.EditorNew.Tools.DraggableTool;
using Sledge.Settings;

namespace Sledge.EditorNew.Tools
{
    public class DummyDraggableTool : BaseDraggableTool
    {
        public DummyDraggableTool()
        {
            var box = new BoxDraggableState(this);
            box.BoxColour = Color.Red;
            box.FillColour = Color.FromArgb(64, Color.Purple);
            States.Add(box);
        }

        public override IEnumerable<string> GetContexts()
        {
            yield return "Dummy Draggable Tool";
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Clip;
        }

        public override string GetName()
        {
            return "Dummy Draggable Tool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return null;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            return HotkeyInterceptResult.Continue;
        }
    }
}