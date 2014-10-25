using System;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Controls;

namespace Sledge.Gui.Controls
{
    public class CheckBox : TextControlBase<ICheckbox>, ICheckbox
    {
        public event EventHandler Toggled
        {
            add { Control.Toggled += value; }
            remove { Control.Toggled -= value; }
        }

        public bool Checked
        {
            get { return Control.Checked; }
            set { Control.Checked = value; }
        }

        public bool Indeterminate
        {
            get { return Control.Indeterminate; }
            set { Control.Indeterminate = value; }
        }

        public bool? Value
        {
            get { return Control.Value; }
            set { Control.Value = value; }
        }
    }
}