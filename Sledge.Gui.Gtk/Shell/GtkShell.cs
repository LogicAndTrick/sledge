using System;
using Gdk;
using Gtk;
using Sledge.Gui.Events;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Shell;
using Size = Sledge.Gui.Interfaces.Size;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace Sledge.Gui.Gtk.Shell
{
    public class GtkShell : Window, IShell
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

        public bool AutoSize { get; set; }

        public ICell Container
        {
            get
            {
                throw new NotImplementedException();
            }
        }

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

        public object BindingSource { get; set; }

        public IControl Implementation { get { return this; } }

        public bool Enabled { get; set; }
        public new bool Focused { get; private set; }
        public Size ActualSize { get; private set; }
        public Size PreferredSize { get; set; }

        public event EventHandler ActualSizeChanged;
        public event EventHandler PreferredSizeChanged;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseWheel;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseClick;
        public event EventHandler MouseDoubleClick;
        public event EventHandler MouseEnter;
        public event EventHandler MouseLeave;

        protected virtual void OnActualSizeChanged()
        {
            if (ActualSizeChanged != null)
            {
                ActualSizeChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnPreferredSizeChanged()
        {
            if (PreferredSizeChanged != null)
            {
                PreferredSizeChanged(this, EventArgs.Empty);
            }
        }

        public void Open()
        {
            Show();
        }

        public void Close()
        {
            Hide();
        }

        public event EventHandler Click
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
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
            _container.Remove(_main);
            _toolbar = new GtkToolbar();
            _container.PackStart(_toolbar, false, false, 0);
            _toolbar.Show();
            _container.PackEnd(_main, true, true, 0);
        }

        public void AddSidebarPanel(IControl panel, SidebarPanelLocation defaultLocation)
        {
            throw new NotImplementedException();
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