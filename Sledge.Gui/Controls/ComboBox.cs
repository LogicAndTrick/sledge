using System;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Controls;
using Sledge.Gui.Interfaces.Models;
using Sledge.Gui.Structures;

namespace Sledge.Gui.Controls
{
    public class ComboBox : TextControlBase<IComboBox>, IComboBox
    {
        public IComboBoxItem SelectedItem
        {
            get { return Control.SelectedItem; }
            set { Control.SelectedItem = value; }
        }

        public int SelectedIndex
        {
            get { return Control.SelectedIndex; }
            set { Control.SelectedIndex = value; }
        }

        public int MaxHeight
        {
            get { return Control.MaxHeight; }
            set { Control.MaxHeight = value; }
        }

        public ItemList<IComboBoxItem> Items
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

        public event EventHandler DropDownOpened
        {
            add { Control.DropDownOpened += value; }
            remove { Control.DropDownOpened -= value; }
        }

        public event EventHandler DropDownClosed
        {
            add { Control.DropDownClosed += value; }
            remove { Control.DropDownClosed -= value; }
        }
    }
}