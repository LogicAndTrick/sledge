using System;

namespace Sledge.Common.Shell.Settings
{
    /// <summary>
    /// An editor interface for a setting key.
    /// </summary>
    public interface ISettingEditor : IDisposable
    {
        /// <summary>
        /// Fires when the setting value has changed
        /// </summary>
        event EventHandler<SettingKey> OnValueChanged;
        
        /// <summary>
        /// Gets or sets the label for the setting
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Gets or sets the value of the setting
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// The control for the editor. The control will be hosted in the settings form.
        /// </summary>
        object Control { get; }

        /// <summary>
        /// Gets or sets the key for the setting. Editor hints should be applied when setting this value.
        /// </summary>
        SettingKey Key { get; set; }
    }
}