using System;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.Geometric
{
    [Serializable]
    public class Vector : Coordinate
    {
        public Coordinate Normal { get; set; }
        public decimal Distance { get; set; }

        public Vector(Coordinate normal, decimal distance)
            : base(0, 0, 0)
        {
            Normal = normal.Normalise();
            Distance = distance;
            var temp = Normal * Distance;
            X = temp.X;
            Y = temp.Y;
            Z = temp.Z;
        }

        public Vector(Coordinate offsets)
            : base(0, 0, 0)
        {
            Set(offsets);
        }

        protected Vector(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Normal = (Coordinate) info.GetValue("Normal", typeof (Coordinate));
            Distance = info.GetDecimal("Distance");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Normal", Normal);
            info.AddValue("Distance", Distance);
        }

        public void SetToZero()
        {
            X = Y = Z = Distance = 0;
        }

        public void Set(Coordinate offsets)
        {
            Distance = offsets.VectorMagnitude();
            if (Distance == 0)
            {
                X = Y = Z = 0;
            }
            else
            {
                X = offsets.X / Distance;
                Y = offsets.Y / Distance;
                Z = offsets.Z / Distance;
            }
        }
    }
}
