using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sledge.Editor.UI.Sidebar
{
    public static class SidebarManager
    {
        public static void Init(Control container)
        {
            var tex = CreatePanel("Textures", new TextureSidebarPanel());

            container.Controls.Add(tex);
        }

        public static SidebarPanel CreatePanel(string text, Control contents)
        {
            var panel = new SidebarPanel { Text = text, Dock = DockStyle.Fill};
            contents.Dock = DockStyle.Fill;
            panel.AddControl(contents);
            return panel;
        }
    }
}
