using System;
using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces.Controls
{
    /// <summary>
    /// Checkbox: true/false toggle button
    /// </summary>
    [ControlInterface]
    public interface ICheckbox : ITextControl
    {
        event EventHandler Toggled;
        bool Checked { get; set; }
        bool Indeterminate { get; set; }
        bool? Value { get; set; }
    }
}