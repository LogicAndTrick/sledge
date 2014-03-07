using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using Sledge.DataStructures.Geometric;

namespace Sledge.Editor.Extensions
{
    public static class GeometricExtensions
    {
        public static Vector3 ToVector3(this Coordinate coordinate)
        {
            return new Vector3((float)coordinate.DX, (float)coordinate.DY, (float)coordinate.DZ);
        }

        public static Coordinate ToCoordinate(this Vector3 vector3)
        {
            return new Coordinate((decimal) vector3.X, (decimal) vector3.Y, (decimal) vector3.Z);
        }
    }
}
