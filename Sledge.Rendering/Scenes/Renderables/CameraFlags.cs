using System;

namespace Sledge.Rendering.Scenes.Renderables
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