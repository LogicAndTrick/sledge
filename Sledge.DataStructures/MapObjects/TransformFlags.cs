using System;

namespace Sledge.DataStructures.MapObjects
{
    [Flags]
    public enum TransformFlags
    {
        None = 1 << 0,
        TextureLock = 1 << 1,
        TextureScalingLock = 1 << 2
    }
}