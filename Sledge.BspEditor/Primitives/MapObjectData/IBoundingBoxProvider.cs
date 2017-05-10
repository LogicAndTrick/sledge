using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public interface IBoundingBoxProvider : IMapObjectData
    {
        Box GetBoundingBox(IMapObject obj);
    }
}
