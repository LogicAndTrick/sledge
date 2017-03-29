using System;
using System.Collections.Generic;
using Sledge.BspEditor.Primitives.MapObjectData;

namespace Sledge.BspEditor.Primitives
{
    /// <summary>
    /// The base interface of all map objects.
    /// </summary>
    public interface IMapObject : IEquatable<IMapObject>
    {
        long ID { get; }
        MapObjectDataCollection Data { get; }

        MapObjectHierarchy Hierarchy { get; }

        IMapObject Clone();
        void Unclone(IMapObject obj);

        void DescendantsChanged();

        IEnumerable<IPrimitive> ToPrimitives();
    }
}