using System;
using System.Collections.Generic;
using System.Globalization;
using OpenTK;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Scenes.Renderables;
using Line = Sledge.Rendering.DataStructures.Line;

namespace Sledge.Rendering.Cameras
{
    public class PerspectiveCamera : Camera
    {
        private Vector3 _direction;
        private Vector3 _lookAt;
        private int _fov;
        private int _clipDistance;
        private Vector3 _position;

        public int FOV
        {
            get { return _fov; }
            set
            {
                _fov = value;
                OnPropertyChanged("FOV");
            }
        }

        public int ClipDistance
        {
            get { return _clipDistance; }
            set
            {
                _clipDistance = value;
                OnPropertyChanged("ClipDistance");
            }
        }

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged("Position");
            }
        }

        public Vector3 Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                _lookAt = Position + _direction;
                OnPropertyChanged("Direction");
            }
        }

        public Vector3 LookAt
        {
            get { return _lookAt; }
            set
            {
                _lookAt = value;
                _direction = _lookAt - Position;
                OnPropertyChanged("LookAt");
            }
        }

        public PerspectiveCamera()
        {
            Position = Vector3.Zero;
            Direction = Vector3.One;
            FOV = 90;
            ClipDistance = 10000;
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
        public override float Zoom { get { return 1; } set {} }

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

        public override Vector3 ScreenToWorld(Vector3 screen, int width, int height)
        {
            screen = new Vector3(screen.X, screen.Y, 1);
            var viewport = new[] { 0, 0, width, height };
            var pm = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), width / (float)height, 1.0f, 50000);
            var vm = Matrix4.LookAt(Position, LookAt, Vector3.UnitZ);
            return MathFunctions.Unproject(screen, viewport, pm, vm);
        }

        public override Vector3 WorldToScreen(Vector3 world, int width, int height)
        {
            var viewport = new[] { 0, 0, width, height };
            var pm = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), width / (float)height, 1.0f, 50000);
            var vm = Matrix4.LookAt(Position, LookAt, Vector3.UnitZ);
            return MathFunctions.Project(world, viewport, pm, vm);
        }

        public override Line CastRayFromScreen(Vector3 screen, int width, int height)
        {
            var near = new Vector3(screen.X, height - screen.Y, 0);
            var far = new Vector3(screen.X, height - screen.Y, 1);
            var pm = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), width / (float)height, 1.0f, 50000);
            var vm = Matrix4.LookAt(Position, LookAt, Vector3.UnitZ);
            var viewport = new[] { 0, 0, width, height };
            var un = MathFunctions.Unproject(near, viewport, pm, vm);
            var uf = MathFunctions.Unproject(far, viewport, pm, vm);
            return new Line(un, uf);
        }

        public override IEnumerable<Plane> GetClippingPlanes(int width, int height)
        {
            var pm = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), width / (float)height, 1.0f, ClipDistance);
            var vm = Matrix4.LookAt(Position, LookAt, Vector3.UnitZ);
            var viewport = new[] { 0, 0, width, height };

            var tlNear = MathFunctions.Unproject(new Vector3(0, height, 0), viewport, pm, vm);
            var tlFar = MathFunctions.Unproject(new Vector3(0, height, 1), viewport, pm, vm);

            var trNear = MathFunctions.Unproject(new Vector3(width, height, 0), viewport, pm, vm);
            var trFar = MathFunctions.Unproject(new Vector3(width, height, 1), viewport, pm, vm);

            var blNear = MathFunctions.Unproject(new Vector3(0, 0, 0), viewport, pm, vm);
            var blFar = MathFunctions.Unproject(new Vector3(0, 0, 1), viewport, pm, vm);

            var brNear = MathFunctions.Unproject(new Vector3(width, 0, 0), viewport, pm, vm);
            var brFar = MathFunctions.Unproject(new Vector3(width, 0, 1), viewport, pm, vm);

            yield return new Plane((LookAt - EyeLocation), EyeLocation);

            yield return new Plane(tlNear, tlFar, blFar);
            yield return new Plane(trNear, trFar, tlFar);
            yield return new Plane(brNear, brFar, trFar);
            yield return new Plane(blNear, blFar, brFar);
        }

        public override float UnitsToPixels(float units)
        {
            return 0;
        }

        public override float PixelsToUnits(float pixels)
        {
            return 0;
        }

        public override Vector3 Flatten(Vector3 notFlat)
        {
            return notFlat;
        }

        public override Vector3 Expand(Vector3 flat)
        {
            return flat;
        }

        public float GetRotation()
        {
            var temp = (LookAt - Position);
            temp.Normalize();
            var rot = Math.Atan2(temp.Y, temp.X);
            if (rot < 0) rot += 2 * Math.PI;
            if (rot > 2 * Math.PI) rot = rot % (2 * Math.PI);
            return (float)rot;
        }

        public void SetRotation(float rotation)
        {
            var temp = (LookAt - Position);
            temp.Normalize();
            var e = GetElevation();
            var x = Math.Cos(rotation) * Math.Sin(e);
            var y = Math.Sin(rotation) * Math.Sin(e);
            LookAt = new Vector3((float)x + Position.X, (float)y + Position.Y, temp.Z + Position.Z);
        }

        public float GetElevation()
        {
            var temp = (LookAt - Position);
            temp.Normalize();
            var elev = Math.Acos(temp.Z);
            return (float)elev;
        }

        public void SetElevation(float elevation)
        {
            if (elevation > (Math.PI * 0.99)) elevation = (float) Math.PI * 0.99f;
            if (elevation < (Math.PI * 0.01)) elevation = (float) Math.PI * 0.01f;
            var rotation = GetRotation();
            var x = Math.Cos(rotation) * Math.Sin(elevation);
            var y = Math.Sin(rotation) * Math.Sin(elevation);
            var z = Math.Cos(elevation);
            LookAt = new Vector3((float)x + Position.X, (float)y + Position.Y, (float)z + Position.Z);
        }

        public void Pan(float degrees)
        {
            var rad = degrees * ((float)Math.PI / 180);
            var rot = GetRotation();
            SetRotation(rot + rad);
        }

        public void Tilt(float degrees)
        {
            SetElevation(GetElevation() + (degrees * ((float)Math.PI / 180)));
        }

        public void Advance(float units)
        {
            var temp = LookAt - Position;
            temp.Normalize();
            var add = temp * (float)units;
            LookAt += add;
            Position += add;
        }

        public void Strafe(float units)
        {
            var right = GetRight();
            var add = right * (float)units;
            LookAt += add;
            Position += add;
        }

        public void Ascend(float units)
        {
            var up = GetUp();
            var add = up * (float)units;
            LookAt += add;
            Position += add;
        }

        public void AscendAbsolute(float units)
        {
            var up = new Vector3(0, 0, (float)units);
            LookAt += up;
            Position += up;
        }

        public Vector3 GetUp()
        {
            var temp = LookAt - Position;
            temp.Normalize();
            var normal = Vector3.Cross(GetRight(), temp);
            normal.Normalize();
            return normal;
        }

        public Vector3 GetRight()
        {
            var temp = LookAt - Position;
            temp.Z = 0;
            temp.Normalize();
            var normal = Vector3.Cross(temp, Vector3.UnitZ);
            normal.Normalize();
            return normal;
        }

        protected override string Serialise()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0},{1},{2}/{3},{4},{5}/{6}/{7}",
                Position.X, Position.Y, Position.Z, _lookAt.X, _lookAt.Y, _lookAt.Z, FOV, ClipDistance);
        }
    }
}