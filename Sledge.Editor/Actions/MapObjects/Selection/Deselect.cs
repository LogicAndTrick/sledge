using System.Collections.Generic;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class Deselect : ChangeSelection
    {
        public Deselect(IEnumerable<MapObject> objects) : base(new MapObject[0], objects)
        {
        }
    }
}