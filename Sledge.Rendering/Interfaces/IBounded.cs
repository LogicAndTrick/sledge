using Sledge.DataStructures.Geometric;

namespace Sledge.Rendering.Interfaces
{
    public interface IBounded : IOrigin
    {
        Box BoundingBox { get; }
    }
}