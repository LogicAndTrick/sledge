namespace Sledge.Gui.Interfaces
{
    public interface IComboBoxItem : IImageListItem
    {
        bool DrawBorder { get; set; }
        string DisplayText { get; set; }
    }
}