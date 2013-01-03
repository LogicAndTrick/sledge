using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Settings
{
    public class HotkeyDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }
        public string DefaultHotkey { get; set; }

        public HotkeyDefinition(string name, string description, string action, string defaultHotkey)
        {
            Name = name;
            Description = description;
            Action = action;
            DefaultHotkey = defaultHotkey;
        }
    }
}
