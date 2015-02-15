using System;
using System.ComponentModel;

namespace Sledge.Editor.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static T FromDescription<T>(string description) where T : struct, IConvertible
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            foreach (T item in Enum.GetValues(type))
            {
                var fi = type.GetField(item.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                var desc = attributes.Length > 0 ? attributes[0].Description : item.ToString();
                if (desc == description) return item;
            }
            return default(T);
        }
    }
}
