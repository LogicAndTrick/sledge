using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Tools.Vertex.Selection
{
    public class VertexHidden : IMapObjectData, IRenderVisibility
    {
        public bool IsRenderHidden => true;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Not serialisable
        }

        public SerialisedObject ToSerialisedObject()
        {
            // Not serialisable
            return null;
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public IMapElement Clone()
        {
            return new VertexHidden();
        }

    }
}
