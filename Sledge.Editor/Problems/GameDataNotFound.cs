using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;

namespace Sledge.Editor.Problems
{
    public class GameDataNotFound : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map)
        {
            foreach (var entity in map.WorldSpawn
                .Find(x => x is Entity).OfType<Entity>()
                .Where(x => x.GameData == null))
            {
                yield return new Problem(GetType(), new[] { entity }, Fix, "Entity class not found: " + entity.EntityData.Name, "This entity class was not found in the current game data. Ensure that the correct FGDs are loaded. Fixing the problem will delete the entities.");
            }
        }

        public IAction Fix(Problem problem)
        {
            throw new System.NotImplementedException();
        }
    }
}