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
        public HotkeysMediator Action { get; set; }
        public object Parameter { get; set; }
        public string DefaultHotkey { get; set; }

        public HotkeyDefinition(string name, string description, HotkeysMediator action, string defaultHotkey)
        {
            Name = name;
            Description = description;
            Action = action;
            DefaultHotkey = defaultHotkey;
        }

        public HotkeyDefinition(string name, string description, HotkeysMediator action, object parameter, string defaultHotkey)
        {
            Name = name;
            Description = description;
            Action = action;
            DefaultHotkey = defaultHotkey;
        }
    }
}
