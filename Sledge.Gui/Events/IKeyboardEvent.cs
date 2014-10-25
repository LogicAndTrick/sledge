using OpenTK.Input;

namespace Sledge.Gui.Events
{
    public interface IKeyboardEvent : IEvent
    {
        bool Control { get; }
        bool Shift { get; }
        bool Alt { get; }
        Key KeyValue { get; }
    }
}