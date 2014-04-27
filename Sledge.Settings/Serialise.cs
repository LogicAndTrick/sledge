using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Sledge.Settings
{
    public static class Serialise
    {
        private static IEnumerable<PropertyInfo> GetProperties()
        {
            var list = new[] {typeof (Grid), typeof (Select), typeof (Steam), typeof (View), typeof(Layout)};
            return list.SelectMany(x => x.GetProperties(BindingFlags.Static | BindingFlags.Public));
        }

        private static string ToString(object obj)
        {
            if (obj == null) return "";
            if (obj is Color)
            {
                var c = (Color) obj;
                return c.R + " " + c.G + " " + c.B;
            }
            return Convert.ToString(obj, CultureInfo.InvariantCulture);
        }

        private static object FromString(Type t, string str)
        {
            if (t.IsEnum)
            {
                return t.GetEnumValues().OfType<Enum>().FirstOrDefault(x => String.Equals(str, x.ToString(), StringComparison.CurrentCultureIgnoreCase))
                       ?? t.GetEnumValues().OfType<Enum>().FirstOrDefault();
            }
            if (t == typeof(decimal))
            {
                // Settings were saved with culture before, need backwards compatibility
                str = str.Replace(',', '.');
            }
            if (t == typeof(Color))
            {
                var spl = str.Split(' ');
                int r, g, b;
                int.TryParse(spl[0], out r);
                int.TryParse(spl[1], out g);
                int.TryParse(spl[2], out b);
                return Color.FromArgb(r, g, b);
            }
            return Convert.ChangeType(str, t, CultureInfo.InvariantCulture);
        }

        public static Dictionary<string, string> SerialiseSettings()
        {
            return GetProperties().ToDictionary(x => x.Name, x => ToString(x.GetValue(null, null)));
        }

        public static void DeserialiseSettings(Dictionary<string, string> dict)
        {
            foreach (var prop in GetProperties().Where(prop => dict.ContainsKey(prop.Name)))
            {
                prop.SetValue(null, FromString(prop.PropertyType, dict[prop.Name]), null);
            }
        }
    }
}
