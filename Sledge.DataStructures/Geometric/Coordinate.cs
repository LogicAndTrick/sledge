using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using OpenTK;
using Sledge.Common;

namespace Sledge.DataStructures.Geometric
{
    [TypeConverter(typeof(CoordinateTypeConverter))]
    [Serializable]
    public class Coordinate : ISerializable
    {
        public static readonly Coordinate MaxValue = new Coordinate(Decimal.MaxValue, Decimal.MaxValue, Decimal.MaxValue);
        public static readonly Coordinate MinValue = new Coordinate(Decimal.MinValue, Decimal.MinValue, Decimal.MinValue);
        public static readonly Coordinate Zero = new Coordinate(0, 0, 0);
        public static readonly Coordinate One = new Coordinate(1, 1, 1);
        public static readonly Coordinate UnitX = new Coordinate(1, 0, 0);
        public static readonly Coordinate UnitY = new Coordinate(0, 1, 0);
        public static readonly Coordinate UnitZ = new Coordinate(0, 0, 1);

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
        
        public Coordinate(decimal x, decimal y, decimal z)
        {
            _x = x;
            _y = y;
            _z = z;
            _dx = (double) x;
            _dy = (double) y;
            _dz = (double) z;
        }

        protected Coordinate(SerializationInfo info, StreamingContext context)
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

        public bool EquivalentTo(Coordinate test, decimal delta = 0.0001m)
        {
            var xd = Math.Abs(_x - test._x);
            var yd = Math.Abs(_y - test._y);
            var zd = Math.Abs(_z - test._z);
            return (xd < delta) && (yd < delta) && (zd < delta);
        }

        public bool Equals(Coordinate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EquivalentTo(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof (Coordinate) && Equals((Coordinate) obj);
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

        public decimal Dot(Coordinate c)
        {
            return ((_x * c._x) + (_y * c._y) + (_z * c._z));
        }

        public Coordinate Cross(Coordinate that)
        {
            var xv = (_y * that._z) - (_z * that._y);
            var yv = (_z * that._x) - (_x * that._z);
            var zv = (_x * that._y) - (_y * that._x);
            return new Coordinate(xv, yv, zv);
        }

        public Coordinate Round(int num = 8)
        {
            return new Coordinate(Math.Round(_x, num), Math.Round(_y, num), Math.Round(_z, num));
        }

        public Coordinate Snap(decimal snapTo)
        {
            return new Coordinate(
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

        public Coordinate Normalise()
        {
            var len = VectorMagnitude();
            return len == 0 ? new Coordinate(0, 0, 0) : new Coordinate(_x / len, _y / len, _z / len);
        }

        public Coordinate Absolute()
        {
            return new Coordinate(Math.Abs(_x), Math.Abs(_y), Math.Abs(_z));
        }

        public static bool operator ==(Coordinate c1, Coordinate c2)
        {
            return Equals(c1, null) ? Equals(c2, null) : c1.Equals(c2);
        }

        public static bool operator !=(Coordinate c1, Coordinate c2)
        {
            return Equals(c1, null) ? !Equals(c2, null) : !c1.Equals(c2);
        }

        public static Coordinate operator +(Coordinate c1, Coordinate c2)
        {
            return new Coordinate(c1._x + c2._x, c1._y + c2._y, c1._z + c2._z);
        }

        public static Coordinate operator -(Coordinate c1, Coordinate c2)
        {
            return new Coordinate(c1._x - c2._x, c1._y - c2._y, c1._z - c2._z);
        }

        public static Coordinate operator -(Coordinate c1)
        {
            return new Coordinate(-c1._x, -c1._y, -c1._z);
        }

        public static Coordinate operator /(Coordinate c, decimal f)
        {
            return f == 0 ? new Coordinate(0, 0, 0) : new Coordinate(c._x / f, c._y / f, c._z / f);
        }

        public static Coordinate operator *(Coordinate c, decimal f)
        {
            return new Coordinate(c._x * f, c._y * f, c._z * f);
        }

        public static Coordinate operator *(decimal f, Coordinate c)
        {
            return c * f;
        }

        public Coordinate ComponentMultiply(Coordinate c)
        {
            return new Coordinate(_x * c._x, _y * c._y, _z * c._z);
        }

        public Coordinate ComponentDivide(Coordinate c)
        {
            var x = c._x == 0 ? 1 : c._x;
            var y = c._y == 0 ? 1 : c._y;
            var z = c._z == 0 ? 1 : c._z;
            return new Coordinate(_x / x, _y / y, _z / z);
        }

        /// <summary>
        /// Treats this vector as a directional unit vector and constructs a euler angle representation of that angle (in radians)
        /// </summary>
        /// <returns></returns>
        public Coordinate ToEulerAngles()
        {
            // http://www.gamedev.net/topic/399701-convert-vector-to-euler-cardan-angles/#entry3651854
            var yaw = DMath.Atan2(_y, _x);
            var pitch = DMath.Atan2(-_z, DMath.Sqrt(_x * _x + _y * _y));
            return new Coordinate(0, pitch, yaw); // HL FGD has X = roll, Y = pitch, Z = yaw
        }

        public override string ToString()
        {
            return "(" + _x.ToString("0.0000", CultureInfo.InvariantCulture) + " " + _y.ToString("0.0000", CultureInfo.InvariantCulture) + " " + _z.ToString("0.0000", CultureInfo.InvariantCulture) + ")";
        }

        public Coordinate Clone()
        {
            return new Coordinate(_x, _y, _z);
        }

        public static Coordinate Parse(string x, string y, string z)
        {
            const NumberStyles ns = NumberStyles.Float;
            return new Coordinate(decimal.Parse(x, ns, CultureInfo.InvariantCulture), decimal.Parse(y, ns, CultureInfo.InvariantCulture), decimal.Parse(z, ns, CultureInfo.InvariantCulture));
        }

        public CoordinateF ToCoordinateF()
        {
            return new CoordinateF((float) _dx, (float) _dy, (float) _dz);
        }
    }

    public class CoordinateTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            if (sourceType == typeof(CoordinateF)) return true;
            if (sourceType == typeof(Vector3)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string)) return true;
            if (destinationType == typeof(CoordinateF)) return true;
            if (destinationType == typeof(Vector3)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                var s = str.Split(' ');
                return new Coordinate(
                    decimal.Parse(s[0], NumberStyles.Float, culture),
                    decimal.Parse(s[1], NumberStyles.Float, culture),
                    decimal.Parse(s[2], NumberStyles.Float, culture));
            }
            if (value is CoordinateF cf)
            {
                return new Coordinate((decimal)cf.X, (decimal)cf.Y, (decimal)cf.Z);
            }
            if (value is Vector3 v)
            {
                return new Coordinate((decimal)v.X, (decimal)v.Y, (decimal)v.Z);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var v = value as Coordinate;
            if (v == null) return null;
            if (destinationType == typeof(string)) return String.Format(culture, "{0} {1} {2}", v.X, v.Y, v.Z);
            if (destinationType == typeof(CoordinateF)) return new CoordinateF((float) v.X, (float) v.Y, (float) v.Z);
            if (destinationType == typeof(Vector3)) return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
