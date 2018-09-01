using System;
using System.Collections.Generic;

namespace Sledge.Common.Shell.Settings
{
    public interface ISettingsStore
    {
        IEnumerable<string> GetKeys();
        bool Contains(string key);
        object Get(Type type, string key, object defaultValue = null);
        T Get<T>(string key, T defaultValue = default(T));
        void Set<T>(string key, T value);
        void Delete(string key);
    }
}