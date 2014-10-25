using System.Drawing;
using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces.Controls
{
    [ControlInterface]
    public interface IPictureBox : IControl
    {
        Image Image { get; set; }
    }
}