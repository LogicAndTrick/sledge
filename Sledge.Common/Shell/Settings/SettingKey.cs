using System;

namespace Sledge.Common.Shell.Settings
{
    public class SettingKey
    {
        public string Group { get; set; }
        public string Key { get; private set; }
        public Type Type { get; private set; }
        public string EditorType { get; set; }
        public string EditorHint { get; set; }
        public string OrderHint { get; set; }

        public SettingKey(string group, string key, Type type)
        {
            Group = group;
            Key = key;
            Type = type;
        }
    }
}
