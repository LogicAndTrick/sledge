using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.Graphics.Helpers;

namespace Sledge.Graphics
{
    public class Camera
    {
        public Vector3d LookAt { get; set; }
        public Vector3d Location { get; set; }

        public Camera()
        {
            LookAt = new Vector3d(1, 0, 0);
            Location = new Vector3d(0, 0, 0);
        }

        public void Position()
        {
            var mode = Matrix.CurrentMode;
            Matrix.Set(MatrixMode.Modelview);
            Matrix4d modelview = Matrix4d.LookAt(Location, LookAt, Vector3d.UnitZ);
            GL.LoadMatrix(ref modelview);
            Matrix.Set(mode);
        }

        public decimal GetRotation()
        {
            var temp = (LookAt - Location);
            temp.Normalize();
            var rot = Math.Atan2(temp.Y, temp.X);
            if (rot < 0) rot += 2 * Math.PI;
            return (decimal) rot;
        }

        public void SetRotation(decimal rotation)
        {
            var temp = (LookAt - Location);
            temp.Normalize();
            var e = GetElevation();
            var x = Math.Cos((double)rotation) * Math.Sin((double)e);
            var y = Math.Sin((double)rotation) * Math.Sin((double)e);
            LookAt = new Vector3d((float)x + Location.X, (float)y + Location.Y, temp.Z + Location.Z);
        }

        public decimal GetElevation()
        {
            var temp = (LookAt - Location);
            temp.Normalize();
            var elev = Math.Acos(temp.Z);
            return (decimal) elev;
        }

        public void SetElevation(decimal elevation)
        {
            if (elevation > ((decimal)Math.PI * 0.99m)) elevation = (decimal)Math.PI * 0.99m;
            if (elevation < ((decimal)Math.PI * 0.01m)) elevation = (decimal)Math.PI * 0.01m;
            var rotation = GetRotation();
            var x = Math.Cos((double)rotation) * Math.Sin((double)elevation);
            var y = Math.Sin((double)rotation) * Math.Sin((double)elevation);
            var z = Math.Cos((double)elevation);
            LookAt = new Vector3d((float)x + Location.X, (float)y + Location.Y, (float)z + Location.Z);
        }

        public void Pan(decimal degrees)
        {
            var rad = degrees * ((decimal) Math.PI / 180);
            var rot = GetRotation();
            SetRotation(rot + rad);
        }

        public void Tilt(decimal degrees)
        {
            SetElevation(GetElevation() + (degrees * ((decimal)Math.PI / 180)));
        }

        public void Advance(decimal units)
        {
            var temp = LookAt - Location;
            temp.Normalize();
            var add = temp * (float)units;
            LookAt += add;
            Location += add;
        }

        public void Strafe(decimal units)
        {
            var right = GetRight();
            var add = right * (double)units;
            LookAt += add;
            Location += add;
        }

        public Vector3d GetUp()
        {
            var temp = LookAt - Location;
            temp.Normalize();
            var normal = Vector3d.Cross(GetRight(), temp);
            normal.Normalize();
            return normal;
        }

        public Vector3d GetRight()
        {
            var temp = LookAt - Location;
            temp.Z = 0;
            temp.Normalize();
            var normal = Vector3d.Cross(temp, Vector3d.UnitZ);
            normal.Normalize();
            return normal;
        }
    }
}
