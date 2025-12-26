using Lv.BIM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lv.BIM.Geometry
{
    [Serializable]
    public class RAB:Base,IPoint
    {
        private double m_r;

        //与BasisX的夹角
        private double m_a;

        //与BasisZ的夹角
        private double m_b;

        public static RAB Zero => new RAB(0.0, 0.0, 0.0);
        public static RAB BasisX => new RAB(1.0, 0.0, 0.0);
        public static RAB BasisY => new RAB(1.0, Math.PI/2, Math.PI / 2);
        public static RAB BasisZ => new RAB(1.0, 0.0, 0.0);

        protected override void GenerateId()
        {
            this.id = this.GetType().Name + ":" + this.GetHashCode().ToString();
        }

        public double this[int idx] => idx switch
        {
            2 => m_b,
            1 => m_a,
            0 => m_r,
            _ => throw new Exception("索引错误"),
        };

        public double B => m_b;

        public double A => m_a;

        public double R => m_r;

        public RAB(double r, double a, double b)
        {

            m_r = r;
            //是否需要限制角度范围，仍在考虑20220403，lv
            //if(a<-Math.PI || a>Math.PI || b < -Math.PI || b > Math.PI)
            //{
            //    throw new Exception("范围出错");
            //}
            
            m_a = a.RadiansTo0_2PI();
            m_b = b.RadiansTo0_2PI();
        }

        public RAB()
        {
            m_r = 0.0;
            m_a = 0.0;
            m_b = 0.0;
            //base..ctor();
        }

        public double GetLength()
        {
            return Math.Abs(m_r);
        }

        public static RAB operator +(RAB left, RAB right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            return left.Add(right);
        }

        public static RAB operator -(RAB source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            return source.Negate();
        }

        public static RAB operator -(RAB left, RAB right)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            return left.Subtract(right);
        }

        public static RAB operator *(RAB left, double value)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }

            return left.Multiply(value);
        }

        public static RAB operator *(double value, RAB right)
        {
            if (null == right)
            {
                throw new Exception("空值错误");
            }

            return right.Multiply(value);
        }

        public static RAB operator /(RAB left, double value)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }

            if (Math.Abs(value) < 1E-09)
            {
                throw new Exception("小值太小");
            }
            return left.Divide(value);
        }

        public bool IsZeroLength()
        {
            double r = m_r;
            int num;
            if (Math.Abs(r) < 1E-09)
            {
                num = 1;
                goto IL_004e;
            }
            else
            {
                num = 0;
                goto IL_004e;
            }

            IL_004e:
            return (byte)num != 0;
        }
        public bool IsUnitLength()
        {
            return (byte)((Math.Abs(GetLength() - 1.0) < 1E-09) ? 1u : 0u) != 0;
        }
        public RAB Normalize()
        {
            if (DotProduct(this) < 1E-18)
            {
                return new RAB(0.0, 0.0, 0.0);
            }
            return this / GetLength();
        }
        public double DotProduct(RAB source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            XYZ xyz = this.ConvertToXYZ();
            XYZ sourcexyz = source.ConvertToXYZ();
            return xyz.DotProduct(sourcexyz);
        }
        public RAB CrossProduct(RAB source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            XYZ xyz = this.ConvertToXYZ();
            XYZ sourcexyz = source.ConvertToXYZ();
            return xyz.CrossProduct(sourcexyz).ToRAB();
        }
        public double TripleProduct(RAB middle, RAB right)
        {
            if (null == middle)
            {
                throw new Exception("空值错误");
            }
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            return CrossProduct(middle).DotProduct(right);
        }
        public RAB Add(RAB source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            XYZ xyz = this.ConvertToXYZ();
            XYZ sourcexyz = source.ConvertToXYZ();
            return xyz.Add(sourcexyz).ToRAB();
        }
        public RAB Subtract(RAB source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            XYZ xyz = this.ConvertToXYZ();
            XYZ sourcexyz = source.ConvertToXYZ();
            return xyz.Subtract(sourcexyz).ToRAB();
        }
        public RAB Negate()
        {
            double r = m_r;
            double b = m_b;
            double a = m_a;
            return new RAB(0-r, a, b);
        }
        public RAB Multiply(double value)
        {
            double r = m_r;
            double b = m_b;
            double a = m_a;
            return new RAB(r* value, a, b);
        }
        public RAB Divide(double value)
        {
            if (Math.Abs(value) < 1E-09)
            {
                throw new Exception("小值太小");
            }
            double r = m_r;
            double b = m_b;
            double a = m_a;
            return new RAB(r / value, a, b);
        }
        public bool IsAlmostEqualTo(RAB source, double tolerance)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            if (tolerance < 0.0)
            {
                throw new Exception("小值错误");
            }
            XYZ xyz = this.ConvertToXYZ();
            XYZ sourcexyz = source.ConvertToXYZ();
            return xyz.IsAlmostEqualTo(sourcexyz, tolerance);
        }
        public bool IsAlmostEqualTo(RAB source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            return IsAlmostEqualTo(source, 1E-09);
        }

        public double DistanceTo(RAB source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            return (this - source).GetLength();
        }

        public double AngleTo(RAB source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            double x = DotProduct(source);
            double num = Math.Atan2(CrossProduct(source).GetLength(), x);
            if (num < 0.0)
            {
                return num + Math.PI * 2.0;
            }
            return num;
        }

        public double AngleOnPlaneTo(RAB right, RAB normal)
        {
            if (null == right)
            {
                throw new Exception("空值错误");
            }
            if (null == normal)
            {
                throw new Exception("空值错误");
            }
            if ((byte)((Math.Abs(normal.GetLength() - 1.0) < 1E-09) ? 1 : 0) == 0)
            {
                throw new Exception("小值错误");
            }
            double num = DotProduct(right);
            double num2 = DotProduct(normal);
            double num3 = right.DotProduct(normal);
            double num4 = Math.Atan2(CrossProduct(right).DotProduct(normal), num - num3 * num2);
            if (num4 < 0.0)
            {
                return num4 + Math.PI * 2.0;
            }
            return num4;
        }
        //平行
        public bool IsParallel(RAB source)
        {
            bool result = false;
            RAB norm0 = this.Normalize();
            RAB norm1 = source.Normalize();
            RAB norm2 = -norm1;
            if (norm0.IsAlmostEqualTo(norm1) || norm0.IsAlmostEqualTo(norm2))
            {
                result = true;
            }
            return result;
        }
        //垂直
        public bool IsPerpendicular(RAB source)
        {
            bool result = false;
            RAB norm0 = this;
            RAB norm1 = source;
            if (Math.Abs(norm0.DotProduct(norm1)) < 1E-09)
            {
                result = true;
            }
            return result;
        }
        public XYZ ConvertToXYZ()
        {
            double r = m_r;
            double a = m_a;
            double b = m_b;
            double x = r * Math.Sin(b) * Math.Cos(a);
            double y = r * Math.Sin(b) * Math.Sin(a);
            double z = r * Math.Cos(b);
            return (new XYZ(x, y, z));
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0},{1},{2})", new object[3]
            {
            this[0].ToString(format, provider),
            this[1].ToString(format, provider),
            this[2].ToString(format, provider)
            });
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public sealed override string ToString()
        {
            return ToString(null, null);
        }
        public bool IsXYZ()
        {
            return false;
        }

        public bool IsUV()
        {
            return false;
        }

        public bool IsRAB()
        {
            return true;
        }

        public bool IsRS()
        {
            return false;
        }
    }

    
}
