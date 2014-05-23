using System;
using System.Linq;
using System.Windows.Forms;
using Sledge.Settings;

namespace Sledge.Editor.UI.Sidebar
{
    public static class SidebarManager
    {
        private static SidebarContainer _container;

        public static void Init(Control container)
        {
            _container = new SidebarContainer { Dock = DockStyle.Fill };

            container.Width = Sledge.Settings.Layout.SidebarWidth;
            container.Resize += (s, e) => Sledge.Settings.Layout.SidebarWidth = container.Width;

            container.Controls.Add(_container);

            CreatePanel("Textures", new TextureSidebarPanel());
            CreatePanel("Visgroups", new VisgroupSidebarPanel());
            CreatePanel("Entities", new EntitySidebarPanel());
            CreatePanel("Brushes", new BrushSidebarPanel());
        }

        private static void CreatePanel(string text, Control contents)
        {
            var panel = new SidebarPanel { Text = text, Dock = DockStyle.Fill, Hidden = !Expanded(text) };
            panel.AddControl(contents);
            _container.Add(panel);
        }

        public static void SaveLayout()
        {
            if (_container == null) return;
            var serialised = String.Join(";", _container.GetControls()
                .OfType<SidebarPanel>()
                .Select(x => x.Text + ":" + (x.Hidden ? '0' : '1')));
            Sledge.Settings.Layout.SidebarLayout = serialised;
        }

        private static bool Expanded(string text)
        {
            return Sledge.Settings.Layout.SidebarLayout.Split(';')
                .Select(x => x.Split(':'))
                .Where(x => x.Length == 2 && x[0] == text)
                .Select(x => x[1])
                .FirstOrDefault() != "0";
        }
    }
}
