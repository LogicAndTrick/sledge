using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sledge.Common.Logging;

namespace Sledge.Common.Shell.Settings
{
    public abstract class StaticSettingsContainer : ISettingsContainer
    {
        protected abstract Type StaticType { get; }
        public string Name { get; }

        private PropertyInfo[] GetProps()
        {
            return StaticType.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Static);
        }

        public IEnumerable<SettingKey> GetKeys()
        {
            foreach (var pi in GetProps())
            {
                yield return new SettingKey(pi.Name, pi.PropertyType);
            }
        }

        public void LoadValues(ISettingsStore store)
        {
            var props = GetProps().ToDictionary(x => x.Name, x => x);
            foreach (var sv in store.GetKeys().Where(x => props.ContainsKey(x)))
            {
                try
                {
                    var v = store.Get(props[sv].PropertyType, sv);
                    props[sv].SetValue(null, v);
                }
                catch (Exception ex)
                {
                    Log.Warning(GetType().Assembly.FullName, $"Setting could not be set: {sv}", ex);
                }
            }
        }

        public void StoreValues(ISettingsStore store)
        {
            foreach (var pi in GetProps())
            {
                store.Set(pi.Name, pi.GetValue(null));
            }
        }
    }
}