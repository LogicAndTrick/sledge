using Sledge.Gui.Interfaces;

namespace Sledge.Gui.Events
{
    public interface IEvent
    {
        IControl Sender { get; }
        bool Handled { get; set; }
    }
}