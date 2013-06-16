using System.Collections.Generic;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;

namespace Sledge.Editor.Problems
{
    public class InvalidSolids : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map)
        {
            foreach (var invalid in map.WorldSpawn.Find(x => x is Solid && !((Solid)x).IsValid()))
            {
                yield return new Problem(GetType(), new[] { invalid }, Fix, "Invalid solid", "This solid is invalid. It is either not convex, has coplanar faces, or has off-plane vertices. Fixing the issue will delete the solid.");
            }
        }

        public IAction Fix(Problem problem)
        {
            throw new System.NotImplementedException();
        }
    }
}