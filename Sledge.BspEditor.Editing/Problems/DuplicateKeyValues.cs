using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.DataStructures.MapObjects;

namespace Sledge.BspEditor.Editing.Problems
{
    public class DuplicateKeyValues : IProblemCheck
    {
        public IEnumerable<Problem> Check(MapDocument document, bool visibleOnly)
        {
            var entities = document.WorldSpawn.Find(x => x is Entity && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden))).OfType<Entity>().ToList();
            foreach (var entity in entities)
            {
                var dupes = from p in entity.EntityData.Properties
                            group p by p.Key.ToLowerInvariant()
                            into g
                            where g.Count() > 1
                            select g;
                if (dupes.Any())
                {
                    yield return new Problem(GetType(), document, new[] { entity }, Fix, "Entity has duplicate keys", "This entity has the same key specified multiple times. Entity keys should be unique. Fixing the problem will remove the duplicate key.");
                }
            }
        }

        public IOperation Fix(Problem problem)
        {
            // todo
            throw new NotImplementedException();
            // var edit = new EditEntityData();
            //foreach (var mo in problem.Objects)
            //{
            //    var ed = mo.GetEntityData().Clone();
            //    var dupes = from p in ed.Properties
            //                group p by p.Key.ToLowerInvariant()
            //                into g
            //                where g.Count() > 1
            //                select g;
            //    foreach (var prop in dupes.SelectMany(dupe => dupe.Skip(1)))
            //    {
            //        ed.Properties.Remove(prop);
            //    }
            //    edit.AddEntity(mo, ed);
            //}
            //return edit;
        }
    }
}