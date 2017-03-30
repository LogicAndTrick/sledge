using System;
using System.Collections.Generic;
using Sledge.BspEditor.Primitives.MapObjectData;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    /// <summary>
    /// The base interface of all map objects.
    /// </summary>
    public interface IMapObject : IEquatable<IMapObject>
    {
        long ID { get; }
        MapObjectDataCollection Data { get; }

        MapObjectHierarchy Hierarchy { get; }

        /// <summary>
        /// Create an exact copy of this object, detached from the tree.
        /// Parent will be null. Children will be clones. The ID will be the same as the original.
        /// </summary>
        /// <returns>A detached clone</returns>
        IMapObject Clone();

        /// <summary>
        /// Force an object to be updated from a source object.
        /// Parent will remain unchanged. ID remains unchanged. Children will be replaced with clones. 
        /// </summary>
        /// <param name="obj">The object to retrieve data from</param>
        void Unclone(IMapObject obj);

        void DescendantsChanged();

        IEnumerable<IPrimitive> ToPrimitives();
    }
}