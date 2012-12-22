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

        // I don't want to have to mess with proper typed binding if I don't have to,
        // so I'm specifying the type in the serialised output.
        private static string ToString(object obj)
        {
            if (obj == null) return "null";
            if (obj is string) return "string[" + obj + "]";
            if (obj is int) return "int[" + obj + "]";
            if (obj is decimal) return "decimal[" + obj + "]";
            if (obj is bool) return "bool[" + Convert.ToString(obj).ToLower() + "]";
            if (obj is Color) return "Color[" + ((Color) obj).ToArgb() + "]";
            return obj.GetType().Name + "[" + obj + "]";
        }

        private static object FromString(string str)
        {
            if (str == "null") return null;
            var spl = str.TrimEnd(']').Split('[');
            var type = spl[0];
            var val = spl[1];
            switch (type)
            {
                case "string":
                    return val;
                case "int":
                    return int.Parse(val);
                case "decimal":
                    return decimal.Parse(val);
                case "bool":
                    return val == "true";
                case "Color":
                    return Color.FromArgb(int.Parse(val));
                case "SnapStyle":
                    return Enum.Parse(typeof(SnapStyle), val, true);
                case "RotationStyle":
                    return Enum.Parse(typeof(RotationStyle), val, true);
                default:
                    throw new ArgumentException("Value not recognised: " + str);
            }
        }

        public static Dictionary<string, string> SerialiseSettings()
        {
            return GetProperties().ToDictionary(x => x.Name, x => ToString(x.GetValue(null, null)));
        }

        public static void DeserialiseSettings(Dictionary<string, string> dict)
        {
            GetProperties().ToList().ForEach(x => x.SetValue(null, FromString(dict[x.Name]), null));
        }
    }
}
