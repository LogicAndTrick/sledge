using System.Collections.Generic;

namespace Sledge.Common.Shell.Commands
{
    public class CommandParameters
    {
        private readonly Dictionary<string, object> _parameters;

        public CommandParameters()
        {
            _parameters = new Dictionary<string, object>();
        }

        public void Add(string key, object value)
        {
            _parameters[key] = value;
        }

        public T Get<T>(string key, T defaultValue = default(T))
        {
            return _parameters.ContainsKey(key) && _parameters[key] is T ? (T) _parameters[key] : defaultValue;
        }
    }
}