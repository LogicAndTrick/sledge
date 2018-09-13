using System.Collections.Generic;

namespace Sledge.Common.Shell.Settings
{
    /// <summary>
    /// Represents a class that holds settings
    /// </summary>
    public interface ISettingsContainer
    {
        /// <summary>
        /// The unique name of the settings container
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Get a list of editable keys exposed by this container.
        /// The container does not have to expose all keys via this method,
        /// only ones that should appear in the settings form.
        /// </summary>
        /// <returns>The editable keys for this container</returns>
        IEnumerable<SettingKey> GetKeys();

        /// <summary>
        /// Load values from the store into this instance
        /// </summary>
        /// <param name="store">The store to load values from</param>
        void LoadValues(ISettingsStore store);

        /// <summary>
        /// Store values from this instance into the store
        /// </summary>
        /// <param name="store">The store to save into</param>
        void StoreValues(ISettingsStore store);
    }
}