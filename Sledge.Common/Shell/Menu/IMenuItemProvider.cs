using System;
using System.Collections.Generic;

namespace Sledge.Common.Shell.Menu
{
    public interface IMenuItemProvider
    {
        event EventHandler MenuItemsChanged;

        IEnumerable<IMenuItem> GetMenuItems();
    }
}