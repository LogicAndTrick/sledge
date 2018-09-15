using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.Common.Threading;

namespace Sledge.BspEditor.Primitives.MapData
{
    /// <summary>
    /// Collection of metadata for a map
    /// </summary>
    public class MapDataCollection : IEnumerable<IMapData>, ISerializable
    {
        private ThreadSafeList<IMapData> Data { get; }

        public MapDataCollection()
        {
            Data = new ThreadSafeList<IMapData>();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Data", Data.ToArray());
        }

        public void Add(IMapData data)
        {
            Data.Add(data);
        }

        public void AddRange(IEnumerable<IMapData> data)
        {
            Data.AddRange(data);
        }

        public void Replace<T>(T data) where T : IMapData
        {
            Data.RemoveAll(x => x is T);
            Data.Add(data);
        }

        public void Remove(IMapData data)
        {
            Data.Remove(data);
        }

        public IEnumerable<T> Get<T>() where T : IMapData
        {
            return Data.OfType<T>();
        }

        public T GetOne<T>() where T : IMapData
        {
            return Data.OfType<T>().FirstOrDefault();
        }

        public MapDataCollection Clone()
        {
            var copy = new MapDataCollection();
            foreach (var d in Data)
            {
                copy.Add((IMapData) d.Clone());
            }
            return copy;
        }

        public MapDataCollection Copy(UniqueNumberGenerator numberGenerator)
        {
            var copy = new MapDataCollection();
            foreach (var d in Data)
            {
                copy.Add((IMapData) d.Copy(numberGenerator));
            }
            return copy;
        }

        public void Clear()
        {
            Data.Clear();
        }

        public IEnumerator<IMapData> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}