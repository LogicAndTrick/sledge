using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace Sledge.DataStructures
{
    public static class OpenTkExtensions
    {
        public static Quaternion QuaternionFromEulerRotation(Vector3 angles)
        {
            return QuaternionFromEulerRotation(angles.X, angles.Y, angles.Z);
        }

        public static Quaternion QuaternionFromEulerRotation(float yaw, float pitch, float roll)
        {
            // http://www.euclideanspace.com/maths/geometry/rotations/conversions/eulerToQuaternionF/index.htm

            var angles = new Vector3(yaw, pitch, roll);
            angles = angles / 2;

            var sy = (float) Math.Sin(angles.Z);
            var sp = (float) Math.Sin(angles.Y);
            var sr = (float) Math.Sin(angles.X);
            var cy = (float) Math.Cos(angles.Z);
            var cp = (float) Math.Cos(angles.Y);
            var cr = (float) Math.Cos(angles.X);

            return new Quaternion(sr * cp * cy - cr * sp * sy,
                                  cr * sp * cy + sr * cp * sy,
                                  cr * cp * sy - sr * sp * cy,
                                  cr * cp * cy + sr * sp * sy);
        }
    }
}
