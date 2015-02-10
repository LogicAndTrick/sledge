using System;

namespace Sledge.Rendering.Cameras
{
    [Flags]
    public enum CameraFlags : uint
    {
        None = 0,
        All = Perspective | Orthographic,

        Perspective = 1 << 0,
        Orthographic = 1 << 1
    }
}