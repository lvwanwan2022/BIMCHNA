using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
	[Serializable]
	/// <summary>Object representing coordinates in 3-dimensional space.</summary>
	/// <remarks>Usually this means a XYZ or a vector in 3-dimensional space, depending on the actual use.</remarks>
	public class XYZ:Base,IPoint,ITransformable<XYZ>,ITransformable
	{
		private double m_x;

		private double m_y; 

		private double m_z;
		//private string s_units;
		protected override void GenerateId()
		{
			this.id = this.GetType().Name + ":" + this.GetHashCode().ToString();
		}
		public static XYZ BasisZ => new XYZ(0.0, 0.0, 1.0);

		public static XYZ BasisY => new XYZ(0.0, 1.0, 0.0);

		public static XYZ BasisX => new XYZ(1.0, 0.0, 0.0);

		public static XYZ Zero => new XYZ(0.0, 0.0, 0.0);
		//public string Units => s_units;
		public double Length => GetLength();
		public double this[int idx] => idx switch
		{
			2 => m_z,
            1 => m_y,
			0 => m_x,
			_ => throw new Exception("索引错误"),
		};

		public double Z => m_z;

		public double Y => m_y;

		public double X => m_x;
		public XYZ XY=>new XYZ(m_x, m_y, 0);
		public UV UV => new UV(m_x, m_y);

		public XYZ(double x, double y, double z)
		{
			
			m_x = x;
			m_y = y;
			m_z = z;
			//s_units = UnitsType.Millimeters;
		}
		//创建带权限的点
		public XYZ(double x, double y, double z,string username,string password="123456")
		{

			m_x = x;
			m_y = y;
			m_z = z;
			SetUserInfo(username, password);
			//s_units = UnitsType.Millimeters;
		}
		public XYZ()
		{
			m_x = 0.0;
			m_y = 0.0;
			m_z = 0.0;
			//s_units = UnitsType.Millimeters;
			//base..ctor();
		}

		public void ChangeX(double x,string username="public",string password="public")
        {
            if (IsAuthorized(username, password))
            {
				m_x = x;
            }
			else
			{
				throw new Exception("权限错误");
			}

		}
		public void ChangeY(double y, string username = "public", string password = "public")
		{
			if (IsAuthorized(username, password))
			{
				m_y = y;
			}
			else
			{
				throw new Exception("权限错误");
			}

		}
		public void ChangeZ(double z, string username = "public", string password = "public")
		{
			if (IsAuthorized(username, password))
			{
				m_z = z;
			}
			else
            {
				throw new Exception("权限错误");
			}
		
		}
		public bool IsZeroLength()
		{
			double x = m_x;
			int num;
			if (Math.Abs(x) < 1E-09)
			{
				double y = m_y;
				if (Math.Abs(y) < 1E-09)
				{
					double z = m_z;
					if (Math.Abs(z) < 1E-09)
					{
						num = 1;
						goto IL_004e;
					}
				}
			}
			num = 0;
			goto IL_004e;
		IL_004e:
			return (byte)num != 0;
		}

		public bool IsUnitLength()
		{
			return (byte)((Math.Abs(GetLength() - 1.0) < 1E-09) ? 1u : 0u) != 0;
		}

		//public unsafe static bool IsWithinLengthLimits(XYZ XYZ)
		//{
		////暂未实现
		//	return true;
		//}

		public XYZ Normalize()
		{
			if (DotProduct(this) < 1E-18)
			{
				return new XYZ(0.0, 0.0, 0.0);
			}
			return this / GetLength();
		}
		public XYZ Unitize()
		{
			if (DotProduct(this) < 1E-18)
			{
				return new XYZ(0.0, 0.0, 0.0);
			}
			return this / GetLength();
		}
		public double GetLength()
		{
			double x = m_x;
			double num = x;
			double num2 = x;
			double y = m_y;
			double num3 = y;
			double num4 = y;
			double z = m_z;
			double num5 = z;
			double num6 = z;
			return Math.Sqrt(num4 * num3 + num2 * num + num6 * num5);
		}

		public static XYZ operator +(XYZ left, XYZ right)
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

		public static XYZ operator -(XYZ source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			return source.Negate();
		}

		public static XYZ operator -(XYZ left, XYZ right)
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
		public static double operator *(XYZ left, XYZ right)
		{
			return left.DotProduct(right);
		}
		public static XYZ operator *(XYZ left, double value)
		{
			if (null == left)
			{
				throw new Exception("空值错误");
			}
			
			return left.Multiply(value);
		}

		public static XYZ operator *(double value, XYZ right)
		{
			if (null == right)
			{
				throw new Exception("空值错误");
			}
	
			return right.Multiply(value);
		}

		public static XYZ operator /(XYZ left, double value)
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

		public double DotProduct(XYZ source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			return source.m_y * m_y + source.m_x * m_x + source.m_z * m_z;
		}

		public XYZ CrossProduct(XYZ source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			double x = m_x;
			double num = x;
			double y = source.m_y;
			double num2 = y;
			double y2 = m_y;
			double num3 = y2;
			double x2 = source.m_x;
			double num4 = x2;
			double z = m_z;
			double num5 = z;
			double num6 = x2;
			double num7 = x;
			double z2 = source.m_z;
			double num8 = z2;
			double num9 = y2;
			double num10 = z2;
			double num11 = z;
			double num12 = y;
			return new XYZ(num10 * num9 - num12 * num11, num6 * num5 - num8 * num7, num2 * num - num4 * num3);
		}

		public double TripleProduct(XYZ middle, XYZ right)
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

		public XYZ Add(XYZ source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			double z = m_z;
			double z2 = source.m_z;
			double y = m_y;
			double y2 = source.m_y;
			double x = m_x;
			double x2 = source.m_x;
			return new XYZ(x2 + x, y2 + y, z2 + z);
		}

		public XYZ Subtract(XYZ source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			double z = m_z;
			double z2 = source.m_z;
			double y = m_y;
			double y2 = source.m_y;
			double x = m_x;
			double x2 = source.m_x;
			return new XYZ(x - x2, y - y2, z - z2);
		}

		public XYZ Negate()
		{
			double z = m_z;
			double y = m_y;
			double x = m_x;
			return new XYZ(0.0 - x, 0.0 - y, 0.0 - z);
		}

		public XYZ Multiply(double value)
		{
			
			double z = m_z;
			double y = m_y;
			double x = m_x;
			return new XYZ(x * value, y * value, z * value);
		}

		public XYZ Divide(double value)
		{
			
			if (Math.Abs(value) < 1E-09)
			{
				throw new Exception("小值太小");
			}
			double z = m_z;
			double y = m_y;
			double x = m_x;
			return new XYZ(x / value, y / value, z / value);
		}
		public XYZ Translation(XYZ vector)
		{
			double x = m_x + vector.X;
			double y = m_y + vector.Y;
			double z = m_z + vector.Z;
			return new XYZ(x, y, z);
		}
		//以下根据
		//罗德里格旋转公式(Rodrigues’ Rotation Formula)：		v ′ = v c o s θ + u × v s i n θ + (u ⋅ v ) u( 1 − c o s θ) v' = v cos \theta + u \times v sin \theta + (u \cdot v) u ( 1 - cos \theta)
		//v ′ =vcosθ+u×vsinθ+(u⋅v)u(1−cosθ)
		/// <summary>
		/// 逆时针旋转弧度后坐标
		/// </summary>
		/// <param name="axis"></param>
		/// <param name="degreeRadian"></param>
		/// <returns></returns>
		public XYZ RotateByAxisAndDegree(XYZ axis,double degreeRadian)
        {
			XYZ result = new XYZ();
			//20220418lvwan取消角度限制
			//if (degreeRadian <= Math.PI*2 && degreeRadian >= 0)
			//{
			XYZ v = this;
				XYZ u = axis.Normalize();
				double θ = degreeRadian;
				result = v * Math.Cos(θ) + u.CrossProduct(v) * Math.Sin(θ) + (u.DotProduct(v)) * u * (1 - Math.Cos(θ));
			//}
			//else
			//{
			//	throw new Exception("角度范围不在π~2π内");
			//}
			return result;
		}
		public bool IsAlmostEqualTo(XYZ source, double tolerance)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			if (tolerance < 0.0)
			{
				throw new Exception("小值错误");
			}
			double num = tolerance * tolerance;
			double num2 = source.DotProduct(source) + DotProduct(this);
			if (num2 < num)
			{
				return true;
			}
			XYZ XYZ = this - source;
			return (byte)((XYZ.DotProduct(XYZ) < num2 * num) ? 1u : 0u) != 0;
		}

		
		public bool IsAlmostEqualTo(XYZ source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			return IsAlmostEqualTo(source, 1E-09);
		}

		public double DistanceTo(XYZ source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			return (this - source).GetLength();
		}
		/// <summary>
		/// ☆☆此处请非常注意，角度为this顺时针转直source的角度
		/// </summary>
		/// <param name="right"></param>
		/// <param name="normal"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public double AngleTo(XYZ source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			double x = DotProduct(source);
			double num = Math.Atan2(CrossProduct(source).GetLength(), x);
            //角度范围
            if (num < 0.0)
            {
                return num + Math.PI * 2.0;
            }
            return num;
		}
		/// <summary>
		/// ☆☆此处请非常注意，角度为this顺时针转直right的角度
		/// </summary>
		/// <param name="right"></param>
		/// <param name="normal"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public double AngleOnPlaneTo(XYZ right, XYZ normal)
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
            //角度范围

            if (num4 < 0.0)
            {
                return num4 + Math.PI * 2.0;
            }
            return num4;
		}

		/// <summary>
		/// 判断是否平行，包含方向相反的平行,且(0,0,0)平行与任何向量
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public bool IsParallelTo(XYZ source)
        {
			bool result = false;
			XYZ norm0 = this.Normalize();
			XYZ norm1 = source.Normalize();
			XYZ norm2 = -norm1;
			if(norm0.IsAlmostEqualTo(XYZ.Zero) || norm1.IsAlmostEqualTo(XYZ.Zero))
            {
				return true;
            }
			if (norm0.IsAlmostEqualTo(norm1) || norm0.IsAlmostEqualTo(norm2))
            {
				result = true;
            }
			return result;
        }
		//垂直
		public bool IsPerpendicularTo(XYZ source)
		{
			bool result = false;
			XYZ norm0 = this;
			XYZ norm1 = source;
			if (Math.Abs(norm0.DotProduct(norm1))< 1E-09)
			{
				result = true;
			}
			return result;
		}
		/// <summary>
		/// 随机生成一条垂线
		/// </summary>
		/// <returns></returns>
		public XYZ PerpendicularVector()
		{
			double sx = X;
			double sy = Y;
			double sz = Z;
			double fx =Math.Abs(sx)<1E-9?0.5: 1/ sx;
			double fy = Math.Abs(sy) < 1E-9 ? -0.5 : -1 / sy;
			double fz = Math.Abs(sx*fx+sy*fy) < 1E-9 ? 0 :- sx * fx - sy * fy;
			return new XYZ(fx, fy, fz);
			
		}
		public UV ToUV(string which = "XY")
        {
			string shibie = which.Replace('1', 'X').Replace('2', 'Y').Replace('3', 'Z');
			shibie = shibie.ToUpper();
            switch (shibie)
            {
				case "XY":				
					return XYToUV();
				case "YZ":
					return YZToUV();
				case "XZ":
					return XZToUV();
				case "YX":
					return new UV(Y, X);
				case "ZY":
					return new UV(Z, Y);
				case "ZX":
					return new UV(Z, X);
				default:
					return XYToUV();
            }
        }
		public UV XYToUV() 
		{
			return new UV(X, Y);
		}
		public UV XZToUV()
		{
			return new UV(X, Z);
		}
		public UV YZToUV()
		{
			return new UV(Y, Z);
		}
		public RAB ToRAB()
		{
			double x = m_x;
			double y = m_y;
			double z = m_z;
			double xy =  Math.Sqrt(x * x + y * y) ;
			double xyz = Math.Sqrt(xy * xy + z * z);
			double r = xyz;
            //以下代码可以用atan2(y,x)代替
			//double a = (xy!=0)?Math.Asin(y / xy):0;
			//if (x< 0)
			//{
			//	if (a>0)
			//	{
			//		a = a + Math.PI / 2;
			//	}
			//	else if (a < 0)
			//	{
			//		a = a - Math.PI / 2;
			//	}
   //             else
   //             {
			//		a = Math.PI;
   //             }
				
			//}
		//atan2(y,x)返回y/x的反正切，角度范围为（-PI~PI）
		double a = (x==0 && y==0)?0:Math.Atan2(y, x);
		//acos范围为[0,PI]
		double b = (xyz != 0) ? Math.Acos(z / xyz):0;

			return (new RAB(r,a, b));
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
		public double[] ToArray()
		{
			return new double[3] { m_x, m_y, m_z };
		}
		public void Deconstruct(out double x, out double y, out double z)
		{
			x = this.m_x;
			y = this.m_y;
			z = this.m_z;
		}

		public bool TransformTo(TransformXYZ transform, out XYZ point)
		{
			point = transform.ApplyToPoint(this);
			return true;
		}
		public bool TransformTo(TransformXYZ transform, out ITransformable point)
		{
			point = transform.ApplyToPoint(this);
			return true;
		}
		public List<double> ToList() => new List<double> { m_x, m_y, m_z };

		public static XYZ FromList(IList<double> list) => new XYZ(list[0], list[1], list[2]);

        public bool IsXYZ()
        {
            return true;
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
			return false;
		}

        
    }
}


