using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;

namespace Sledge.Editor.Problems
{
    public class DuplicateKeyValues : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map)
        {
            var entities = map.WorldSpawn.Find(x => x is Entity).OfType<Entity>().ToList();
            foreach (var entity in entities)
            {
                var dupes = from p in entity.EntityData.Properties
                            group p by p.Key.ToLowerInvariant()
                            into g
                            where g.Count() > 1
                            select g;
                if (dupes.Any())
                {
                    yield return new Problem(GetType(), new[] { entity }, Fix, "Entity has duplicate keys", "This entity has the same key specified multiple times. Entity keys should be unique. Fixing the problem will remove the duplicate key.");
                }
            }
        }

        public IAction Fix(Problem problem)
        {
            throw new System.NotImplementedException();
        }
    }
}