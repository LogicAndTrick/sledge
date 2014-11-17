using System;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Controls;
using Sledge.Gui.Structures;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsTextBox : WinFormsControl, ITextBox
    {
        private Size _preferredSize = new Size(100, 20);

        public WinFormsTextBox()
            : base(new TextBox())
        {
            Control.TextChanged += UpdatePreferredSize;
            Control.FontChanged += UpdatePreferredSize;
        }

        private void UpdatePreferredSize(object sender, EventArgs e)
        {
            var measure = TextRenderer.MeasureText(String.IsNullOrEmpty(Control.Text) ? " " : Control.Text, Control.Font);
            var ps = new Size(measure.Width + 10, measure.Height + 10);
            if (ps.Width == _preferredSize.Width && ps.Height == _preferredSize.Height) return;
            _preferredSize = ps;
            OnPreferredSizeChanged();
        }

        protected override Size DefaultPreferredSize
        {
            get { return _preferredSize; }
        }
    }
}