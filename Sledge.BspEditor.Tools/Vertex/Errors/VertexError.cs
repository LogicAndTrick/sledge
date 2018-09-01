using System.Collections.Generic;
using Sledge.BspEditor.Tools.Vertex.Selection;

namespace Sledge.BspEditor.Tools.Vertex.Errors
{
    public class VertexError
    {
        public string Key { get; set; }
        public VertexSolid Solid { get; set; }
        public List<MutableFace> Faces { get; set; }
        public List<MutableVertex> Points { get; set; }

        public VertexError(string key, VertexSolid solid)
        {
            Key = key;
            Solid = solid;
            Faces = new List<MutableFace>();
            Points = new List<MutableVertex>();
        }

        public VertexError Add(params MutableFace[] faces)
        {
            Faces.AddRange(faces);
            return this;
        }

        public VertexError Add(IEnumerable<MutableFace> faces)
        {
            Faces.AddRange(faces);
            return this;
        }

        public VertexError Add(params MutableVertex[] points)
        {
            Points.AddRange(points);
            return this;
        }

        public VertexError Add(IEnumerable<MutableVertex> points)
        {
            Points.AddRange(points);
            return this;
        }
    }
}