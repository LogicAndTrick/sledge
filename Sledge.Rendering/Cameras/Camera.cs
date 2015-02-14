using OpenTK;

namespace Sledge.Rendering.Cameras
{
    public abstract class Camera
    {
        public CameraFlags Flags { get; set; }
        public CameraRenderOptions RenderOptions { get; set; }
        public abstract Vector3 EyeLocation { get; }
        public abstract Matrix4 GetCameraMatrix();
        public abstract Matrix4 GetViewportMatrix(int width, int height);
        public abstract Matrix4 GetModelMatrix();
    }
}