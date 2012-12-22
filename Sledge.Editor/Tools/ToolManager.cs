using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Editor.UI;
using Sledge.UI;

namespace Sledge.Editor.Tools
{
    public static class ToolManager
    {
        public static List<BaseTool> Tools { get; private set; }
        public static BaseTool ActiveTool { get; private set; }

        static ToolManager()
        {
            Tools = new List<BaseTool>();
        }

        public static void Init()
        {
            Tools.Add(new SelectTool());
            Tools.Add(new ResizeTool());
            Tools.Add(new RotateTool());
            Tools.Add(new SkewTool());
            Tools.Add(new BrushTool());
            Tools.Add(new TextureTool());
            Tools.Add(new DisplacementTool());
            Tools.Add(new ClipTool());

            Activate(Tools[0]);
        }

        public static void Activate(BaseTool tool)
        {
            if (tool == ActiveTool) return;
            if (ActiveTool != null) ActiveTool.ToolDeselected();
            ActiveTool = tool;
            if (ActiveTool != null) ActiveTool.ToolSelected();
        }
    }
}
