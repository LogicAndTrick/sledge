using System.Collections.Generic;
using System.Linq;

namespace Sledge.BspEditor.Primitives.MapData
{
    /// <summary>
    /// Collection of metadata for a map
    /// </summary>
    public class MapDataCollection
    {
        public List<IMapData> Data { get; }

        public MapDataCollection()
        {
            Data = new List<IMapData>();
        }

        public void Add(IMapData data)
        {
            Data.Add(data);
        }

        public void AddRange(IEnumerable<IMapData> data)
        {
            Data.AddRange(data);
        }

        public void Remove(IMapData data)
        {
            Data.Remove(data);
        }

        public IEnumerable<T> Get<T>() where T : IMapData
        {
            return Data.OfType<T>();
        }
    }
}