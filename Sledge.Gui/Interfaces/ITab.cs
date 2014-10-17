namespace Sledge.Gui.Interfaces
{
    public interface ITab : IListItem
    {
        bool Dirty { get; set; }
        bool Closable { get; set; }
    }
}