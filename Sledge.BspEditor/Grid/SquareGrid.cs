using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using Sledge.BspEditor.Properties;
using Sledge.Common;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Grid
{
    /// <summary>
    /// The standard square grid
    /// </summary>
    [AutoTranslate]
    [Export(typeof(IGrid))]
    public class SquareGrid : IGrid
    {
        // todo !grid derive high/low/step from map/environment
        public decimal High { get; set; } = 1024;
        public decimal Low { get; set; } = -1024;
        public decimal Step { get; set; } = 16;

        public string Name { get; set; }
        public string Details { get; set; }

        public virtual Image Icon => Resources.SquareGrid;

        public int Spacing
        {
            get => (int) Math.Log((double) Step, 2);
            set => Step = DMath.Pow(2, value);
        }

        private int _hideSmallerThan = 4;
        private int _hideFactor = 8;
        private int _highlight1LineNum = 8;
        private int _highlight2UnitNum = 1024;


        public Coordinate Snap(Coordinate coordinate)
        {
            return coordinate.Snap(Step);
        }

        public Coordinate AddStep(Coordinate coordinate, Coordinate add)
        {
            return coordinate + add * Step;
        }

        private decimal GetActualStep(decimal scale)
        {
            var step = Step;
            var actualDist = step * scale;
            while (actualDist < _hideSmallerThan)
            {
                step *= _hideFactor;
                actualDist *= _hideFactor;
            }
            return step;
        }

        public virtual IEnumerable<GridLine> GetLines(Coordinate normal, decimal scale, Coordinate worldMinimum, Coordinate worldMaximum)
        {
            var lower = Low;
            var upper = High;
            var step = GetActualStep(scale);

            Func<Coordinate, Coordinate> tform;
            if (normal == Coordinate.UnitX) tform = x => new Coordinate(Low, x.X, x.Y);
            else if (normal == Coordinate.UnitY) tform = x => new Coordinate(x.X, Low, x.Y);
            else if (normal == Coordinate.UnitZ) tform = x => new Coordinate(x.X, x.Y, Low);
            else throw new ArgumentOutOfRangeException(nameof(normal), @"Only UnitX, UnitY, and UnitZ are valid grid normal axes.");

            for (var f = lower; f <= upper; f += step)
            {
                if ((f < worldMinimum.X || f > worldMaximum.X) && (f < worldMinimum.Y || f > worldMaximum.Y)) continue;

                var i = (int) f;

                var type = GridLineType.Standard;
                if (i == 0) type = GridLineType.Axis;
                else if (i == Low || i == High) type = GridLineType.Boundary;
                else if (i % _highlight2UnitNum == 0) type = GridLineType.Secondary;
                else if (i % (int) (step * _highlight1LineNum) == 0) type = GridLineType.Primary;

                yield return new GridLine(type, tform(new Coordinate(lower, f, 0)), tform(new Coordinate(upper, f, 0)));
                yield return new GridLine(type, tform(new Coordinate(f, lower, 0)), tform(new Coordinate(f, upper, 0)));
            }
        }
    }
}