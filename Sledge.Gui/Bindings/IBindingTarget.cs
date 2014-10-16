using System;
using System.Collections.Generic;

namespace Sledge.Gui.Bindings
{
    public interface IBindingTarget
    {
        object BindingSource { get; set; }
        Binding Bind(string property, string sourceProperty, BindingDirection direction = BindingDirection.Auto, Dictionary<string, object> meta = null);
        void UnbindAll();
        void Unbind(string property);
    }
}