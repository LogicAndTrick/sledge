using System.Drawing;

namespace Sledge.Gui.Interfaces
{
    public interface IImageListItem : IListItem
    {
        bool HasImage { get; }
        Image Image { get; set; }
        int ImageWidth { get; }
        int ImageHeight { get; }
        bool DisposeImage { get; }
    }
}