namespace Sledge.Gui.Interfaces.Models
{
    public interface ICheckboxListItem : IListItem
    {
        bool Checked { get; set; }
        bool Indeterminate { get; set; }
    }
}