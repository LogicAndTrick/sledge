using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    /// <summary>
    /// Perform: Adds the given objects to the map, selecting them if required.
    /// Reverse: Removes the objects from the map, deselecting if required.
    /// </summary>
    public class Create : CreateEditDelete
    {
        public Create(IEnumerable<MapObject> objects)
        {
            Create(objects);
        }

        public Create(params MapObject[] objects)
        {
            Create(objects);
        }
    }
}