using System.Drawing;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Menu
{
    /// <summary>
    /// A menu item that invokes a command.
    /// </summary>
    public class CommandMenuItem : IMenuItem
    {
        private readonly ICommand _command;

        public string ID => "Command:" + _command.GetID();
        public string Name => _command.Name;
        public string Description => _command.Details;
        public bool AllowedInToolbar { get; }
        public string Section { get; }
        public string Path { get; }
        public string Group { get; }
        public string OrderHint { get; }
        public Image Icon { get; }
        public string ShortcutText { get; }
        public bool IsToggle => (_command as IMenuItemExtendedProperties)?.IsToggle == true;

        public CommandMenuItem(ICommand command, string section, string path, string group, string orderHint, Image icon, string shortcutText, bool allowedInToolbar)
        {
            Section = section;
            Path = path;
            Group = group;
            OrderHint = orderHint;
            Icon = icon;
            ShortcutText = shortcutText;
            AllowedInToolbar = allowedInToolbar;
            _command = command ?? this as ICommand;
        }

        public bool IsInContext(IContext context)
        {
            return _command.IsInContext(context);
        }

        public async Task Invoke(IContext context)
        {
            await Oy.Publish("Command:Run", new CommandMessage(_command.GetID()));
        }

        public bool GetToggleState(IContext context)
        {
            return (_command as IMenuItemExtendedProperties)?.GetToggleState(context) == true;
        }
    }
}