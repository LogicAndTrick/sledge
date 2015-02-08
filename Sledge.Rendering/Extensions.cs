using System.Drawing;
using OpenTK;
using Sledge.DataStructures.Geometric;

namespace Sledge.Rendering
{
    public static class Extensions
    {
        public static Vector3 ToVector3(this Coordinate coordinate)
        {
            return new Vector3((float)coordinate.DX, (float)coordinate.DY, (float)coordinate.DZ);
        }

        public static Coordinate ToCoordinate(this Vector3 vector3)
        {
            return new Coordinate((decimal)vector3.X, (decimal)vector3.Y, (decimal)vector3.Z);
        }

        public static int ToAbgr(this Color color)
        {
            return (color.A << 24) | (color.B << 16) | (color.G << 8) | color.R;
        }
    }
}