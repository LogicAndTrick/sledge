using System;
using System.Linq;

namespace Sledge.Extensions
{
    public static class DMath
    {
        public static readonly decimal PI;

        static DMath()
        {
            PI = (decimal)Math.PI;
        }

        public static decimal Sqrt(decimal val)
        {
            return (decimal)Math.Sqrt((double) val);
        }

        public static decimal Pow(decimal b, decimal e)
        {
            return (decimal)Math.Pow((double) b, (double) e);
        }

        public static decimal Tan(decimal angle)
        {
            return (decimal)Math.Tan((double)angle);
        }

        public static decimal Atan(decimal angle)
        {
            return (decimal)Math.Atan((double)angle);
        }

        public static decimal Atan2(decimal angle1, decimal angle2)
        {
            return (decimal)Math.Atan2((double)angle1, (double)angle2);
        }

        public static decimal Cos(decimal angle)
        {
            return (decimal)Math.Cos((double) angle);
        }

        public static decimal Sin(decimal angle)
        {
            return (decimal)Math.Sin((double)angle);
        }

        public static decimal Asin(decimal angle)
        {
            return (decimal)Math.Asin((double)angle);
        }

        public static decimal Acos(decimal value)
        {
            return (decimal)Math.Acos((double) value);
        }

        public static decimal DegreesToRadians(decimal degrees)
        {
            return degrees * PI / 180;
        }

        public static decimal RadiansToDegrees(decimal radians)
        {
            return radians * 180 / PI;
        }

        public static decimal Abs(decimal d)
        {
            return d < 0 ? -d : d;
        }

        public static decimal Clamp(decimal d, decimal min, decimal max)
        {
            return (d < min) ? min : (d > max) ? max : d;
        }
    }
}
