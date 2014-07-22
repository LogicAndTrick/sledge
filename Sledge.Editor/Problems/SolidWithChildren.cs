using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;

namespace Sledge.Editor.Problems
{
    public class SolidWithChildren : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map, bool visibleOnly)
        {
            foreach (var solid in map.WorldSpawn
                .Find(x => x is Solid && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)))
                .OfType<Solid>()
                .Where(x => x.HasChildren))
            {
                yield return new Problem(GetType(), map, new[] { solid }, Fix, "Solid has children", "A solid with children was found. A solid cannot have any contents. Fixing the issue will move the children outside of the solid's group.");
            }
        }

        public IAction Fix(Problem problem)
        {
            return new Reparent(problem.Objects[0].Parent.ID, problem.Objects[0].GetChildren());
        }
    }
}