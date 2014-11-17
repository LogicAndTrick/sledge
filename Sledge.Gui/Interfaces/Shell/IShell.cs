using System;
using System.Collections.Generic;
using Sledge.Gui.Interfaces.Containers;

namespace Sledge.Gui.Interfaces.Shell
{
    public interface IShell : IWindow
    {
        IMenu Menu { get; }
        IToolbar Toolbar { get; }
        // 

        void AddMenu();
        void AddToolbar();
        IDockPanel AddDockPanel(IControl panel, DockPanelLocation defaultLocation);
    }

    public enum DockPanelLocation
    {
        Left,
        Right,
        Bottom
    }

    public interface IStatusBar : IDisposable
    {
        
    }
}
