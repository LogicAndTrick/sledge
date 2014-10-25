using System;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Models;
using Sledge.Gui.Structures;

namespace Sledge.Gui.Interfaces.Controls
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