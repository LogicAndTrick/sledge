using System;

namespace Sledge.Common
{
    public static class MathHelper
    {
        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }
    }
}
