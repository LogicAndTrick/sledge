using System;
using System.Runtime.Serialization;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.Transformations
{
    [Serializable]
    public class UnitScale : IUnitTransformation
    {
        public Coordinate Scalar { get; set; }
        public Coordinate Origin { get; set; }

        public UnitScale(Coordinate scalar, Coordinate origin)
        {
            Scalar = scalar;
            Origin = origin;
        }

        protected UnitScale(SerializationInfo info, StreamingContext context)
        {
            Scalar = (Coordinate) info.GetValue("Scalar", typeof (Coordinate));
            Origin = (Coordinate) info.GetValue("Origin", typeof (Coordinate));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Scalar", Scalar);
            info.AddValue("Origin", Origin);
        }

        public Coordinate Transform(Coordinate c)
        {
            return (c - Origin).ComponentMultiply(Scalar) + Origin;
        }
    }
}
