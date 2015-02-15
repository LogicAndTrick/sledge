using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Rendering;
using Sledge.Settings;

namespace Sledge.Editor.Tools.VMTool
{
    public abstract class VMSubTool : BaseTool
    {
        public Control Control { get; set; }
        public VMTool MainTool { get; set; }

        protected VMSubTool(VMTool mainTool)
        {
            MainTool = mainTool;
        }

        public override Image GetIcon()
        {
            throw new NotImplementedException();
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return null;
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

        public abstract List<VMPoint> GetVerticesAtPoint(int x, int y, MapViewport viewport);
        public abstract void DragStart(List<VMPoint> clickedPoints);
        public abstract void DragMove(Coordinate distance);
        public abstract void DragEnd();

        public abstract void Render2D(MapViewport viewport);
        public abstract void Render3D(MapViewport viewport);
        public abstract void SelectionChanged();
        public abstract bool ShouldDeselect(List<VMPoint> vtxs);
        public abstract bool NoSelection();
        public abstract bool No3DSelection();
        public abstract bool DrawVertices();
    }
}
