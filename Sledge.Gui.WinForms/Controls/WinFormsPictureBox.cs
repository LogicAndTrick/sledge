using System;
using System.Drawing;
using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Controls;
using Size = Sledge.Gui.Structures.Size;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsPictureBox : WinFormsControl, IPictureBox
    {
        private readonly PictureBox _picture;

        public WinFormsPictureBox() : base(new PictureBox())
        {
            _picture = (PictureBox)Control;
        }

        protected override Size DefaultPreferredSize
        {
            get
            {
                if (_picture.Image != null) return new Size(_picture.Image.Width, _picture.Image.Height);
                return new Size(100, 100);
            }
        }

        public Image Image
        {
            get { return _picture.Image; }
            set { _picture.Image = value; }
        }
    }
}