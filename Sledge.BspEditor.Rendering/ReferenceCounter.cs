using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.BspEditor.Rendering
{
    public class ReferenceCounter<T> : IEnumerable<T>
    {
        private readonly Dictionary<T, int> _counter;

        public ReferenceCounter()
        {
            _counter = new Dictionary<T, int>();
        }

        public void Increment(T instance)
        {
            if (_counter.ContainsKey(instance)) _counter[instance]++;
            else _counter[instance] = 1;
        }

        public void Increment(IEnumerable<T> instances)
        {
            foreach (var i in instances) Increment(i);
        }

        public bool Decrement(T instance)
        {
            if (_counter.ContainsKey(instance))
            {
                _counter[instance]--;
                if (_counter[instance] <= 0)
                {
                    _counter.Remove(instance);
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<T> Decrement(IEnumerable<T> instances)
        {
            return instances.Where(Decrement).ToList(); // tolist to evaluate immediately
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _counter.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}