using System.Drawing;
using Sledge.Gui.Interfaces;

namespace Sledge.Gui.Controls
{
    public class PictureBox : ControlBase<IPictureBox>, IPictureBox
    {
        public Image Image
        {
            get { return Control.Image; }
            set { Control.Image = value; }
        }
    }
}