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
	/// 二维平面坐标系坐标点或向量
	/// </summary>
	public class UV:Base,IPoint,ITransformable<UV>,ITransformable,IParameterizable
    {
		private double m_u;

		private double m_v;

		//private PlaneXYZ base_plane;

		public static UV BasisV => new UV(0.0, 1.0);

		public static UV BasisU => new UV(1.0, 0.0);

		public static UV Zero => new UV(0.0, 0.0);
		public double Angle => Math.Atan2(m_v,m_u)<0 ? Math.Atan2(m_v, m_u)+Math.PI*2: Math.Atan2(m_v, m_u);
		public double Length => GetLength();
		protected override void GenerateId()
		{
			this.id = this.GetType().Name + ":" + this.GetHashCode().ToString();
		}
		public double this[int idx] => idx switch
		{
			1 => m_v,
			0 => m_u,
			_ => throw new Exception("索引错误"),
		};

		public double V => m_v;

		public double U => m_u;
		public double Y => m_v;

		public double X => m_u;
		public XYZ XYZ => new XYZ(m_u, m_v, 0);

		public UV(double u, double v)
		{
			m_u = u;
			m_v = v;
			
		}
		/// <summary>
		/// 由角度构造UV向量
		/// </summary>
		/// <param name="sita"></param>
		public UV(double sita)
		{
			m_u = Math.Cos(sita);
			m_v = Math.Sin(sita);
		}
		
		public UV()
		{
			m_u = 0.0;
			m_v = 0.0;
			//base_plane = new PlaneXYZ();
			//base..ctor();
		}

		//public unsafe UV(UV* nativeUV)
		//{
		//	m_u = *(double*)nativeUV;
		//	m_v = *(double*)((ulong)(nint)nativeUV + 8uL);
		//}

		public UV Normalize()
		{
			if (DotProduct(this) < 1E-09)
			{
				return new UV(0.0, 0.0);
			}
			return this / GetLength();
		}

		public double GetLength()
		{
			double u = m_u;
			double num = u;
			double num2 = u;
			double v = m_v;
			double num3 = v;
			double num4 = v;
			return Math.Sqrt(num4 * num3 + num2 * num);
		}

	 
		public bool IsZeroLength()
		{
			double u = m_u;
			int num;
			if (Math.Abs(u) < 1E-09)
			{
				double v = m_v;
				if (Math.Abs(v) < 1E-09)
				{
					num = 1;
					goto IL_0036;
				}
			}
			num = 0;
			goto IL_0036;
		IL_0036:
			return (byte)num != 0;
		}

		public XYZ ToXYZ(double Zvalue=0.0)
        {
			return new XYZ(U, V, Zvalue);
        }
		public bool IsUnitLength()
		{
			return (byte)((Math.Abs(GetLength() - 1.0) < 1E-09) ? 1u : 0u) != 0;
		}

		public static UV operator +(UV left, UV right)
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

		public static UV operator -(UV source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			return source.Negate();
		}

		public static UV operator -(UV left, UV right)
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

		public static UV operator *(UV left, double value)
		{
			if (null == left)
			{
				throw new Exception("空值错误");
			}
			
			return left.Multiply(value);
		}

		public static UV operator *(double value, UV right)
		{
			if (null == right)
			{
				throw new Exception("空值错误");
			}
			
			return right.Multiply(value);
		}

		public static UV operator /(UV left, double value)
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

		public double DotProduct(UV source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			return source.m_v * m_v + source.m_u * m_u;
		}

		public double CrossProduct(UV source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			return source.m_v * m_u - source.m_u * m_v;
		}

		public UV Add(UV source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			double v = m_v;
			double v2 = source.m_v;
			double u = m_u;
			double u2 = source.m_u;
			return new UV(u2 + u, v2 + v);
		}

		public UV Subtract(UV source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			double v = m_v;
			double v2 = source.m_v;
			double u = m_u;
			double u2 = source.m_u;
			return new UV(u - u2, v - v2);
		}

		public UV Negate()
		{
			double v = m_v;
			double u = m_u;
			return new UV(0.0 - u, 0.0 - v);
		}

		public UV Multiply(double value)
		{
			
			double v = m_v;
			double u = m_u;
			return new UV(u * value, v * value);
		}

		public UV Divide(double value)
		{
			
			if (Math.Abs(value) < 1E-09)
			{
				throw new Exception("小值太小");
			}
			double v = m_v;
			double u = m_u;
			return new UV(u / value, v / value);
		}

	 
		public bool IsAlmostEqualTo(UV source, double tolerance)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			if (tolerance < 0.0)
			{
				throw new Exception("小值太小");
			}
			double num = tolerance * tolerance;
			double num2 = source.DotProduct(source) + DotProduct(this);
			if (num2 < num)
			{
				return true;
			}
			UV UV = this - source;
			return (byte)((UV.DotProduct(UV) < num2 * num) ? 1u : 0u) != 0;
		}

	 
		public bool IsAlmostEqualTo(UV source)
		{
			return IsAlmostEqualTo(source, 1E-09);
		}

		public double DistanceTo(UV source)
		{
			if (null == source)
			{
				throw new Exception("空值错误");
			}
			return (this - source).GetLength();
		}

		//以下根据
		//x1=xcosθ-ysinθ;y1=ycosθ+xsinθ
		//20220418lvwan修改以上公式
		public UV RotateByDegree(double degreeRadian)
		{
			UV result = new UV();
			double u = m_u;
			double v = m_v;
			//取消角度限制
			//if (degreeRadian <= Math.PI*2 && degreeRadian >= 0)
			//{
				double θ = degreeRadian;
				result = new UV(u*Math.Cos(θ)-v*Math.Sin(θ), v * Math.Cos(θ) + u * Math.Sin(θ));
			//}
			//else
			//{
			//	throw new Exception("角度范围不在0~2π内");
			//}
			return result;
		}
		//平行
		public bool IsParallelTo(UV source)
		{
			bool result = false;
			UV norm0 = this.Normalize();
			UV norm1 = source.Normalize();
			UV norm2 = -norm1;
			if (norm0.IsAlmostEqualTo(norm1) || norm0.IsAlmostEqualTo(norm2))
			{
				result = true;
			}
			return result;
		}
		//垂直
		public bool IsPerpendicularTo(UV source)
		{
			bool result = false;
			UV norm0 = this;
			UV norm1 = source;
			if (Math.Abs(norm0.DotProduct(norm1)) < 1E-09)
			{
				result = true;
			}
			return result;
		}

		public double AngleTo(UV source)
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

        public RS ConvertToRS()
        {
			double sita = Math.Atan2(m_v, m_u).RadiansTo0_2PI();
			double len = GetLength();
            RS result = new RS(len,sita);

            return result;
        }
        public sealed override string ToString()
		{
			return ToString(null, null);
		}
		public double[] ToArray()
		{
			return new double[2] { m_u, m_v };
		}

		public void Deconstruct(out double x, out double y)
		{
			x = this.m_u;
			y = this.m_v;
		}

		//public bool TransformTo(Transform transform, out UV point)
		//{
		//	point = transform.ApplyToPoint(this);
		//	return true;
		//}
		public List<double> ToList() => new List<double> { m_u, m_v};

		public static UV FromList(IList<double> list) => new UV(list[0], list[1]);

		public bool IsXYZ()
		{
			return false;
		}

		public bool IsUV()
		{
			return true;
		}

		public bool IsRAB()
		{
			return false;
		}

		public bool IsRS()
		{
			return false;
		}

        public bool TransformTo(TransformXYZ transform, out UV transformed)
        {
			transformed = transform.ApplyToUV(this);
			return true;
        }
		public bool TransformTo(TransformXYZ transform, out ITransformable transformed)
		{
			transformed = transform.ApplyToUV(this);
			return true;
		}
	}
}
