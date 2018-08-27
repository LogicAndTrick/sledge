using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hooks;
using Sledge.Shell.Commands;

namespace Sledge.Shell.Registers
{
    /// <summary>
    /// Collects all exported commands and runs then when requested.
    /// This class should be the only thing to ever run commands in the application.
    /// </summary>
    [Export]
    [Export(typeof(IStartupHook))]
    [Export(typeof(IActivatorProvider))]
    public class CommandRegister : IStartupHook, IActivatorProvider
    {
        // Store the context (the command register is one of the few things that should need static access to the context)
        [Import] private IContext _context;
        [ImportMany] private IEnumerable<Lazy<ICommand>> _importedCommands;
        
        public Task OnStartup()
        {
            // Register exported commands
            foreach (var export in _importedCommands)
            {
                Add(export.Value);
            }

            // Listen for dynamically added/removed commands
            Oy.Subscribe<ICommand>("Command:Register", c => Add(c));
            Oy.Subscribe<ICommand>("Command:Unregisted", c => Remove(c));

            // Hook to run a command
            Oy.Subscribe<CommandMessage>("Command:Run", Run);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Run a command message
        /// </summary>
        /// <param name="message">The message to run</param>
        /// <returns>Task that will complete after the command runs</returns>
        private async Task Run(CommandMessage message)
        {
            var cmd = Get(message.CommandID);
            if (cmd != null && cmd.IsInContext(_context))
            {
                await Oy.Publish("Command:Intercept", message);
                if (message.Intercepted) return;
                await cmd.Invoke(_context, message.Parameters);
            }
        }

        private readonly ConcurrentDictionary<string, ICommand> _commands;

        public CommandRegister()
        {
            _commands = new ConcurrentDictionary<string, ICommand>();
        }

        public IEnumerable<IActivator> SearchActivators(string keywords)
        {
            var filter = (keywords ?? "").Trim().Split(' ');
            return _commands.Values.Where(x => filter.All(f => x.Name.IndexOf(f, StringComparison.Ordinal) >= 0)).Select(x => new CommandActivator(x));
        }

        /// <summary>
        /// Register a command
        /// </summary>
        /// <param name="command">The command to add</param>
        private void Add(ICommand command)
        {
            _commands[command.GetID()] = command;
        }

        /// <summary>
        /// Get a command by id
        /// </summary>
        /// <param name="id">The command id</param>
        /// <returns>The command or null if it's not found</returns>
        public ICommand Get(string id)
        {
            return _commands.ContainsKey(id) ? _commands[id] : null;
        }

        /// <summary>
        /// Unregister a command
        /// </summary>
        /// <param name="command">The command to remove</param>
        private void Remove(ICommand command)
        {
            Remove(command.GetID());
        }

        /// <summary>
        /// Unregister a command by id
        /// </summary>
        /// <param name="id">The command id to remove</param>
        private void Remove(string id)
        {
            ICommand o;
            _commands.TryRemove(id, out o);
        }
    }
}