using System.Numerics;

namespace Sledge.Rendering.Cameras
{
    public interface ICamera
    {
        CameraType Type { get; }
        int Width { get; set; }
        int Height { get; set; }

        Vector3 Location { get; }
        Matrix4x4 View { get; }
        Matrix4x4 Projection { get; }

        Vector3 Position { get; }

        // 2D methods
        float Zoom { get; }
        Vector3 ScreenToWorld(Vector3 screen);
        Vector3 WorldToScreen(Vector3 world);
        float UnitsToPixels(float units);
        float PixelsToUnits(float pixels);
        Vector3 Flatten(Vector3 notFlat);
        Vector3 Expand(Vector3 flat);
    }
}