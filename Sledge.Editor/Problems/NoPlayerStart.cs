using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;

namespace Sledge.Editor.Problems
{
    public class NoPlayerStart : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map)
        {
            if (map.WorldSpawn.Find(x => x is Entity && x.GetEntityData() != null && string.Equals(x.GetEntityData().Name, "info_player_start", StringComparison.InvariantCultureIgnoreCase)).Any()) yield break;
            yield return new Problem(GetType(), Fix, "This map has no player start", "There is no info_player_start entity in this map. The player will spawn at the origin instead. This may place the player inside geometry or in the void. Fixing the issue will place a player start entity at the map origin. It is recommended that you fix this problem manually.");
        }

        public IAction Fix(Problem problem)
        {
            throw new System.NotImplementedException();
        }
    }
}