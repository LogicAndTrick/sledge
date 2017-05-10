using System;
using System.Collections.Generic;
using Sledge.Common;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Grid
{
    public class SquareGrid : IGrid
    {
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Step { get; set; }

        public int Spacing
        {
            get => (int) Math.Log((double) Step, 2);
            set => Step = DMath.Pow(2, value);
        }

        public int HideSmallerThan { get; } = 4;
        public int HideFactor { get; } = 8;
        public int Highlight1LineNum { get; } = 8;
        public int Highlight2UnitNum { get; } = 1024;

        public SquareGrid(decimal high, decimal low, decimal step)
        {
            High = high;
            Low = low;
            Step = step;
        }

        public Coordinate Snap(Coordinate coordinate)
        {
            return coordinate.Snap(Step);
        }

        public Coordinate AddStep(Coordinate coordinate, Coordinate add)
        {
            return coordinate + add * Step;
        }

        private decimal GetActualStep(decimal step, decimal scale)
        {
            var actualDist = step * scale;
            while (actualDist < HideSmallerThan)
            {
                step *= HideFactor;
                actualDist *= HideFactor;
            }
            return step;
        }

        public virtual IEnumerable<GridLine> GetLines(Coordinate normal, decimal scale, Coordinate worldMinimum, Coordinate worldMaximum)
        {
            var lower = Low;
            var upper = High;
            var step = GetActualStep(Step, scale);

            Func<Coordinate, Coordinate> tform;
            Func<Coordinate, Coordinate> rform;
            if (normal == Coordinate.UnitX)
            {
                tform = x => new Coordinate(Low, x.X, x.Y);
                rform = x => new Coordinate(x.Y, x.Z, 0);
            }
            else if (normal == Coordinate.UnitY)
            {
                tform = x => new Coordinate(x.X, Low, x.Y);
                rform = x => new Coordinate(x.X, x.Z, 0);
            }
            else if (normal == Coordinate.UnitZ)
            {
                tform = x => new Coordinate(x.X, x.Y, Low);
                rform = x => new Coordinate(x.X, x.Y, 0);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(normal), @"Only UnitX, UnitY, and UnitZ are valid grid normal axes.");
            }

            worldMinimum = rform(worldMinimum);
            worldMaximum = rform(worldMaximum);

            for (var f = lower; f <= upper; f += step)
            {
                if ((f < worldMinimum.X || f > worldMaximum.X) && (f < worldMinimum.Y || f > worldMaximum.Y)) continue;

                var i = (int) f;

                var type = GridLineType.Standard;
                if (i == 0) type = GridLineType.Axis;
                else if (i == Low || i == High) type = GridLineType.Boundary;
                else if (i % Highlight2UnitNum == 0) type = GridLineType.Secondary;
                else if (i % (int) (step * Highlight1LineNum) == 0) type = GridLineType.Primary;

                yield return new GridLine(type, tform(new Coordinate(lower, f, 0)), tform(new Coordinate(upper, f, 0)));
                yield return new GridLine(type, tform(new Coordinate(f, lower, 0)), tform(new Coordinate(f, upper, 0)));
            }
        }
    }
}