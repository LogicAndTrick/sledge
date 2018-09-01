using System.ComponentModel.Composition;
using System.Numerics;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class Origin : IMapObjectData, ITransformable
    {
        public Vector3 Location { get; set; }

        public Origin(Vector3 location)
        {
            Location = location;
        }

        public Origin(SerialisedObject obj)
        {
            Location = obj.Get<Vector3>("Location");
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

        public void Transform(Matrix4x4 matrix)
        {
            Location = Vector3.Transform(Location, matrix);
        }
    }
}