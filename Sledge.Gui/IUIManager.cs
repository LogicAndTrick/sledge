using System.Reflection;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Shell;
using Sledge.Gui.Resources;

namespace Sledge.Gui
{
    public interface IUIManager
    {
        IShell Shell { get; }
        IStringProvider StringProvider { get; set; }

        void Start();

        IWindow CreateWindow();
        T Construct<T>() where T : IControl;

        // other stuff...

    }
}