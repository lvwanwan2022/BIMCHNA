namespace LNLib.Mathematics
{
    /// <summary>
    /// 数学常量
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 浮点数精度
        /// </summary>
        public const double DoubleEpsilon = 1.0e-8;
        
        /// <summary>
        /// 距离精度
        /// </summary>
        public const double DistanceEpsilon = 1.0e-6;

        /// <summary>
        /// NURBS最大阶数
        /// </summary>
        public const int NURBSMaxDegree = 25;

        /// <summary>
        /// 圆周率
        /// </summary>
        public const double Pi = 3.14159265358979323846;

        /// <summary>
        /// 2倍圆周率
        /// </summary>
        public const double TwoPi = 2.0 * Pi;

        /// <summary>
        /// 半圆周率
        /// </summary>
        public const double HalfPi = Pi / 2.0;

        /// <summary>
        /// 弧度到角度的转换系数
        /// </summary>
        public const double RadToDeg = 180.0 / Pi;

        /// <summary>
        /// 角度到弧度的转换系数
        /// </summary>
        public const double DegToRad = Pi / 180.0;
    }
} 