using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.Transformations
{
    public class UnitMatrixMult : IUnitTransformation
    {
        public Matrix Matrix { get; set; }

        public UnitMatrixMult(decimal [] matrix)
        {
            Matrix = new Matrix(matrix);
        }

        public UnitMatrixMult(Matrix matrix)
        {
            Matrix = matrix;
        }

        public Coordinate Transform(Coordinate c)
        {
            var x = Matrix[0] * c.X + Matrix[1] * c.Y + Matrix[2] * c.Z + Matrix[3];
            var y = Matrix[4] * c.X + Matrix[5] * c.Y + Matrix[6] * c.Z + Matrix[7];
            var z = Matrix[8] * c.X + Matrix[9] * c.Y + Matrix[10] * c.Z + Matrix[11];
            return new Coordinate(x, y, z);
        }
    }
}
