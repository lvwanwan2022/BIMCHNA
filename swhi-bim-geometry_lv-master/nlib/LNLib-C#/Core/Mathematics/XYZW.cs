using System;
using LNLib.Geometry.Curve;

namespace LNLib.Mathematics
{
    /// <summary>
    /// 表示带权重的三维点
    /// </summary>
    public class XYZW : IWeightable<XYZW>
    {
        private double _wx;
        private double _wy;
        private double _wz;
        private double _w;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="wx">X坐标乘以权重</param>
        /// <param name="wy">Y坐标乘以权重</param>
        /// <param name="wz">Z坐标乘以权重</param>
        /// <param name="w">权重</param>
        public XYZW(double wx, double wy, double wz, double w)
        {
            _wx = wx;
            _wy = wy;
            _wz = wz;
            _w = w;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="point">三维点</param>
        /// <param name="weight">权重</param>
        public XYZW(XYZ point, double weight)
        {
            _wx = point.X * weight;
            _wy = point.Y * weight;
            _wz = point.Z * weight;
            _w = weight;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public XYZW()
        {
            _wx = 0;
            _wy = 0;
            _wz = 0;
            _w = 0;
        }

        /// <summary>
        /// 获取X坐标乘以权重
        /// </summary>
        public double WX => _wx;

        /// <summary>
        /// 获取Y坐标乘以权重
        /// </summary>
        public double WY => _wy;

        /// <summary>
        /// 获取Z坐标乘以权重
        /// </summary>
        public double WZ => _wz;

        /// <summary>
        /// 获取权重
        /// </summary>
        public double W => _w;

        /// <summary>
        /// 获取X坐标
        /// </summary>
        public double X 
        { 
            get 
            {
                if (Math.Abs(_w) < Constants.DoubleEpsilon)
                    throw new DivideByZeroException("权重不能为零");
                return _wx / _w;
            }
        }

        /// <summary>
        /// 获取Y坐标
        /// </summary>
        public double Y
        {
            get
            {
                if (Math.Abs(_w) < Constants.DoubleEpsilon)
                    throw new DivideByZeroException("权重不能为零");
                return _wy / _w;
            }
        }

        /// <summary>
        /// 获取Z坐标
        /// </summary>
        public double Z
        {
            get
            {
                if (Math.Abs(_w) < Constants.DoubleEpsilon)
                    throw new DivideByZeroException("权重不能为零");
                return _wz / _w;
            }
        }

        /// <summary>
        /// 将带权重的点投影为三维点
        /// </summary>
        /// <returns>三维点</returns>
        public XYZ ProjectToXYZ()
        {
            if (Math.Abs(_w) < Constants.DoubleEpsilon)
                throw new DivideByZeroException("权重不能为零");
            return new XYZ(_wx / _w, _wy / _w, _wz / _w);
        }

        /// <summary>
        /// 将带权重的点投影为三维点 (ProjectToXYZ的别名)
        /// </summary>
        /// <returns>三维点</returns>
        public XYZ ToXYZ()
        {
            return ProjectToXYZ();
        }

        /// <summary>
        /// 将带权重的点投影为三维点，并指定是否使用权重
        /// </summary>
        /// <param name="weight">权重值</param>
        /// <returns>三维点</returns>
        public XYZ ToXYZ(double weight)
        {
            if (Math.Abs(weight) < Constants.DoubleEpsilon)
                throw new DivideByZeroException("权重不能为零");
            return new XYZ(_wx / weight, _wy / weight, _wz / weight);
        }

        /// <summary>
        /// 加法运算
        /// </summary>
        /// <param name="other">另一个XYZW</param>
        /// <returns>相加后的结果</returns>
        public XYZW Add(XYZW other)
        {
            return new XYZW(
                _wx + other._wx,
                _wy + other._wy,
                _wz + other._wz,
                _w + other._w);
        }

        /// <summary>
        /// 减法运算
        /// </summary>
        /// <param name="other">另一个XYZW</param>
        /// <returns>相减后的结果</returns>
        public XYZW Subtract(XYZW other)
        {
            return new XYZW(
                _wx - other._wx,
                _wy - other._wy,
                _wz - other._wz,
                _w - other._w);
        }

        /// <summary>
        /// 乘法运算
        /// </summary>
        /// <param name="scalar">缩放因子</param>
        /// <returns>缩放后的结果</returns>
        public XYZW Multiply(double scalar)
        {
            return new XYZW(
                _wx * scalar,
                _wy * scalar,
                _wz * scalar,
                _w * scalar);
        }

        /// <summary>
        /// 除法运算
        /// </summary>
        /// <param name="scalar">缩放因子</param>
        /// <returns>缩放后的结果</returns>
        public XYZW Divide(double scalar)
        {
            if (Math.Abs(scalar) < Constants.DoubleEpsilon)
                throw new DivideByZeroException("除数不能为零");

            return new XYZW(
                _wx / scalar,
                _wy / scalar,
                _wz / scalar,
                _w / scalar);
        }

        /// <summary>
        /// 判断是否接近零
        /// </summary>
        /// <returns>如果接近零则返回true</returns>
        public bool IsZero()
        {
            return Math.Abs(_wx) < Constants.DoubleEpsilon &&
                   Math.Abs(_wy) < Constants.DoubleEpsilon &&
                   Math.Abs(_wz) < Constants.DoubleEpsilon &&
                   Math.Abs(_w) < Constants.DoubleEpsilon;
        }

        /// <summary>
        /// 判断是否几乎相等
        /// </summary>
        /// <param name="other">另一个XYZW</param>
        /// <returns>如果几乎相等则返回true</returns>
        public bool IsAlmostEqualTo(XYZW other)
        {
            return Math.Abs(_wx - other._wx) < Constants.DoubleEpsilon &&
                   Math.Abs(_wy - other._wy) < Constants.DoubleEpsilon &&
                   Math.Abs(_wz - other._wz) < Constants.DoubleEpsilon &&
                   Math.Abs(_w - other._w) < Constants.DoubleEpsilon;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"(WX:{_wx}, WY:{_wy}, WZ:{_wz}, W:{_w})";
        }
    }
} 