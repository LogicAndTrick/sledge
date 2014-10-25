using System.ComponentModel;

namespace Sledge.Gui.Interfaces.Models
{
    public interface IListItem : INotifyPropertyChanged
    {
        string Text { get; set; }
        object Value { get; set; }
        bool Selected { get; set; }
    }
}