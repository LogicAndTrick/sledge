using System.Runtime.Serialization;

namespace Sledge.BspEditor.Primitives.MapData
{
    /// <summary>
    /// Base interface for generic map metadata
    /// </summary>
    public interface IMapData : ISerializable
    {
        IMapData Clone();
    }
}