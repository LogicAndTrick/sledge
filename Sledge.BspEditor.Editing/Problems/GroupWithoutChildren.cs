using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.DataStructures.MapObjects;

namespace Sledge.BspEditor.Editing.Problems
{
    public class GroupWithoutChildren : IProblemCheck
    {
        public IEnumerable<Problem> Check(MapDocument document, bool visibleOnly)
        {
            foreach (var group in document.WorldSpawn
                .Find(x => x is Group && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)))
                .OfType<Group>()
                .Where(x => !x.GetChildren().Any()))
            {
                yield return new Problem(GetType(), document, new[] { @group }, Fix, "Group has no children", "This group is empty. A group must have contents. Fixing the problem will delete the group.");
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