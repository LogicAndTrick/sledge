using System.Collections.Generic;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;

namespace Sledge.BspEditor.Editing.Problems
{
    public interface IProblemCheck
    {
        IEnumerable<Problem> Check(MapDocument document, bool visibleOnly);
        IOperation Fix(Problem problem);
    }
}
