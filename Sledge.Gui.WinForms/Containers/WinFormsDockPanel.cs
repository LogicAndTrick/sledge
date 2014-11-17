using System.Windows.Forms;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;
using WeifenLuo.WinFormsUI.Docking;

namespace Sledge.Gui.WinForms.Containers
{
    public class WinFormsDockPanel : WinFormsContainer, IDockPanel
    {
        private string _textKey;
        private readonly DockContent _dockContent;

        internal WinFormsDockPanel(DockContent container) : base(container)
        {
            _dockContent = container;
            _dockContent.HideOnClose = true;
        }

        protected override void CalculateLayout()
        {
            Control.SuspendLayout();

            foreach (var winFormsControl in Children)
            {
                winFormsControl.Control.Dock = DockStyle.Fill;
            }

            Control.ResumeLayout();
        }

        public void Set(IControl child)
        {
            Insert(0, child);
        }

        public override string TextKey
        {
            get { return _textKey; }
            set
            {
                _textKey = value;
                _dockContent.TabText = UIManager.Manager.StringProvider.Fetch(_textKey);
            }
        }

        public override string Text
        {
            get { return _dockContent.TabText; }
            set
            {
                _dockContent.TabText = value;
                _textKey = null;
            }
        }

        public override void Dispose()
        {
            _dockContent.Hide();
            base.Dispose();
        }
    }
}