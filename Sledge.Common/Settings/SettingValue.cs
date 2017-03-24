using System;

namespace Sledge.Common.Settings
{
    public class SettingValue
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public SettingValue(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}