using System;
using System.Collections.Generic;

namespace Sledge.Gui.Shell
{
    public interface IToolbar : IDisposable
    {
        IList<IToolbarItem> Items { get; }
        IToolbarItem AddToolbarItem(string identifier, string text);
    }
}