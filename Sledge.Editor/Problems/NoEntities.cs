using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;

namespace Sledge.Editor.Problems
{
    public class NoEntities : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map)
        {
            if (map.WorldSpawn.Find(x => x is Entity).Any()) yield break;
            yield return new Problem(GetType(), Fix, "The map has no entities", "There were no entities found in the map. A map needs at least one entity to avoid a leak. Fixing the issue will place the default point entity in the map origin. It is recommended that you fix this issue manually.");
        }

        public IAction Fix(Problem problem)
        {
            throw new System.NotImplementedException();
        }
    }
}