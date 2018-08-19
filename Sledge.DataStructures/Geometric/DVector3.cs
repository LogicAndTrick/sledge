using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Runtime.Serialization;
using Sledge.Common;

namespace Sledge.DataStructures.Geometric
{
    [TypeConverter(typeof(DVector3TypeConverter))]
    [Serializable]
    public class DVector3 : ISerializable
    {
        public static readonly DVector3 MaxValue = new DVector3(Decimal.MaxValue, Decimal.MaxValue, Decimal.MaxValue);
        public static readonly DVector3 MinValue = new DVector3(Decimal.MinValue, Decimal.MinValue, Decimal.MinValue);
        public static readonly DVector3 Zero = new DVector3(0, 0, 0);
        public static readonly DVector3 One = new DVector3(1, 1, 1);
        public static readonly DVector3 UnitX = new DVector3(1, 0, 0);
        public static readonly DVector3 UnitY = new DVector3(0, 1, 0);
        public static readonly DVector3 UnitZ = new DVector3(0, 0, 1);

        #region X, Y, Z
        private decimal _z;
        private decimal _y;
        private decimal _x;

        private double _dx;
        private double _dy;
        private double _dz;

        public decimal X
        {
            get { return _x; }
            set
            {
                _x = value;
                _dx = (double)value;
            }
        }

        public decimal Y
        {
            get { return _y; }
            set
            {
                _y = value;
                _dy = (double)value;
            }
        }

        public decimal Z
        {
            get { return _z; }
            set
            {
                _z = value;
                _dz = (double)value;
            }
        }

        public double DX
        {
            get { return _dx; }
            set
            {
                _dx = value;
                _x = (decimal)value;
            }
        }

        public double DY
        {
            get { return _dy; }
            set
            {
                _dy = value;
                _y = (decimal)value;
            }
        }

        public double DZ
        {
            get { return _dz; }
            set
            {
                _dz = value;
                _z = (decimal)value;
            }
        }
        #endregion
        
        public DVector3(decimal x, decimal y, decimal z)
        {
            _x = x;
            _y = y;
            _z = z;
            _dx = (double) x;
            _dy = (double) y;
            _dz = (double) z;
        }

        protected DVector3(SerializationInfo info, StreamingContext context)
        {
            X = info.GetDecimal("X");
            Y = info.GetDecimal("Y");
            Z = info.GetDecimal("Z");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("Z", Z);
        }

        public bool EquivalentTo(DVector3 test, decimal delta = 0.0001m)
        {
            var xd = Math.Abs(_x - test._x);
            var yd = Math.Abs(_y - test._y);
            var zd = Math.Abs(_z - test._z);
            return (xd < delta) && (yd < delta) && (zd < delta);
        }

