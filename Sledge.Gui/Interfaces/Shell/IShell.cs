using System;
using System.Collections.Generic;

namespace Sledge.Gui.Interfaces.Shell
{
    public interface IShell : IWindow
    {
        IMenu Menu { get; }
        IToolbar Toolbar { get; }
        // 

        void AddMenu();
        void AddToolbar();
        void AddSidebarPanel(IControl panel, SidebarPanelLocation defaultLocation);
    }

    public enum SidebarPanelLocation
    {
        Left,
        Right,
        Bottom
    }

    public interface IStatusBar : IDisposable
    {
        
    }
}
