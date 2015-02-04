using OpenTK;

namespace Sledge.Rendering.Cameras
{
    public abstract class Camera
    {
        public abstract Matrix4 GetCameraMatrix();
        public abstract Matrix4 GetViewportMatrix(int width, int height);
    }
}