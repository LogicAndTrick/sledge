using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.DataStructures.GameData
{
    public enum VariableType
    {
        Axis,
        Angle,
        AngleNegativePitch,
        Bool,
        Boolean = Bool,
        Choices,
        Color255,
        Color1,
        Decal,
        FilterClass,
        Flags,
        Float,
        InstanceFile,
        InstanceVariable,
        InstanceParm,
        Integer,
        Material,
        NodeId,
        NodeDest,
        NPCClass,
        Origin,
        Other,
        ParticleSystem,
        PointEntityClass,
        Scale, // Quake 3
        Scene,
        Script,
        ScriptList,
        SideList,
        Sky, // Quake 2
        Sound,
        Sprite,
        String,
        Studio,
        TargetDestination,
        TargetNameOrClass,
        TargetSource,
        Vecline,
        Vector,
        Void
    }
}
