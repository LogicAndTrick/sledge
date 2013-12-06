using System;
using System.Globalization;

namespace Sledge.DataStructures.Geometric
{
    public class Coordinate
    {
        public readonly static Coordinate MaxValue = new Coordinate(Decimal.MaxValue, Decimal.MaxValue, Decimal.MaxValue);
        public readonly static Coordinate MinValue = new Coordinate(Decimal.MinValue, Decimal.MinValue, Decimal.MinValue);
        public readonly static Coordinate Zero = new Coordinate(0, 0, 0);
        public readonly static Coordinate One = new Coordinate(1, 1, 1);
        public readonly static Coordinate UnitX = new Coordinate(1, 0, 0);
        public readonly static Coordinate UnitY = new Coordinate(0, 1, 0);
        public readonly static Coordinate UnitZ = new Coordinate(0, 0, 1);

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
                _dx = (double) value;
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
            X = x;
            Y = y;
            Z = z;
        }

        public bool EquivalentTo(Coordinate test, decimal delta = 0.0001m)
        {
            var xd = Math.Abs(X - test.X);
            var yd = Math.Abs(Y - test.Y);
            var zd = Math.Abs(Z - test.Z);
            return (xd < delta) && (yd < delta) && (zd < delta);
        }

        public bool Equals(Coordinate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.X == X && other.Y == Y && other.Z == Z;
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
                var result = X.GetHashCode();
                result = (result*397) ^ Y.GetHashCode();
                result = (result*397) ^ Z.GetHashCode();
                return result;
            }
        }

        public decimal Dot(Coordinate c)
        {
            return ((X * c.X) + (Y * c.Y) + (Z * c.Z));
        }

        public Coordinate Cross(Coordinate that)
        {
            var xv = (Y * that.Z) - (Z * that.Y);
            var yv = (Z * that.X) - (X * that.Z);
            var zv = (X * that.Y) - (Y * that.X);
            return new Coordinate(xv, yv, zv);
        }

        public Coordinate Round(int num = 4)
        {
            return new Coordinate(Math.Round(X, num), Math.Round(Y, num), Math.Round(Z, num));
        }

        public Coordinate Snap(decimal snapTo)
        {
            return new Coordinate(
                Math.Round(X / snapTo) * snapTo,
                Math.Round(Y / snapTo) * snapTo,
                Math.Round(Z / snapTo) * snapTo
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
            return len == 0 ? new Coordinate(0, 0, 0) : new Coordinate(X / len, Y / len, Z / len);
        }

        public Coordinate Absolute()
        {
            return new Coordinate(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
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
            return new Coordinate(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z);
        }

        public static Coordinate operator -(Coordinate c1, Coordinate c2)
        {
            return new Coordinate(c1.X - c2.X, c1.Y - c2.Y, c1.Z - c2.Z);
        }

        public static Coordinate operator -(Coordinate c1)
        {
            return new Coordinate(-c1.X, -c1.Y, -c1.Z);
        }

        public static Coordinate operator /(Coordinate c, decimal f)
        {
            return f == 0 ? new Coordinate(0, 0, 0) : new Coordinate(c.X / f, c.Y / f, c.Z / f);
        }

        public static Coordinate operator *(Coordinate c, decimal f)
        {
            return new Coordinate(c.X * f, c.Y * f, c.Z * f);
        }

        public static Coordinate operator *(decimal f, Coordinate c)
        {
            return c * f;
        }

        public Coordinate ComponentMultiply(Coordinate c)
        {
            return new Coordinate(X * c.X, Y * c.Y, Z * c.Z);
        }

        public Coordinate ComponentDivide(Coordinate c)
        {
            var x = c.X == 0 ? 1 : c.X;
            var y = c.Y == 0 ? 1 : c.Y;
            var z = c.Z == 0 ? 1 : c.Z;
            return new Coordinate(X / x, Y / y, Z / z);
        }

        public override string ToString()
        {
            return "(" + X.ToString("0.0000") + " " + Y.ToString("0.0000") + " " + Z.ToString("0.0000") + ")";
        }

        public Coordinate Clone()
        {
            return new Coordinate(X, Y, Z);
        }

        public static Coordinate Parse(string x, string y, string z)
        {
            const NumberStyles ns = NumberStyles.Float;
            return new Coordinate(decimal.Parse(x, ns, CultureInfo.InvariantCulture), decimal.Parse(y, ns, CultureInfo.InvariantCulture), decimal.Parse(z, ns, CultureInfo.InvariantCulture));
        }

        public CoordinateF ToCoordinateF()
        {
            return new CoordinateF((float) DX, (float) DY, (float) DZ);
        }
    }
}
