using System.Collections.Generic;
using Sledge.BspEditor.Tools.Vertex.Selection;

namespace Sledge.BspEditor.Tools.Vertex.Errors
{
    public interface IVertexErrorCheck
    {
        IEnumerable<VertexError> GetErrors(VertexSolid solid);
    }
}
