using System;
using System.Runtime.Serialization;
using Sledge.DataStructures.Geometric;
using Sledge.Extensions;

namespace Sledge.DataStructures.Transformations
{
    [Serializable]
    public class UnitRotate : IUnitTransformation
    {
        public decimal Rotation { get; set; }
        public Line Axis { get; set; }

        public UnitRotate(decimal scalar, Line axis)
        {
            Rotation = scalar;
            Axis = axis;
        }

        protected UnitRotate(SerializationInfo info, StreamingContext context)
        {
            Rotation = info.GetInt32("Rotation");
            Axis = (Line) info.GetValue("Axis", typeof (Line));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Rotation", Rotation);
            info.AddValue("Axis", Axis);
        }

        /**
         * http://paulbourke.net/geometry/rotate/
         */
        public Coordinate Transform(Coordinate c)
        {
            var p = c - Axis.Start;
            var r = (Axis.End - Axis.Start).Normalise();

            var costheta = DMath.Cos(Rotation);
            var sintheta = DMath.Sin(Rotation);

            decimal x = 0, y = 0, z = 0;

            x += (costheta + (1 - costheta) * r.X * r.X) * p.X;
            x += ((1 - costheta) * r.X * r.Y - r.Z * sintheta) * p.Y;
            x += ((1 - costheta) * r.X * r.Z + r.Y * sintheta) * p.Z;

            y += ((1 - costheta) * r.X * r.Y + r.Z * sintheta) * p.X;
            y += (costheta + (1 - costheta) * r.Y * r.Y) * p.Y;
            y += ((1 - costheta) * r.Y * r.Z - r.X * sintheta) * p.Z;

            z += ((1 - costheta) * r.X * r.Z - r.Y * sintheta) * p.X;
            z += ((1 - costheta) * r.Y * r.Z + r.X * sintheta) * p.Y;
            z += (costheta + (1 - costheta) * r.Z * r.Z) * p.Z;

            return new Coordinate(x, y, z) + Axis.Start;
        }
    }
}
