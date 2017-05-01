using System;

namespace Sledge.Common.Shell.Settings
{
    public class SettingKey
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }

        public SettingKey(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
