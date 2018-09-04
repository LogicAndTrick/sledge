using System;
using System.Linq;
using System.Numerics;
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
            var origin = obj.Data.GetOne<Origin>()?.Location ?? Vector3.Zero;
            if (name == null) return null;

            // Get the class (must be point)
            var cls = _data.Classes.FirstOrDefault(x => String.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase) && x.ClassType == ClassType.Point);
            if (cls == null) return null;

            // Default to 16x16
            var sub = new Vector3(-8, -8, -8);
            var add = new Vector3(8, 8, 8);

            // Get the size behaviour
            var behav = cls.Behaviours.SingleOrDefault(x => x.Name == "size");
            if (behav != null && behav.Values.Count >= 6)
            {
                sub = behav.GetVector3(0) ?? Vector3.Zero;
                add = behav.GetVector3(1) ?? Vector3.Zero;
            }
            else if (cls.Name == "infodecal")
            {
                // Special handling for infodecal if it's not specified
                sub = Vector3.One * -4;
                add = Vector3.One * 4;
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