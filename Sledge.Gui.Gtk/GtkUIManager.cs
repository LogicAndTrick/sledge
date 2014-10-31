using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Gtk.Shell;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Dialogs;
using Sledge.Gui.Interfaces.Shell;
using Sledge.Gui.Resources;

namespace Sledge.Gui.Gtk
{
    [UIImplementation("GTK", 20, UIPlatform.Windows, UIPlatform.Linux)]
    public class GtkUIManager : IUIManager
    {
        private readonly GtkShell _shell;
        private IStringProvider _stringProvider = new PassthroughStringProvider();

        public IShell Shell
        {
            get { return _shell; }
        }

        public IStringProvider StringProvider
        {
            get { return _stringProvider; }
            set { _stringProvider = value ?? new PassthroughStringProvider(); }
        }

        public GtkUIManager()
        {
            Application.Init();
            _shell = new GtkShell();
            CacheImplementations();
        }

        private Dictionary<Type, Type> _implementations;

        private void CacheImplementations()
        {
            _implementations = new Dictionary<Type, Type>();
            foreach (var type in typeof(GtkUIManager).Assembly.GetTypes())
            {
                var attrs = type.GetCustomAttributes(typeof(ControlImplementationAttribute), false).OfType<ControlImplementationAttribute>().ToList();
                if (!attrs.Any()) continue;
                var interfaces = type.GetInterfaces();
                var direct = interfaces.Where(x => !interfaces.Any(y => y.GetInterfaces().Contains(x)));
                foreach (var iface in direct)
                {
                    if (!iface.GetCustomAttributes(typeof(ControlInterfaceAttribute), false).Any()) continue;
                    _implementations.Add(iface, type);
                }
            }
        }

        public void Start()
        {
            _shell.Open();
            Application.Run();
        }

        public IWindow CreateWindow()
        {
            return new GtkWindow();
        }

        public T Construct<T>() where T : IControl
        {
            return (T)Activator.CreateInstance(_implementations[typeof(T)]);
        }

        public T ConstructDialog<T>() where T : IDialog
        {
            return (T)Activator.CreateInstance(_implementations[typeof(T)]);
        }
    }
}
