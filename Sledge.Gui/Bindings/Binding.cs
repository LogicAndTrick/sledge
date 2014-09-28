using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Sledge.Gui.Bindings
{


    public interface IBindingTarget
    {
        object BindingSource { get; set; }
    }

    public class Binding
    {
        IBindingTarget Target { get; set; }
        string SourceProperty { get; set; }
        string TargetProperty { get; set; }
        BindingDirection Direction { get; set; }

        public void Bind()
        {
            
        }

        public void Update()
        {
            
        }
    }

    public enum BindingDirection
    {
        Dual, // binding works in two directions
        Forwards, // source -> target only
        Backwards, // target -> source only 
        None // why???
    }
}
