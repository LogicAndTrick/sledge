using System;
using System.Globalization;
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

        protected PerspectiveCamera(string serialised) : this()
        {
            var tags = (serialised ?? "").Split(',', '/');
            if (tags.Length < 3) return;

            float p, x = 0, y = 0, z = 0;

            if (float.TryParse(tags[0], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) x = p;
            if (float.TryParse(tags[1], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) y = p;
            if (float.TryParse(tags[2], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) z = p;
            Position = new Vector3(x, y, z);
            
            if (tags.Length < 6) return;

            x = y = z = 1;
            if (float.TryParse(tags[3], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) x = p;
            if (float.TryParse(tags[4], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) y = p;
            if (float.TryParse(tags[5], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) z = p;
            _lookAt = new Vector3(x, y, z);
            
            if (tags.Length < 8) return;

            int i;
            if (int.TryParse(tags[6], NumberStyles.Integer, CultureInfo.InvariantCulture, out i)) FOV = i;
            if (int.TryParse(tags[7], NumberStyles.Integer, CultureInfo.InvariantCulture, out i)) ClipDistance = i;
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

        public override Matrix4 GetModelMatrix()
        {
            return Matrix4.Identity;
        }

        protected override string Serialise()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0},{1},{2}/{3},{4},{5}/{6}/{7}",
                Position.X, Position.Y, Position.Z, _lookAt.X, _lookAt.Y, _lookAt.Z, FOV, ClipDistance);
        }
    }
}