using System.Runtime.Serialization;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.Transformations
{
    public interface IUnitTransformation : ISerializable
    {
        Coordinate Transform(Coordinate c);
    }
}
