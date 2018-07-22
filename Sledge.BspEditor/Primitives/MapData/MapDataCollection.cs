using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Sledge.BspEditor.Primitives.MapData
{
    /// <summary>
    /// Collection of metadata for a map
    /// </summary>
    public class MapDataCollection : IEnumerable<IMapData>, ISerializable, INotifyPropertyChanged
    {
        public List<IMapData> Data { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MapDataCollection()
        {
            Data = new List<IMapData>();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Data", Data.ToArray());
        }

        public void Add(IMapData data)
        {
            Data.Add(data);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapData." + data.GetType().Name));
        }

        public void AddRange(IEnumerable<IMapData> data)
        {
            var l = data.ToList();
            Data.AddRange(l);
            foreach (var d in l.Select(x => x.GetType().Name).Distinct())
            {

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapData." + d));
            }
        }

        public void Replace<T>(T data) where T : IMapData
        {
            Data.RemoveAll(x => x is T);
            Data.Add(data);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapData." + typeof(T).Name));
        }

        public void Remove(IMapData data)
        {
            Data.Remove(data);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapData." + data.GetType().Name));
        }

        public int Remove(Predicate<IMapData> test)
        {
            var ra = Data.RemoveAll(test);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapData"));
            return ra;
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