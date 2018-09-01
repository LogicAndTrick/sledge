using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;

namespace Sledge.BspEditor.Editing.Problems
{
    public class SolidEntityWithEntityChildren : IProblemCheck
    {
        public IEnumerable<Problem> Check(MapDocument document, bool visibleOnly)
        {
            foreach (var entity in document.WorldSpawn
                .Find(x => x is Entity && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)))
                .OfType<Entity>()
                .Where(x => x.GameData != null)
                .Where(x => x.GameData.ClassType == ClassType.Solid)
                .Where(x => x.GetChildren().SelectMany(y => y.FindAll()).Any(y => !(y is Group) && !(y is Solid))))
            {
                yield return new Problem(GetType(), document, new[] { entity }, Fix, "Brush entity has child entities", "A brush entity with child entities was found. A brush entity must only have solid contents. Fixing the problem will move the child entities outside of the entity's group.");
            }
        }

        public IOperation Fix(Problem problem)
        {
            // todo
            throw new NotImplementedException();
            // return new Reparent(problem.Objects[0].Parent.ID, problem.Objects[0].GetChildren().SelectMany(x => x.Find(y => y is Entity)));
        }
    }
}