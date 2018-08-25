using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Tools.Vertex.Selection
{
    public class MutableSolid
    {
        public List<MutableFace> Faces { get; }
        public Box BoundingBox => new Box(Faces.SelectMany(x => x.Vertices.Select(v => v.Position)));

        public MutableSolid(Solid solid)
        {
            Faces = solid.Faces.Select(x => new MutableFace(x)).ToList();
        }
    }
}