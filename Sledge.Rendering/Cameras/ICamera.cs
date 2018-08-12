using System.Numerics;

namespace Sledge.Rendering.Cameras
{
    public interface ICamera
    {
        int Width { get; set; }
        int Height { get; set; }
        Vector3 Location { get; }
        Matrix4x4 View { get; }
        Matrix4x4 Projection { get; }
    }
}