using System;
using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces.Controls
{
    /// <summary>
    /// Button: thing that gets clicked
    /// </summary>
    [ControlInterface]
    public interface IButton : ITextControl
    {
        event EventHandler Clicked;
    }
}