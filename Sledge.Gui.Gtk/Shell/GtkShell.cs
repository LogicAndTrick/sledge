using System;
using Gdk;
using Gtk;
using Sledge.Gui.Shell;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace Sledge.Gui.Gtk.Shell
{
    public class GtkShell : Window, IShell
    {
        private GtkMenu _menu;

        public IMenu Menu
        {
            get { return _menu; }
        }

        public IToolbar Toolbar { get; private set; }

        private VBox _container;
        private Widget _main;

        public GtkShell() : base((WindowType) WindowType.Toplevel)
        {
            Title = "";
            _container = new VBox { Spacing = 1 };
            Add(_container);
            _main = new VBox();
            _container.PackEnd(_main);
            _container.ShowAll();
        }

        public event EventHandler WindowLoaded;

        public event EventHandler<HandledEventArgs> WindowClosing;

        public event EventHandler WindowClosed;

        public void AddMenu()
        {
            _container.Remove(_main);
            _menu = new GtkMenu();
            _container.PackStart(_menu, false, false, 0);
            _menu.Show();
            _container.PackEnd(_main, true, true, 0);
        }

        public void AddToolbar()
        {
            throw new NotImplementedException();
            _container.Remove(_main);
            //
            _container.PackEnd(_main, true, true, 0);
        }

        protected override bool OnDeleteEvent(Event evnt)
        {
            if (WindowClosing != null)
            {
                var hea = new HandledEventArgs();
                WindowClosing(this, hea);
                if (hea.Handled) return true;
            }
            Application.Quit();
            if (WindowClosed != null)
            {
                WindowClosed(this, EventArgs.Empty);
            }
            return base.OnDeleteEvent(evnt);
        }

        protected override void OnRealized()
        {
            if (WindowLoaded != null)
            {
                WindowLoaded(this, EventArgs.Empty);
            }
            base.OnRealized();
        }
    }
}