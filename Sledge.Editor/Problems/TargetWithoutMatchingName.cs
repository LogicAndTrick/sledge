using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Entities;

namespace Sledge.Editor.Problems
{
    public class TargetWithoutMatchingName : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map, bool visibleOnly)
        {
            var entities = map.WorldSpawn
                .Find(x => x is Entity && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)))
                .OfType<Entity>()
                .Where(x => x.GameData != null).ToList();
            foreach (var entity in entities.Where(x => !String.IsNullOrWhiteSpace(x.EntityData.GetPropertyValue("target"))))
            {
                var target = entity.EntityData.GetPropertyValue("target");
                var tname = entities.FirstOrDefault(x => x.EntityData.GetPropertyValue("targetname") == target);
                if (tname == null) yield return new Problem(GetType(), map, new[] { entity }, Fix, "Entity target has no matching named entity", "This entity's target value doesn't have an matching named entity. Each target should have a matching target name. Fixing the problem will reset the target's value to a blank string.");
            }
        }

        public IAction Fix(Problem problem)
        {
            var edit = new EditEntityData();
            foreach (var mo in problem.Objects)
            {
                var ed = mo.GetEntityData().Clone();
                var prop = ed.Properties.FirstOrDefault(x => x.Key.ToLowerInvariant() == "target");
                if (prop != null)
                {
                    ed.Properties.Remove(prop);
                    edit.AddEntity(mo, ed);
                }
            }
            return edit;
        }
    }
}