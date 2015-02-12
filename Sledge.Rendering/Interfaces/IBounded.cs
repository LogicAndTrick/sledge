

using Sledge.Rendering.DataStructures;

namespace Sledge.Rendering.Interfaces
{
    public interface IBounded : IOrigin
    {
        Box BoundingBox { get; }
    }
}