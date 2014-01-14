using System;
using System.Windows.Forms;

namespace Sledge.Editor.UI.Sidebar
{
    public static class SidebarManager
    {
        public static void Init(Control container)
        {
            var table = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoScroll = true };
            
            table.RowStyles.Clear();
            table.Resize += (s,e) => ResizeTable(table);
            table.ControlAdded += (s, e) => ResizeTable(table);
            table.Layout += (s, e) => ResizeTable(table);

            container.Controls.Add(table);

            CreatePanel("Textures", new TextureSidebarPanel(), table);
            CreatePanel("Visgroups", new VisgroupSidebarPanel(), table);
            CreatePanel("Entities", new EntitySidebarPanel(), table);
            CreatePanel("Brushes", new BrushSidebarPanel(), table);
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

        public static SidebarPanel CreatePanel(string text, Control contents, TableLayoutPanel table)
        {
            var panel = new SidebarPanel { Text = text, Dock = DockStyle.Fill };
            panel.AddControl(contents);

            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.Controls.Add(panel);

            return panel;
        }
    }
}
