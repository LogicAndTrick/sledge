using System;

namespace Sledge.Common.Shell.Settings
{
    /// <summary>
    /// A setting key and associated metadata
    /// </summary>
    public class SettingKey
    {
        /// <summary>
        /// The container for the key. This field is used when editing settings only.
        /// </summary>
        public string Container { get; set; }

        /// <summary>
        /// The settings group for this key.
        /// This field determines what page of the settings form the key appears on.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// The key of the setting.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// The data type of the setting.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// A hint to the settings form on what editor type this key requires.
        /// </summary>
        public string EditorType { get; set; }

        /// <summary>
        /// Additional data to pass to the editor.
        /// </summary>
        public string EditorHint { get; set; }

        /// <summary>
        /// Create a setting key
        /// </summary>
        /// <param name="group">The group the setting should appear in</param>
        /// <param name="key">The setting key</param>
        /// <param name="type">The data type of the setting</param>
        public SettingKey(string group, string key, Type type)
        {
            Group = group;
            Key = key;
            Type = type;
        }
    }
}
