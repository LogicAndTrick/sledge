using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;

namespace Sledge.Editor.Problems
{
    public class DuplicateFaceIDs : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map, bool visibleOnly)
        {
            var dupes = from o in map.WorldSpawn.Find(x => x is Solid && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)))
                            .OfType<Solid>()
                            .SelectMany(x => x.Faces)
                        group o by o.ID
                        into g
                        where g.Count() > 1
                        select g;
            foreach (var dupe in dupes)
            {
                yield return new Problem(GetType(), map, dupe, Fix, "Multiple faces have the same ID", "More than one face was found with the same ID. Each face ID should be unique. Fixing this problem will assign the duplicated faces a new ID.");
            }
        }

        public IAction Fix(Problem problem)
        {
            return new EditFace(problem.Faces, (d, x) => x.ID = d.Map.IDGenerator.GetNextFaceID(), true);
        }
    }
}