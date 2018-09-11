using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Menu
{
    /// <summary>
    /// An interface a menu item can implement to expose various extended options.
    /// </summary>
    public interface IMenuItemExtendedProperties
    {
        /// <summary>
        /// True if this is a toggle item, and should have a checkbox icon.
        /// </summary>
        bool IsToggle { get; }

        /// <summary>
        /// For a toggle item, get the current toggle state for this item.
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>True if the toggle state is currently on</returns>
        bool GetToggleState(IContext context);
    }
}