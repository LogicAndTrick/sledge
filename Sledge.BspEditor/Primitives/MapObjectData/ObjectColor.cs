using System.Drawing;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class ObjectColor : IMapObjectData
    {
        public Color Color { get; set; }

        public ObjectColor(Color color)
        {
            Color = color;
        }

        public IMapObjectData Clone()
        {
            return new ObjectColor(Color);
        }
    }
}