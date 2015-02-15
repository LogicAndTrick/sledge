using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;

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