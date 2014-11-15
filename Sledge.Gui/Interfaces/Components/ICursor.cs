using System.Drawing;
using Sledge.Gui.Attributes;
using Sledge.Gui.Components;

namespace Sledge.Gui.Interfaces.Components
{
    [ControlInterface]
    public interface ICursor
    {
        void SetCursor(IControl control, CursorType type);
        void SetCursor(IControl control, Image cursorImage);
        void HideCursor(IControl control);
        void ShowCursor(IControl control);
    }
}