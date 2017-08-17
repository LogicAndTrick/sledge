using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Menu;
using Sledge.Shell.Registers;

namespace Sledge.Shell.Components
{
    [Export(typeof(IMenuItemProvider))]
    public class CommandMenuItemProvider : IMenuItemProvider
    {
        [ImportMany] private IEnumerable<Lazy<ICommand>> _commands;
        
        // Store the hotkey register so we know what the hotkey for each command is
        [Import] private HotkeyRegister _hotkeys;

        public event EventHandler MenuItemsChanged;

        public IEnumerable<IMenuItem> GetMenuItems()
        {
            foreach (var export in _commands)
            {
                var ty = export.Value.GetType();
                var mia = ty.GetCustomAttributes(typeof(MenuItemAttribute), false).OfType<MenuItemAttribute>().FirstOrDefault();
                if (mia == null) continue;
                var icon = ty.GetCustomAttributes(typeof(MenuImageAttribute), false).OfType<MenuImageAttribute>().FirstOrDefault();

                var hotkey = _hotkeys.GetHotkey("Command:" + export.Value.GetID());
                var shortcut = _hotkeys.GetHotkeyString(hotkey);

                var allow = ty.GetCustomAttributes(typeof(AllowToolbarAttribute), false).OfType<AllowToolbarAttribute>().FirstOrDefault();

                yield return new CommandMenuItem(export.Value, mia.Section, mia.Path, mia.Group, mia.OrderHint, icon?.Image, shortcut ?? "", allow?.Allowed != false);
            }
        }
    }
}
