using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.Editor.Documents;
using Sledge.Settings;

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
            Tools.Add(new SelectTool.SelectTool());
            Tools.Add(new CameraTool());
            Tools.Add(new EntityTool());
            Tools.Add(new BrushTool());
            Tools.Add(new TextureTool.TextureTool());
            Tools.Add(new DecalTool());
            //Tools.Add(new DisplacementTool());
            Tools.Add(new ClipTool());
            Tools.Add(new VMTool.VMTool());
            Tools.Add(new CordonTool());
            //Tools.Add(new SketchTool());
        }

        public static void Deactivate(bool preventHistory = false)
        {
            if (ActiveTool != null)
            {
                ActiveTool.ToolDeselected(preventHistory);
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

        public static void Activate(BaseTool tool, bool preventHistory = false)
        {
            if (tool == ActiveTool) return;
            if (DocumentManager.CurrentDocument == null) return;
            if (ActiveTool != null) Deactivate(preventHistory);
            ActiveTool = tool;
            if (ActiveTool != null) ActiveTool.ToolSelected(preventHistory);
            Mediator.Publish(EditorMediator.ToolSelected);
        }

        public static void Activate(Type toolType, bool preventHistory = false)
        {
            Activate(Tools.FirstOrDefault(x => x.GetType() == toolType), preventHistory);
        }

        public static void Activate(HotkeyTool hotkeyTool, bool preventHistory = false)
        {
            var hk = Tools.FirstOrDefault(x => x.GetHotkeyToolType() == hotkeyTool);
            if (hk != null) Activate(hk, preventHistory);
        }
    }
}
