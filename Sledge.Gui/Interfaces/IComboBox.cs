using System;
using System.Collections.Generic;
using System.Drawing;
using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces
{
    [ControlInterface]
    public interface IComboBox : ITextControl
    {
        object SelectedItem { get; set; }
        int SelectedIndex { get; set; }

        IList<object> Items { get; }

        event EventHandler SelectedItemChanged;
        event EventHandler SelectedIndexChanged;
    }

    [ControlInterface]
    public interface IPictureBox : IControl
    {
        Image Image { get; set; }
    }
}