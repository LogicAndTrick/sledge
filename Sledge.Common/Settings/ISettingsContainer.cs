using System.Collections.Generic;

namespace Sledge.Common.Settings
{
    public interface ISettingsContainer
    {
        string Name { get; }
        IEnumerable<SettingKey> GetKeys();
        void SetValues(IEnumerable<SettingValue> values);
        IEnumerable<SettingValue> GetValues();
    }
}