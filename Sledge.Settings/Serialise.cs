using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Sledge.Settings
{
    public static class Serialise
    {
        private static IEnumerable<PropertyInfo> GetProperties()
        {
            var list = new[] {typeof (Grid), typeof (Select), typeof (Steam), typeof (View)};
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
            return obj.ToString();
        }

        private static object FromString(Type t, string str)
        {
            if (t.IsEnum) return Enum.Parse(t, str);
            if (t == typeof(Color))
            {
                var spl = str.Split(' ');
                int r, g, b;
                int.TryParse(spl[0], out r);
                int.TryParse(spl[1], out g);
                int.TryParse(spl[2], out b);
                return Color.FromArgb(r, g, b);
            }
            return Convert.ChangeType(str, t);
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
