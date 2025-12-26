using Lv.BIM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
    [Serializable]
    /// <summary>
    /// 定义为二维极坐标系坐标
    /// </summary>
    public class RS : Base,IPoint
    {
        private double m_r;

        //与BasisX的夹角
        private double m_a;


        public static RS Zero => new RS(0.0, 0.0);
        public static RS BasisX => new RS(1.0, 0.0);
        public static RS BasisY => new RS(1.0, Math.PI / 2);


        protected override void GenerateId()
        {
            this.id = this.GetType().Name + ":" + this.GetHashCode().ToString();
        }

        public double this[int idx] => idx switch
        {
            1 => m_a,
            0 => m_r,
            _ => throw new Exception("索引错误"),
        };


        public double Sita => m_a;
        public double Angle => m_a;
        public double R => m_r;

        public RS(double r, double sita)
        {

            m_r = r;
            //是否需要限制角度范围，仍在考虑20220403，lv
            //if(a<-Math.PI || a>Math.PI || b < -Math.PI || b > Math.PI)
            //{
            //    throw new Exception("范围出错");
            //}

            m_a = sita.RadiansTo0_2PI();
        }

        public RS()
        {
            m_r = 0.0;
            m_a = 0.0;
            //base..ctor();
        }
        public RS(double sita)
        {

            m_r = 1.0;
            //是否需要限制角度范围，仍在考虑20220403，lv
            //if(a<-Math.PI || a>Math.PI || b < -Math.PI || b > Math.PI)
            //{
            //    throw new Exception("范围出错");
            //}

            m_a = sita.RadiansTo0_2PI();
        }
        public double GetLength()
        {
            return Math.Abs(m_r);
        }

        public static RS operator +(RS left, RS right)
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

        public static RS operator -(RS source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            return source.Negate();
        }

        public static RS operator -(RS left, RS right)
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

        public static RS operator *(RS left, double value)
        {
            if (null == left)
            {
                throw new Exception("空值错误");
            }

            return left.Multiply(value);
        }

        public static RS operator *(double value, RS right)
        {
            if (null == right)
            {
                throw new Exception("空值错误");
            }

            return right.Multiply(value);
        }

        public static RS operator /(RS left, double value)
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
        public RS Normalize()
        {
            if (DotProduct(this) < 1E-18)
            {
                return new RS(0.0, 0.0);
            }
            return this / GetLength();
        }
        public double DotProduct(RS source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            UV xy = this.ConvertToUV();
            UV sourcexy = source.ConvertToUV();
            return xy.DotProduct(sourcexy);
        }
        public double CrossProduct(RS source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            UV xy = this.ConvertToUV();
            UV sourcexy = source.ConvertToUV();
            return xy.CrossProduct(sourcexy);
        }
        //public double TripleProduct(RS middle, RS right)
        //{
        //    if (null == middle)
        //    {
        //        throw new Exception("空值错误");
        //    }
        //    if (null == right)
        //    {
        //        throw new Exception("空值错误");
        //    }
        //    return CrossProduct(middle).DotProduct(right);
        //}
        public RS Add(RS source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            UV xy = this.ConvertToUV();
            UV sourcexy = source.ConvertToUV();
            return xy.Add(sourcexy).ConvertToRS();
        }
        public RS Subtract(RS source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            UV xy = this.ConvertToUV();
            UV sourcexy = source.ConvertToUV();
            return xy.Subtract(sourcexy).ConvertToRS();
        }
        public RS Negate()
        {
            double r = m_r;
            double a = m_a;
            return new RS(0 - r, a);
        }
        public RS Multiply(double value)
        {
            double r = m_r;
            double a = m_a;
            return new RS(r * value, a);
        }
        public RS Divide(double value)
        {
            if (Math.Abs(value) < 1E-09)
            {
                throw new Exception("小值太小");
            }
            double r = m_r;
            double a = m_a;
            return new RS(r / value, a);
        }
        public bool IsAlmostEqualTo(RS source, double tolerance)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            if (tolerance < 0.0)
            {
                throw new Exception("小值错误");
            }
            UV xy = this.ConvertToUV();
            UV sourcexy = source.ConvertToUV();
            return xy.IsAlmostEqualTo(sourcexy, tolerance);
        }
        public bool IsAlmostEqualTo(RS source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            return IsAlmostEqualTo(source, 1E-09);
        }

        public double DistanceTo(RS source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            return (this - source).GetLength();
        }

        public double AngleTo(RS source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            double x = DotProduct(source);
            double num = Math.Atan2(CrossProduct(source), x);
            if (num < 0.0)
            {
                return num + Math.PI * 2.0;
            }
            return num;
        }

        public double AngleOnPlaneTo(RS right, RS normal)
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
            double num4 = Math.Atan2(CrossProduct(right), num - num3 * num2);
            if (num4 < 0.0)
            {
                return num4 + Math.PI * 2.0;
            }
            return num4;
        }
        public RS RotateByDegree(double radians)
        {
            return new RS(m_r, m_a + radians);
        }
        //平行
        public bool IsParallel(RS source)
        {
            bool result = false;
            RS norm0 = this.Normalize();
            RS norm1 = source.Normalize();
            RS norm2 = -norm1;
            if (norm0.IsAlmostEqualTo(norm1) || norm0.IsAlmostEqualTo(norm2))
            {
                result = true;
            }
            return result;
        }
        //垂直
        public bool IsPerpendicular(RS source)
        {
            bool result = false;
            RS norm0 = this;
            RS norm1 = source;
            if (Math.Abs(norm0.DotProduct(norm1)) < 1E-09)
            {
                result = true;
            }
            return result;
        }
        public UV ConvertToUV()
        {
            double r = m_r;
            double a = m_a;
            double x = r * Math.Cos(a);
            double y = r * Math.Sin(a);
            return (new UV(x, y));
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0},{1})", new object[2]
            {
            this[0].ToString(format, provider),
            this[1].ToString(format, provider)
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
        public List<double> ToList() => new List<double> { m_r, m_a };

        public static RS FromList(IList<double> list) => new RS(list[0], list[1]);
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
            return false;
        }

        public bool IsRS()
        {
            return true;
        }
    }
}
