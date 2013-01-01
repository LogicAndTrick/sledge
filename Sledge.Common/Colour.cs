using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Sledge.Common
{
    public static class Colour
    {
        private static Random _rand;

        static Colour()
        {
            _rand = new Random();
        }

        public static Color GetRandomColour()
        {
            return Color.FromArgb(255, _rand.Next(0, 256), _rand.Next(0, 256), _rand.Next(0, 256));
        }

        /// <summary>
        /// Brush colours only vary from shades of green and blue
        /// </summary>
        public static Color GetRandomBrushColour()
        {
            return Color.FromArgb(255, 0, _rand.Next(128, 256), _rand.Next(128, 256));
        }

        public static Color GetRandomLightColour()
        {
            return Color.FromArgb(255, _rand.Next(128, 256), _rand.Next(128, 256), _rand.Next(128, 256));
        }

        public static Color GetRandomDarkColour()
        {
            return Color.FromArgb(255, _rand.Next(0, 128), _rand.Next(0, 128), _rand.Next(0, 128));
        }
    }
}
