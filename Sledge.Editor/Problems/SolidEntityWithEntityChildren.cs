using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;

namespace Sledge.Editor.Problems
{
    public class SolidEntityWithEntityChildren : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map)
        {
            foreach (var entity in map.WorldSpawn
                .Find(x => x is Entity).OfType<Entity>()
                .Where(x => x.GameData != null)
                .Where(x => x.GameData.ClassType == ClassType.Solid)
                .Where(x => x.Children.SelectMany(y => y.FindAll()).Any(y => !(y is Group) && !(y is Solid))))
            {
                yield return new Problem(GetType(), new[] { entity }, Fix, "Brush entity has child entities", "A brush entity with child entities was found. A brush entity must only have solid contents. Fixing the problem will move the child entities outside of the entity's group.");
            }
        }

        public IAction Fix(Problem problem)
        {
            throw new System.NotImplementedException();
        }
    }
}