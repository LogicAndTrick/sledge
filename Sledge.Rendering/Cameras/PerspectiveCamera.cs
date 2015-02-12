using OpenTK;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.Cameras
{
    public class PerspectiveCamera : Camera
    {
        private Vector3 _direction;
        private Vector3 _lookAt;

        public int FOV { get; set; }
        public int ClipDistance { get; set; }
        public Vector3 Position { get; set; }

        public Vector3 Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                _lookAt = Position + _direction;
            }
        }

        public Vector3 LookAt
        {
            get { return _lookAt; }
            set
            {
                _lookAt = value;
                _direction = _lookAt - Position;
            }
        }

        public PerspectiveCamera()
        {
            Position = Vector3.Zero;
            Direction = Vector3.One;
            FOV = 90;
            ClipDistance = 1000;
            Flags = CameraFlags.Perspective;
            RenderOptions = new CameraRenderOptions
                            {
                                RenderFacePolygons = true,
                                RenderFacePolygonTextures = true,
                                RenderFacePolygonLighting = LightingFlags.Ambient | LightingFlags.Dynamic,
                                RenderLineWireframe = true,
                                RenderFaceWireframe = false,
                                RenderFacePoints = false,
                                RenderLinePoints = false
                            };
        }

        public override Vector3 EyeLocation { get { return Position; } }

        public override Matrix4 GetCameraMatrix()
        {
            return Matrix4.LookAt(Position, _lookAt, Vector3.UnitZ);
        }

        public override Matrix4 GetViewportMatrix(int width, int height)
        {
            const float near = 1.0f;
            var ratio = width / (float)height;
            if (ratio <= 0) ratio = 1;
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), ratio, near, ClipDistance);
        }
    }
}