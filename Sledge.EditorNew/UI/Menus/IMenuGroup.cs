using System.Collections.Generic;

namespace Sledge.EditorNew.UI.Menus
{
    public interface IMenuGroup
    {
        string Path { get; }
        IEnumerable<IMenuItem> GetMenuItems();
    }
}