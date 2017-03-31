using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sledge.Common.Context;

namespace Sledge.Common.Menu
{
    public interface IMenuItem
    {
        string ID { get; }
        string Name { get; }
        string Description { get; }

        /// <summary>
        /// The section of the menu item.
        /// For example, File, View, Help, etc.
        /// </summary>
        string Section { get; }
        
        /// <summary>
        /// The location of the menu item in the menu, relative to the Section.
        /// Blank will put it directly into the menu, each folder will be a submenu.
        /// For example: "View" or "View/Window"
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The group of the menu item. Null is the default group.
        /// </summary>
        string Group { get; }

        bool IsInContext(IContext context);

        Task Invoke();
    }
}
