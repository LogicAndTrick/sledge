using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace Sledge.Common.Transport
{
    /// <summary>
    /// Common extensions for serialised objects
    /// </summary>
    public static class SerialisedObjectExtensions
    {
        /// <summary>
        /// Set a property value for a serialised object. The value will be converted with a type converter, if one exists.
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="so">The serialised object</param>
        /// <param name="key">The property key to set</param>
        /// <param name="value">The value to set</param>
        /// <param name="replace">True to replace any properties with the same key</param>
        public static void Set<T>(this SerialisedObject so, string key, T value, bool replace = true)
        {
            var conv = TypeDescriptor.GetConverter(typeof(T));
            var v = conv.ConvertToString(null, CultureInfo.InvariantCulture, value);
            if (replace) so.Properties.RemoveAll(s => s.Key == key);
            so.Properties.Add(new KeyValuePair<string, string>(key, v));
        }

        /// <summary>
        /// Get a property value from a serialised object. The value will be converted with a type converter, if one exists.
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="so">The serialised object</param>
        /// <param name="key">The property key to get</param>
        /// <param name="defaultValue">The default value to use if the key doesn't exists, or couldn't be converted</param>
        /// <returns>The property value, or the default value if the key wasn't found</returns>
        public static T Get<T>(this SerialisedObject so, string key, T defaultValue = default(T))
        {
            var match = so.Properties.Where(x => x.Key == key).ToList();
            if (!match.Any()) return defaultValue;
            try
            {
                var val = match[0].Value;
                var conv = TypeDescriptor.GetConverter(typeof(T));
                return (T) conv.ConvertFromString(null, CultureInfo.InvariantCulture, val);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Set a property value to a colour
        /// </summary>
        /// <param name="so">The serialised object</param>
        /// <param name="key">The property key to set</param>
        /// <param name="color">The value to set</param>
        public static void SetColor(this SerialisedObject so, string key, Color color)
        {
            var r = Convert.ToString(color.R, CultureInfo.InvariantCulture);
            var g = Convert.ToString(color.G, CultureInfo.InvariantCulture);
            var b = Convert.ToString(color.B, CultureInfo.InvariantCulture);
            Set(so, key, $"{r} {g} {b}");
        }

        /// <summary>
        /// Get a colour property from the serialised object
        /// </summary>
        /// <param name="so">The serialised object</param>
        /// <param name="key">The property key to get</param>
        /// <returns>The property value as a colour</returns>
        public static Color GetColor(this SerialisedObject so, string key)
        {
            var str = Get<string>(so, key) ?? "";
            var spl = str.Split(' ');
            if (spl.Length != 3) spl = new[] {"0", "0", "0"};
            byte.TryParse(spl[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var r);
            byte.TryParse(spl[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var g);
            byte.TryParse(spl[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var b);
            return Color.FromArgb(255, r, g, b);
        }
    }
}