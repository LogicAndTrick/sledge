using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Numerics;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class EntityData : IMapObjectData
    {
        public string Name { get; set; }
        public int Flags { get; set; }
        public Dictionary<string, string> Properties { get; set; }

        public EntityData()
        {
            Name = "";
            Properties = new Dictionary<string, string>();
        }

        public EntityData(SerialisedObject obj)
        {
            Name = "";
            Properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var prop in obj.Properties)
            {
                if (prop.Key == "Name") Name = prop.Value;
                else if (prop.Key == "Flags") Flags = Convert.ToInt32(prop.Value, CultureInfo.InvariantCulture);
                else Properties[prop.Key] = prop.Value;
            }
        }

        [Export(typeof(IMapElementFormatter))]
        public class ActiveTextureFormatter : StandardMapElementFormatter<EntityData> { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Properties", Properties);
        }

        public Vector3? GetVector3(string key)
        {
            if (!Properties.ContainsKey(key)) return null;

            var spl = (Properties[key] ?? "").Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (spl.Length < 3) return null;

            if (float.TryParse(spl[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
                && float.TryParse(spl[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)
                && float.TryParse(spl[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
            {
                return new Vector3(x, y, z);
            }
            return null;
        }

        public T Get<T>(string key, T defaultValue = default(T))
        {
            if (!Properties.ContainsKey(key)) return defaultValue;
            try
            {
                var val = Properties[key];
                var conv = TypeDescriptor.GetConverter(typeof(T));
                return (T)conv.ConvertFromString(null, CultureInfo.InvariantCulture, val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public void Set<T>(string key, T value)
        {
            var conv = TypeDescriptor.GetConverter(typeof(T));
            var v = conv.ConvertToString(null, CultureInfo.InvariantCulture, value);
            Properties[key] = v;
        }

        public void Unset(string key)
        {
            if (Properties.ContainsKey(key)) Properties.Remove(key);
        }

        public IMapElement Clone()
        {
            var ed = new EntityData();
            ed.Name = Name;
            ed.Flags = Flags;
            ed.Properties = new Dictionary<string, string>(Properties);
            return ed;
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("EntityData");
            foreach (var p in Properties)
            {
                so.Set(p.Key, p.Value);
            }
            so.Set("Name", Name);
            so.Set("Flags", Flags);
            return so;
        }
    }
}