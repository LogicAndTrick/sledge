using System.ComponentModel;

namespace Sledge.Gui.Interfaces
{
    public interface IListItem : INotifyPropertyChanged
    {
        string Text { get; set; }
        object Value { get; set; }
        bool Selected { get; set; }
    }
}