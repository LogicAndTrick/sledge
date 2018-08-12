using System.Collections.Concurrent;

namespace Sledge.BspEditor.Primitives
{
    /// <summary>
    /// A class that generates unique numbers
    /// </summary>
    public class UniqueNumberGenerator
    {
        public static readonly UniqueNumberGenerator Instance = new UniqueNumberGenerator();

        private readonly ConcurrentDictionary<string, long> _ids;

        public UniqueNumberGenerator()
        {
            _ids = new ConcurrentDictionary<string, long>();
        }

        public long Next(string type)
        {
            lock (_ids)
            {
                var current = _ids.ContainsKey(type) ? _ids[type] : 0;
                var next = current + 1;
                _ids[type] = next;
                return next;
            }
        }

        public void Seed(string type, long id)
        {
            if (id < 1) return;

            lock (_ids)
            {
                var current = _ids.ContainsKey(type) ? _ids[type] : 0;
                if (id > current) _ids[type] = id;
            }
        }

        public void Reset(string type)
        {
            lock (_ids)
            {
                long o;
                _ids.TryRemove(type, out o);
            }
        }

        public void Reset()
        {
            lock (_ids)
            {
                _ids.Clear();
            }
        }
    }
}