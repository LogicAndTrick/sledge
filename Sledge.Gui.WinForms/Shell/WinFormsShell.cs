using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Interfaces.Shell;
using Sledge.Gui.WinForms.Containers;
using Sledge.Gui.WinForms.Controls;
using IContainer = Sledge.Gui.Interfaces.IContainer;

namespace Sledge.Gui.WinForms.Shell
{
    public class WinFormsShell : WinFormsWindow, IShell
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

        public new ICell Container
        {
            get { return ContainerWrapper; }
        }

        protected override void CreateWrapper()
        {
            _container = new ToolStripContainer { Dock = DockStyle.Fill };
            Form.Controls.Add(_container);
            var dockFill = new WinFormsDockedPanel { Dock = DockStyle.Fill };
            _container.ContentPanel.Controls.Add(dockFill);
            ContainerWrapper = new WinFormsCell(dockFill);
            ContainerWrapper.PreferredSizeChanged += ContainerPreferredSizeChanged;
        }

        private void ContainerPreferredSizeChanged(object sender, EventArgs e)
        {
            OnPreferredSizeChanged();
        }

        public void AddMenu()
        {
            _menu = new WinFormsMenu();
            Form.MainMenuStrip = _menu;
            Form.Controls.Add(_menu);
        }

        public void AddToolbar()
        {
            _toolbar = new WinFormsToolbar();
            _container.TopToolStripPanel.Controls.Add(_toolbar);
        }

        public IVerticalBox _leftPanel;
        public IVerticalBox _rightPanel;

        public void AddSidebarPanel(IControl panel, SidebarPanelLocation defaultLocation)
        {
            switch (defaultLocation)
            {
                case SidebarPanelLocation.Left:
                    if (_leftPanel == null)
                    {
                        var lp = new WinFormsDockedPanel { Dock = DockStyle.Left };
                        lp.DockDimension = panel.PreferredSize.Width;
                        var cont = new WinFormsVerticalScrollContainer();
                        cont.Set(_leftPanel = new WinFormsVerticalBox());
                        cont.Control.Dock = DockStyle.Fill;
                        lp.Controls.Add(cont.Control);
                        _container.ContentPanel.Controls.Add(lp);
                    }
                    _leftPanel.Add(panel);
                    break;
                case SidebarPanelLocation.Right:
                    if (_rightPanel == null)
                    {
                        var rp = new WinFormsDockedPanel { Dock = DockStyle.Right };
                        rp.DockDimension = panel.PreferredSize.Width;
                        var cont = new WinFormsVerticalScrollContainer();
                        cont.Set(_rightPanel = new WinFormsVerticalBox());
                        cont.Control.Dock = DockStyle.Fill;
                        rp.Controls.Add(cont.Control);
                        _container.ContentPanel.Controls.Add(rp);
                    }
                    _rightPanel.Add(panel);
                    break;
                case SidebarPanelLocation.Bottom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
