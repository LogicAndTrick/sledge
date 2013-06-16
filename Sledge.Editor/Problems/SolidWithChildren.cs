using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;

namespace Sledge.Editor.Problems
{
    public class SolidWithChildren : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map)
        {
            foreach (var solid in map.WorldSpawn
                .Find(x => x is Solid).OfType<Solid>()
                .Where(x => x.Children.Any()))
            {
                yield return new Problem(GetType(), new[] { solid }, Fix, "Solid has children", "A solid with children was found. A solid cannot have any contents. Fixing the issue will move the children outside of the solid's group.");
            }
        }

        public IAction Fix(Problem problem)
        {
            throw new System.NotImplementedException();
        }
    }
}