        public bool Equals(DVector3 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EquivalentTo(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof (DVector3) && Equals((DVector3) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = _x.GetHashCode();
                result = (result*397) ^ _y.GetHashCode();
                result = (result*397) ^ _z.GetHashCode();
                return result;
            }
        }

        public void Set(DVector3 value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
        }

        public decimal Dot(DVector3 c)
        {
            return ((_x * c._x) + (_y * c._y) + (_z * c._z));
        }

        public DVector3 Cross(DVector3 that)
        {
            var xv = (_y * that._z) - (_z * that._y);
            var yv = (_z * that._x) - (_x * that._z);
            var zv = (_x * that._y) - (_y * that._x);
            return new DVector3(xv, yv, zv);
        }

        public DVector3 Round(int num = 8)
        {
            return new DVector3(Math.Round(_x, num), Math.Round(_y, num), Math.Round(_z, num));
        }

        public DVector3 Snap(decimal snapTo)
        {
            return new DVector3(
                Math.Round(_x / snapTo) * snapTo,
                Math.Round(_y / snapTo) * snapTo,
                Math.Round(_z / snapTo) * snapTo
            );
        }

        public decimal VectorMagnitude()
        {
            return (decimal)Math.Sqrt(Math.Pow(_dx, 2) + Math.Pow(_dy, 2) + Math.Pow(_dz, 2));
        }

        public decimal LengthSquared()
        {
            return (decimal) (Math.Pow(_dx, 2) + Math.Pow(_dy, 2) + Math.Pow(_dz, 2));
        }

        public DVector3 Normalise()
        {
            var len = VectorMagnitude();
            return len == 0 ? new DVector3(0, 0, 0) : new DVector3(_x / len, _y / len, _z / len);
        }

        public DVector3 Absolute()
        {
            return new DVector3(Math.Abs(_x), Math.Abs(_y), Math.Abs(_z));
        }

        public static bool operator ==(DVector3 c1, DVector3 c2)
        {
            return Equals(c1, null) ? Equals(c2, null) : c1.Equals(c2);
        }

        public static bool operator !=(DVector3 c1, DVector3 c2)
        {
            return Equals(c1, null) ? !Equals(c2, null) : !c1.Equals(c2);
        }

        public static DVector3 operator +(DVector3 c1, DVector3 c2)
        {
            return new DVector3(c1._x + c2._x, c1._y + c2._y, c1._z + c2._z);
        }

        public static DVector3 operator -(DVector3 c1, DVector3 c2)
        {
            return new DVector3(c1._x - c2._x, c1._y - c2._y, c1._z - c2._z);
        }

        public static DVector3 operator -(DVector3 c1)
        {
            return new DVector3(-c1._x, -c1._y, -c1._z);
        }

        public static DVector3 operator /(DVector3 c, decimal f)
        {
            return f == 0 ? new DVector3(0, 0, 0) : new DVector3(c._x / f, c._y / f, c._z / f);
        }

        public static DVector3 operator *(DVector3 c, decimal f)
        {
            return new DVector3(c._x * f, c._y * f, c._z * f);
        }

        public static DVector3 operator *(decimal f, DVector3 c)
        {
            return c * f;
        }

        public DVector3 ComponentMultiply(DVector3 c)
        {
            return new DVector3(_x * c._x, _y * c._y, _z * c._z);
        }

        public DVector3 ComponentDivide(DVector3 c)
        {
            var x = c._x == 0 ? 1 : c._x;
            var y = c._y == 0 ? 1 : c._y;
            var z = c._z == 0 ? 1 : c._z;
            return new DVector3(_x / x, _y / y, _z / z);
        }

        /// <summary>
        /// Treats this vector as a directional unit vector and constructs a euler angle representation of that angle (in radians)
        /// </summary>
        /// <returns></returns>
        public DVector3 ToEulerAngles()
        {
            // http://www.gamedev.net/topic/399701-convert-vector-to-euler-cardan-angles/#entry3651854
            var yaw = DMath.Atan2(_y, _x);
            var pitch = DMath.Atan2(-_z, DMath.Sqrt(_x * _x + _y * _y));
            return new DVector3(0, pitch, yaw); // HL FGD has X = roll, Y = pitch, Z = yaw
        }

        public override string ToString()
        {
            return "(" + _x.ToString("0.0000", CultureInfo.InvariantCulture) + " " + _y.ToString("0.0000", CultureInfo.InvariantCulture) + " " + _z.ToString("0.0000", CultureInfo.InvariantCulture) + ")";
        }

        public DVector3 Clone()
        {
            return new DVector3(_x, _y, _z);
        }

        public static DVector3 Parse(string x, string y, string z)
        {
            const NumberStyles ns = NumberStyles.Float;
            return new DVector3(decimal.Parse(x, ns, CultureInfo.InvariantCulture), decimal.Parse(y, ns, CultureInfo.InvariantCulture), decimal.Parse(z, ns, CultureInfo.InvariantCulture));
        }

        public Vector3 ToVector3()
        {
            return new Vector3((float) _dx, (float) _dy, (float) _dz);
        }
    }
}