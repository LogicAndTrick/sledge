using System.Collections.Generic;
using System.Linq;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    /// <summary>
    /// Collection of metadata for an object
    /// </summary>
    public class MapObjectDataCollection
    {
        public HashSet<int> Visgroups { get; }
        public List<IMapObjectData> Data { get; }

        public MapObjectDataCollection()
        {
            Data = new List<IMapObjectData>();
            Visgroups = new HashSet<int>();
        }

        public void Add(IMapObjectData data)
        {
            Data.Add(data);
        }

        public void AddRange(IEnumerable<IMapObjectData> data)
        {
            Data.AddRange(data);
        }

        public void Remove(IMapObjectData data)
        {
            Data.Remove(data);
        }

        public IEnumerable<T> Get<T>() where T : IMapObjectData
        {
            return Data.OfType<T>();
        }

        public T GetOne<T>() where T : IMapObjectData
        {
            return Data.OfType<T>().FirstOrDefault();
        }
    }
}