using System;
using System.Linq;
using Sledge.EditorNew.Tools;
using Sledge.EditorNew.Tools.BrushTool;
using Sledge.EditorNew.Tools.SelectTool;
using Sledge.Gui;
using Sledge.Gui.Containers;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Interfaces.Shell;
using Sledge.Gui.Structures;

namespace Sledge.EditorNew.Bootstrap
{
    public static class ToolBootstrapper
    {
        private static VerticalBox _toolPanel;
        private static IDockPanel _toolDockPanel;

        public static void Bootstrap()
        {
            _toolPanel = new VerticalBox {Margin = new Padding(3, 3, 3, 3)};
            var btn = new Button {PreferredSize = new Size(40, 40)};
            _toolPanel.Add(btn);
            _toolDockPanel = UIManager.Manager.Shell.AddDockPanel(_toolPanel, DockPanelLocation.Left);
            _toolDockPanel.TextKey = "Shell/DockPanels/Tools/Title";
            _toolPanel.Remove(btn);

            ToolManager.ToolAdded += ToolAdded;
            ToolManager.ToolRemoved += ToolRemoved;

            ToolManager.AddTool(new SelectTool());
            ToolManager.AddTool(new BrushTool());
            ToolManager.AddTool(new DummyDraggableTool());

            /*
             
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
             * 
             */
        }

        private static void ToolRemoved(object sender, BaseTool tool)
        {
            var button = _toolPanel.Children.FirstOrDefault(x => x.Tag == tool);
            if (button != null) _toolPanel.Remove(button);
        }

        private static void ToolAdded(object sender, BaseTool tool)
        {
            var button = new Button
            {
                PreferredSize = new Size(40, 40),
                Image = tool.GetIcon(),
                Tag = tool
            };
            _toolPanel.Add(button);
            button.Clicked += (s, args) => ToolManager.Activate(tool);
        }
    }
}