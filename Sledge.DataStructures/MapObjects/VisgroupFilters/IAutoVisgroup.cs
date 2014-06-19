using System;
using System.Linq;
using Sledge.DataStructures.GameData;

namespace Sledge.DataStructures.MapObjects.VisgroupFilters
{

    /*
     * These are all the auto visgroups I could find in Hammer 4, plus a bit more:
     *   - = Source only
     *   + = Source and GoldSource
     *   * = GoldSource only
     * Entities
     *   + Brush Entities -> entity type
     *   + Point Entities -> entity type
     *   + Triggers -> entity classes
     *   + Lights -> entity classes
     *   + Nodes -> entity classes
     *   - NPCs -> entity classes
     * Tool Brushes
     *   - Occluders -> entity class
     *   - Areaportals -> entity class
     *   - Area Portal -> face texture
     *   - Fog -> face texture
     *   + Hint -> face texture
     *   - Occluder -> face texture
     *   + Origin -> face texture
     *   + Skip -> face texture
     *   + Trigger -> face texture
     *   * Bevel -> face texture
     * Custom -> FGD @AutoVisGroups
     * World Geometry -> all non-entity solids
     *   - Nodraw -> face texture
     *   * Null -> face texture
     *   - Displacements -> face displacement
     *   + Sky -> face texture
     *   - Black -> face texture
     *   + Water -> face texture metadata
     * World Details
     *   - Func Detail -> entity class
     *   - Props -> entity classes
     * Clips
     *   - Control -> face texture
     *   - NPC -> face texture
     *   - Player -> face texture
     *   + Clip -> face texture
     * Block
     *   - LOS -> face texture
     *   - Bullets -> face texture
     *   - Light -> face texture
     * Invisible
     *   - Invisible -> face texture
     *   - Ladder -> face texture
     *   
     * For GoldSource / milestone 1:
     * Entities
     *   + Brush Entities -> entity type
     *   + Point Entities -> entity type
     *   + Triggers -> entity classes
     *   + Lights -> entity classes
     *   + Nodes -> entity classes
     * Tool Brushes
     *   + Hint -> face texture
     *   + Origin -> face texture
     *   + Skip -> face texture
     *   + Trigger -> face texture
     *   * Bevel -> face texture
     *   + Clip -> face texture
     * World Geometry -> all non-entity solids
     *   * Null -> face texture
     *   + Sky -> face texture
     *   + Water -> face texture metadata
     *   
     */

    public interface IVisgroupFilter
    {
        string Group { get; }
        string Name { get; }
        bool IsMatch(MapObject mapObject);
    }

    public class BrushEntitiesVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "Entities"; } }
        public string Name { get { return "Brush Entities"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Entity && ((Entity)x).GameData != null && ((Entity)x).GameData.ClassType == ClassType.Solid;
        }
    }

    public class PointEntitiesVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "Entities"; } }
        public string Name { get { return "Point Entities"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Entity && ((Entity)x).GameData != null && ((Entity)x).GameData.ClassType != ClassType.Solid;
        }
    }

    public class TriggersVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "Entities"; } }
        public string Name { get { return "Triggers"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Entity && x.GetEntityData().Name.StartsWith("trigger_");
        }
    }

    public class LightsVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "Entities"; } }
        public string Name { get { return "Lights"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Entity && x.GetEntityData().Name.StartsWith("light");
        }
    }

    public class NodesVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "Entities"; } }
        public string Name { get { return "Nodes"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Entity && x.GetEntityData().Name.Contains("_node");
        }
    }

    public class HintVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "Tool Brushes"; } }
        public string Name { get { return "Hint"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Solid && ((Solid)x).Faces.Any(y => String.Equals(y.Texture.Name, "hint", StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public class OriginVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "Tool Brushes"; } }
        public string Name { get { return "Origin"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Solid && ((Solid)x).Faces.Any(y => String.Equals(y.Texture.Name, "origin", StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public class SkipVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "Tool Brushes"; } }
        public string Name { get { return "Skip"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Solid && ((Solid)x).Faces.Any(y => String.Equals(y.Texture.Name, "skip", StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public class TriggerVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "Tool Brushes"; } }
        public string Name { get { return "Trigger"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Solid && ((Solid)x).Faces.Any(y => String.Equals(y.Texture.Name, "aaatrigger", StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public class BevelVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "Tool Brushes"; } }
        public string Name { get { return "Bevel"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Solid && ((Solid)x).Faces.Any(y => String.Equals(y.Texture.Name, "bevel", StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public class BrushesVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "World Geometry"; } }
        public string Name { get { return "Brushes"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Solid && x.FindClosestParent(y => y is Entity) == null;
        }
    }

    public class NullVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "World Geometry"; } }
        public string Name { get { return "Null"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Solid && x.FindClosestParent(y => y is Entity) == null && ((Solid)x).Faces.Any(y => String.Equals(y.Texture.Name, "null", StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public class SkyVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "World Geometry"; } }
        public string Name { get { return "Sky"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Solid && x.FindClosestParent(y => y is Entity) == null && ((Solid)x).Faces.Any(y => String.Equals(y.Texture.Name, "sky", StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public class WaterVisgroupFilter : IVisgroupFilter
    {
        public string Group { get { return "World Geometry"; } }
        public string Name { get { return "Water"; } }
        public bool IsMatch(MapObject x)
        {
            return x is Solid && x.FindClosestParent(y => y is Entity) == null && ((Solid)x).Faces.Any(y => y.Texture.Name.StartsWith("!"));
        }
    }
}
