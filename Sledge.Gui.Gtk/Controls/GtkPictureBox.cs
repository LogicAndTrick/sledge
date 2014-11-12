using System.Drawing.Imaging;
using System.IO;
using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Controls;
using Image = System.Drawing.Image;

namespace Sledge.Gui.Gtk.Controls
{
    [ControlImplementation("GTK")]
    public class GtkPictureBox : GtkControl, IPictureBox
    {
        private VBox _container;
        private Widget _imageWidget;
        private Image _image;

        public GtkPictureBox() : base(new VBox(false, 0))
        {
            _container = (VBox)Control;
        }

        public Image Image
        {
            get { return _image; }
            set
            {
                if (_imageWidget != null)
                {
                    _container.Remove(_imageWidget);
                    _imageWidget.Dispose();
                    _imageWidget = null;
                }
                _image = value;
                if (_image != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        value.Save(ms, ImageFormat.Png);
                        ms.Seek(0, SeekOrigin.Begin);
                        _imageWidget = new global::Gtk.Image(ms);
                        _container.PackStart(_imageWidget, false, false, 0);
                    }
                }
            }
        }
    }
}