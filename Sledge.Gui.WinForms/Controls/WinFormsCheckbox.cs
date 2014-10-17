using System;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsCheckbox : WinFormsControl, ICheckbox
    {
        private readonly CheckBox _checkBox;

        public WinFormsCheckbox() : base(new CheckBox())
        {
            _checkBox = (CheckBox) Control;
        }

        protected override Size DefaultPreferredSize
        {
            get { return new Size(100, FontSize * 2); }
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