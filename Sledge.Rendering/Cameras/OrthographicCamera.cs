using OpenTK;

namespace Sledge.Rendering.Cameras
{
    public class OrthographicCamera : Camera
    {
        public Vector3 Position { get; set; }
        public decimal Zoom { get; set; }

        public OrthographicCamera()
        {
            Position = Vector3.Zero;
            Zoom = 1;
            Flags = CameraFlags.Orthographic;
            RenderOptions = new CameraRenderOptions
                            {
                                RenderFaceWireframe = true,
                                RenderFacePoints = true,
                                RenderLineWireframe = true,
                                RenderLinePoints = true,
                                RenderFacePolygons = false
                            };
        }

        public override Vector3 EyeLocation { get { return Vector3.UnitZ * float.MaxValue; } }

        public override Matrix4 GetCameraMatrix()
        {
            var translate = Matrix4.CreateTranslation((float) -Position.X, (float) -Position.Y, 0);
            var scale = Matrix4.CreateScale(new Vector3((float) Zoom, (float) Zoom, 0));
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