using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.MapObjects
{
    public class DisplacementPoint
    {
        public Displacement Parent { get; set; }

        public Vertex CurrentPosition { get; set; }
        public Coordinate InitialPosition { get; set; }

        public Vector Displacement { get; set; }
        public Vector OffsetDisplacement { get; set; }

        public int XIndex { get; set; }
        public int YIndex { get; set; }
        public decimal Alpha { get; set; }

        /// <summary>
        /// Shorthand for CurrentPosition.Location.
        /// </summary>
        public Coordinate Location
        {
            get { return CurrentPosition.Location; }
        }

        public DisplacementPoint(Displacement parent, int x, int y)
        {
            Parent = parent;
            XIndex = x;
            YIndex = y;
            CurrentPosition = new Vertex(Coordinate.Zero, parent);
            InitialPosition = Coordinate.Zero;
            Displacement = new Vector(Coordinate.UnitZ, 0);
            OffsetDisplacement = new Vector(Coordinate.UnitZ, 0);
            Alpha = 0;
        }

        public IEnumerable<DisplacementPoint> GetAdjacentPoints()
        {
            yield return Parent.GetPoint(XIndex + 1, YIndex + 0);
            yield return Parent.GetPoint(XIndex - 1, YIndex + 0);
            yield return Parent.GetPoint(XIndex + 0, YIndex + 1);
            yield return Parent.GetPoint(XIndex + 0, YIndex - 1);
            yield return Parent.GetPoint(XIndex - 1, YIndex - 1);
            yield return Parent.GetPoint(XIndex - 1, YIndex + 1);
            yield return Parent.GetPoint(XIndex + 1, YIndex - 1);
            yield return Parent.GetPoint(XIndex + 1, YIndex + 1);
        }
    }
}
