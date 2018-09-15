using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.Common.Threading;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    /// <summary>
    /// Collection of metadata for an object
    /// </summary>
    public class MapObjectDataCollection : IEnumerable<IMapObjectData>, ISerializable
    {
        private ThreadSafeList<IMapObjectData> Data { get; }

        public MapObjectDataCollection()
        {
            Data = new ThreadSafeList<IMapObjectData>();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Data", Data.ToArray());
        }

        public void Add(IMapObjectData data)
        {
            Data.Add(data);
        }

        public void Replace<T>(T data) where T : IMapObjectData
        {
            Data.RemoveAll(x => x is T);
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

        public int Remove(Predicate<IMapObjectData> test)
        {
            return Data.RemoveAll(test);
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
                copy.Add(d.Clone() as IMapObjectData);
            }
            return copy;
        }

        public MapObjectDataCollection Copy(UniqueNumberGenerator numberGenerator)
        {
            var copy = new MapObjectDataCollection();
            foreach (var d in Data)
            {
                copy.Add(d.Copy(numberGenerator) as IMapObjectData);
            }
            return copy;
        }

        public void Clear()
        {
            Data.Clear();
        }

        public IEnumerator<IMapObjectData> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}