using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.EditorNew.Language;
using Sledge.EditorNew.Tools;
using Sledge.Gui;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.EditorNew.Bootstrap
{
    public static class SidebarBootstrapper
    {
        private static List<IDockPanel> _toolPanels;
        private static IDockPanel _helpPanel;
        private static Label _helpLabel;

        public static void Bootstrap()
        {
            _toolPanels = new List<IDockPanel>();

            _helpLabel = new Label();
            _helpPanel = UIManager.Manager.Shell.AddDockPanel(_helpLabel, DockPanelLocation.Right);
            _helpPanel.TextKey = "Shell/DockPanels/ContextualHelp/Title";

            ToolManager.ToolSelected += ToolSelected;
        }

        private static void ToolSelected(object sender, BaseTool tool)
        {
            foreach (var child in _toolPanels)
            {
                foreach (var control in child.Children.ToList())
                {
                    child.Remove(control);
                }
                child.Dispose();
            }
            _helpLabel.TextKey = null;
            if (tool != null)
            {
                var controls = tool.GetSidebarControls();
                foreach (var tsc in controls)
                {
                    var dp = UIManager.Manager.Shell.AddDockPanel(tsc.Control, DockPanelLocation.Right);
                    dp.TextKey = tsc.TextKey;
                    _toolPanels.Add(dp);
                }
                _helpLabel.TextKey = tool.GetContextualHelpTextKey();
            }
        }
    }
}