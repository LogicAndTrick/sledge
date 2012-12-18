using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.Transformations
{
    public class UnitScale : IUnitTransformation
    {
        public Coordinate Scalar { get; set; }
        public Coordinate Origin { get; set; }

        public UnitScale(Coordinate scalar, Coordinate origin)
        {
            Scalar = scalar;
            Origin = origin;
        }

        public Coordinate Transform(Coordinate c)
        {
            return (c - Origin).ComponentMultiply(Scalar) + Origin;
        }
    }
}
