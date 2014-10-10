namespace Sledge.Gui.Bindings
{
    public enum BindingDirection
    {
        Dual, // binding works in two directions
        Forwards, // source -> target only
        Backwards, // target -> source only 
        None // why???
    }
}