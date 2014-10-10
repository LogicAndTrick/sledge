using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Sledge.Gui.Bindings
{
    public class Binding : Dictionary<string, object>
    {
        public IBindingTarget Target { get; private set; }
        public string SourceProperty { get; private set; }
        public string TargetProperty { get; private set; }
        public BindingDirection Direction { get; private set; }

        public Binding(IBindingTarget target, string targetProperty, string sourceProperty, BindingDirection direction)
        {
            Target = target;
            TargetProperty = targetProperty;
            SourceProperty = sourceProperty;
            Direction = direction;
        }
    }
}
