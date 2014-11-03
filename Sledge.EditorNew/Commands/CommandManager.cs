using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.EditorNew.Commands
{
    public static class CommandManager
    {
        private static readonly List<ICommand> Commands = new List<ICommand>();
        private static readonly List<string> Contexts = new List<string> { DefaultCommandContexts.None };

        public static void Register(ICommand command)
        {
            Commands.Add(command);
        }

        public static IEnumerable<ICommand> GetCommands()
        {
            return Commands.ToList();
        }

        public static ICommand GetCommand(string identifier)
        {
            return Commands.FirstOrDefault(x => x.Identifier == identifier);
        }

        public static IEnumerable<ICommand> GetCommandsByGroup(string group)
        {
            return Commands.Where(x => x.Group == group);
        }

        public static IEnumerable<ICommand> GetCommandsByContext(string context)
        {
            return Commands.Where(x => x.Context == context);
        }

        public static void PushContext(string context)
        {
            Contexts.Add(context);
        }

        public static void PopContext(string context)
        {
            Contexts.Remove(context);
        }

        public static bool HasContext(string context)
        {
            return Contexts.Contains(context);
        }
    }
}
