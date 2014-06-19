using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.Geometric
{
    /// <summary>
    /// Represents a 4x4 MatrixF. Shamelessly taken in its entirety from OpenTK's MatrixF4 structure. http://www.opentk.com/
    /// </summary>
    [Serializable]
    public class MatrixF : ISerializable
    {
        public static MatrixF Zero
        {
            get { return new MatrixF(); }
        }

        public static MatrixF Identity
        {
            get
            {
                return new MatrixF(1, 0, 0, 0,
                                  0, 1, 0, 0,
                                  0, 0, 1, 0,
                                  0, 0, 0, 1);
            }
        }

        public float[] Values { get; private set; }

        public float this[int i]
        {
            get { return Values[i]; }
            set { Values[i] = value; }
        }

        public CoordinateF X { get { return new CoordinateF(Values[0], Values[1], Values[2]); } }
        public CoordinateF Y { get { return new CoordinateF(Values[4], Values[5], Values[6]); } }
        public CoordinateF Z { get { return new CoordinateF(Values[8], Values[9], Values[10]); } }
        public CoordinateF Shift { get { return new CoordinateF(Values[12], Values[13], Values[14]); } }

        public MatrixF()
        {
            Values = new float[]
                         {
                             0, 0, 0, 0,
                             0, 0, 0, 0,
                             0, 0, 0, 0,
                             0, 0, 0, 0
                         };
        }

        protected MatrixF(SerializationInfo info, StreamingContext context)
        {
            Values = (float[]) info.GetValue("Values", typeof (float[]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Values", Values);
        }

        public MatrixF(params float[] values)
        {
            if (values.Length != 16) throw new Exception("A MatrixF must be 16 values long.");
            Values = values;
        }

        public float Determinant()
        {
            return
                Values[0] * Values[5] * Values[10] * Values[15] - Values[0] * Values[5] * Values[11] * Values[14] + Values[0] * Values[6] * Values[11] * Values[13] - Values[0] * Values[6] * Values[9] * Values[15]
              + Values[0] * Values[7] * Values[9] * Values[14] - Values[0] * Values[7] * Values[10] * Values[13] - Values[1] * Values[6] * Values[11] * Values[12] + Values[1] * Values[6] * Values[8] * Values[15]
              - Values[1] * Values[7] * Values[8] * Values[14] + Values[1] * Values[7] * Values[10] * Values[12] - Values[1] * Values[4] * Values[10] * Values[15] + Values[1] * Values[4] * Values[11] * Values[14]
              + Values[2] * Values[7] * Values[8] * Values[13] - Values[2] * Values[7] * Values[9] * Values[12] + Values[2] * Values[4] * Values[9] * Values[15] - Values[2] * Values[4] * Values[11] * Values[13]
              + Values[2] * Values[5] * Values[11] * Values[12] - Values[2] * Values[5] * Values[8] * Values[15] - Values[3] * Values[4] * Values[9] * Values[14] + Values[3] * Values[4] * Values[10] * Values[13]
              - Values[3] * Values[5] * Values[10] * Values[12] + Values[3] * Values[5] * Values[8] * Values[14] - Values[3] * Values[6] * Values[8] * Values[13] + Values[3] * Values[6] * Values[9] * Values[12];
        }

        public MatrixF Inverse()
        {
            int[] colIdx = { 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1 };

            // convert the MatrixF to an array for easy looping
            var inverse = new[,]
                              {
                                  {Values[0], Values[1], Values[2], Values[3]},
                                  {Values[4], Values[5], Values[6], Values[7]},
                                  {Values[8], Values[9], Values[10], Values[11]},
                                  {Values[12], Values[13], Values[14], Values[15]}
                              };
            var icol = 0;
            var irow = 0;
            for (var i = 0; i < 4; i++)
            {
                // Find the largest pivot value
                var maxPivot = 0f;
                for (var j = 0; j < 4; j++)
                {
                    if (pivotIdx[j] != 0)
                    {
                        for (var k = 0; k < 4; ++k)
                        {
                            if (pivotIdx[k] == -1)
                            {
                                var absVal = Math.Abs(inverse[j, k]);
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
                // check for singular MatrixF
                if (Math.Abs(pivot - 0) < 0.0001)
                {
                    throw new InvalidOperationException("MatrixF is singular and cannot be inverted.");
                }

                // Scale row so it has a unit diagonal
                var oneOverPivot = 1f / pivot;
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

            return new MatrixF(
                inverse[0, 0], inverse[0, 1], inverse[0, 2], inverse[0, 3],
                inverse[1, 0], inverse[1, 1], inverse[1, 2], inverse[1, 3],
                inverse[2, 0], inverse[2, 1], inverse[2, 2], inverse[2, 3],
                inverse[3, 0], inverse[3, 1], inverse[3, 2], inverse[3, 3]);
        }

        public MatrixF Transpose()
        {
            return new MatrixF(Values[0], Values[4], Values[8], Values[12],
                              Values[1], Values[5], Values[9], Values[13],
                              Values[2], Values[6], Values[10], Values[14],
                              Values[3], Values[7], Values[11], Values[15]);
        }

        public MatrixF Translate(CoordinateF translation)
        {
            return new MatrixF(Values[0], Values[1], Values[2], Values[3],
                              Values[4], Values[5], Values[6], Values[7],
                              Values[8], Values[9], Values[10], Values[11],
                              Values[12] + translation.X, Values[13] + translation.Y, Values[14] + translation.Z, Values[15]);
        }

        public bool EquivalentTo(MatrixF other, float delta = 0.0001f)
        {
            for (var i = 0; i < 16; i++)
            {
                if (Math.Abs(Values[i] - other.Values[i]) >= delta) return false;
            }
            return true;
        }

        public bool Equals(MatrixF other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            for (var i = 0; i < 16; i++)
            {
                if (Math.Abs(Values[i] - other.Values[i]) > 0.0001) return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MatrixF)) return false;
            return Equals((MatrixF) obj);
        }

        public override int GetHashCode()
        {
            return (Values != null ? Values.GetHashCode() : 0);
        }

        public static bool operator ==(MatrixF left, MatrixF right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MatrixF left, MatrixF right)
        {
            return !Equals(left, right);
        }

        public static CoordinateF operator *(CoordinateF left, MatrixF right)
        {
            return new CoordinateF(
                right[12] + left.X * right[0] + left.Y * right[4] + left.Z * right[8],
                right[13] + left.X * right[1] + left.Y * right[5] + left.Z * right[9],
                right[14] + left.X * right[2] + left.Y * right[6] + left.Z * right[10]);
        }

        public static MatrixF operator *(MatrixF mat, float scalar)
        {
            return new MatrixF(mat.Values.Select(x => x * scalar).ToArray());
        }

        public static MatrixF operator *(float scalar, MatrixF mat)
        {
            return new MatrixF(mat.Values.Select(x => x * scalar).ToArray());
        }

        public static MatrixF operator * (MatrixF left, MatrixF right)
        {
            return new MatrixF(
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

        public static MatrixF operator +(MatrixF left, MatrixF right)
        {
            var mat = new MatrixF();
            for (var i = 0; i < 16; i++) mat[i] = left[i] + right[i];
            return mat;
        }

        public static MatrixF operator -(MatrixF left, MatrixF right)
        {
            var mat = new MatrixF();
            for (var i = 0; i < 16; i++) mat[i] = left[i] - right[i];
            return mat;
        }

        public static MatrixF Rotation(CoordinateF axis, float angle)
        {
            var cos = (float) Math.Cos(-angle);
            var sin = (float) Math.Sin(-angle);
            var t = 1f - cos;
            axis = axis.Normalise();

            return new MatrixF(t * axis.X * axis.X + cos, t * axis.X * axis.Y - sin * axis.Z, t * axis.X * axis.Z + sin * axis.Y, 0,
                              t * axis.X * axis.Y + sin * axis.Z, t * axis.Y * axis.Y + cos, t * axis.Y * axis.Z - sin * axis.X, 0,
                              t * axis.X * axis.Z - sin * axis.Y, t * axis.Y * axis.Z + sin * axis.X, t * axis.Z * axis.Z + cos, 0,
                              0, 0, 0, 1);
        }

        public static MatrixF Rotation(QuaternionF quaternion)
        {
            var aa = quaternion.GetAxisAngle();
            return Rotation(aa.Item1, aa.Item2);
        }

        public static MatrixF RotationX(float angle)
        {
            var cos = (float) Math.Cos(angle);
            var sin = (float) Math.Sin(angle);

            return new MatrixF(1, 0, 0, 0,
                              0, cos, sin, 0,
                              0, -sin, cos, 0,
                              0, 0, 0, 1);
        }

        public static MatrixF RotationY(float angle)
        {
            var cos = (float) Math.Cos(angle);
            var sin = (float) Math.Sin(angle);

            return new MatrixF(cos, 0, -sin, 0,
                              0, 1, 0, 0,
                              sin, 0, cos, 0,
                              0, 0, 0, 1);
        }

        public static MatrixF RotationZ(float angle)
        {
            var cos = (float) Math.Cos(angle);
            var sin = (float) Math.Sin(angle);

            return new MatrixF(cos, sin, 0, 0,
                              -sin, cos, 0, 0,
                              0, 0, 1, 0,
                              0, 0, 0, 1);
        }

        public static MatrixF Translation(CoordinateF translation)
        {
            var m = Identity;
            m.Values[12] = translation.X;
            m.Values[13] = translation.Y;
            m.Values[14] = translation.Z;
            return m;
        }

        public static MatrixF Scale(CoordinateF scale)
        {
            var m = Identity;
            m.Values[0] = scale.X;
            m.Values[5] = scale.Y;
            m.Values[10] = scale.Z;
            return m;
        }
    }
}
