using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.EditorNew.Documents;
using Sledge.Settings;

namespace Sledge.EditorNew.Tools
{
    public static class ToolManager
    {
        public delegate void ToolEventHandler(object sender, BaseTool tool);

        public static event ToolEventHandler ToolAdded;
        public static event ToolEventHandler ToolRemoved;
        public static event ToolEventHandler ToolSelected;

        private static void OnToolAdded(BaseTool tool)
        {
            var handler = ToolAdded;
            if (handler != null) handler(null, tool);
        }

        private static void OnToolRemoved(BaseTool tool)
        {
            var handler = ToolRemoved;
            if (handler != null) handler(null, tool);
        }

        private static void OnToolSelected(BaseTool tool)
        {
            var handler = ToolSelected;
            if (handler != null) handler(null, tool);
        }

        public static List<BaseTool> Tools { get; private set; }
        public static BaseTool ActiveTool { get; private set; }

        static ToolManager()
        {
            Tools = new List<BaseTool>();
        }

        public static void AddTool(BaseTool tool)
        {
            Tools.Add(tool);
            tool.SetDocument((Document) DocumentManager.CurrentDocument);
            OnToolAdded(tool);
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
            OnToolSelected(ActiveTool);
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
