using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Sledge.Common
{
    public static class Colour
    {
        private static readonly Random Rand;

        static Colour()
        {
            Rand = new Random();
        }

        public static Color GetRandomColour()
        {
            return Color.FromArgb(255, Rand.Next(0, 256), Rand.Next(0, 256), Rand.Next(0, 256));
        }

        /// <summary>
        /// Brush colours only vary from shades of green and blue
        /// </summary>
        public static Color GetRandomBrushColour()
        {
            return Color.FromArgb(255, 0, Rand.Next(128, 256), Rand.Next(128, 256));
        }

        /// <summary>
        /// Group colours only vary from shades of green and red
        /// </summary>
        public static Color GetRandomGroupColour()
        {
            return Color.FromArgb(255, Rand.Next(128, 256), Rand.Next(128, 256), 0);
        }

        public static Color GetRandomLightColour()
        {
            return Color.FromArgb(255, Rand.Next(128, 256), Rand.Next(128, 256), Rand.Next(128, 256));
        }

        public static Color GetRandomDarkColour()
        {
            return Color.FromArgb(255, Rand.Next(0, 128), Rand.Next(0, 128), Rand.Next(0, 128));
        }

        public static Color GetDefaultEntityColour()
        {
            return Color.FromArgb(255, 255, 0, 255);
        }

        public static Color Vary(this Color color, int by = 10)
        {
            by = Rand.Next(-by, by);
            return Color.FromArgb(color.A, Math.Min(255, Math.Max(0, color.R + by)), Math.Min(255, Math.Max(0, color.G + by)), Math.Min(255, Math.Max(0, color.B + by)));
        }

        public static Color Darken(this Color color, int by = 20)
        {
            return Color.FromArgb(color.A, Math.Max(0, color.R - by), Math.Max(0, color.G - by), Math.Max(0, color.B - by));
        }

        public static Color Lighten(this Color color, int by = 20)
        {
            return Color.FromArgb(color.A, Math.Min(255, color.R + by), Math.Min(255, color.G + by), Math.Min(255, color.B + by));
        }

        public static Color Blend(this Color color, Color other)
        {
            return Color.FromArgb(
                (byte) ((color.A) / 255f * (other.A / 255f) * 255),
                (byte) ((color.R) / 255f * (other.R / 255f) * 255),
                (byte) ((color.G) / 255f * (other.G / 255f) * 255),
                (byte) ((color.B) / 255f * (other.B / 255f) * 255)
                );
        }
    }
}
