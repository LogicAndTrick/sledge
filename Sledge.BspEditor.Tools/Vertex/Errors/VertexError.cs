using System.Collections.Generic;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Tools.Vertex.Selection;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Tools.Vertex.Errors
{
    public class VertexError
    {
        public string Key { get; set; }
        public VertexSolid Solid { get; set; }
        public List<Face> Faces { get; set; }
        public List<Coordinate> Points { get; set; }

        public VertexError(string key, VertexSolid solid)
        {
            Key = key;
            Solid = solid;
            Faces = new List<Face>();
            Points = new List<Coordinate>();
        }
    }
}