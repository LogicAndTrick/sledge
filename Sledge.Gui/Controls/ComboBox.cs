using System;
using System.Collections.Generic;
using Sledge.Gui.Interfaces;

namespace Sledge.Gui.Controls
{
    public class ComboBox : TextControlBase<IComboBox>, IComboBox
    {
        public object SelectedItem
        {
            get { return Control.SelectedItem; }
            set { Control.SelectedItem = value; }
        }

        public int SelectedIndex
        {
            get { return Control.SelectedIndex; }
            set { Control.SelectedIndex = value; }
        }

        public IList<object> Items
        {
            get { return Control.Items; }
        }

        public event EventHandler SelectedItemChanged
        {
            add { Control.SelectedItemChanged += value; }
            remove { Control.SelectedItemChanged -= value; }
        }

        public event EventHandler SelectedIndexChanged
        {
            add { Control.SelectedIndexChanged += value; }
            remove { Control.SelectedIndexChanged -= value; }
        }
    }
}