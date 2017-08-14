using System;
using System.Runtime.Serialization;
using OpenTK;
using Sledge.Common;

namespace Sledge.DataStructures.Geometric
{
    /// <summary>
    /// Represents a 4x4 matrix. Shamelessly taken in its entirety from OpenTK's Matrix4 structure. http://www.opentk.com/
    /// </summary>
    [Serializable]
    public class Matrix : ISerializable
    {
        public static Matrix Zero
        {
            get { return new Matrix(); }
        }

        public static Matrix Identity
        {
            get
            {
                return new Matrix(1, 0, 0, 0,
                                  0, 1, 0, 0,
                                  0, 0, 1, 0,
                                  0, 0, 0, 1);
            }
        }

        public decimal[] Values { get; private set; }

        public decimal this[int i]
        {
            get { return Values[i]; }
            set { Values[i] = value; }
        }

        public Coordinate X { get { return new Coordinate(Values[0], Values[1], Values[2]); } }
        public Coordinate Y { get { return new Coordinate(Values[4], Values[5], Values[6]); } }
        public Coordinate Z { get { return new Coordinate(Values[8], Values[9], Values[10]); } }
        public Coordinate Shift { get { return new Coordinate(Values[3], Values[7], Values[11]); } }

        public Matrix()
        {
            Values = new decimal[]
                         {
                             0, 0, 0, 0,
                             0, 0, 0, 0,
                             0, 0, 0, 0,
                             0, 0, 0, 0
                         };
        }

