using System;
using System.Linq;
using System.Text;

namespace Sledge.Gui.Shell
{
    public interface IShell : IWindow
    {
        IMenu Menu { get; }
        IToolbar Toolbar { get; }

        void AddMenu();
        void AddToolbar();
    }

    public interface IToolbar : IDisposable
    {
        
    }

    public interface IToolbarItem : IDisposable
    {
        string Text { get; set; }

    }

    public interface ISidebar : IDisposable
    {
        
    }

    public interface ISidebarPanel : IDisposable
    {
        
    }

    public interface IStatusBar : IDisposable
    {
        
    }
}
