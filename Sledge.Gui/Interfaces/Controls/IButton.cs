using System;
using System.Drawing;
using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces.Controls
{
    /// <summary>
    /// Button: thing that gets clicked
    /// </summary>
    [ControlInterface]
    public interface IButton : ITextControl
    {
        Image Image { get; set; }
        event EventHandler Clicked;
    }
}