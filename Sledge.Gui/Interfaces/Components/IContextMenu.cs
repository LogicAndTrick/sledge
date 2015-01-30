using System;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.Gui.Interfaces.Components
{
    [ControlInterface]
    public interface IContextMenu : IMenu
    {
        event EventHandler Opened;
        event EventHandler Closed;

        void Open(IControl control, int x, int y);
        void Close();
    }
}