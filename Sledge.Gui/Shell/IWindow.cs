using System;

namespace Sledge.Gui.Shell
{
    public interface IWindow : IDisposable
    {
        string Title { get; set; }

        event EventHandler WindowLoaded;
        event EventHandler<HandledEventArgs> WindowClosing;
        event EventHandler WindowClosed;
    }
}