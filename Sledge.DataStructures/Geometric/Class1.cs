using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.Common.Shell.Hooks;

namespace Sledge.DataStructures.Geometric
{
    [Export(typeof(IInitialiseHook))]
    public class Vector3TypeConverter : TypeConverter, IInitialiseHook
    {
        public Task OnInitialise()
        {
            TypeDescriptor.AddAttributes(typeof(Vector3), new TypeConverterAttribute(typeof(Vector3TypeConverter)));
            return Task.CompletedTask;
        }

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
                // valid formats: `[X Y Z]` ; `(X, Y, Z)` ; `X Y Z`; `<X, Y, Z>`
                if (str.Length > 1 && str[0] == '[' && str[str.Length - 1] == ']') str = str.Substring(1, str.Length - 2);
                else if (str.Length > 1 && str[0] == '(' && str[str.Length - 1] == ')') str = str.Substring(1, str.Length - 2);
                else if (str.Length > 1 && str[0] == '<' && str[str.Length - 1] == '>') str = str.Substring(1, str.Length - 2);

                var s = str.Split(' ');
                return new Vector3(
                    float.Parse(s[0], NumberStyles.Float, culture),
                    float.Parse(s[1], NumberStyles.Float, culture),
                    float.Parse(s[2], NumberStyles.Float, culture)
                );
            }
            if (value is Vector3 v)
            {
                return new Vector3(v.X, v.Y, v.Z);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(value is Vector3)) return null;
            var v = (Vector3)value;
            if (destinationType == typeof(string)) return String.Format(culture, "{0} {1} {2}", v.X, v.Y, v.Z);
            if (destinationType == typeof(Vector3)) return new Vector3(v.X, v.Y, v.Z);
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
