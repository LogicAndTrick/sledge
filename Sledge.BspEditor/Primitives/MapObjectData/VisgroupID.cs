using System.Runtime.Serialization;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class VisgroupID : IMapObjectData
    {
        public long ID { get; set; }

        public VisgroupID(long id)
        {
            ID = id;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
        }

        public IMapObjectData Clone()
        {
            return new VisgroupID(ID);
        }
    }
}