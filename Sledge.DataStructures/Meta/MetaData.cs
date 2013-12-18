using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.DataStructures.Meta
{
    public class MetaData
    {
        private readonly Dictionary<string, object> _data;

        public MetaData()
        {
            _data = new Dictionary<string, object>();
        }

        public void Set<T>(string key, T value)
        {
            _data[key] = value;
        }

        public void Unset(string key)
        {
            _data.Remove(key);
        }

        public T Get<T>(string key)
        {
            if (!_data.ContainsKey(key)) return default(T);
            var v = _data[key];
            if (v is T) return (T)v;
            return default(T);
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _data.Values.OfType<T>();
        }

        public bool Has<T>(string key)
        {
            return _data.ContainsKey(key) && _data[key] is T;
        }

        public MetaData Clone()
        {
            var m = new MetaData();
            foreach (var kv in _data) m._data.Add(kv.Key, kv.Value);
            return m;
        }
    }
}
