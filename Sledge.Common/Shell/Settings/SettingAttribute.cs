using System;

namespace Sledge.Common.Shell.Settings
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SettingAttribute : Attribute
    {
        public string SettingName { get; set; }

        public SettingAttribute()
        {
        }

        public SettingAttribute(string settingName)
        {
            SettingName = settingName;
        }
    }
}