namespace Sledge.Gui.Interfaces.Models
{
    public interface IComboBoxItem : IImageListItem
    {
        bool DrawBorder { get; set; }
        string DisplayText { get; set; }
    }
}