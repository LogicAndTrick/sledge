using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Selection
{
    public class SelectFace : ChangeFaceSelection
    {
        public SelectFace(IEnumerable<Face> objects) : base(objects, new Face[0])
        {
        }

        public SelectFace(params Face[] objects) : base(objects, new Face[0])
        {
        }
    }
}