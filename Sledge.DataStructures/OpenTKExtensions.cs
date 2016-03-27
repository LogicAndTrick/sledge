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
            Quaternion rotateX = Quaternion.FromAxisAngle(Vector3.UnitX, yaw);
            Quaternion rotateY = Quaternion.FromAxisAngle(Vector3.UnitY, pitch);
            Quaternion rotateZ = Quaternion.FromAxisAngle(Vector3.UnitZ, roll);

            Quaternion.Multiply(ref rotateZ, ref rotateY, out rotateY);
            Quaternion.Multiply(ref rotateX, ref rotateY, out rotateY);
            return rotateY;
        }
    }
}
