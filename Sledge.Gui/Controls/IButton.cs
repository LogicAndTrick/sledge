using System;
using Sledge.Gui.Attributes;

namespace Sledge.Gui.Controls
{
    /// <summary>
    /// Button: thing that gets clicked
    /// </summary>
    [ControlInterface]
    public interface IButton : IControl
    {
        string Text { get; set; }
        bool Enabled { get; set; }
        event EventHandler Clicked;
    }
}