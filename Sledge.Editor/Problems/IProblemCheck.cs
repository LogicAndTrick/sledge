using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;

namespace Sledge.Editor.Problems
{
    public interface IProblemCheck
    {
        IEnumerable<Problem> Check(Map map, bool visibleOnly);
        IAction Fix(Problem problem);
    }
}
