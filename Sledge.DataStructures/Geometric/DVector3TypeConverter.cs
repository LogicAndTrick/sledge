using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;

namespace Sledge.DataStructures.Geometric
{
    public class DVector3TypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            if (sourceType == typeof(Vector3)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string)) return true;
            if (destinationType == typeof(Vector3)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                // valid formats: `[X Y Z]` ; `(X, Y, Z)` ; `X Y Z`
                if (str.Length > 1 && str[0] == '[' && str[str.Length - 1] == ']') str = str.Substring(1, str.Length - 2);
                else if (str.Length > 1 && str[0] == '(' && str[str.Length - 1] == ')') str = str.Substring(1, str.Length - 2);

                var s = str.Split(' ');
                return new DVector3(
                    decimal.Parse(s[0], NumberStyles.Float, culture),
                    decimal.Parse(s[1], NumberStyles.Float, culture),
                    decimal.Parse(s[2], NumberStyles.Float, culture));
            }
            if (value is Vector3 v)
            {
                return new DVector3((decimal)v.X, (decimal)v.Y, (decimal)v.Z);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var v = value as DVector3;
            if (v == null) return null;
            if (destinationType == typeof(string)) return String.Format(culture, "{0} {1} {2}", v.X, v.Y, v.Z);
            if (destinationType == typeof(Vector3)) return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
