using System.Collections.Generic;
using System.Drawing;
using Sledge.EditorNew.Properties;
using Sledge.EditorNew.Tools.DraggableTool;
using Sledge.Settings;

namespace Sledge.EditorNew.Tools.SelectTool
{
    public class SelectTool : BaseDraggableTool
    {
        public SelectTool()
        {
            States = new Stack<IDraggableState>();

            var box = new BoxDraggableState(this);
            box.BoxColour = Color.Yellow;
            box.FillColour = Color.FromArgb(64, Color.White);
            States.Push(box);
        }

        public override IEnumerable<string> GetContexts()
        {
            yield return "Select Tool";
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Select;
        }

        public override string GetName()
        {
            return "Select Tool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Selection;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            return HotkeyInterceptResult.Continue;
        }
    }
}