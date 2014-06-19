using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.Geometric
{
    /// <summary>
    /// A cloud is a wrapper around a collection of points, allowing
    /// various useful operations to be performed on them.
    /// </summary>
    [Serializable]
    public class Cloud : ISerializable
    {
        public List<Coordinate> Points { get; private set; }
        public Box BoundingBox { get; private set; }

        public Coordinate MinX { get; private set; }
        public Coordinate MinY { get; private set; }
        public Coordinate MinZ { get; private set; }
        public Coordinate MaxX { get; private set; }
        public Coordinate MaxY { get; private set; }
        public Coordinate MaxZ { get; private set; }

        public Cloud(IEnumerable<Coordinate> points)
        {
            Points = new List<Coordinate>(points);
            BoundingBox = new Box(points);
            MinX = MinY = MinZ = MaxX = MaxY = MaxZ = null;
            foreach (var p in points)
            {
                if (MinX == null || p.X < MinX.X) MinX = p;
                if (MinY == null || p.Y < MinY.Y) MinY = p;
                if (MinZ == null || p.Z < MinZ.Z) MinZ = p;
                if (MaxX == null || p.X > MaxX.X) MaxX = p;
                if (MaxY == null || p.Y > MaxY.Y) MaxY = p;
                if (MaxZ == null || p.Z > MaxZ.Z) MaxZ = p; 
            }
        }

        protected Cloud(SerializationInfo info, StreamingContext context) : this((Coordinate[]) info.GetValue("Points", typeof(Coordinate[])))
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Points", Points.ToArray());
        }

        /// <summary>
        /// Get a list of the 6 points that define the outermost extents of this cloud.
        /// </summary>
        /// <returns>A list of the 6 (Min|Max)(X|Y|Z) values of this cloud.</returns>
        public IEnumerable<Coordinate> GetExtents()
        {
            return new[]
                       {
                           MinX, MinY, MinZ,
                           MaxX, MaxY, MaxZ
                        };
        }
    }
}
