using System.Linq;

namespace Sledge.Common.Shell.Commands
{
    /// <summary>
    /// Common command extension methods
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Get the ID of a command. This can be overridden by a <see cref="CommandIDAttribute"/>.
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>The command's ID</returns>
        public static string GetID(this ICommand command)
        {
            var ty = command.GetType();
            var mt = ty.GetCustomAttributes(typeof(CommandIDAttribute), false).OfType<CommandIDAttribute>().FirstOrDefault();
            return mt?.ID ?? ty.FullName;
        }
    }
}