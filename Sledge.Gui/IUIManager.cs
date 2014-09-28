using System;
using Sledge.Gui.Controls;
using Sledge.Gui.Shell;

namespace Sledge.Gui
{
    public interface IUIManager
    {
        IShell Shell { get; }

        void Start();

        IWindow CreateWindow();
        T Construct<T>() where T : IControl;

        // other stuff...

    }
}