using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class PointEntityGameDataBoundingBoxProvider : IBoundingBoxProvider
    {
        private readonly GameData _data;

        public PointEntityGameDataBoundingBoxProvider(GameData data)
        {
            _data = data;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Meh
        }

        public IMapElement Clone()
        {
            return new PointEntityGameDataBoundingBoxProvider(_data);
        }

        public Box GetBoundingBox(IMapObject obj)
        {
            // Try and get a bounding box for point entities
            var name = obj.Data.GetOne<EntityData>()?.Name;
            var origin = obj.Data.GetOne<Origin>()?.Location ?? Coordinate.Zero;
            if (name == null) return null;

            // Get the class (must be point)
            var cls = _data.Classes.FirstOrDefault(x => String.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase) && x.ClassType == ClassType.Point);
            if (cls == null) return null;

            // Default to 16
            var sub = new Coordinate(-16, -16, -16);
            var add = new Coordinate(16, 16, 16);

            // Get the size behaviour
            var behav = cls.Behaviours.SingleOrDefault(x => x.Name == "size");
            if (behav != null && behav.Values.Count >= 6)
            {
                sub = behav.GetCoordinate(0);
                add = behav.GetCoordinate(1);
            }
            else if (cls.Name == "infodecal")
            {
                // Special handling for infodecal if it's not specified
                sub = Coordinate.One * -4;
                add = Coordinate.One * 4;
            }
            return new Box(origin + sub, origin + add);
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject(nameof(PointEntityGameDataBoundingBoxProvider));
            return so;
        }
    }
}