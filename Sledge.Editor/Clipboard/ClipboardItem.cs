using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.Clipboard
{
    public class ClipboardItem
    {
        public List<MapObject> CopiedObjects { get; set; }
        public string StringRepresentation { get; private set; }

        /// <summary>
        /// Create a clipboard item for the given objects.
        /// </summary>
        /// <param name="copiedObjects">The objects to copy. Clones of these will be created, rather than references to the same objects.</param>
        public ClipboardItem(IEnumerable<MapObject> copiedObjects)
        {
            CopiedObjects = copiedObjects.Select(x => x.Clone()).ToList();
            StringRepresentation = ""; // TODO VMF
        }
    }
}