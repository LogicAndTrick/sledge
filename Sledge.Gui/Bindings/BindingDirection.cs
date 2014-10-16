using System;

namespace Sledge.Gui.Bindings
{
    [Flags]
    public enum BindingDirection
    {
        Forwards = 0x01, // source -> target only
        Backwards = 0x02, // target -> source only 
        Dual = Forwards | Backwards, // binding works in two directions
        Auto = 0x80
    }
}