using System;
using System.Runtime.Serialization;
using OpenTK;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures.Transformations
{
    [Serializable]
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

        public UnitMatrixMult(Matrix4 mat)
        {
            Matrix = new Matrix(new[]
            {
                (decimal) mat.M11, (decimal) mat.M21, (decimal) mat.M31, (decimal) mat.M41,
                (decimal) mat.M12, (decimal) mat.M22, (decimal) mat.M32, (decimal) mat.M42,
                (decimal) mat.M13, (decimal) mat.M23, (decimal) mat.M33, (decimal) mat.M43,
                (decimal) mat.M14, (decimal) mat.M24, (decimal) mat.M34, (decimal) mat.M44
            });
        }

        protected UnitMatrixMult(SerializationInfo info, StreamingContext context)
        {
            Matrix = (Matrix) info.GetValue("Matrix", typeof (Matrix));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Matrix", Matrix);
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
