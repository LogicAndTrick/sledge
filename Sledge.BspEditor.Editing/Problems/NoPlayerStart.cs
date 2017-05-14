using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Editing.Problems
{
    public class NoPlayerStart : IProblemCheck
    {
        public IEnumerable<Problem> Check(MapDocument document, bool visibleOnly)
        {
            if (document.Map.Root.Find(x => x is Entity && string.Equals(x.Data.GetOne<EntityData>()?.Name, "info_player_start", StringComparison.InvariantCultureIgnoreCase)).Any()) yield break;
            yield return new Problem(GetType(), document, Fix, "This document has no player start", "There is no info_player_start entity in this document. The player will spawn at the origin instead. This may place the player inside geometry or in the void. Fixing the issue will place a player start entity at the document origin. It is recommended that you fix this problem manually.");
        }

        public IOperation Fix(Problem problem)
        {
            // todo
            throw new NotImplementedException();
            // return new Create(problem.Map.Root.ID, new Entity(problem.Map.IDGenerator.GetNextObjectID())
            //{
            //    EntityData = new EntityData { Name = "info_player_start" },
            //                          ClassName = "info_player_start",
            //                          Colour = Colour.GetDefaultEntityColour(),
            //                          Origin = Coordinate.Zero
            //                      });
        }
    }
}