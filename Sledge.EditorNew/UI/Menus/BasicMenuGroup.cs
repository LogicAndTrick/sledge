using System.Collections.Generic;
using System.Linq;

namespace Sledge.EditorNew.UI.Menus
{
    public class BasicMenuGroup : List<IMenuItem>, IMenuGroup
    {
        public BasicMenuGroup(string path, params IMenuItem[] items)
        {
            Path = path;
            AddRange(items);
        }

        public string Path { get; private set; }

        public IEnumerable<IMenuItem> GetMenuItems()
        {
            return this.ToList();
        }
    }
}