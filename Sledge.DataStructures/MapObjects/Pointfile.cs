using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    public class Pointfile
    {
        public List<Line> Lines { get; set; }

        public Pointfile()
        {
            Lines = new List<Line>();
        }

        public static Pointfile Parse(IEnumerable<string> lines)
        {
            var pf = new Pointfile();
            var list = lines.ToList();
            if (!list.Any()) return pf;

            // Format detection: look at one line
            // .lin format: coordinate - coordinate
            // .pts format: coordinate
            var detect = list[0].Split(' ');
            var lin = detect.Length == 7;
            var pts = detect.Length == 3;
            if (!lin && !pts) throw new Exception("Invalid pointfile format.");

            Coordinate previous = null;
            foreach (var line in list)
            {
                var split = line.Split(' ');
                var point = Coordinate.Parse(split[0], split[1], split[2]);
                if (lin)
                {
                    var point2 = Coordinate.Parse(split[4], split[5], split[6]);
                    pf.Lines.Add(new Line(point2, point));
                }
                else // pts
                {
                    if (previous != null) pf.Lines.Add(new Line(previous, point));
                    previous = point;
                }
            }

            return pf;
        }
    }
}
