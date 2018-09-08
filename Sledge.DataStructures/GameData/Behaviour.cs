using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;
using System.Drawing;
using System.Globalization;
using System.Numerics;

namespace Sledge.DataStructures.GameData
{
    public class Behaviour
    {
        public string Name { get; set; }
        public List<string> Values { get; set; }

        public Behaviour(string name, params string[] values)
        {
            Name = name;
            Values = new List<string>(values);
        }

        public Vector3? GetVector3(int index)
        {
            var first = index * 3;
            return Values.Count < first + 3
                ? (Vector3?) null
                : NumericsExtensions.Parse(Values[first], Values[first + 1], Values[first + 2], NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        public Color GetColour(int index)
        {
            var coord = GetVector3(index);
            return coord == null ? Color.White : Color.FromArgb((int) coord.Value.X, (int) coord.Value.Y, (int) coord.Value.Z);
        }
    }
}
