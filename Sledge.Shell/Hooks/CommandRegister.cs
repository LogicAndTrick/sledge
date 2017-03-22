using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Gimme;
using LogicAndTrick.Gimme.Providers;
using LogicAndTrick.Oy;
using Sledge.Common.Commands;
using Sledge.Common.Hooks;

namespace Sledge.Shell.Hooks
{
    [Export(typeof(IInitialiseHook))]
    [Export(typeof(IResourceProvider<>))]
    public class CommandRegister : SyncResourceProvider<ICommand>, IInitialiseHook
    {
        public Task OnInitialise(CompositionContainer container)
        {
            // Register exported commands
            foreach (var export in container.GetExports<ICommand>())
            {
                Add(export.Value);
            }

            // Listen for dynamically added/removed commands
            Oy.Subscribe<ICommand>("Command:Register", c => Add(c));
            Oy.Subscribe<ICommand>("Command:Unregisted", c => Remove(c));

            // Hook to run a command
            Oy.Subscribe<CommandMessage>("Command:Run", Run);

            // Register the resource provider
            Gimme.Register(this);

            return Task.FromResult(0);
        }

        // In the unlikely case anybody needs a command as a resource
        public override bool CanProvide(string location)
        {
            return location == "meta://command";
        }

        public override IEnumerable<ICommand> Fetch(string location, List<string> resources)
        {
            return resources.Select(Get).Where(g => g != null);
        }

        private async Task Run(CommandMessage message)
        {
            var cmd = Get(message.CommandID);
            if (cmd != null && cmd.IsInContext()) await cmd.Invoke(message.Parameters);
        }

        private readonly ConcurrentDictionary<string, ICommand> _commands;

        public CommandRegister()
        {
            _commands = new ConcurrentDictionary<string, ICommand>();
        }

        private void Add(ICommand command)
        {
            _commands[command.GetID()] = command;
        }

        private ICommand Get(string id)
        {
            return _commands.ContainsKey(id) ? _commands[id] : null;
        }

        private void Remove(ICommand command)
        {
            Remove(command.GetID());
        }

        private void Remove(string id)
        {
            ICommand o;
            _commands.TryRemove(id, out o);
        }
    }
}