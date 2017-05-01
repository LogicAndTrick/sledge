using System.Reflection;

namespace Sledge.Common.Shell.Settings
{
    public static class SettingStoreExtensions
    {
        public static void StoreInstance(this ISettingsStore store, object instance)
        {
            if (instance == null) return;
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
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

        public static void LoadInstance(this ISettingsStore store, object instance)
        {
            if (instance == null) return;
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
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