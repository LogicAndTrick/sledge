using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Common.Mediator;
using Sledge.Editor.Documents;
using Sledge.Editor.Tools.TransformationTools;
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
            Tools.Add(new EntityTool());
            Tools.Add(new BrushTool());
            Tools.Add(new TextureTool());
            Tools.Add(new DecalTool());
            Tools.Add(new DisplacementTool());
            Tools.Add(new ClipTool());
            Tools.Add(new VMTool());
        }

        public static void Deactivate()
        {
            if (ActiveTool != null)
            {
                ActiveTool.ToolDeselected();
                Mediator.UnsubscribeAll(ActiveTool);
            }
            ActiveTool = null;
        }

        public static void SetDocument(Document doc)
        {
            var active = ActiveTool;
            Deactivate();
            Tools.ForEach(x => x.SetDocument(doc));
            Activate(active);
        }

        public static void Activate(BaseTool tool)
        {
            if (tool == ActiveTool) return;
            if (ActiveTool != null) Deactivate();
            ActiveTool = tool;
            if (ActiveTool != null) ActiveTool.ToolSelected();
        }
    }
}
