using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class DeselectFace : ChangeFaceSelection
    {
        public DeselectFace(IEnumerable<Face> objects) : base(new Face[0], objects)
        {
        }

        public DeselectFace(params Face[] objects) : base(new Face[0], objects)
        {
        }
    }
}