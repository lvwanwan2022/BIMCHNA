using System;

namespace LNLib
{
    /// <summary>
    /// 数学工具类
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// 计算二项式系数 C(n,k)
        /// </summary>
        /// <param name="n">上标</param>
        /// <param name="k">下标</param>
        /// <returns>二项式系数</returns>
        public static double Binomial(int n, int k)
        {
            if (k < 0 || k > n) return 0;
            if (k == 0 || k == n) return 1;
            
            // 使用递归计算二项式系数
            return Binomial(n - 1, k - 1) + Binomial(n - 1, k);
        }
        
        /// <summary>
        /// 判断两个浮点数是否近似相等
        /// </summary>
        /// <param name="a">第一个数</param>
        /// <param name="b">第二个数</param>
        /// <param name="epsilon">容差，默认为DoubleEpsilon</param>
        /// <returns>是否近似相等</returns>
        public static bool IsAlmostEqualTo(double a, double b, double epsilon = Mathematics.Constants.DoubleEpsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }
    }
} 