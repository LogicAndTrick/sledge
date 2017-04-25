using System.Collections.Generic;

namespace Sledge.Common.Shell.Menu
{
    public interface IMenuItemProvider
    {
        IEnumerable<IMenuItem> GetMenuItems();
    }
}