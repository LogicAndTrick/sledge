using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;
using System.Drawing;

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

        public Coordinate GetCoordinate(int index)
        {
            var first = index * 3;
            return Values.Count < first + 3 ?
                null : Coordinate.Parse(Values[first], Values[first + 1], Values[first + 2]);
        }

        public Color GetColour(int index)
        {
            var coord = GetCoordinate(index);
            return coord == null ? Color.White : Color.FromArgb((int) coord.X, (int) coord.Y, (int) coord.Z);
        }
    }
}
