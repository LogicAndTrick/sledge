using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Threading;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapData
{
    [Serializable]
    public class AutomaticVisgroup : IMapData
    {
        public bool AffectsRendering => false;

        public string Path { get; set; }
        public string Key { get; set; }
        public bool Visible { get; set; } = true;
        public Predicate<IMapObject> IsMatch { get; private set; }
        public ISet<IMapObject> Objects { get; set; }

        public AutomaticVisgroup(Predicate<IMapObject> isMatch)
        {
            IsMatch = isMatch;
            Objects = new ThreadSafeSet<IMapObject>();
        }

        public AutomaticVisgroup(SerialisedObject obj, Predicate<IMapObject> isMatch)
        {
            IsMatch = isMatch;
            Objects = new ThreadSafeSet<IMapObject>();
        }

        protected AutomaticVisgroup(SerializationInfo info, StreamingContext context)
        {
            Path = info.GetString("Path");
            Key = info.GetString("Key");
            Visible = info.GetBoolean("Visible");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Path", Path);
            info.AddValue("Key", Key);
            info.AddValue("Visible", Visible);
        }

        public virtual IMapElement Clone()
        {
            return new AutomaticVisgroup(IsMatch)
            {
                Path = Path,
                Key = Key,
                Visible = Visible
            };
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public SerialisedObject ToSerialisedObject()
        {
            return null;
        }
    }
}