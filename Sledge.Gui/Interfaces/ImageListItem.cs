using System.Drawing;

namespace Sledge.Gui.Interfaces
{
    public class ImageListItem : ListItem, IImageListItem
    {
        private Image _image;

        public virtual Image Image
        {
            get { return _image; }
            set
            {
                if (_image == value) return;
                _image = value;
                OnPropertyChanged("Image");
            }
        }

        public virtual bool HasImage { get { return Image != null; } }
        public virtual int ImageWidth { get { return Image != null ? Image.Width : 0; } }
        public virtual int ImageHeight { get { return Image != null ? Image.Height : 0; } }
        public bool DisposeImage { get { return false; } }

        public static implicit operator ImageListItem(string text)
        {
            return new ImageListItem { Text = text };
        }
    }
}