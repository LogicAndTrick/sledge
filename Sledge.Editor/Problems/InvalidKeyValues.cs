using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Entities;

namespace Sledge.Editor.Problems
{
    public class InvalidKeyValues : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map, bool visibleOnly)
        {
            // MultiManagers require invalid key/values to work, exclude them from the search
            var entities = map.WorldSpawn
                .Find(x => x is Entity && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)))
                .OfType<Entity>()
                .Where(x => x.GameData != null)
                .Where(x => !String.Equals(x.EntityData.Name, "multi_manager", StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            foreach (var entity in entities)
            {
                var valid = true;
                foreach (var prop in entity.EntityData.Properties)
                {
                    if (!entity.GameData.Properties.Any(x => String.Equals(x.Name, prop.Key, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        valid = false;
                    }
                }
                if (!valid) yield return new Problem(GetType(), map, new[] { entity }, Fix, "Entity has invalid key/value pairs", "There are key/value pairs that are not specified in the game data. Ensure the latest FGDs are loaded. Fixing the problem will remove the invalid keys.");
            }
        }

        public IAction Fix(Problem problem)
        {
            var edit = new EditEntityData();
            foreach (var mo in problem.Objects.OfType<Entity>().Where(x => x.GameData != null))
            {
                var ed = mo.GetEntityData().Clone();
                foreach (var prop in mo.EntityData.Properties)
                {
                    if (!mo.GameData.Properties.Any(x => String.Equals(x.Name, prop.Key, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        ed.Properties.Remove(prop);
                    }
                }
                edit.AddEntity(mo, ed);
            }
            return edit;
        }
    }
}