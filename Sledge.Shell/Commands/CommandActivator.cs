using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Commands;

namespace Sledge.Shell.Commands
{
    internal class CommandActivator : IActivator
    {
        private readonly ICommand _command;
        public string Group => "Commands";
        public string Name => _command.Name;
        public string Description => _command.Details;

        public CommandActivator(ICommand command)
        {
            _command = command;
        }

        public async Task Activate()
        {
            await Oy.Publish("Command:Run", _command.Invoke(new CommandParameters()));
        }
    }
}