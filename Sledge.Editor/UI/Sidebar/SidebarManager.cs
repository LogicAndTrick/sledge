using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Tools;
using Sledge.Settings;

namespace Sledge.Editor.UI.Sidebar
{
    public class SidebarManager : IMediatorListener
    {
        private static SidebarManager _instance;

        private SidebarManager()
        {
            Mediator.Subscribe(EditorMediator.ToolSelected, this);
        }

        private static SidebarContainer _container;
        private static SidebarPanel _toolPanel;

        public static void Init(Control container)
        {
            _instance = new SidebarManager();

            _container = new SidebarContainer { Dock = DockStyle.Fill };

            container.Width = Sledge.Settings.Layout.SidebarWidth;
            container.Resize += (s, e) => Sledge.Settings.Layout.SidebarWidth = container.Width;

            container.Controls.Add(_container);

            CreatePanel("Textures", new TextureSidebarPanel());
            CreatePanel("Visgroups", new VisgroupSidebarPanel());
            CreatePanel("Entities", new EntitySidebarPanel());
            CreatePanel("Brushes", new BrushSidebarPanel());
        }

        private static SidebarPanel CreatePanel(string text, Control contents, bool insert = false)
        {
            var panel = new SidebarPanel { Text = text, Dock = DockStyle.Fill, Hidden = !Expanded(text) };
            panel.AddControl(contents);
            if (insert) _container.Insert(panel, 0);
            else _container.Add(panel);
            return panel;
        }

        private static void RemovePanel(SidebarPanel panel)
        {
            _container.Remove(panel);
            panel.Controls.Clear();
            panel.Dispose();
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

        public static void ToolSelected()
        {
            if (_toolPanel != null) RemovePanel(_toolPanel);
            _toolPanel = null;
            if (ToolManager.ActiveTool == null) return;

            var control = ToolManager.ActiveTool.GetSidebarControl();
            if (control == null) return;

            _toolPanel = CreatePanel(ToolManager.ActiveTool.GetName(), control, true);
        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }
    }
}
