using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Editor.Tools.VMTool.Actions;
using Sledge.Settings;

namespace Sledge.Editor.Tools.VMTool.SubTools
{
    public abstract class VMSubTool : BaseTool
    {
        protected readonly VMTool _tool;
        public abstract Control Control { get; }

        protected VMSubTool(VMTool tool)
        {
            _tool = tool;
            UseValidation = true;
            Active = false;
        }

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

        protected IEnumerable<VMPoint> GetVisiblePoints()
        {
            return _tool.GetVisiblePoints();
        }

        protected void PerformAction(VMAction action)
        {
            _tool.PerformAction(action);
        }

        public virtual void StartPointDrag(MapViewport viewport, ViewportEvent e, Coordinate startLocation) { }
        public virtual void PointDrag(MapViewport viewport, ViewportEvent viewportEvent, Coordinate lastPosition, Coordinate position) { }
        public virtual void EndPointDrag(MapViewport viewport, ViewportEvent e, Coordinate endLocation) { }

        public virtual void SelectionChanged() { }

        public abstract IEnumerable<IDraggable> GetDraggables();
        public abstract bool CanDragPoint(VMPoint point);
    }
}