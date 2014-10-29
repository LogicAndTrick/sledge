using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Gtk.Shell;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Shell;
using Sledge.Gui.Resources;

namespace Sledge.Gui.Gtk
{
    [UIImplementation("GTK", 20, UIPlatform.Windows, UIPlatform.Linux)]
    public class GtkUIManager : IUIManager
    {
        private readonly GtkShell _shell;

        public IShell Shell
        {
            get { return _shell; }
        }

        public IStringProvider StringProvider { get; set; }

        public GtkUIManager()
        {
            Application.Init();
            _shell = new GtkShell();
        }

        public void Start()
        {
            _shell.Show();
            Application.Run();
        }

        public IWindow CreateWindow()
        {
            throw new System.NotImplementedException();
        }

        public T Construct<T>() where T : IControl
        {
            throw new System.NotImplementedException();
        }
    }
}
