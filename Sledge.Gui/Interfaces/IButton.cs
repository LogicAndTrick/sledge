using System;
using System.Collections;
using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces
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