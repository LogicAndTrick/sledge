using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.DataStructures.MapObjects;

namespace Sledge.BspEditor.Editing.Problems
{
    public class GameDataNotFound : IProblemCheck
    {
        public IEnumerable<Problem> Check(MapDocument document, bool visibleOnly)
        {
            foreach (var entity in document.WorldSpawn
                .Find(x => x is Entity && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)))
                .OfType<Entity>()
                .Where(x => x.GameData == null))
            {
                yield return new Problem(GetType(), document, new[] { entity }, Fix, "Entity class not found: " + entity.EntityData.Name, "This entity class was not found in the current game data. Ensure that the correct FGDs are loaded. Fixing the problem will delete the entities.");
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