using System;
using Sledge.Gui.Bindings;

namespace Sledge.Gui.Controls
{
    public interface IControl : IBindingTarget
    {
        Size ActualSize { get; }
        Size PreferredSize { get; }
        event EventHandler ActualSizeChanged;
        event EventHandler PreferredSizeChanged;
    }
}