using System;
using Sledge.Gui.Interfaces;

namespace Sledge.Gui.Shell
{
    public interface IWindow : IControl
    {
        string Title { get; set; }
        bool AutoSize { get; set; }

        ICell Container { get; }

        void Open();
        void Close();

        event EventHandler WindowLoaded;
        event EventHandler<HandledEventArgs> WindowClosing;
        event EventHandler WindowClosed;
    }
}