        protected Matrix(SerializationInfo info, StreamingContext context)
        {
            Values = (decimal[]) info.GetValue("Values", typeof (decimal[]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Values", Values);
        }

        public Matrix(params decimal[] values)
        {
            if (values.Length != 16) throw new Exception("A matrix must be 16 values long.");
            Values = values;
        }

        public decimal Determinant()
        {
            return
                Values[0] * Values[5] * Values[10] * Values[15] - Values[0] * Values[5] * Values[11] * Values[14] + Values[0] * Values[6] * Values[11] * Values[13] - Values[0] * Values[6] * Values[9] * Values[15]
              + Values[0] * Values[7] * Values[9] * Values[14] - Values[0] * Values[7] * Values[10] * Values[13] - Values[1] * Values[6] * Values[11] * Values[12] + Values[1] * Values[6] * Values[8] * Values[15]
              - Values[1] * Values[7] * Values[8] * Values[14] + Values[1] * Values[7] * Values[10] * Values[12] - Values[1] * Values[4] * Values[10] * Values[15] + Values[1] * Values[4] * Values[11] * Values[14]
              + Values[2] * Values[7] * Values[8] * Values[13] - Values[2] * Values[7] * Values[9] * Values[12] + Values[2] * Values[4] * Values[9] * Values[15] - Values[2] * Values[4] * Values[11] * Values[13]
              + Values[2] * Values[5] * Values[11] * Values[12] - Values[2] * Values[5] * Values[8] * Values[15] - Values[3] * Values[4] * Values[9] * Values[14] + Values[3] * Values[4] * Values[10] * Values[13]
              - Values[3] * Values[5] * Values[10] * Values[12] + Values[3] * Values[5] * Values[8] * Values[14] - Values[3] * Values[6] * Values[8] * Values[13] + Values[3] * Values[6] * Values[9] * Values[12];
        }

        public Matrix Inverse()
        {
            int[] colIdx = { 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1 };

            // convert the matrix to an array for easy looping
            var inverse = new[,]
                              {
                                  {Values[0], Values[1], Values[2], Values[12]},
                                  {Values[4], Values[5], Values[6], Values[13]},
                                  {Values[8], Values[9], Values[10], Values[14]},
                                  {Values[3], Values[7], Values[11], Values[15]}
                              };
            var icol = 0;
            var irow = 0;
            for (var i = 0; i < 4; i++)
            {
                // Find the largest pivot value
                var maxPivot = 0m;
                for (var j = 0; j < 4; j++)
                {
                    if (pivotIdx[j] != 0)
                    {
                        for (var k = 0; k < 4; ++k)
                        {
                            if (pivotIdx[k] == -1)
                            {
                                var absVal = DMath.Abs(inverse[j, k]);
                                if (absVal > maxPivot)
                                {
                                    maxPivot = absVal;
                                    irow = j;
                                    icol = k;
                                }
                            }
                            else if (pivotIdx[k] > 0)
                            {
                                return this;
                            }
                        }
                    }
                }

                ++(pivotIdx[icol]);

                // Swap rows over so pivot is on diagonal
                if (irow != icol)
                {
                    for (var k = 0; k < 4; ++k)
                    {
                        var f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                var pivot = inverse[icol, icol];
                // check for singular matrix
                if (pivot == 0)
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                }

                // Scale row so it has a unit diagonal
                var oneOverPivot = 1m / pivot;
                inverse[icol, icol] = 1;
                for (var k = 0; k < 4; ++k) inverse[icol, k] *= oneOverPivot;

                // Do elimination of non-diagonal elements
                for (var j = 0; j < 4; ++j)
                {
                    // check this isn't on the diagonal
                    if (icol != j)
                    {
                        var f = inverse[j, icol];
                        inverse[j, icol] = 0;
                        for (var k = 0; k < 4; ++k) inverse[j, k] -= inverse[icol, k] * f;
                    }
                }
            }

            for (var j = 3; j >= 0; --j)
            {
                var ir = rowIdx[j];
                var ic = colIdx[j];
                for (var k = 0; k < 4; ++k)
                {
                    var f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            return new Matrix(
                inverse[0, 0], inverse[0, 1], inverse[0, 2], inverse[3, 0],
                inverse[1, 0], inverse[1, 1], inverse[1, 2], inverse[3, 1],
                inverse[2, 0], inverse[2, 1], inverse[2, 2], inverse[3, 2],
                inverse[0, 3], inverse[1, 3], inverse[2, 3], inverse[3, 3]);
        }

        public Matrix Transpose()
        {
            return new Matrix(Values[0], Values[4], Values[8], Values[12],
                              Values[1], Values[5], Values[9], Values[13],
                              Values[2], Values[6], Values[10], Values[14],
                              Values[3], Values[7], Values[11], Values[15]);
        }

        public Matrix Translate(Coordinate translation)
        {
            return new Matrix(Values[0], Values[1], Values[2], Values[3] + translation.X,
                              Values[4], Values[5], Values[6], Values[7] + translation.Y,
                              Values[8], Values[9], Values[10], Values[11] + translation.Z,
                              Values[12], Values[13], Values[14], Values[15]);
        }

        public bool EquivalentTo(Matrix other, decimal delta = 0.0001m)
        {
            for (var i = 0; i < 16; i++)
            {
                if (DMath.Abs(Values[i] - other.Values[i]) >= delta) return false;
            }
            return true;
        }

        public Matrix4 ToGLSLMatrix4()
        {
            return new Matrix4(
                (float)this[0],
                (float)this[1],
                (float)this[2],
                (float)this[3],
                (float)this[4],
                (float)this[5],
                (float)this[6],
                (float)this[7],
                (float)this[8],
                (float)this[9],
                (float)this[10],
                (float)this[11],
                (float)this[12],
                (float)this[13],
                (float)this[14],
                (float)this[15]
                );
        }

        public Matrix4 ToOpenTKMatrix4()
        {
            return new Matrix4(
                (float)this[0],
                (float)this[1],
                (float)this[2],
                (float)this[12],
                (float)this[4],
                (float)this[5],
                (float)this[6],
                (float)this[13],
                (float)this[8],
                (float)this[9],
                (float)this[10],
                (float)this[14],
                (float)this[3],
                (float)this[7],
                (float)this[11],
                (float)this[15]
                );
        }

        public bool Equals(Matrix other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            for (var i = 0; i < 16; i++)
            {
                if (Values[i] != other.Values[i]) return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Matrix)) return false;
            return Equals((Matrix) obj);
        }

        public override int GetHashCode()
        {
            return (Values != null ? Values.GetHashCode() : 0);
        }

        public static bool operator ==(Matrix left, Matrix right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Matrix left, Matrix right)
        {
            return !Equals(left, right);
        }

        public static Coordinate operator *(Coordinate left, Matrix right)
        {
            return new Coordinate(
                right[3] + left.X * right[0] + left.Y * right[4] + left.Z * right[8],
                right[7] + left.X * right[1] + left.Y * right[5] + left.Z * right[9],
                right[11] + left.X * right[2] + left.Y * right[6] + left.Z * right[10]);
        }

        public static Matrix operator * (Matrix left, Matrix right)
        {
            return new Matrix(
                left.Values[0] * right.Values[0] + left.Values[1] * right.Values[4] + left.Values[2] * right.Values[8] + left.Values[3] * right.Values[12],
                left.Values[0] * right.Values[1] + left.Values[1] * right.Values[5] + left.Values[2] * right.Values[9] + left.Values[3] * right.Values[13],
                left.Values[0] * right.Values[2] + left.Values[1] * right.Values[6] + left.Values[2] * right.Values[10] + left.Values[3] * right.Values[14],
                left.Values[0] * right.Values[3] + left.Values[1] * right.Values[7] + left.Values[2] * right.Values[11] + left.Values[3] * right.Values[15],
                left.Values[4] * right.Values[0] + left.Values[5] * right.Values[4] + left.Values[6] * right.Values[8] + left.Values[7] * right.Values[12],
                left.Values[4] * right.Values[1] + left.Values[5] * right.Values[5] + left.Values[6] * right.Values[9] + left.Values[7] * right.Values[13],
                left.Values[4] * right.Values[2] + left.Values[5] * right.Values[6] + left.Values[6] * right.Values[10] + left.Values[7] * right.Values[14],
                left.Values[4] * right.Values[3] + left.Values[5] * right.Values[7] + left.Values[6] * right.Values[11] + left.Values[7] * right.Values[15],
                left.Values[8] * right.Values[0] + left.Values[9] * right.Values[4] + left.Values[10] * right.Values[8] + left.Values[11] * right.Values[12],
                left.Values[8] * right.Values[1] + left.Values[9] * right.Values[5] + left.Values[10] * right.Values[9] + left.Values[11] * right.Values[13],
                left.Values[8] * right.Values[2] + left.Values[9] * right.Values[6] + left.Values[10] * right.Values[10] + left.Values[11] * right.Values[14],
                left.Values[8] * right.Values[3] + left.Values[9] * right.Values[7] + left.Values[10] * right.Values[11] + left.Values[11] * right.Values[15],
                left.Values[12] * right.Values[0] + left.Values[13] * right.Values[4] + left.Values[14] * right.Values[8] + left.Values[15] * right.Values[12],
                left.Values[12] * right.Values[1] + left.Values[13] * right.Values[5] + left.Values[14] * right.Values[9] + left.Values[15] * right.Values[13],
                left.Values[12] * right.Values[2] + left.Values[13] * right.Values[6] + left.Values[14] * right.Values[10] + left.Values[15] * right.Values[14],
                left.Values[12] * right.Values[3] + left.Values[13] * right.Values[7] + left.Values[14] * right.Values[11] + left.Values[15] * right.Values[15]);
        }

        public static Matrix operator +(Matrix left, Matrix right)
        {
            var mat = new Matrix();
            for (var i = 0; i < 16; i++) mat[i] = left[i] + right[i];
            return mat;
        }

        public static Matrix operator -(Matrix left, Matrix right)
        {
            var mat = new Matrix();
            for (var i = 0; i < 16; i++) mat[i] = left[i] - right[i];
            return mat;
        }

        public static Matrix Rotation(Coordinate axis, decimal angle)
        {
            var cos = DMath.Cos(-angle);
            var sin = DMath.Sin(-angle);
            var t = 1m - cos;
            axis = axis.Normalise();

            return new Matrix(t * axis.X * axis.X + cos, t * axis.X * axis.Y - sin * axis.Z, t * axis.X * axis.Z + sin * axis.Y, 0,
                              t * axis.X * axis.Y + sin * axis.Z, t * axis.Y * axis.Y + cos, t * axis.Y * axis.Z - sin * axis.X, 0,
                              t * axis.X * axis.Z - sin * axis.Y, t * axis.Y * axis.Z + sin * axis.X, t * axis.Z * axis.Z + cos, 0,
                              0, 0, 0, 1);
        }

        public static Matrix Rotation(Quaternion quaternion)
        {
            var aa = quaternion.GetAxisAngle();
            return Rotation(aa.Item1, aa.Item2);
        }

        public static Matrix RotationX(decimal angle)
        {
            var cos = DMath.Cos(angle);
            var sin = DMath.Sin(angle);

            return new Matrix(1, 0, 0, 0,
                              0, cos, sin, 0,
                              0, -sin, cos, 0,
                              0, 0, 0, 1);
        }

        public static Matrix RotationY(decimal angle)
        {
            var cos = DMath.Cos(angle);
            var sin = DMath.Sin(angle);

            return new Matrix(cos, 0, -sin, 0,
                              0, 1, 0, 0,
                              sin, 0, cos, 0,
                              0, 0, 0, 1);
        }

        public static Matrix RotationZ(decimal angle)
        {
            var cos = DMath.Cos(angle);
            var sin = DMath.Sin(angle);

            return new Matrix(cos, sin, 0, 0,
                              -sin, cos, 0, 0,
                              0, 0, 1, 0,
                              0, 0, 0, 1);
        }

        public static Matrix Translation(Coordinate translation)
        {
            var m = Identity;
            m.Values[3] = translation.X;
            m.Values[7] = translation.Y;
            m.Values[11] = translation.Z;
            return m;
        }

        public static Matrix Scale(Coordinate scale)
        {
            var m = Identity;
            m.Values[0] = scale.X;
            m.Values[5] = scale.Y;
            m.Values[10] = scale.Z;
            return m;
        }

        public static Matrix FromOpenTKMatrix4(Matrix4 mat)
        {
            return new Matrix(
                (decimal)mat.Row0.X,
                (decimal)mat.Row0.Y,
                (decimal)mat.Row0.Z,
                (decimal)mat.Row3.X,

                (decimal)mat.Row1.X,
                (decimal)mat.Row1.Y,
                (decimal)mat.Row1.Z,
                (decimal)mat.Row3.Y,

                (decimal)mat.Row2.X,
                (decimal)mat.Row2.Y,
                (decimal)mat.Row2.Z,
                (decimal)mat.Row3.Z,

                (decimal)mat.Row0.W,
                (decimal)mat.Row1.W,
                (decimal)mat.Row2.W,
                (decimal)mat.Row3.W
                );
        }

        public static Matrix FromGLSLMatrix4(Matrix4 mat)
        {
            return new Matrix(
                (decimal)mat.Row0.X,
                (decimal)mat.Row0.Y,
                (decimal)mat.Row0.Z,
                (decimal)mat.Row0.W,

                (decimal)mat.Row1.X,
                (decimal)mat.Row1.Y,
                (decimal)mat.Row1.Z,
                (decimal)mat.Row1.W,

                (decimal)mat.Row2.X,
                (decimal)mat.Row2.Y,
                (decimal)mat.Row2.Z,
                (decimal)mat.Row2.W,

                (decimal)mat.Row3.X,
                (decimal)mat.Row3.Y,
                (decimal)mat.Row3.Z,
                (decimal)mat.Row3.W
                );
        }
    }
}
