using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Gui.Controls;

namespace Sledge.Gui.Shell
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

    public interface ITabStrip : IDisposable
    {
        TabStripItem ActiveTab { get; set; }
        IList<TabStripItem> Tabs { get; }
        event EventHandler TabSelected;
    }

    public class TabStripItem
    {
        public string Identifier { get; set; }
        public string Text { get; set; }
        public bool Dirty { get; set; }
    }
}
