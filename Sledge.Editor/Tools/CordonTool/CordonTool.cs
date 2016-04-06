using System;
using System.Drawing;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Properties;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Settings;

namespace Sledge.Editor.Tools.CordonTool
{
    public class CordonTool : BaseDraggableTool
    {
        private readonly CordonBoxDraggableState _cordonBox;

        public CordonTool()
        {
            _cordonBox = new CordonBoxDraggableState(this);
            _cordonBox.BoxColour = Color.Red;
            _cordonBox.FillColour = Color.FromArgb(View.SelectionBoxBackgroundOpacity, Color.LightGray);
            _cordonBox.State.Changed += CordonBoxChanged;
            States.Add(_cordonBox);

            Usage = ToolUsage.View2D;
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Cordon;
        }

        public override string GetName()
        {
            return "CordonTool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Cordon;
        }

        public override string GetContextualHelp()
        {
            return "Manipulate the box to define the cordon bounds for the map.";
        }

        public override void ToolSelected(bool preventHistory)
        {
            _cordonBox.Update();
            base.ToolSelected(preventHistory);
        }

        private void CordonBoxChanged(object sender, EventArgs e)
        {
            if (_cordonBox.State.Action == BoxAction.Drawn)
            {
                Document.Map.CordonBounds = new Box(_cordonBox.State.Start, _cordonBox.State.End);
            }
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.OperationsPasteSpecial:
                case HotkeysMediator.OperationsPaste:
                    return HotkeyInterceptResult.SwitchToSelectTool;
            }
            return HotkeyInterceptResult.Continue;
        }
    }
}
