using System.Drawing;
using System.Threading.Tasks;
using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Menu
{
    /// <summary>
    /// An item that is added to the shell's menu.
    /// </summary>
    public interface IMenuItem : IContextAware, IMenuItemExtendedProperties
    {
        /// <summary>
        /// The ID of the menu item
        /// </summary>
        string ID { get; }

        /// <summary>
        /// The name to display in the menu item
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A description of the menu item
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// The icon for the menu item. Can be null.
        /// </summary>
        Image Icon { get; }
        
        /// <summary>
        /// True if this item should be shown in the toolbar.
        /// </summary>
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

        /// <summary>
        /// Invoke this menu item's action.
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>Completion task</returns>
        Task Invoke(IContext context);
    }
}
