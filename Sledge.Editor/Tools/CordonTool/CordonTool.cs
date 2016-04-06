using System;
using System.Drawing;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Properties;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Settings;

namespace Sledge.Editor.Tools.CordonTool
{
    public class CordonTool : BaseDraggableTool
    {
        private readonly CordonBoxDraggableState _cordonBox;
        private bool _showing;

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
            _showing = Document.Map.Cordon;
            Document.Map.Cordon = false;
            Document.RenderObjects(new[] { Document.Map.WorldSpawn });

            _cordonBox.Update();
            base.ToolSelected(preventHistory);
        }

        public override void ToolDeselected(bool preventHistory)
        {
            Document.Map.Cordon = _showing;
            Document.RenderObjects(new[] { Document.Map.WorldSpawn });
            Mediator.Publish(EditorMediator.UpdateToolstrip);

            base.ToolDeselected(preventHistory);
        }

        private void CordonBoxChanged(object sender, EventArgs e)
        {
            if (_cordonBox.State.Action == BoxAction.Drawn)
            {
                Document.Map.CordonBounds = new Box(_cordonBox.State.Start, _cordonBox.State.End);
                Document.RenderObjects(new [] { Document.Map.WorldSpawn });
            }
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.OperationsPasteSpecial:
                case HotkeysMediator.OperationsPaste:
                    return HotkeyInterceptResult.SwitchToSelectTool;
                case HotkeysMediator.ToggleCordon:
                    return HotkeyInterceptResult.Abort;
            }
            return HotkeyInterceptResult.Continue;
        }
    }
}
