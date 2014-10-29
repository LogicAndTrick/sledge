using System;
using Sledge.Common.Mediator;

namespace Sledge.EditorNew.Commands
{
    public class MediatorCommand : ICommand
    {
        public string Group { get; private set; }
        public string Identifier { get; private set; }
        public string Context { get; private set; }
        public string Message { get; private set; }
        public object Parameter { get; private set; }

        public MediatorCommand(string group, string identifier, string context, Enum message, object parameter = null)
            : this(group, identifier, context, message.ToString(), parameter)
        {
        }

        public MediatorCommand(string group, string identifier, string context, string message, object parameter = null)
        {
            Identifier = identifier;
            Group = group;
            Context = context;
            Message = message;
            Parameter = parameter;
        }

        public void Fire()
        {
            Mediator.Publish(Message, Parameter);
        }
    }
}