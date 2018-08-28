using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Grid
{
    public class SquareGrid : IGrid
    {
        public float High { get; set; }
        public float Low { get; set; }
        public float Step { get; set; }

        public int Spacing
        {
            get => (int) Math.Log(Step, 2);
            set => Step = (float) Math.Pow(2, Math.Max(Math.Min(value, 10), -2));
        }

        public int HideSmallerThan => SquareGridFactory.GridHideSmallerThan;
        public int HideFactor => SquareGridFactory.GridHideFactor;
        public int Highlight1LineNum => SquareGridFactory.GridPrimaryHighlight;
        public int Highlight2UnitNum => SquareGridFactory.GridSecondaryHighlight;

        public SquareGrid(float high, float low, float step)
        {
            High = high;
            Low = low;
            Step = step;
        }

        public string Description => Step.ToString(CultureInfo.InvariantCulture);

        public Vector3 Snap(Vector3 vector)
        {
            return vector.Snap(Step);
        }

        public Vector3 AddStep(Vector3 vector, Vector3 add)
        {
            return vector + add * Step;
        }

        private float GetActualStep(float step, float scale)
        {
            var actualDist = step * scale;
            while (actualDist < HideSmallerThan && HideFactor > 0)
            {
                step *= HideFactor;
                actualDist *= HideFactor;
            }
            return step;
        }

        public virtual IEnumerable<GridLine> GetLines(Vector3 normal, float scale, Vector3 worldMinimum, Vector3 worldMaximum)
        {
            var lower = Low;
            var upper = High;
            var step = GetActualStep(Step, scale);

            Func<Vector3, Vector3> tform;
            Func<Vector3, Vector3> rform;
            if (normal == Vector3.UnitX)
            {
                tform = x => new Vector3(Low, x.X, x.Y);
                rform = x => new Vector3(x.Y, x.Z, 0);
            }
            else if (normal == Vector3.UnitY)
            {
                tform = x => new Vector3(x.X, Low, x.Y);
                rform = x => new Vector3(x.X, x.Z, 0);
            }
            else if (normal == Vector3.UnitZ)
            {
                tform = x => new Vector3(x.X, x.Y, Low);
                rform = x => new Vector3(x.X, x.Y, 0);
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
                if (f != i) type = GridLineType.Fractional;
                else if (i == 0) type = GridLineType.Axis;
                else if (Math.Abs(i - Low) < 0.01f || Math.Abs(i - High) < 0.01f) type = GridLineType.Boundary;
                else if (Highlight2UnitNum > 0 && i % Highlight2UnitNum == 0) type = GridLineType.Secondary;
                else if (Highlight1LineNum > 0 && i % (int) (step * Highlight1LineNum) == 0) type = GridLineType.Primary;

                yield return new GridLine(type, tform(new Vector3(lower, f, 0)), tform(new Vector3(upper, f, 0)));
                yield return new GridLine(type, tform(new Vector3(f, lower, 0)), tform(new Vector3(f, upper, 0)));
            }
        }
    }
}