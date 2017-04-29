using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    public interface IMapElement
    {
        /// <summary>
        /// Convert this object into a serialised object
        /// </summary>
        /// <returns></returns>
        SerialisedObject ToSerialisedObject();
    }
}