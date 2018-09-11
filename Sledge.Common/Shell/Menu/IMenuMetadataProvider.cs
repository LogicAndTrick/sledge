using System.Collections.Generic;

namespace Sledge.Common.Shell.Menu
{
    /// <summary>
    /// An interface that provides menu group and section metadata.
    /// </summary>
    public interface IMenuMetadataProvider
    {
        /// <summary>
        /// Get the list of menu sections
        /// </summary>
        /// <returns>Menu section list</returns>
        IEnumerable<MenuSection> GetMenuSections();

        /// <summary>
        /// Get the list of menu groups
        /// </summary>
        /// <returns>Menu group list</returns>
        IEnumerable<MenuGroup> GetMenuGroups();
    }
}