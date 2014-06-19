using System;
using System.Runtime.Serialization;
using Sledge.DataStructures.Transformations;

namespace Sledge.DataStructures.Geometric
{
    [Serializable]
    public class Line : ISerializable
    {
        public Coordinate Start { get; set; }
        public Coordinate End { get; set; }

        public readonly static Line AxisX = new Line(Coordinate.Zero, Coordinate.UnitX);
        public readonly static Line AxisY = new Line(Coordinate.Zero, Coordinate.UnitY);
        public static readonly Line AxisZ = new Line(Coordinate.Zero, Coordinate.UnitZ);

        public Line(Coordinate start, Coordinate end)
        {
            Start = start;
            End = end;
        }

        protected Line(SerializationInfo info, StreamingContext context)
        {
            Start = (Coordinate) info.GetValue("Start", typeof (Coordinate));
            End = (Coordinate) info.GetValue("End", typeof (Coordinate));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Start", Start);
            info.AddValue("End", End);
        }

        public Line Reverse()
        {
            return new Line(End, Start);
        }

        public Coordinate ClosestPoint(Coordinate point)
        {
            // http://paulbourke.net/geometry/pointline/

            var delta = End - Start;
            var den = delta.LengthSquared();
            if (den == 0) return Start; // Start and End are the same

            var numPoint = (point - Start).ComponentMultiply(delta);
            var num = numPoint.X + numPoint.Y + numPoint.Z;
            var u = num / den;

            if (u < 0) return Start; // Point is before the segment start
            if (u > 1) return End;   // Point is after the segment end
            return Start + u * delta;
        }

        /// <summary>
        /// Determines if this line is behind, in front, or spanning a plane.
        /// </summary>
        /// <param name="p">The plane to test against</param>
        /// <returns>A PlaneClassification value.</returns>
        public PlaneClassification ClassifyAgainstPlane(Plane p)
        {
            var start = p.OnPlane(Start);
            var end = p.OnPlane(End);

            if (start == 0 && end == 0) return PlaneClassification.OnPlane;
            if (start <= 0 && end <= 0) return PlaneClassification.Back;
            if (start >= 0 && end >= 0) return PlaneClassification.Front;
            return PlaneClassification.Spanning;
        }

        public Line Transform(IUnitTransformation transform)
        {
            return new Line(transform.Transform(Start), transform.Transform(End));
        }

        public bool EquivalentTo(Line other, decimal delta = 0.0001m)
        {
            return (Start.EquivalentTo(other.Start, delta) && End.EquivalentTo(other.End, delta))
                || (End.EquivalentTo(other.Start, delta) && Start.EquivalentTo(other.End, delta));
        }

        public bool Equals(Line other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (Equals(other.Start, Start) && Equals(other.End, End))
                || (Equals(other.End, Start) && Equals(other.Start, End));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Line)) return false;
            return Equals((Line) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Start != null ? Start.GetHashCode() : 0) * 397) ^ (End != null ? End.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Line left, Line right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Line left, Line right)
        {
            return !Equals(left, right);
        }
    }
}
