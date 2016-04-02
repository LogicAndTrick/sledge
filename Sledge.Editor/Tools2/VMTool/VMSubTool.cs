using System;
using System.Collections.Generic;
using System.Drawing;
using Sledge.Editor.Tools;
using Sledge.Editor.Tools2.DraggableTool;
using Sledge.Settings;

namespace Sledge.Editor.Tools2.VMTool
{
    public abstract class VMSubTool : BaseTool
    {

        public override Image GetIcon()
        {
            throw new NotImplementedException();
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            throw new NotImplementedException();
        }
        
        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            return HotkeyInterceptResult.Continue;
        }

        public abstract IEnumerable<IDraggable> GetDraggables();
        public abstract bool CanDragPoint(VMPoint point);
    }
}