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
                yield return new SettingKey(pi.Name, "", pi.PropertyType);
            }
        }

        public void SetValues(IEnumerable<SettingValue> values)
        {
            var props = GetProps().ToDictionary(x => x.Name, x => x);
            foreach (var sv in values.Where(x => props.ContainsKey(x.Name)))
            {
                try
                {
                    props[sv.Name].SetValue(null, Convert.ChangeType(sv.Value, props[sv.Name].PropertyType));
                }
                catch (Exception ex)
                {
                    Log.Warning(GetType().Assembly.FullName, $"Setting could not be set: {sv.Name}", ex);
                }
            }
        }

        public IEnumerable<SettingValue> GetValues()
        {
            foreach (var pi in GetProps())
            {
                yield return new SettingValue(pi.Name, Convert.ToString(pi.GetValue(null)));
            }
        }
    }
}