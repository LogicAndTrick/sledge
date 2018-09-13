using System.Reflection;

namespace Sledge.Common.Shell.Settings
{
    /// <summary>
    /// Common extensions for settings stores
    /// </summary>
    public static class SettingStoreExtensions
    {
        /// <summary>
        /// Store fields and properties from an instance marked with <see cref="SettingAttribute" /> into the store.
        /// </summary>
        /// <param name="store">The settings store</param>
        /// <param name="instance">The instance to store</param>
        public static void StoreInstance(this ISettingsStore store, object instance)
        {
            if (instance == null) return;
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            foreach (var mem in instance.GetType().GetMembers(flags))
            {
                var sa = mem.GetCustomAttribute<SettingAttribute>();
                if (sa == null) continue;

                var name = sa.SettingName ?? mem.Name;

                var prop = mem as PropertyInfo;
                var field = mem as FieldInfo;

                if (prop != null) store.Set(name, prop.GetValue(instance));
                else if (field != null) store.Set(name, field.GetValue(instance));
            }
        }

        /// <summary>
        /// Load values from the store into fields and properties in an instance marked with <see cref="SettingAttribute" />.
        /// </summary>
        /// <param name="store">The settings store</param>
        /// <param name="instance">The instance to load</param>
        public static void LoadInstance(this ISettingsStore store, object instance)
        {
            if (instance == null) return;
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            foreach (var mem in instance.GetType().GetMembers(flags))
            {
                var sa = mem.GetCustomAttribute<SettingAttribute>();
                if (sa == null) continue;

                var name = sa.SettingName ?? mem.Name;

                var prop = mem as PropertyInfo;
                var field = mem as FieldInfo;

                if (prop != null) prop.SetValue(instance, store.Get(prop.PropertyType, name, prop.GetValue(instance)));
                else if (field != null) field.SetValue(instance, store.Get(field.FieldType, name, field.GetValue(instance)));
            }
        }
    }
}