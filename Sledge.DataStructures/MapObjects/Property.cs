using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    public class Property
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public Color GetColour(Color defaultIfInvalid)
        {
            var spl = Value.Split(' ');
            if (spl.Length != 4) return defaultIfInvalid;
            int r, g, b, i;
            if (int.TryParse(spl[0], out r) && int.TryParse(spl[1], out g) && int.TryParse(spl[2], out b) && int.TryParse(spl[3], out i))
            {
                return Color.FromArgb(r, g, b);
            }
            return defaultIfInvalid;
        }

        public Coordinate GetCoordinate(Coordinate defaultIfInvalid)
        {
            var spl = Value.Split(' ');
            if (spl.Length != 3) return defaultIfInvalid;
            decimal x, y, z;
            if (decimal.TryParse(spl[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x)
                && decimal.TryParse(spl[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y)
                && decimal.TryParse(spl[2], NumberStyles.Float, CultureInfo.InvariantCulture, out z))
            {
                return new Coordinate(x, y, z);
            }
            return defaultIfInvalid;
        }

        public Property Clone()
        {
            return new Property
                       {
                           Key = Key,
                           Value = Value
                       };
        }
    }
}
