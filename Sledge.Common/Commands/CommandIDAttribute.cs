using System;

namespace Sledge.Common.Commands
{
    public class CommandIDAttribute : Attribute
    {
        public string ID { get; set; }

        public CommandIDAttribute(string id)
        {
            ID = id;
        }
    }
}