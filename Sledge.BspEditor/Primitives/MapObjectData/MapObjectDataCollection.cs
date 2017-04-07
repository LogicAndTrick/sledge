using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    /// <summary>
    /// Collection of metadata for an object
    /// </summary>
    public class MapObjectDataCollection : ISerializable
    {
        public List<IMapObjectData> Data { get; }

        public MapObjectDataCollection()
        {
            Data = new List<IMapObjectData>();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Data", Data.ToArray());
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

        public MapObjectDataCollection Clone()
        {
            var copy = new MapObjectDataCollection();
            foreach (var d in Data)
            {
                copy.Add(d.Clone());
            }
            return copy;
        }
    }
}