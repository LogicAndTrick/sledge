using System.Linq;

namespace Sledge.Common.Shell.Commands
{
    public static class CommandExtensions
    {
        public static string GetID(this ICommand command)
        {
            var ty = command.GetType();
            var mt = ty.GetCustomAttributes(typeof(CommandIDAttribute), false).OfType<CommandIDAttribute>().FirstOrDefault();
            return mt?.ID ?? ty.FullName;
        }
    }
}