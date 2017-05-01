using System.Collections.Generic;

namespace Sledge.Common.Shell.Settings
{
    public interface ISettingsContainer
    {
        string Name { get; }
        IEnumerable<SettingKey> GetKeys();
        void LoadValues(ISettingsStore store);
        void StoreValues(ISettingsStore store);
    }
}