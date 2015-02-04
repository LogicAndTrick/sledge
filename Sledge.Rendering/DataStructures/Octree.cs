using System.Text;
using Sledge.DataStructures.Geometric;

namespace Sledge.Rendering.DataStructures
{
    public class Octree : OctreeNode
    {
        public Octree(decimal worldSize, int limit = 100) : base(null, null, new Box(-Coordinate.One * worldSize, Coordinate.One * worldSize), limit)
        {
            Root = this;
        }
    }
}
