using System;

namespace LNLib.Mathematics
{
    /// <summary>
    /// 表示三维位置/向量/偏移
    /// </summary>
    public struct XYZ
    {
        private double[] _xyz;

        public XYZ()
        {
            _xyz = new double[3];
        }

        public XYZ(double x, double y, double z)
        {
            _xyz = new double[3] { x, y, z };
        }

        public double X
        {
            get => _xyz[0];
            set => _xyz[0] = value;
        }

        public double Y
        {
            get => _xyz[1];
            set => _xyz[1] = value;
        }

        public double Z
        {
            get => _xyz[2];
            set => _xyz[2] = value;
        }

        public bool IsZero(double epsilon = Constants.DoubleEpsilon)
        {
            return Math.Abs(X) < epsilon && Math.Abs(Y) < epsilon && Math.Abs(Z) < epsilon;
        }

        public bool IsUnit(double epsilon = Constants.DoubleEpsilon)
        {
            return Math.Abs(Length() - 1.0) < epsilon;
        }

        public bool IsAlmostEqualTo(XYZ another)
        {
            return Math.Abs(X - another.X) < Constants.DoubleEpsilon && 
                   Math.Abs(Y - another.Y) < Constants.DoubleEpsilon &&
                   Math.Abs(Z - another.Z) < Constants.DoubleEpsilon;
        }

        public double Length()
        {
            return Math.Sqrt(SqrLength());
        }

        public double SqrLength()
        {
            return X * X + Y * Y + Z * Z;
        }

        public double AngleTo(XYZ another)
        {
            double dot = DotProduct(another);
            double len1 = Length();
            double len2 = another.Length();
            
            if (len1 < Constants.DoubleEpsilon || len2 < Constants.DoubleEpsilon)
                return 0.0;

            double cos = dot / (len1 * len2);
            cos = Math.Max(-1.0, Math.Min(1.0, cos));
            return Math.Acos(cos);
        }

        public XYZ Normalize()
        {
            double len = Length();
            if (len < Constants.DoubleEpsilon)
                return new XYZ();
            return new XYZ(X / len, Y / len, Z / len);
        }

        public XYZ Add(XYZ another)
        {
            return new XYZ(X + another.X, Y + another.Y, Z + another.Z);
        }

        public XYZ Subtract(XYZ another)
        {
            return new XYZ(X - another.X, Y - another.Y, Z - another.Z);
        }
        
        public XYZ Multiply(double scalar)
        {
            return new XYZ(X * scalar, Y * scalar, Z * scalar);
        }

        public XYZ Negate()
        {
            return new XYZ(-X, -Y, -Z);
        }

        /// <summary>
        /// Negative是Negate的别名
        /// </summary>
        /// <returns>取反后的向量</returns>
        public XYZ Negative()
        {
            return Negate();
        }

        public double DotProduct(XYZ another)
        {
            return X * another.X + Y * another.Y + Z * another.Z;
        }

        public XYZ CrossProduct(XYZ another)
        {
            return new XYZ(
                Y * another.Z - Z * another.Y,
                Z * another.X - X * another.Z,
                X * another.Y - Y * another.X);
        }

        public double Distance(XYZ another)
        {
            return Subtract(another).Length();
        }

        /// <summary>
        /// 计算与另一点的平方距离
        /// </summary>
        /// <param name="another">另一个点</param>
        /// <returns>平方距离</returns>
        public double DistanceSquared(XYZ another)
        {
            return Subtract(another).SqrLength();
        }

        public static XYZ CreateRandomOrthogonal(XYZ xyz)
        {
            if (xyz.IsZero())
                return new XYZ();

            XYZ randomVector;
            if (Math.Abs(xyz.Z) < Constants.DoubleEpsilon)
                randomVector = new XYZ(0, 0, 1);
            else
                randomVector = new XYZ(1, 0, 0);

            return xyz.CrossProduct(randomVector).Normalize();
        }

        public double this[int index]
        {
            get
            {
                if (index < 0 || index > 2)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _xyz[index];
            }
            set
            {
                if (index < 0 || index > 2)
                    throw new ArgumentOutOfRangeException(nameof(index));
                _xyz[index] = value;
            }
        }

        public static XYZ operator +(XYZ a, XYZ b) => a.Add(b);
        public static XYZ operator -(XYZ a, XYZ b) => a.Subtract(b);
        public static XYZ operator -(XYZ a) => a.Negate();
        public static XYZ operator *(XYZ a, double d) => new XYZ(a.X * d, a.Y * d, a.Z * d);
        public static XYZ operator *(double d, XYZ a) => a * d;
        public static XYZ operator /(XYZ a, double d) => new XYZ(a.X / d, a.Y / d, a.Z / d);
        public static double operator *(XYZ a, XYZ b) => a.DotProduct(b);
        public static XYZ operator ^(XYZ a, XYZ b) => a.CrossProduct(b);
    }
} 