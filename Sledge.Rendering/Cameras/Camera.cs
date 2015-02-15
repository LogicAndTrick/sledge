using System.Reflection;
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

        protected abstract string Serialise();

        public static string Serialise(Camera camera)
        {
            return camera.GetType().Name + '/' + camera.Serialise();
        }

        public static Camera Deserialise(string serialised)
        {
            var split = serialised.Split(new[] {'/'}, 2);
            if (split.Length != 2) return null;

            var ty = typeof (Camera).Assembly.GetType(split[0]);
            if (ty == null || !typeof (Camera).IsAssignableFrom(ty)) return null;

            var ctor = ty.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof (string)}, null);
            if (ctor == null) return null;

            return (Camera) ctor.Invoke(new object[] {split[1]});
        }
    }
}