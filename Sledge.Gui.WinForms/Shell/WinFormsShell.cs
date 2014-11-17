using System;
using System.Windows.Forms;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Interfaces.Shell;
using Sledge.Gui.WinForms.Containers;
using WeifenLuo.WinFormsUI.Docking;

namespace Sledge.Gui.WinForms.Shell
{
    public class WinFormsShell : WinFormsWindow, IShell
    {
        private ToolStripContainer _container;
        private WinFormsMenu _menu;
        private WinFormsToolbar _toolbar;
        private DockPanel _dockPanel;

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

            _dockPanel = new DockPanel
            {
                Dock = DockStyle.Fill,
                DocumentStyle = DocumentStyle.DockingSdi,
                DockLeftPortion = 10,
                DockRightPortion = 10,
                DockBottomPortion = 10
            };
            _container.ContentPanel.Controls.Add(_dockPanel);

            var documentContainer = new DockContent{AllowEndUserDocking = false, TabText = "Viewports", DockAreas = DockAreas.Document};
            documentContainer.Show(_dockPanel, DockState.Document);

            ContainerWrapper = new WinFormsCell(documentContainer);
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

        public IDockPanel AddDockPanel(IControl panel, DockPanelLocation defaultLocation)
        {
            var dock = DockState.DockLeft;
            var ps = panel.PreferredSize;

            switch (defaultLocation)
            {
                case DockPanelLocation.Left:
                    dock = DockState.DockLeft;
                    if (_dockPanel.DockLeftPortion < ps.Width + 10) _dockPanel.DockLeftPortion = ps.Width + 10;
                    break;
                case DockPanelLocation.Right:
                    dock = DockState.DockRight;
                    if (_dockPanel.DockRightPortion < ps.Width + 10) _dockPanel.DockRightPortion = ps.Width + 10;
                    break;
                case DockPanelLocation.Bottom:
                    dock = DockState.DockBottom;
                    if (_dockPanel.DockBottomPortion < ps.Height + 10) _dockPanel.DockBottomPortion = ps.Height + 10;
                    break;
            }

            var dc = new DockContent
            {
                DockAreas = DockAreas.DockBottom | DockAreas.DockRight | DockAreas.DockLeft | DockAreas.Float
            };
            var dp = new WinFormsDockPanel(dc);
            dp.Set(panel);
            dc.Show(_dockPanel, dock);

            return dp;
        }
    }
}
