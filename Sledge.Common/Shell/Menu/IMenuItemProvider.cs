using System;
using System.Collections.Generic;

namespace Sledge.Common.Shell.Menu
{
    /// <summary>
    /// A provider of menu items
    /// </summary>
    public interface IMenuItemProvider
    {
        /// <summary>
        /// Fires when the menu items have changed, and should be re-added from this provider.
        /// </summary>
        event EventHandler MenuItemsChanged;

        /// <summary>
        /// Get the menu items
        /// </summary>
        /// <returns>List of menu items</returns>
        IEnumerable<IMenuItem> GetMenuItems();
    }
}