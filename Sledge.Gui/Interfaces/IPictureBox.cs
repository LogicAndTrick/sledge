using System.Drawing;
using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces
{
    [ControlInterface]
    public interface IPictureBox : IControl
    {
        Image Image { get; set; }
    }
}