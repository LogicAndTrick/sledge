using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class Origin : IMapObjectData, ITransformable
    {
        public Coordinate Location { get; set; }

        public Origin(Coordinate location)
        {
            Location = location;
        }

        public Origin(SerialisedObject obj)
        {
            Location = obj.Get<Coordinate>("Location");
        }

        [Export(typeof(IMapElementFormatter))]
        public class ActiveTextureFormatter : StandardMapElementFormatter<Origin> { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Location", Location);
        }

        public IMapElement Clone()
        {
            return new Origin(Location);
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("Origin");
            so.Set("Location", Location);
            return so;
        }

        public void Transform(Matrix matrix)
        {
            Location = Location * matrix;
        }
    }
}