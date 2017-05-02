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

        /// <summary>
        /// Creates a deep copy of this map element with new IDs if required.
        /// The clone will be detached from the tree (if applicable).
        /// </summary>
        /// <param name="numberGenerator">A number generator to generate the new ID</param>
        /// <returns>A copy of this map element</returns>
        IMapElement Copy(UniqueNumberGenerator numberGenerator);

        /// <summary>
        /// Create an exact deep clone of this map element.
        /// The clone will be detached from the tree (if applicable).
        /// </summary>
        /// <returns>A clone of this map element</returns>
        IMapElement Clone();
    }
}