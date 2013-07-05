using System.Collections.Generic;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    /// <summary>
    /// Perform: Removes the given objects from the map, and from the selection if required.
    /// Reverse: Adds the given objects back to their original parents, and reselects them if required.
    /// </summary>
    public class Delete : CreateEditDelete
    {
        public Delete(IEnumerable<long> ids)
        {
            Delete(ids);
        }
    }
}
