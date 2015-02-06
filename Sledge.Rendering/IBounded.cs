using Sledge.DataStructures.Geometric;

namespace Sledge.Rendering
{
    public interface IBounded : IOrigin
    {
        Box BoundingBox { get; }
    }
}