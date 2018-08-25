using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Tools.Vertex.Selection
{
    public class VertexSolid
    {
        public Solid Real { get; set; }
        public MutableSolid Copy { get; set; }
        public bool IsDirty { get; set; }

        public VertexSolid(Solid solid)
        {
            Real = solid;
            Copy = new MutableSolid(solid);
        }

        public void Reset()
        {
            Copy = new MutableSolid(Real);
            IsDirty = false;
        }
    }
}