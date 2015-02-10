using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.Cameras
{
    public class OrthographicCamera : Camera
    {
        public Coordinate Position { get; set; }
        public decimal Zoom { get; set; }

        public OrthographicCamera()
        {
            Position = Coordinate.Zero;
            Zoom = 1;
            Flags = CameraFlags.Orthographic;
        }

        public override Matrix4 GetCameraMatrix()
        {
            var translate = Matrix4.CreateTranslation((float) -Position.X, (float) -Position.Y, 0);
            var scale = Matrix4.Scale(new Vector3((float) Zoom, (float) Zoom, 0));
            return translate * scale;
        }

        public override Matrix4 GetViewportMatrix(int width, int height)
        {
            const float near = -1000000;
            const float far = 1000000;
            return Matrix4.CreateOrthographic(width, height, near, far);
        }
    }
}