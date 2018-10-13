using System;
using System.Globalization;
using System.Numerics;
using Sledge.Common;

namespace Sledge.Rendering.Cameras
{
    public class PerspectiveCamera : ICamera
    {
        private const float Pi = (float) Math.PI;

        private Vector3 _position;
        private Vector3 _angles;

        public CameraType Type => CameraType.Perspective;

        public float FOV { get; set; }
        public float ClipDistance { get; set; }

        public Vector3 Position
        {
            get => _position;
            set
            {
                if (float.IsNaN(value.X) || float.IsInfinity(value.X)) value.X = _position.X;
                if (float.IsNaN(value.Y) || float.IsInfinity(value.Y)) value.Y = _position.Y;
                if (float.IsNaN(value.Z) || float.IsInfinity(value.Z)) value.Z = _position.Z;
                _position = value;
            }
        }

        public Vector3 Angles
        {
            get => _angles;
            set
            {
                if (float.IsNaN(value.X) || float.IsInfinity(value.X)) value.X = _angles.X;
                if (float.IsNaN(value.Y) || float.IsInfinity(value.Y)) value.Y = _angles.Y;
                if (float.IsNaN(value.Z) || float.IsInfinity(value.Z)) value.Z = _angles.Z;
                _angles = value;
            }
        }

        public Vector3 Direction
        {
            get
            {
                var rot = Matrix4x4.CreateRotationX(_angles.Y) * Matrix4x4.CreateRotationZ(_angles.X);
                return Vector3.Transform(-Vector3.UnitZ, rot);
            }
            set
            {
                var norm = value.LengthSquared() <= 0.01f ? Vector3.UnitY : Vector3.Normalize(value);
                _angles.Y = (float) (Math.Asin(norm.Z) + Math.PI / 2);
                _angles.X = (float) (Math.Atan2(-norm.Y, -norm.X) + Math.PI / 2);

                // Try and stop invalid values from getting in
                if (float.IsInfinity(_angles.X) || float.IsNaN(_angles.X)) _angles.X = 0;
                if (float.IsInfinity(_angles.Y) || float.IsNaN(_angles.Y)) _angles.Y = 0;
            }
        }

        public Vector3 EyeLocation => Position;
        public float Zoom { get => 1; set { } }

        public int Width { get; set; }
        public int Height { get; set; }
        public Vector3 Location => Position;
        public Matrix4x4 View => GetCameraMatrix();
        public Matrix4x4 Projection => GetViewportMatrix(Width, Height);

        public PerspectiveCamera()
        {
            _position = Vector3.Zero;
            _angles = new Vector3(0, Pi / 2, 0);
            FOV = 90;
            ClipDistance = 10000;
        }

