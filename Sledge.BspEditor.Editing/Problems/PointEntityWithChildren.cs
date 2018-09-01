using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;

namespace Sledge.BspEditor.Editing.Problems
{
    public class PointEntityWithChildren : IProblemCheck
    {
        public IEnumerable<Problem> Check(MapDocument document, bool visibleOnly)
        {
            foreach (var entity in document.WorldSpawn
                .Find(x => x is Entity && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)))
                .OfType<Entity>()
                .Where(x => x.GameData != null)
                .Where(x => x.GameData.ClassType != ClassType.Solid && x.GetChildren().Any()))
            {
                yield return new Problem(GetType(), document, new[] { entity }, Fix, "Point entity has children", "A point entity with children was found. A point entity cannot have any contents. Fixing the issue will move the children outside of the entity's group.");
            }
        }

        public IOperation Fix(Problem problem)
        {
            // todo
            throw new NotImplementedException();
            // return new Reparent(problem.Objects[0].Parent.ID, problem.Objects[0].GetChildren());
        }
    }
}