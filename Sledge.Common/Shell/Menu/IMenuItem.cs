using System.Drawing;
using System.Threading.Tasks;
using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Menu
{
    public interface IMenuItem : IContextAware, IMenuItemExtendedProperties
    {
        string ID { get; }
        string Name { get; }
        string Description { get; }
        
        Image Icon { get; }
        
        bool AllowedInToolbar { get; }

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

        /// <summary>
        /// A string to sort this item within the group
        /// </summary>
        string OrderHint { get; }

        /// <summary>
        /// The shortcut text of the menu item
        /// </summary>
        string ShortcutText { get; }

        Task Invoke(IContext context);
    }
}
