using System;
using System.Collections.Generic;
using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces
{
    [ControlInterface]
    public interface IComboBox : ITextControl
    {
        IComboBoxItem SelectedItem { get; set; }
        int SelectedIndex { get; set; }
        int MaxHeight { get; set; }

        ItemList<IComboBoxItem> Items { get; }

        event EventHandler SelectedItemChanged;
        event EventHandler SelectedIndexChanged;

        event EventHandler DropDownOpened;
        event EventHandler DropDownClosed;
    }
}