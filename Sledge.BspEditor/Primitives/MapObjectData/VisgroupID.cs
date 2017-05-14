using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class VisgroupID : IMapObjectData, IObjectVisibility
    {
        public long ID { get; set; }
        public bool IsHidden { get; set; }

        public VisgroupID(long id)
        {
            ID = id;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
        }

        public IMapElement Clone()
        {
            return new VisgroupID(ID);
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("VisgroupID");
            so.Set("ID", ID);
            return so;
        }
    }
}