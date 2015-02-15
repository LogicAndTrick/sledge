using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    /// <summary>
    /// Perform: Adds the given objects to the map, selecting them if required.
    /// Reverse: Removes the objects from the map, deselecting if required.
    /// </summary>
    public class Create : CreateEditDelete
    {
        public Create(long parentId, IEnumerable<MapObject> objects)
        {
            Create(parentId, objects);
        }

        public Create(long parentId, params MapObject[] objects)
        {
            Create(parentId, objects);
        }
    }
}