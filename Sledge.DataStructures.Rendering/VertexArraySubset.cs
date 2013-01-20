using Sledge.Common;
using Sledge.Graphics.Shaders;

namespace Sledge.DataStructures.Rendering
{
    /// <summary>
    /// A subset is a range of indices for a solid vertex array.
    /// </summary>
    public class VertexArraySubset<T>
    {
        public T Instance { get; private set; }
        public int Start { get; private set; }
        public int Count { get; private set; }

        public VertexArraySubset(T instance, int start, int count)
        {
            Instance = instance;
            Start = start;
            Count = count;
        }
    }
}