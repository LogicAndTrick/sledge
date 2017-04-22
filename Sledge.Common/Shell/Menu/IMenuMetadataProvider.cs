using System.Collections.Generic;

namespace Sledge.Common.Shell.Menu
{
    public interface IMenuMetadataProvider
    {
        IEnumerable<MenuSection> GetMenuSections();
        IEnumerable<MenuGroup> GetMenuGroups();
    }
}