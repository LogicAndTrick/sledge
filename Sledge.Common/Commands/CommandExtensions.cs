namespace Sledge.Common.Commands
{
    public static class CommandExtensions
    {
        public static string GetID(this ICommand command)
        {
            return command.GetType().FullName;
        }
    }
}