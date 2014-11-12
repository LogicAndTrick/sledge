using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;
using Sledge.EditorNew.Actions;

namespace Sledge.EditorNew.Problems
{
    public interface IProblemCheck
    {
        IEnumerable<Problem> Check(Map map, bool visibleOnly);
        IAction Fix(Problem problem);
    }
}
