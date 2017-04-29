using System.Drawing;
using System.Runtime.Serialization;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class ObjectColor : IMapObjectData
    {
        public Color Color { get; set; }

        public ObjectColor(Color color)
        {
            Color = color;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Color", Color);
        }

        public IMapObjectData Clone()
        {
            return new ObjectColor(Color);
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("ObjectColor");
            so.SetColor("Color", Color);
            return so;
        }
    }
}