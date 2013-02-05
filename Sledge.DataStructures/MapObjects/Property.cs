using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

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
