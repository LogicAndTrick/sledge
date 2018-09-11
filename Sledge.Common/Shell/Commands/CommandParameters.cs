using System.Collections.Generic;

namespace Sledge.Common.Shell.Commands
{
    /// <summary>
    /// A collection of parameters for a command.
    /// </summary>
    public class CommandParameters
    {
        private readonly Dictionary<string, object> _parameters;

        /// <summary>
        /// Create an instance of the command parameters class
        /// </summary>
        public CommandParameters()
        {
            _parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Add a key and value to the parameters
        /// </summary>
        /// <param name="key">Parameter key</param>
        /// <param name="value">Parameter value</param>
        public void Add(string key, object value)
        {
            _parameters[key] = value;
        }

        /// <summary>
        /// Get a value from the parameters
        /// </summary>
        /// <typeparam name="T">Parameter type</typeparam>
        /// <param name="key">The key to get</param>
        /// <param name="defaultValue">The value to use if the key wasn't found or was the wrong type</param>
        /// <returns>The parameter value, or the default value if the parameter wasn't found</returns>
        public T Get<T>(string key, T defaultValue = default(T))
        {
            return _parameters.ContainsKey(key) && _parameters[key] is T ? (T) _parameters[key] : defaultValue;
        }
    }
}