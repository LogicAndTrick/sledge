using System;
using System.Collections.Generic;

namespace Sledge.Gui.Interfaces.Shell
{
    public interface IMenu : IDisposable
    {
        IList<IMenuItem> Items { get; }
        IMenuItem AddMenuItem(string key, string text = null);
    }
}