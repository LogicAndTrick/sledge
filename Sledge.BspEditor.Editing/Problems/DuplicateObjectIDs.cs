using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.DataStructures.MapObjects;

namespace Sledge.BspEditor.Editing.Problems
{
    public class DuplicateObjectIDs : IProblemCheck
    {
        public IEnumerable<Problem> Check(MapDocument document, bool visibleOnly)
        {
            var dupes = from o in document.WorldSpawn.Find(x => (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)))
                        group o by o.ID
                        into g
                        where g.Count() > 1
                        select g;
            foreach (var dupe in dupes)
            {
                yield return new Problem(GetType(), document, dupe, Fix, "Multiple objects have the same ID", "More than one object has the same ID. Each object ID should be unique. Fixing the problem will assign the duplicates new IDs.");
            }
        }

        public IOperation Fix(Problem problem)
        {
            // todo
            throw new NotImplementedException();
            // return new ReplaceObjects(problem.Objects, problem.Objects.Select(x =>
            // {
            //     var c = x.Clone();
            //     c.ID = problem.Map.IDGenerator.GetNextObjectID();
            //     return c;
            // }));
        }
    }
}