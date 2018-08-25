using System.Numerics;

namespace Sledge.BspEditor.Tools.Vertex.Selection
{
    public class MutableVertex
    {
        public Vector3 Position { get; private set; }

        public MutableVertex(Vector3 position)
        {
            Position = position;
        }

        public void Set(Vector3 position)
        {
            Position = position;
        }
    }
}
