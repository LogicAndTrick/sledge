using System.Drawing;
using System.Runtime.Serialization;

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
    }
}