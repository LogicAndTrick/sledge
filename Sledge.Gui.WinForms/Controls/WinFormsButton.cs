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
    public class WinFormsButton : WinFormsControl, IButton
    {
        private Size _preferredSize = new Size(100, 20);
        private readonly Button _button;

        public WinFormsButton()
            : base(new Button())
        {
            _button = (Button)Control;
            Control.TextChanged += UpdatePreferredSize;
            Control.FontChanged += UpdatePreferredSize;
        }

        private void UpdatePreferredSize(object sender, EventArgs e)
        {
            var measure = TextRenderer.MeasureText(String.IsNullOrEmpty(Control.Text) ? " " : Control.Text, Control.Font);
            var addW = Image == null ? 0 : Image.Width + 3;
            var height = Image == null ? measure.Height : Math.Max(measure.Height, Image.Height);
            var ps = new Size(measure.Width + 10 + addW, height + 10);
            if (ps.Width == _preferredSize.Width && ps.Height == _preferredSize.Height) return;
            _preferredSize = ps;
            OnPreferredSizeChanged();
        }

        protected override Size DefaultPreferredSize
        {
            get { return _preferredSize; }
        }

        public Image Image
        {
            get { return _button.Image; }
            set
            {
                _button.Image = value;
                UpdatePreferredSize(null, null);
            }
        }

        public event EventHandler Clicked
        {
            add { Control.Click += value; }
            remove { Control.Click -= value; }
        }
    }
}