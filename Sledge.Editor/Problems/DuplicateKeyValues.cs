using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Entities;
using Sledge.Editor.Actions.MapObjects.Operations;

namespace Sledge.Editor.Problems
{
    public class DuplicateKeyValues : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map, bool visibleOnly)
        {
            var entities = map.WorldSpawn.Find(x => x is Entity && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden))).OfType<Entity>().ToList();
            foreach (var entity in entities)
            {
                var dupes = from p in entity.EntityData.Properties
                            group p by p.Key.ToLowerInvariant()
                            into g
                            where g.Count() > 1
                            select g;
                if (dupes.Any())
                {
                    yield return new Problem(GetType(), map, new[] { entity }, Fix, "Entity has duplicate keys", "This entity has the same key specified multiple times. Entity keys should be unique. Fixing the problem will remove the duplicate key.");
                }
            }
        }

        public IAction Fix(Problem problem)
        {
            var edit = new EditEntityData();
            foreach (var mo in problem.Objects)
            {
                var ed = mo.GetEntityData().Clone();
                var dupes = from p in ed.Properties
                            group p by p.Key.ToLowerInvariant()
                            into g
                            where g.Count() > 1
                            select g;
                foreach (var prop in dupes.SelectMany(dupe => dupe.Skip(1)))
                {
                    ed.Properties.Remove(prop);
                }
                edit.AddEntity(mo, ed);
            }
            return edit;
        }
    }
}