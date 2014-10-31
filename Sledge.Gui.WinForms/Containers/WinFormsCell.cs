using System.Windows.Forms;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Containers;

namespace Sledge.Gui.WinForms.Containers
{
    public class WinFormsCell : WinFormsContainer, ICell
    {
        public WinFormsCell() : base(new Panel())
        {
            
        }

        internal WinFormsCell(Control container) : base(container)
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