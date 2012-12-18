﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Sledge.DataStructures.Geometric
{
    public class Box
    {
        public Coordinate Start { get; private set; }
        public Coordinate End { get; private set; }
        public Coordinate Center { get; private set; }

        public Box(Coordinate start, Coordinate end)
        {
            Start = start;
            End = end;
            Center = (Start + End) / 2;
        }

        public Box(IEnumerable<Coordinate> coordinates)
        {
            if (!coordinates.Any())
            {
                throw new Exception("Cannot create a bounding box out of zero coordinates.");
            }
            var min = new Coordinate(decimal.MaxValue, decimal.MaxValue, decimal.MaxValue);
            var max = new Coordinate(decimal.MinValue, decimal.MinValue, decimal.MinValue);
            foreach (var vertex in coordinates)
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
            Center = (Start + End) / 2;
        }

        public Box(IEnumerable<Box> boxes)
        {
            if (!boxes.Any())
            {
                throw new Exception("Cannot create a bounding box out of zero other boxes.");
            }
            var min = new Coordinate(decimal.MaxValue, decimal.MaxValue, decimal.MaxValue);
            var max = new Coordinate(decimal.MinValue, decimal.MinValue, decimal.MinValue);
            foreach (var box in boxes)
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
            Center = (Start + End) / 2;
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

        public Coordinate[][] GetBoxFaces()
        {
            var topLeftBack = new Coordinate(Start.X, End.Y, End.Z);
            var topRightBack = End.Clone();
            var topLeftFront = new Coordinate(Start.X, Start.Y, End.Z);
            var topRightFront = new Coordinate(End.X, Start.Y, End.Z);

            var bottomLeftBack = new Coordinate(Start.X, End.Y, Start.Z);
            var bottomRightBack = new Coordinate(End.X, End.Y, Start.Z);
            var bottomLeftFront = Start.Clone();
            var bottomRightFront = new Coordinate(End.X, Start.Y, Start.Z);
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
            var topLeftBack = new Coordinate(Start.X, End.Y, End.Z);
            var topRightBack = End.Clone();
            var topLeftFront = new Coordinate(Start.X, Start.Y, End.Z);
            var topRightFront = new Coordinate(End.X, Start.Y, End.Z);

            var bottomLeftBack = new Coordinate(Start.X, End.Y, Start.Z);
            var bottomRightBack = new Coordinate(End.X, End.Y, Start.Z);
            var bottomLeftFront = Start.Clone();
            var bottomRightFront = new Coordinate(End.X, Start.Y, Start.Z);

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

        public Box Clone()
        {
            return new Box(Start.Clone(), End.Clone());
        }
    }
}
