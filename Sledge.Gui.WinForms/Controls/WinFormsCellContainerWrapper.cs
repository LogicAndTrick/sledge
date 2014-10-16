using System.Windows.Forms;
using Sledge.Gui.Interfaces;

namespace Sledge.Gui.WinForms.Controls
{
    public class WinFormsCellContainerWrapper : WinFormsContainer, ICell
    {
        public WinFormsCellContainerWrapper(Control container) : base(container)
        {
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
    }
}