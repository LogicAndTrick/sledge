using System;
using System.Drawing.Imaging;
using System.IO;
using Gtk;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Controls;
using Image = System.Drawing.Image;

namespace Sledge.Gui.Gtk.Controls
{
    [ControlImplementation("GTK")]
    public class GtkButton : GtkTextControl, IButton
    {
        private Button _button;
        private Image _image;

        public GtkButton() : base(new Button())
        {
            _button = (Button) Control;
        }

        protected override string ControlText
        {
            get { return _button.Label; }
            set { _button.Label = value; }
        }

        public Image Image
        {
            get { return _image; }
            set
            {
                _image = value;
                if (value != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        value.Save(ms, ImageFormat.Png);
                        ms.Seek(0, SeekOrigin.Begin);
                        _button.Image = new global::Gtk.Image(ms);
                    }
                }
                else
                {
                    _button.Image = null;
                }
            }
        }

        public event EventHandler Clicked
        {
            add { _button.Clicked += value; }
            remove { _button.Clicked -= value; }
        }
    }
}