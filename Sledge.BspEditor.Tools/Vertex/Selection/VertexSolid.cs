using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Tools.Vertex.Selection
{
    public class VertexSolid
    {
        public Solid Real { get; set; }
        public Solid Copy { get; set; }
        public bool IsDirty { get; set; }

        public VertexSolid(Solid solid)
        {
            Real = solid;
            Copy = (Solid) solid.Clone();
            Copy.Data.Remove(x => x is VertexHidden);
        }
    }
}