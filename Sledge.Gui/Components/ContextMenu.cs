using System;
using System.Collections.Generic;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Components;
using Sledge.Gui.Interfaces.Shell;

namespace Sledge.Gui.Components
{
    public class ContextMenu : IContextMenu
    {
        private IContextMenu Menu { get; set; }

        public event EventHandler Opened
        {
            add { Menu.Opened += value; }
            remove { Menu.Opened -= value; }
        }

        public event EventHandler Closed
        {
            add { Menu.Closed += value; }
            remove { Menu.Closed -= value; }
        }

        public IList<IMenuItem> Items
        {
            get { return Menu.Items; }
        }

        public ContextMenu()
        {
            Menu = UIManager.Manager.ConstructComponent<IContextMenu>();
        }

        public void Dispose()
        {
            Menu.Dispose();
        }

        public IMenuItem AddMenuItem(string key, string text = null)
        {
            return Menu.AddMenuItem(key, text);
        }

        public void Open(IControl control, int x, int y)
        {
            Menu.Open(control, x, y);
        }

        public void Close()
        {
            Menu.Close();
        }
    }
}