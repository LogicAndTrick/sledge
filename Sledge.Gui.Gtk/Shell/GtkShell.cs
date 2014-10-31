using System;
using Gtk;
using Sledge.Gui.Gtk.Containers;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.Gui.Gtk.Shell
{
    public class GtkShell : GtkWindow, IShell
    {
        private GtkMenu _menu;
        private GtkToolbar _toolbar;

        public IMenu Menu
        {
            get { return _menu; }
        }

        public IToolbar Toolbar
        {
            get { return _toolbar; }
        }

        private readonly VBox _container;

        public GtkShell()
        {
            Title = "";
            _container = new VBox { Spacing = 1 };
            Window.Add(_container);
            Window.SetSizeRequest(800, 600);
            _container.PackEnd(ContainerWrapper.Control);
            _container.ShowAll();

            WindowClosed += (sender, args) => Application.Quit();
        }

        protected override void CreateWrapper()
        {
            ContainerWrapper = new GtkCell();
            ContainerWrapper.PreferredSizeChanged += ContainerPreferredSizeChanged;
        }

        private void ContainerPreferredSizeChanged(object sender, EventArgs e)
        {
            OnPreferredSizeChanged();
        }

        public void AddMenu()
        {
            _container.Remove(ContainerWrapper.Control);
            _menu = new GtkMenu();
            _container.PackStart(_menu, false, false, 0);
            _menu.Show();
            _container.PackEnd(ContainerWrapper.Control, true, true, 0);
        }

        public void AddToolbar()
        {
            _container.Remove(ContainerWrapper.Control);
            _toolbar = new GtkToolbar();
            _container.PackStart(_toolbar, false, false, 0);
            _toolbar.Show();
            _container.PackEnd(ContainerWrapper.Control, true, true, 0);
        }

        public void AddSidebarPanel(IControl panel, SidebarPanelLocation defaultLocation)
        {
            throw new NotImplementedException();
        }
    }
}