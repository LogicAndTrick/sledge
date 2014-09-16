using System;
using System.ComponentModel;
using System.Windows.Forms;
using Sledge.Gui.Shell;

namespace Sledge.Gui.WinForms.Shell
{
    public class WinFormsShell : Form, IShell
    {
        private ToolStripContainer _container;
        private WinFormsMenu _menu;
        private WinFormsToolbar _toolbar;

        public new IMenu Menu
        {
            get { return _menu; }
        }

        public IToolbar Toolbar
        {
            get { return _toolbar; }
        }

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        public WinFormsShell()
        {
            _container = new ToolStripContainer { Dock = DockStyle.Fill };
            Controls.Add(_container);
        }

        public void AddMenu()
        {
            _menu = new WinFormsMenu();
            MainMenuStrip = _menu;
            Controls.Add(_menu);
        }

        public void AddToolbar()
        {
            _toolbar = new WinFormsToolbar();
            _container.TopToolStripPanel.Controls.Add(_toolbar);
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
