using System.Drawing;
using OpenTK.Input;

namespace Sledge.Gui.Events
{
    public interface IMouseEvent : IEvent
    {
        MouseButton Button { get; }
        int Clicks { get; }
        int X { get; }
        int Y { get; }
        int Delta { get; }
        Point Location { get; }
    }
}