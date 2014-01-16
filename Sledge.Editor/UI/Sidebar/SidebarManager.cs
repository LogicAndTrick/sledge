using System;
using System.Windows.Forms;
using Sledge.Settings;

namespace Sledge.Editor.UI.Sidebar
{
    public static class SidebarManager
    {
        public static void Init(Control container)
        {
            var scrollPanel = new SidebarContainer {Dock = DockStyle.Fill};

            container.Width = Layout.SidebarWidth;
            container.Resize += (s, e) => Layout.SidebarWidth = container.Width;

            container.Controls.Add(scrollPanel);

            CreatePanel("Textures", new TextureSidebarPanel(), scrollPanel);
            CreatePanel("Visgroups", new VisgroupSidebarPanel(), scrollPanel);
            CreatePanel("Entities", new EntitySidebarPanel(), scrollPanel);
            CreatePanel("Brushes", new BrushSidebarPanel(), scrollPanel);
        }

        private static void ResizeTable(TableLayoutPanel panel)
        {
            if (panel.HorizontalScroll.Visible)
            {
                // resize the panel to remove the horizontal scrollbar
                panel.Padding = new Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 0);
                panel.Padding = new Padding(0, 0, 0, 0);
            }
        }

        public static SidebarPanel CreatePanel(string text, Control contents, SidebarContainer container)
        {
            var panel = new SidebarPanel { Text = text, Dock = DockStyle.Fill };
            panel.AddControl(contents);

            container.Add(panel);

            return panel;
        }
    }
}
