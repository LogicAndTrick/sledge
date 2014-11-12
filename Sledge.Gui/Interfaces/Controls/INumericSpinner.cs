using System;
using Sledge.Gui.Attributes;

namespace Sledge.Gui.Interfaces.Controls
{
    [ControlInterface]
    public interface INumericSpinner : IControl
    {
        decimal Value { get; set; }
        decimal Minimum { get; set; }
        decimal Maximum { get; set; }
        decimal Increment { get; set; }
        int Precision { get; set; }
        event EventHandler ValueChanged;
    }
}