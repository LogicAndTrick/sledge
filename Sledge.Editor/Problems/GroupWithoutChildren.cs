using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;

namespace Sledge.Editor.Problems
{
    public class GroupWithoutChildren : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map)
        {
            foreach (var group in map.WorldSpawn
                .Find(x => x is Group).OfType<Group>()
                .Where(x => !x.Children.Any()))
            {
                yield return new Problem(GetType(), new[] { @group }, Fix, "Group has no children", "This group is empty. A group must have contents. Fixing the problem will delete the group.");
            }
        }

        public IAction Fix(Problem problem)
        {
            throw new System.NotImplementedException();
        }
    }
}