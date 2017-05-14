using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.DataStructures.MapObjects;

namespace Sledge.BspEditor.Editing.Problems
{
    public class DuplicateFaceIDs : IProblemCheck
    {
        public IEnumerable<Problem> Check(MapDocument document, bool visibleOnly)
        {
            var dupes = from o in document.WorldSpawn.Find(x => x is Solid && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)))
                            .OfType<Solid>()
                            .SelectMany(x => x.Faces)
                        group o by o.ID
                        into g
                        where g.Count() > 1
                        select g;
            foreach (var dupe in dupes)
            {
                yield return new Problem(GetType(), document, dupe, Fix, "Multiple faces have the same ID", "More than one face was found with the same ID. Each face ID should be unique. Fixing this problem will assign the duplicated faces a new ID.");
            }
        }

        public IOperation Fix(Problem problem)
        {
            // todo
            throw new NotImplementedException();
            // return new EditFace(problem.Faces, (d, x) => x.ID = d.Map.IDGenerator.GetNextFaceID(), true);
        }
    }
}