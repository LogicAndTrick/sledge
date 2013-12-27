using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Settings
{
    public class HotkeyDefinition
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public HotkeysMediator Action { get; set; }
        public object Parameter { get; set; }
        public string[] DefaultHotkeys { get; set; }

        public HotkeyDefinition(string name, string description, HotkeysMediator action, params string[] defaultHotkeys)
        {
            ID = action.ToString();
            Name = name;
            Description = description;
            Action = action;
            DefaultHotkeys = defaultHotkeys;
        }

        public HotkeyDefinition(string name, string description, HotkeysMediator action, object parameter, params string[] defaultHotkeys)
        {
            ID = action + (parameter != null ? "." + parameter : "");
            Name = name;
            Description = description;
            Action = action;
            DefaultHotkeys = defaultHotkeys;
            Parameter = parameter;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
