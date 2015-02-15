using System;
using System.Globalization;
using OpenTK;

namespace Sledge.Rendering.Cameras
{
    public class OrthographicCamera : Camera
    {
        public enum OrthographicType
        {
            Top,
            Front,
            Side
        }

        private static readonly Matrix4 TopMatrix = Matrix4.Identity;
        private static readonly Matrix4 FrontMatrix = new Matrix4(Vector4.UnitZ, Vector4.UnitX, Vector4.UnitY, Vector4.UnitW);
        private static readonly Matrix4 SideMatrix = new Matrix4(Vector4.UnitX, Vector4.UnitZ, Vector4.UnitY, Vector4.UnitW);

        private static Matrix4 GetMatrixFor(OrthographicType dir)
        {
            switch (dir)
            {
                case OrthographicType.Top:
                    return TopMatrix;
                case OrthographicType.Front:
                    return FrontMatrix;
                case OrthographicType.Side:
                    return SideMatrix;
                default:
                    throw new ArgumentOutOfRangeException("dir");
            }
        }

        public Vector3 Position { get; set; }
        public float Zoom { get; set; }
        public OrthographicType Type { get; set; }

        public OrthographicCamera(OrthographicType type)
        {
            Type = type;
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

        protected OrthographicCamera(string serialised) : this(OrthographicType.Top)
        {
            var tags = (serialised ?? "").Split(',', '/');
            if (tags.Length < 1) return;

            OrthographicType t;
            if (Enum.TryParse(tags[0], true, out t)) Type = t;

            if (tags.Length < 4) return;

            float p, x = 0, y = 0, z = 0;

            if (float.TryParse(tags[1], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) x = p;
            if (float.TryParse(tags[2], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) y = p;
            if (float.TryParse(tags[3], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) z = p;
            Position = new Vector3(x, y, z);

            if (tags.Length < 5) return;
            if (float.TryParse(tags[4], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) Zoom = p;
        }

        public override Vector3 EyeLocation { get { return Vector3.UnitZ * float.MaxValue; } }

        public override Matrix4 GetCameraMatrix()
        {
            var translate = Matrix4.CreateTranslation(-Position.X, -Position.Y, 0);
            var scale = Matrix4.CreateScale(new Vector3(Zoom, Zoom, 0));
            return translate * scale;
        }

        public override Matrix4 GetViewportMatrix(int width, int height)
        {
            const float near = -1000000;
            const float far = 1000000;
            return Matrix4.CreateOrthographic(width, height, near, far);
        }

        public override Matrix4 GetModelMatrix()
        {
            return GetMatrixFor(Type);
        }

        protected override string Serialise()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}/{1},{2},{3}/{4}",
                Type, Position.X, Position.Y, Position.Z, Zoom);
        }
    }
}