using System;

namespace Sledge.Common
{
    /// <summary>
    /// Common math-related functions
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="degrees">The angle in degrees</param>
        /// <returns>The angle in radians</returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="radians">The angle in radians</param>
        /// <returns>The angle in degrees</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }
    }
}
