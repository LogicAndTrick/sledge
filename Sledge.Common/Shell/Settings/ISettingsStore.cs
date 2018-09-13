using System;
using System.Collections.Generic;

namespace Sledge.Common.Shell.Settings
{
    /// <summary>
    /// A store which holds setting keys and values
    /// </summary>
    public interface ISettingsStore
    {
        /// <summary>
        /// Get all the keys in this store
        /// </summary>
        /// <returns>The list of keys</returns>
        IEnumerable<string> GetKeys();

        /// <summary>
        /// Test if a key exists in this store
        /// </summary>
        /// <param name="key">The key to test</param>
        /// <returns>True if the key exists in this store</returns>
        bool Contains(string key);

        /// <summary>
        /// Get a value from the store
        /// </summary>
        /// <param name="type">The value's data type</param>
        /// <param name="key">The key to fetch</param>
        /// <param name="defaultValue">The default value to use if the data couldn't be fetched</param>
        /// <returns>The value from the store, or the default value if the key wasn't found or was the wrong type</returns>
        object Get(Type type, string key, object defaultValue = null);

        /// <summary>
        /// Get a value from the store
        /// </summary>
        /// <typeparam name="T">The value's data type</typeparam>
        /// <param name="key">The key to fetch</param>
        /// <param name="defaultValue">The default value to use if the data couldn't be fetched</param>
        /// <returns>The value from the store, or the default value if the key wasn't found or was the wrong type</returns>
        T Get<T>(string key, T defaultValue = default(T));

        /// <summary>
        /// Set a value in the store
        /// </summary>
        /// <typeparam name="T">The value's data type</typeparam>
        /// <param name="key">The key to save</param>
        /// <param name="value">The value to store</param>
        void Set<T>(string key, T value);

        /// <summary>
        /// Delete a key from the store
        /// </summary>
        /// <param name="key">The key to delete</param>
        void Delete(string key);
    }
}