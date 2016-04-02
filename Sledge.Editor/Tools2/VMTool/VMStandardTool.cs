using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools2.DraggableTool;
using Sledge.Editor.Tools2.VMTool.Actions;
using Sledge.Editor.UI;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Editor.Tools2.VMTool
{
    public class VMStandardTool : VMSubTool
    {
        private readonly VMTool _tool;

        public VMStandardTool(VMTool tool)
        {
            _tool = tool;
        }

        public override IEnumerable<IDraggable> GetDraggables()
        {
            return _tool.GetVisiblePoints().OrderBy(x => x.IsSelected ? 1 : 0);
        }

        public override bool CanDragPoint(VMPoint point)
        {
            return true;
        }

        public override string GetName()
        {
            return "Standard";
        }

        public override string GetContextualHelp()
        {
            // todo
            return "";
        }
    }
}
