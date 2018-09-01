using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.DataStructures.MapObjects;

namespace Sledge.BspEditor.Editing.Problems
{
    public class InvalidSolids : IProblemCheck
    {
        public IEnumerable<Problem> Check(MapDocument document, bool visibleOnly)
        {
            foreach (var invalid in document.WorldSpawn.Find(x => x is Solid && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)) && !((Solid)x).IsValid()))
            {
                yield return new Problem(GetType(), document, new[] { invalid }, Fix, "Invalid solid", "This solid is invalid. It is either not convex, has coplanar faces, or has off-plane vertices. Fixing the issue will delete the solid.");
            }
        }

        public IOperation Fix(Problem problem)
        {
            // todo
            throw new NotImplementedException();
            // return new Delete(problem.Objects.Select(x => x.ID));
        }
    }
}