using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;

namespace Sledge.Editor.Problems
{
    public class TargetWithoutMatchingName : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map)
        {
            var entities = map.WorldSpawn
                .Find(x => x is Entity).OfType<Entity>()
                .Where(x => x.GameData != null).ToList();
            foreach (var entity in entities.Where(x => !String.IsNullOrWhiteSpace(x.EntityData.GetPropertyValue("target"))))
            {
                var target = entity.EntityData.GetPropertyValue("target");
                var tname = entities.FirstOrDefault(x => x.EntityData.GetPropertyValue("targetname") == target);
                if (tname == null) yield return new Problem(GetType(), new[] { entity }, Fix, "Entity target has no matching named entity", "This entity's target value doesn't have an matching named entity. Each target should have a matching target name. Fixing the problem will reset the target's value to a blank string.");
            }
        }

        public IAction Fix(Problem problem)
        {
            throw new System.NotImplementedException();
        }
    }
}