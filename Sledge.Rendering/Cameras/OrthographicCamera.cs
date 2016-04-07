using System;
using System.Collections.Generic;
using System.Globalization;
using OpenTK;
using Sledge.Rendering.DataStructures;

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
        private Vector3 _position;
        private float _zoom;
        private OrthographicType _type;

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

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                const float u = 131072;
                const float l = -131072;
                _position = new Vector3(Math.Min(u, Math.Max(l, value.X)), Math.Min(u, Math.Max(l, value.Y)), Math.Min(u, Math.Max(l, value.Z)));
                OnPropertyChanged("Position");
            }
        }

        public override float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = Math.Min(256, Math.Max(0.001f, value));
                OnPropertyChanged("Zoom");
            }
        }

        public OrthographicType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }

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

        public override Vector3 EyeLocation { get { return (Vector3.UnitZ * float.MaxValue) + _position; } }

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

        public override Vector3 ScreenToWorld(Vector3 screen, int width, int height)
        {
            screen = new Vector3(screen.X, height - screen.Y, screen.Z);
            var cs = new Vector3(width / 2f, height / 2f, 0);
            var flat = Position + ((screen - cs) / Zoom);
            return Expand(flat);
        }

        public override Vector3 WorldToScreen(Vector3 world, int width, int height)
        {
            var flat = Flatten(world);
            var cs = new Vector3(width / 2f, height / 2f, 0);
            var screen = cs + ((flat - Position) * Zoom);
            return new Vector3(screen.X, height - screen.Y, screen.Z);
        }

        public override Line CastRayFromScreen(Vector3 screen, int width, int height)
        {
            screen = new Vector3(screen.X, screen.Y, 0);
            var world = ScreenToWorld(screen, width, height);
            return null; // todo
        }

        public override IEnumerable<Plane> GetClippingPlanes(int width, int height)
        {
            var min = ScreenToWorld(Vector3.Zero, width, height);
            var max = ScreenToWorld(new Vector3(width, height, 0), width, height);
            yield return new Plane(Expand(Vector3.UnitX), min);
            yield return new Plane(-Expand(Vector3.UnitY), min);
            yield return new Plane(-Expand(Vector3.UnitX), max);
            yield return new Plane(Expand(Vector3.UnitY), max);
        }

        public override float UnitsToPixels(float units)
        {
            return units * Zoom;
        }

        public override float PixelsToUnits(float pixels)
        {
            return pixels / Zoom;
        }

        public override Vector3 Flatten(Vector3 notFlat)
        {
            switch (Type)
            {
                case OrthographicType.Top:
                    return new Vector3(notFlat.X, notFlat.Y, 0);
                case OrthographicType.Front:
                    return new Vector3(notFlat.Y, notFlat.Z, 0);
                case OrthographicType.Side:
                    return new Vector3(notFlat.X, notFlat.Z, 0);
                default:
                    throw new ArgumentOutOfRangeException("Type");
            }
        }

        public override Vector3 Expand(Vector3 flat)
        {
            switch (Type)
            {
                case OrthographicType.Top:
                    return new Vector3(flat.X, flat.Y, 0);
                case OrthographicType.Front:
                    return new Vector3(0, flat.X, flat.Y);
                case OrthographicType.Side:
                    return new Vector3(flat.X, 0, flat.Y);
                default:
                    throw new ArgumentOutOfRangeException("Type");
            }
        }

        protected override string Serialise()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}/{1},{2},{3}/{4}",
                Type, Position.X, Position.Y, Position.Z, Zoom);
        }
    }
}