using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Menu;

namespace Sledge.Shell.Components
{
    [Export(typeof(IMenuItemProvider))]
    public class CommandMenuItemProvider : IMenuItemProvider
    {
        [ImportMany] private IEnumerable<Lazy<ICommand>> _commands;
        public IEnumerable<IMenuItem> GetMenuItems()
        {
            foreach (var export in _commands)
            {
                var ty = export.Value.GetType();
                var mia = ty.GetCustomAttributes(typeof(MenuItemAttribute), false).OfType<MenuItemAttribute>().FirstOrDefault();
                if (mia == null) continue;
                var icon = ty.GetCustomAttributes(typeof(MenuImageAttribute), false).OfType<MenuImageAttribute>().FirstOrDefault();
                yield return new CommandMenuItem(export.Value, mia.Section, mia.Path, mia.Group, mia.OrderHint, icon?.Image);
            }
        }
    }
}
