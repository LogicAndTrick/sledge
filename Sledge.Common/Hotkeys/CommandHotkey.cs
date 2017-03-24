﻿using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Commands;

namespace Sledge.Common.Hotkeys
{
    public class CommandHotkey : IHotkey
    {
        private readonly ICommand _command;
        private readonly object _parameters;

        public string ID => "Command:" + _command.GetID();
        public string Name => _command.Name;
        public string Description => _command.Details;
        public string DefaultHotkey { get; }

        public CommandHotkey(ICommand command, object parameters = null, string defaultHotkey = null)
        {
            _command = command ?? this as ICommand;
            _parameters = parameters;
            DefaultHotkey = defaultHotkey;
        }

        public async Task Invoke()
        {
            await Oy.Publish("Command:Run", new CommandMessage(_command.GetID(), _parameters));
        }
    }
}