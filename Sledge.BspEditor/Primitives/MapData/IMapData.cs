using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapData
{
    /// <summary>
    /// Base interface for generic map metadata
    /// </summary>
    public interface IMapData : ISerializable, IMapElement
    {
    }
}