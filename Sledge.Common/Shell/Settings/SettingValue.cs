namespace Sledge.Common.Shell.Settings
{
    public class SettingValue
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        public SettingValue(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}