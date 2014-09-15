using System;
using System.ComponentModel;
using System.Windows.Forms;
using Sledge.Gui.Shell;

namespace Sledge.Gui.WinForms.Shell
{
    public class WinFormsShell : Form, IShell
    {
        private WinFormsMenu _menu;

        public new IMenu Menu
        {
            get { return _menu; }
        }

        public IToolbar Toolbar { get; private set; }

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        public void AddMenu()
        {
            _menu = new WinFormsMenu();
            MainMenuStrip = _menu;
            Controls.Add(_menu);
        }

        public void AddToolbar()
        {
            throw new NotImplementedException();
        }

        public event EventHandler WindowLoaded
        {
            add { Shown += value; }
            remove { Shown -= value; }
        }

        public event EventHandler<HandledEventArgs> WindowClosing;

        public event EventHandler WindowClosed
        {
            add { Closed += value; }
            remove { Closed -= value; }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (WindowClosing != null)
            {
                var hea = new HandledEventArgs();
                WindowClosing(this, hea);
                if (hea.Handled) e.Cancel = true;
            }
            base.OnClosing(e);
        }
    }
}
