namespace Sledge.Gui.Interfaces.Models
{
    public interface ITab : IListItem
    {
        bool Dirty { get; set; }
        bool Closable { get; set; }
    }
}