using System;

namespace Sledge.Gui.Interfaces
{
    public interface ICheckboxListItem : IListItem
    {
        bool Checked { get; set; }
        bool Indeterminate { get; set; }
    }
}