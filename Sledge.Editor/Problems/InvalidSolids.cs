using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;

namespace Sledge.Editor.Problems
{
    public class InvalidSolids : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map, bool visibleOnly)
        {
            foreach (var invalid in map.WorldSpawn.Find(x => x is Solid && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)) && !((Solid)x).IsValid()))
            {
                yield return new Problem(GetType(), map, new[] { invalid }, Fix, "Invalid solid", "This solid is invalid. It is either not convex, has coplanar faces, or has off-plane vertices. Fixing the issue will delete the solid.");
            }
        }

        public IAction Fix(Problem problem)
        {
            return new Delete(problem.Objects.Select(x => x.ID));
        }
    }
}