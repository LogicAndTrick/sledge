using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.Geometric
{
    [Serializable]
    public class Box : ISerializable
    {
        public static readonly Box Empty = new Box(Vector3.Zero, Vector3.Zero);

        public Vector3 Start { get; }
        public Vector3 End { get; }

        public Vector3 Center => (Start + End) / 2f;

        /// <summary>
        /// The X value difference of this box
        /// </summary>
        public float Width => End.X - Start.X;

        /// <summary>
        /// The Y value difference of this box
        /// </summary>
        public float Length => End.Y - Start.Y;

        /// <summary>
        /// The Z value difference of this box
        /// </summary>
        public float Height => End.Z - Start.Z;

        /// <summary>
        /// Get the smallest dimension of this box
        /// </summary>
        public float SmallestDimension => Math.Min(Width, Math.Min(Length, Height));

        /// <summary>
        /// Get the largest dimension of this box
        /// </summary>
        public float LargestDimension => Math.Max(Width, Math.Max(Length, Height));

        /// <summary>
        /// Get the width (X), length(Y), and height (Z) of this box as a vector.
        /// </summary>
        public Vector3 Dimensions => new Vector3(Width, Length, Height);

        public Box(Vector3 start, Vector3 end) : this(new[] { start, end})
        {
        }

        public Box(IEnumerable<Vector3> vectors)
        {
            var list = vectors.ToList();
            if (!list.Any())
            {
                throw new Exception("Cannot create a bounding box out of zero Vectors.");
            }
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (var vertex in list)
            {
                min.X = Math.Min(vertex.X, min.X);
                min.Y = Math.Min(vertex.Y, min.Y);
                min.Z = Math.Min(vertex.Z, min.Z);
                max.X = Math.Max(vertex.X, max.X);
                max.Y = Math.Max(vertex.Y, max.Y);
                max.Z = Math.Max(vertex.Z, max.Z);
            }
            Start = min;
            End = max;
        }

        public Box(IEnumerable<Box> boxes)
        {
            var list = boxes.ToList();
            if (!list.Any())
            {
                throw new Exception("Cannot create a bounding box out of zero other boxes.");
            }
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (var box in list)
            {
                min.X = Math.Min(box.Start.X, min.X);
                min.Y = Math.Min(box.Start.Y, min.Y);
                min.Z = Math.Min(box.Start.Z, min.Z);
                max.X = Math.Max(box.End.X, max.X);
                max.Y = Math.Max(box.End.Y, max.Y);
                max.Z = Math.Max(box.End.Z, max.Z);
            }
            Start = min;
            End = max;
        }

        protected Box(SerializationInfo info, StreamingContext context)
        {
            Start = (Vector3) info.GetValue("Start", typeof (Vector3));
            End = (Vector3) info.GetValue("End", typeof (Vector3));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Start", Start);
            info.AddValue("End", End);
        }

        public bool IsEmpty()
        {
            return Math.Abs(Width) < 0.0001f && Math.Abs(Height) < 0.0001f && Math.Abs(Length) < 0.0001f;
        }
        public IEnumerable<Vector3> GetBoxPoints()
        {
            yield return new Vector3(Start.X, End.Y, End.Z);
            yield return End;
            yield return new Vector3(Start.X, Start.Y, End.Z);
            yield return new Vector3(End.X, Start.Y, End.Z);

            yield return new Vector3(Start.X, End.Y, Start.Z);
            yield return new Vector3(End.X, End.Y, Start.Z);
            yield return Start;
            yield return new Vector3(End.X, Start.Y, Start.Z);
        }

        public Polyhedron ToPolyhedron()
        {
            return new Polyhedron(GetBoxFaces().Select(x => new Polygon(x)));
        }

        public Plane[] GetBoxPlanes()
        {
            var planes = new Plane[6];
            var faces = GetBoxFaces();
            for (var i = 0; i < 6; i++)
            {
                planes[i] = new Plane(faces[i][0], faces[i][1], faces[i][2]);
            }
            return planes;
        }

        public Vector3[][] GetBoxFaces()
        {
            var topLeftBack = new Vector3(Start.X, End.Y, End.Z);
            var topRightBack = End;
            var topLeftFront = new Vector3(Start.X, Start.Y, End.Z);
            var topRightFront = new Vector3(End.X, Start.Y, End.Z);

            var bottomLeftBack = new Vector3(Start.X, End.Y, Start.Z);
            var bottomRightBack = new Vector3(End.X, End.Y, Start.Z);
            var bottomLeftFront = Start;
            var bottomRightFront = new Vector3(End.X, Start.Y, Start.Z);
            return new[]
                       {
                           new[] {topLeftFront, topRightFront, bottomRightFront, bottomLeftFront},
                           new[] {topRightBack, topLeftBack, bottomLeftBack, bottomRightBack},
                           new[] {topLeftBack, topLeftFront, bottomLeftFront, bottomLeftBack},
                           new[] {topRightFront, topRightBack, bottomRightBack, bottomRightFront},
                           new[] {topLeftBack, topRightBack, topRightFront, topLeftFront},
                           new[] {bottomLeftFront, bottomRightFront, bottomRightBack, bottomLeftBack}
                       };
        }

        public IEnumerable<Line> GetBoxLines()
        {
            var topLeftBack = new Vector3(Start.X, End.Y, End.Z);
            var topRightBack = End;
            var topLeftFront = new Vector3(Start.X, Start.Y, End.Z);
            var topRightFront = new Vector3(End.X, Start.Y, End.Z);

            var bottomLeftBack = new Vector3(Start.X, End.Y, Start.Z);
            var bottomRightBack = new Vector3(End.X, End.Y, Start.Z);
            var bottomLeftFront = Start;
            var bottomRightFront = new Vector3(End.X, Start.Y, Start.Z);

            yield return new Line(topLeftBack, topRightBack);
            yield return new Line(topLeftFront, topRightFront);
            yield return new Line(topLeftBack, topLeftFront);
            yield return new Line(topRightBack, topRightFront);

            yield return new Line(topLeftBack, bottomLeftBack);
            yield return new Line(topLeftFront, bottomLeftFront);
            yield return new Line(topRightBack, bottomRightBack);
            yield return new Line(topRightFront, bottomRightFront);

            yield return new Line(bottomLeftBack, bottomRightBack);
            yield return new Line(bottomLeftFront, bottomRightFront);
            yield return new Line(bottomLeftBack, bottomLeftFront);
            yield return new Line(bottomRightBack, bottomRightFront);
        }

        /// <summary>
        /// Returns true if this box overlaps the given box in any way
        /// </summary>
        public bool IntersectsWith(Box that)
        {
            if (Start.X >= that.End.X) return false;
            if (that.Start.X >= End.X) return false;

            if (Start.Y >= that.End.Y) return false;
            if (that.Start.Y >= End.Y) return false;

            if (Start.Z >= that.End.Z) return false;
            if (that.Start.Z >= End.Z) return false;

            return true;
        }

        /// <summary>
        /// Returns true if this box is completely inside the given box
        /// </summary>
        public bool ContainedWithin(Box that)
        {
            if (Start.X < that.Start.X) return false;
            if (Start.Y < that.Start.Y) return false;
            if (Start.Z < that.Start.Z) return false;

            if (End.X > that.End.X) return false;
            if (End.Y > that.End.Y) return false;
            if (End.Z > that.End.Z) return false;

            return true;
        }

        /* http://www.gamedev.net/community/forums/topic.asp?topic_id=338987 */
        /// <summary>
        /// Returns true if this box intersects the given line
        /// </summary>
        public bool IntersectsWith(Line that)
        {
            var start = that.Start;
            var finish = that.End;

            if (start.X < Start.X && finish.X < Start.X) return false;
            if (start.X > End.X && finish.X > End.X) return false;

            if (start.Y < Start.Y && finish.Y < Start.Y) return false;
            if (start.Y > End.Y && finish.Y > End.Y) return false;

            if (start.Z < Start.Z && finish.Z < Start.Z) return false;
            if (start.Z > End.Z && finish.Z > End.Z) return false;

            var d = (finish - start) / 2;
            var e = (End - Start) / 2;
            var c = start + d - ((Start + End) / 2);
            var ad = d.Absolute();

            if (Math.Abs(c.X) > e.X + ad.X) return false;
            if (Math.Abs(c.Y) > e.Y + ad.Y) return false;
            if (Math.Abs(c.Z) > e.Z + ad.Z) return false;

            var dca = d.Cross(c).Absolute();

            if (dca.X > e.Y * ad.Z + e.Z * ad.Y) return false;
            if (dca.Y > e.Z * ad.X + e.X * ad.Z) return false;
            if (dca.Z > e.X * ad.Y + e.Y * ad.X) return false;

            return true;
        }

        /// <summary>
        /// Returns true if the given Vector3 is inside this box.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool Vector3IsInside(Vector3 c)
        {
            return c.X >= Start.X && c.Y >= Start.Y && c.Z >= Start.Z
                   && c.X <= End.X && c.Y <= End.Y && c.Z <= End.Z;
        }

        public Box Transform(Matrix4x4 transform)
        {
            return new Box(GetBoxPoints().Select(x => Vector3.Transform(x, transform)));
        }

        public Box Clone()
        {
            return new Box(Start, End);
        }
    }
}
