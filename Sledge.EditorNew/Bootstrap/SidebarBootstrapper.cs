using System.Collections.Generic;
using System.Linq;
using Sledge.EditorNew.Tools;
using Sledge.Gui;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.EditorNew.Bootstrap
{
    public static class SidebarBootstrapper
    {
        private static List<IDockPanel> _dockPanels;

        public static void Bootstrap()
        {
            _dockPanels = new List<IDockPanel>();
            ToolManager.ToolSelected += ToolSelected;
        }

        private static void ToolSelected(object sender, BaseTool tool)
        {
            foreach (var child in _dockPanels)
            {
                foreach (var control in child.Children.ToList())
                {
                    child.Remove(control);
                }
                child.Dispose();
            }
            if (tool != null)
            {
                var controls = tool.GetSidebarControls();
                foreach (var tsc in controls)
                {
                    var dp = UIManager.Manager.Shell.AddDockPanel(tsc.Control, DockPanelLocation.Right);
                    dp.TextKey = tsc.TextKey;
                    _dockPanels.Add(dp);
                }
            }
        }
    }
}