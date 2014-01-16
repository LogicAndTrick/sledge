using System;
using System.Windows.Forms;

namespace Sledge.Editor.UI.Sidebar
{
    public static class SidebarManager
    {
        public static void Init(Control container)
        {
            /*var table = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
            
            table.RowStyles.Clear();
            table.Resize += (s,e) => ResizeTable(table);
            table.ControlAdded += (s, e) => ResizeTable(table);
            table.Layout += (s, e) => ResizeTable(table);*/

            var scrollPanel = new SidebarContainer {Dock = DockStyle.Fill};

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
