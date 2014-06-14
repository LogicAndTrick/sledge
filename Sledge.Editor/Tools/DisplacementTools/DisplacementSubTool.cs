using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Settings;

namespace Sledge.Editor.Tools.DisplacementTools
{
    public abstract class DisplacementSubTool : BaseTool
    {
        public Control Control { get; set; }

        public override Image GetIcon()
        {
            throw new NotImplementedException();
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return null;
        }

        public override string GetContextualHelp()
        {
            // todo 
            throw new NotImplementedException();
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.OperationsCopy:
                case HotkeysMediator.OperationsCut:
                case HotkeysMediator.OperationsPaste:
                case HotkeysMediator.OperationsPasteSpecial:
                case HotkeysMediator.OperationsDelete:
                    return HotkeyInterceptResult.Abort;
            }
            return HotkeyInterceptResult.Continue;
        }
    }
}
