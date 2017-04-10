using System;

namespace Sledge.Common.Shell.Settings
{
    public class SettingKey
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Type Type { get; private set; }

        public SettingKey(string name, string description, Type type)
        {
            Name = name;
            Description = description;
            Type = type;
        }
    }
}
