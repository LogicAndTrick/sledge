using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Controls;
using Size = Sledge.Gui.Structures.Size;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsLabel : WinFormsControl, ILabel
    {
        private Size _preferredSize = new Size(50, 20);

        public WinFormsLabel()
            : base(new Label{TextAlign = ContentAlignment.MiddleLeft})
        {
            Control.TextChanged += UpdatePreferredSize;
            Control.FontChanged += UpdatePreferredSize;
        }

        private void UpdatePreferredSize(object sender, EventArgs e)
        {
            var measure = TextRenderer.MeasureText(String.IsNullOrEmpty(Control.Text) ? " " : Control.Text, Control.Font);
            var ps = new Size(measure.Width, measure.Height);
            if (ps.Width == _preferredSize.Width && ps.Height == _preferredSize.Height) return;
            _preferredSize = ps;
            OnPreferredSizeChanged();
        }

        protected override Size DefaultPreferredSize
        {
            get
            {
                return _preferredSize;
            }
        }
    }
}