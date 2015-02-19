using System;
using System.Linq;
using System.Reflection;
using OpenTK;
using Sledge.Rendering.DataStructures;

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

        public abstract Vector3 ScreenToWorld(Vector3 screen, int width, int height);
        public abstract Vector3 WorldToScreen(Vector3 world, int width, int height);
        public abstract Line CastRayFromScreen(Vector3 screen, int width, int height);

        public abstract float UnitsToPixels(float units);
        public abstract float PixelsToUnits(float pixels);

        public abstract Vector3 Flatten(Vector3 notFlat);
        public abstract Vector3 Expand(Vector3 flat);

        protected abstract string Serialise();

        public static string Serialise(Camera camera)
        {
            return camera.GetType().Name + '/' + camera.Serialise();
        }

        public static Camera Deserialise(string serialised)
        {
            var split = serialised.Split(new[] {'/'}, 2);
            if (split.Length != 2) return null;

            var ty = typeof (Camera).Assembly.GetTypes().FirstOrDefault(x => String.Equals(x.Name, split[0], StringComparison.InvariantCultureIgnoreCase));
            if (ty == null || !typeof (Camera).IsAssignableFrom(ty)) return null;

            var ctor = ty.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof (string)}, null);
            if (ctor == null) return null;

            return (Camera) ctor.Invoke(new object[] {split[1]});
        }
    }
}