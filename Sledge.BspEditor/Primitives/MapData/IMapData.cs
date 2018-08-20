using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Primitives.MapData
{
    /// <summary>
    /// Base interface for generic map metadata
    /// </summary>
    public interface IMapData : ISerializable, IMapElement
    {
        bool AffectsRendering { get; }
    }
}