        internal PerspectiveCamera(string serialised) : this()
        {
            var tags = (serialised ?? "").Split(',', '/');
            if (tags.Length < 3) return;

            float p, x = 0, y = 0, z = 0;

            if (float.TryParse(tags[0], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) x = p;
            if (float.TryParse(tags[1], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) y = p;
            if (float.TryParse(tags[2], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) z = p;
            if (float.IsNaN(x) || float.IsInfinity(x)) x = 0;
            if (float.IsNaN(y) || float.IsInfinity(y)) y = 0;
            if (float.IsNaN(z) || float.IsInfinity(z)) z = 0;
            _position = new Vector3(x, y, z);

            if (tags.Length < 6) return;

            x = y = z = 1;
            if (float.TryParse(tags[3], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) x = p;
            if (float.TryParse(tags[4], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) y = p;
            if (float.TryParse(tags[5], NumberStyles.Float, CultureInfo.InvariantCulture, out p)) z = p;
            if (float.IsNaN(x) || float.IsInfinity(x)) x = 0;
            if (float.IsNaN(y) || float.IsInfinity(y)) y = 0;
            if (float.IsNaN(z) || float.IsInfinity(z)) z = 0;
            _angles = new Vector3(x, y, z);

            if (tags.Length < 8) return;

            int i;
            if (int.TryParse(tags[6], NumberStyles.Integer, CultureInfo.InvariantCulture, out i)) FOV = i;
            if (int.TryParse(tags[7], NumberStyles.Integer, CultureInfo.InvariantCulture, out i)) ClipDistance = i;
        }

        private Matrix4x4 GetCameraMatrix()
        {
            var rot = Matrix4x4.CreateFromYawPitchRoll(-Angles.Z, -Angles.Y, -Angles.X);
            var mov = Matrix4x4.CreateTranslation(-_position);
            return mov * rot;
        }

        private Matrix4x4 GetViewportMatrix(int width, int height)
        {
            const float near = 1.0f;
            var ratio = width / (float)height;
            if (ratio <= 0) ratio = 1;
            return Matrix4x4.CreatePerspectiveFieldOfView((float) MathHelper.DegreesToRadians(FOV), ratio, near, ClipDistance);
        }

        public Vector3 ScreenToWorld(Vector3 screen)
        {
            screen = new Vector3(screen.X, screen.Y, 1);
            var viewport = new[] { 0, 0, Width, Height };
            var pm = Matrix4x4.CreatePerspectiveFieldOfView((float)MathHelper.DegreesToRadians(FOV), Width / (float)Height, 1.0f, 50000);
            var vm = GetCameraMatrix();
            return MathFunctions.Unproject(screen, viewport, pm, vm);
        }

        public Vector3 WorldToScreen(Vector3 world)
        {
            var viewport = new[] { 0, 0, Width, Height };
            var pm = Matrix4x4.CreatePerspectiveFieldOfView((float)MathHelper.DegreesToRadians(FOV), Width / (float)Height, 1.0f, 50000);
            var vm = GetCameraMatrix();

            var proj = MathFunctions.Project(world, viewport, pm, vm);
            proj.Y = Height - proj.Y;
            return proj;
        }

        public (Vector3, Vector3) CastRayFromScreen(Vector3 screen)
        {
            var near = new Vector3(screen.X, Height - screen.Y, 0);
            var far = new Vector3(screen.X, Height - screen.Y, 1);
            var pm = Matrix4x4.CreatePerspectiveFieldOfView((float)MathHelper.DegreesToRadians(FOV), Width / (float)Height, 1.0f, 50000);
            var vm = GetCameraMatrix();
            var viewport = new[] { 0, 0, Width, Height };
            var un = MathFunctions.Unproject(near, viewport, pm, vm);
            var uf = MathFunctions.Unproject(far, viewport, pm, vm);
            return (un, uf);
        }

        //public IEnumerable<Plane> GetClippingPlanes(int width, int height)
        //{
        //    var pm = Matrix4x4.CreatePerspectiveFieldOfView((float)MathHelper.DegreesToRadians(FOV), width / (float)height, 1.0f, ClipDistance);
        //    var vm = GetCameraMatrix();
        //    var viewport = new[] { 0, 0, width, height };
        //
        //    var tlNear = MathFunctions.Unproject(new Vector3(0, height, 0), viewport, pm, vm);
        //    var tlFar = MathFunctions.Unproject(new Vector3(0, height, 1), viewport, pm, vm);
        //
        //    var trNear = MathFunctions.Unproject(new Vector3(width, height, 0), viewport, pm, vm);
        //    var trFar = MathFunctions.Unproject(new Vector3(width, height, 1), viewport, pm, vm);
        //
        //    var blNear = MathFunctions.Unproject(new Vector3(0, 0, 0), viewport, pm, vm);
        //    var blFar = MathFunctions.Unproject(new Vector3(0, 0, 1), viewport, pm, vm);
        //
        //    var brNear = MathFunctions.Unproject(new Vector3(width, 0, 0), viewport, pm, vm);
        //    var brFar = MathFunctions.Unproject(new Vector3(width, 0, 1), viewport, pm, vm);
        //    
        //    yield return new Plane((LookAt - EyeLocation), EyeLocation);
        //
        //    yield return new Plane(tlNear, tlFar, blFar);
        //    yield return new Plane(trNear, trFar, tlFar);
        //    yield return new Plane(brNear, brFar, trFar);
        //    yield return new Plane(blNear, blFar, brFar);
        //}

        public float UnitsToPixels(float units)
        {
            return 0;
        }

        public float PixelsToUnits(float pixels)
        {
            return 0;
        }

        public Vector3 Flatten(Vector3 notFlat)
        {
            return notFlat;
        }

        public Vector3 Expand(Vector3 flat)
        {
            return flat;
        }

        public void Pan(float degrees)
        {
            var rad = degrees * (Pi / 180);
            _angles.X += rad;
        }

        public void Tilt(float degrees)
        {
            var rad = degrees * (Pi / 180);
            _angles.Y -= rad;
            if (_angles.Y < 0.01f) _angles.Y = 0.01f;
            if (_angles.Y > Pi - 0.01f) _angles.Y = Pi - 0.01f;
        }

        public void Advance(float units)
        {
            if (float.IsNaN(units) || float.IsInfinity(units)) return;
            var add = Direction * units;
            _position += add;
        }

        public void Strafe(float units)
        {
            if (float.IsNaN(units) || float.IsInfinity(units)) return;
            var add = GetRight() * units;
            _position += add;
        }

        public void Ascend(float units)
        {
            if (float.IsNaN(units) || float.IsInfinity(units)) return;
            var add = GetUp() * units;
            _position += add;
        }

        public void AscendAbsolute(float units)
        {
            if (float.IsNaN(units) || float.IsInfinity(units)) return;
            _position += Vector3.UnitZ * units;
        }

        public Vector3 GetUp()
        {
            var normal = Vector3.Cross(GetRight(), Direction);
            normal = Vector3.Normalize(normal);
            return normal;
        }
        
        public Vector3 GetRight()
        {
            var temp = Direction;
            temp.Z = 0;
            if (temp.Length() < 0.001f) temp = Vector3.UnitY;
            temp = Vector3.Normalize(temp);

            var normal = Vector3.Cross(temp, Vector3.UnitZ);
            normal = Vector3.Normalize(normal);

            return normal;
        }

        public string Serialise()
        {
            return String.Format(CultureInfo.InvariantCulture,
                "{0},{1},{2}/{3},{4},{5}/{6}/{7}",
                _position.X, _position.Y, _position.Z,
                _angles.X, _angles.Y, _angles.Z,
                FOV,
                ClipDistance
            );
        }
    }
}