using System.Linq;
using OpenTK;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.DataStructures
{
    public class Octree<T> : OctreeNode<T> where T : IBounded
    {
        public Octree(float worldSize = 32768, int limit = 100)
            : base(null, null, new Box(-Vector3.One * worldSize, Vector3.One * worldSize), limit)
        {
            Root = this;
        }
    }
}
