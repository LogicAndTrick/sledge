using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Graphics.Arrays
{
    public class ArraySpecification
    {
        public List<ArrayIndex> Indices { get; private set; }
        public int Stride { get; private set; }

        public ArraySpecification(params ArrayIndex[] indices)
        {
            Indices = indices.ToList();
            Stride = indices.Sum(x => x.Size);
            var offset = 0;
            foreach (var ai in indices)
            {
                ai.Offset = offset;
                offset += ai.Size;
            }
        }
    }
}
