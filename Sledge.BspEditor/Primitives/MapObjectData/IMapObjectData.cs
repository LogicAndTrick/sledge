using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Primitives.MapObjectData
{
    /// <summary>
    /// Base interface for generic map object metadata
    /// </summary>
    public interface IMapObjectData : ISerializable, IMapElement
    {

    }
}