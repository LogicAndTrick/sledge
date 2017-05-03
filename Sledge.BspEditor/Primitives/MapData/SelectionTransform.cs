using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapData
{
    public class SelectionTransform : IMapData
    {
        public Matrix Transform { get; set; }

        public SelectionTransform() : this(Matrix.Identity)
        {
            
        }
    
        public SelectionTransform(Matrix transform)
        {
            Transform = transform;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Transform", Transform);
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("SelectionTransform");
            so.Set("Transform", Transform);
            return so;
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public IMapElement Clone()
        {
            return new SelectionTransform(Transform);
        }
    }
}