using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;

namespace Sledge.BspEditor.Editing.Problems
{
    public class SolidEntityWithoutChildren : IProblemCheck
    {
        public IEnumerable<Problem> Check(MapDocument document, bool visibleOnly)
        {
            foreach (var entity in document.WorldSpawn
                .Find(x => x is Entity && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)))
                .OfType<Entity>()
                .Where(x => x.GameData != null)
                .Where(x => x.GameData.ClassType == ClassType.Solid)
                .Where(x => !x.GetChildren().SelectMany(y => y.FindAll()).Any(y => y is Solid)))
            {
                yield return new Problem(GetType(), document, new[] { entity }, Fix, "Brush entity has no solid children", "A brush entity with no solid children was found. A brush entity must have solid contents. Fixing the problem will delete the entity.");
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