using System;

namespace Sledge.Gui.Bindings
{
    public interface IBindingTarget
    {
        object BindingSource { get; set; }
        Binding Bind(string property, string sourceProperty, BindingDirection direction = BindingDirection.Dual);
        void UnbindAll();
        void Unbind(string property);
    }
}