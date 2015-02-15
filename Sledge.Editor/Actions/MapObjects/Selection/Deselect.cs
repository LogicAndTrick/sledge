using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class Deselect : ChangeSelection
    {
        public Deselect(IEnumerable<MapObject> objects) : base(new MapObject[0], objects)
        {
        }
    }
}