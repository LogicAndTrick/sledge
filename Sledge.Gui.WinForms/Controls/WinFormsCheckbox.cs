using System;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Controls;
using Sledge.Gui.Structures;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsCheckbox : WinFormsControl, ICheckbox
    {
        private Size _preferredSize = new Size(100, 20);
        private readonly CheckBox _checkBox;

        public WinFormsCheckbox() : base(new CheckBox())
        {
            _checkBox = (CheckBox)Control;
            Control.TextChanged += UpdatePreferredSize;
            Control.FontChanged += UpdatePreferredSize;
        }

        private void UpdatePreferredSize(object sender, EventArgs e)
        {
            var measure = TextRenderer.MeasureText(String.IsNullOrEmpty(Control.Text) ? " " : Control.Text, Control.Font);
            var ps = new Size(measure.Width + 20, measure.Height + 5);
            if (ps.Width == _preferredSize.Width && ps.Height == _preferredSize.Height) return;
            _preferredSize = ps;
            OnPreferredSizeChanged();
        }

        protected override Size DefaultPreferredSize
        {
            get { return _preferredSize; }
        }

        public event EventHandler Toggled
        {
            add { _checkBox.CheckStateChanged += value; }
            remove { _checkBox.CheckStateChanged -= value; }
        }

        public bool Checked
        {
            get { return _checkBox.CheckState == CheckState.Checked; }
            set { _checkBox.CheckState = value ? CheckState.Checked : CheckState.Unchecked; }
        }

        public bool Indeterminate
        {
            get { return _checkBox.CheckState == CheckState.Indeterminate; }
            set { _checkBox.CheckState = value ? CheckState.Indeterminate : CheckState.Unchecked; }
        }

        public bool? Value
        {
            get
            {
                switch (_checkBox.CheckState)
                {
                    case CheckState.Unchecked:
                        return false;
                    case CheckState.Checked:
                        return true;
                    default:
                        return null;
                }
            }
            set
            {
                switch (value)
                {
                    case true:
                        _checkBox.CheckState = CheckState.Checked;
                        break;
                    case false:
                        _checkBox.CheckState = CheckState.Unchecked;
                        break;
                    case null:
                        _checkBox.CheckState = CheckState.Indeterminate;
                        break;
                }
            }
        }
    }
}