using System.Collections.Concurrent;

namespace Sledge.Common.Commands
{
    public class CommandRegister
    {
        private readonly ConcurrentDictionary<string, ICommand> _commands;

        public CommandRegister()
        {
            _commands = new ConcurrentDictionary<string, ICommand>();
        }

        public void Add(ICommand command)
        {
            _commands[command.GetID()] = command;
        }

        public ICommand Get(string id)
        {
            return _commands.ContainsKey(id) ? _commands[id] : null;
        }

        public void Remove(ICommand command)
        {
            Remove(command.GetID());
        }

        public void Remove(string id)
        {
            ICommand o;
            _commands.TryRemove(id, out o);
        }
    }
}