using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    /// <summary>
    /// Collection of metadata for an object
    /// </summary>
    public class MapObjectDataCollection : IEnumerable<IMapObjectData>, ISerializable, INotifyPropertyChanged
    {
        public List<IMapObjectData> Data { get; }

        public event PropertyChangedEventHandler PropertyChanged;

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data." + data.GetType().Name));
        }

        public void Replace<T>(T data) where T : IMapObjectData
        {
            Data.RemoveAll(x => x is T);
            Data.Add(data);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data." + typeof(T).Name));
        }

        public void AddRange(IEnumerable<IMapObjectData> data)
        {
            var l = data.ToList();
            Data.AddRange(l);
            foreach (var d in l.Select(x => x.GetType().Name).Distinct())
            {

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data." + d));
            }
        }

        public void Remove(IMapObjectData data)
        {
            Data.Remove(data);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data." + data.GetType().Name));
        }

        public int Remove(Predicate<IMapObjectData> test)
        {
            var ra = Data.RemoveAll(test);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data"));
            return ra;
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