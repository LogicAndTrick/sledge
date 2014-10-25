using System.Collections.Generic;

namespace Sledge.Gui.Structures
{
    public class ContainerMetadata : Dictionary<string, object>
    {
        public T Get<T>(string name, T defaultValue = default (T))
        {
            if (!ContainsKey(name)) return defaultValue;
            var v = this[name];
            if (!(v is T)) return defaultValue;
            return (T) v;
        }
    }
}