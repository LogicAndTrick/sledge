using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Sledge.Common.Transport
{
    public static class SerialisedObjectExtensions
    {
        public static void Set<T>(this SerialisedObject so, string key, T value)
        {
            var conv = TypeDescriptor.GetConverter(typeof(T));
            var v = conv.ConvertToString(null, CultureInfo.InvariantCulture, value);
            so.Properties[key] = v;
        }

        public static T Get<T>(this SerialisedObject so, string key, T defaultValue = default(T))
        {
            if (!so.Properties.ContainsKey(key)) return defaultValue;
            try
            {
                var val = so.Properties[key];
                var conv = TypeDescriptor.GetConverter(typeof(T));
                return (T) conv.ConvertFromString(null, CultureInfo.InvariantCulture, val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static void SetColor(this SerialisedObject so, string key, Color color)
        {
            var r = Convert.ToString(color.R, CultureInfo.InvariantCulture);
            var g = Convert.ToString(color.G, CultureInfo.InvariantCulture);
            var b = Convert.ToString(color.B, CultureInfo.InvariantCulture);
            Set(so, key, $"{r} {g} {b}");
        }

        public static Color GetColor(this SerialisedObject so, string key)
        {
            var str = Get<string>(so, key) ?? "";
            var spl = str.Split(' ');
            if (spl.Length != 3) spl = new[] {"0", "0", "0"};
            byte.TryParse(spl[0], NumberStyles.Any, CultureInfo.InvariantCulture, out byte r);
            byte.TryParse(spl[1], NumberStyles.Any, CultureInfo.InvariantCulture, out byte g);
            byte.TryParse(spl[2], NumberStyles.Any, CultureInfo.InvariantCulture, out byte b);
            return Color.FromArgb(255, r, g, b);
        }
    }
}