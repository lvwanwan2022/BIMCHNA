using System;
using LNLib;

namespace LNLib.Mathematics
{
    /// <summary>
    /// 表示二维参数点的结构体
    /// </summary>
    public struct UV
    {
        private double _u;
        private double _v;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="u">U参数</param>
        /// <param name="v">V参数</param>
        public UV(double u, double v)
        {
            _u = u;
            _v = v;
        }

        /// <summary>
        /// 获取U参数
        /// </summary>
        public double U => _u;

        /// <summary>
        /// 获取V参数
        /// </summary>
        public double V => _v;

        /// <summary>
        /// 加法运算符重载
        /// </summary>
        /// <param name="uv1">第一个UV点</param>
        /// <param name="uv2">第二个UV点</param>
        /// <returns>加法结果</returns>
        public static UV operator +(UV uv1, UV uv2)
        {
            return new UV(uv1._u + uv2._u, uv1._v + uv2._v);
        }

        /// <summary>
        /// 减法运算符重载
        /// </summary>
        /// <param name="uv1">第一个UV点</param>
        /// <param name="uv2">第二个UV点</param>
        /// <returns>减法结果</returns>
        public static UV operator -(UV uv1, UV uv2)
        {
            return new UV(uv1._u - uv2._u, uv1._v - uv2._v);
        }

        /// <summary>
        /// 乘法运算符重载（标量乘法）
        /// </summary>
        /// <param name="d">标量</param>
        /// <param name="uv">UV点</param>
        /// <returns>乘法结果</returns>
        public static UV operator *(double d, UV uv)
        {
            return new UV(d * uv._u, d * uv._v);
        }

        /// <summary>
        /// 乘法运算符重载（标量乘法）
        /// </summary>
        /// <param name="uv">UV点</param>
        /// <param name="d">标量</param>
        /// <returns>乘法结果</returns>
        public static UV operator *(UV uv, double d)
        {
            return new UV(uv._u * d, uv._v * d);
        }

        /// <summary>
        /// 除法运算符重载（标量除法）
        /// </summary>
        /// <param name="uv">UV点</param>
        /// <param name="d">标量</param>
        /// <returns>除法结果</returns>
        public static UV operator /(UV uv, double d)
        {
            if (Math.Abs(d) < Constants.DoubleEpsilon)
                throw new DivideByZeroException("除数不能为零");
            return new UV(uv._u / d, uv._v / d);
        }

        /// <summary>
        /// 判断两个UV点是否相等
        /// </summary>
        /// <param name="uv1">第一个UV点</param>
        /// <param name="uv2">第二个UV点</param>
        /// <returns>是否相等</returns>
        public static bool operator ==(UV uv1, UV uv2)
        {
            return MathUtils.IsAlmostEqualTo(uv1._u, uv2._u) && MathUtils.IsAlmostEqualTo(uv1._v, uv2._v);
        }

        /// <summary>
        /// 判断两个UV点是否不相等
        /// </summary>
        /// <param name="uv1">第一个UV点</param>
        /// <param name="uv2">第二个UV点</param>
        /// <returns>是否不相等</returns>
        public static bool operator !=(UV uv1, UV uv2)
        {
            return !MathUtils.IsAlmostEqualTo(uv1._u, uv2._u) || !MathUtils.IsAlmostEqualTo(uv1._v, uv2._v);
        }

        /// <summary>
        /// 判断对象是否相等
        /// </summary>
        /// <param name="obj">比较对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is UV))
                return false;
            UV uv = (UV)obj;
            return this == uv;
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns>哈希码</returns>
        public override int GetHashCode()
        {
            return _u.GetHashCode() ^ _v.GetHashCode();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"UV({_u}, {_v})";
        }

        /// <summary>
        /// 距离计算
        /// </summary>
        /// <param name="other">另一个UV点</param>
        /// <returns>距离</returns>
        public double Distance(UV other)
        {
            double du = _u - other._u;
            double dv = _v - other._v;
            return Math.Sqrt(du * du + dv * dv);
        }

        /// <summary>
        /// 点乘
        /// </summary>
        /// <param name="other">另一个UV点</param>
        /// <returns>点乘结果</returns>
        public double DotProduct(UV other)
        {
            return _u * other._u + _v * other._v;
        }

        /// <summary>
        /// 计算长度（二维向量的模）
        /// </summary>
        /// <returns>长度</returns>
        public double Length()
        {
            return Math.Sqrt(_u * _u + _v * _v);
        }

        /// <summary>
        /// 标准化（使长度为1）
        /// </summary>
        /// <returns>标准化后的UV</returns>
        public UV Normalize()
        {
            double length = Length();
            if (Math.Abs(length) < Constants.DoubleEpsilon)
                throw new InvalidOperationException("零向量不能被标准化");
            return new UV(_u / length, _v / length);
        }
    }
} 