using System;
using Sledge.Gui.Shell;

namespace Sledge.Gui
{
    public interface IUIManager
    {
        IShell Shell { get; }

        void Start();

        // other stuff...

    }
}