using System;
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
    }
}