using System;

namespace Sledge.Common.Shell.Settings
{
    /// <summary>
    /// Used to tag a field or property to be automatically saved or loaded
    /// via the StoreInstance and LoadInstance extension methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SettingAttribute : Attribute
    {
        /// <summary>
        /// The name of the setting. If not specified, the property/field name will be used.
        /// </summary>
        public string SettingName { get; }

        /// <summary>
        /// Tag a field as a setting, using the name of the field for the setting key.
        /// </summary>
        public SettingAttribute()
        {
        }

        /// <summary>
        /// Tag a field as a setting, using a custom name for the setting key.
        /// </summary>
        public SettingAttribute(string settingName)
        {
            SettingName = settingName;
        }
    }
}