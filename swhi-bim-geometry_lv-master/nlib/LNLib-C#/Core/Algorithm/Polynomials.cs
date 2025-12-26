using System;
using System.Collections.Generic;
using LNLib.Mathematics;

namespace LNLib.Algorithm
{
    /// <summary>
    /// 多项式计算工具类
    /// </summary>
    public static class Polynomials
    {
        /// <summary>
        /// 霍纳法则计算幂基函数曲线上的点
        /// NURBS Book第2版第20页
        /// Algorithm A1.1
        /// </summary>
        /// <param name="degree">曲线次数</param>
        /// <param name="coefficients">系数</param>
        /// <param name="paramT">参数t</param>
        /// <returns>函数值</returns>
        public static double Horner(int degree, double[] coefficients, double paramT)
        {
            double result = coefficients[0];
            for (int i = 1; i <= degree; i++)
            {
                result = result * paramT + coefficients[i];
            }
            return result;
        }

        /// <summary>
        /// 计算伯恩斯坦多项式的值
        /// NURBS Book第2版第7页
        /// Algorithm A1.2
        /// </summary>
        /// <param name="index">指标</param>
        /// <param name="degree">次数</param>
        /// <param name="paramT">参数t</param>
        /// <returns>多项式值</returns>
        public static double Bernstein(int index, int degree, double paramT)
        {
            if (index < 0 || index > degree)
                return 0.0;

            if (degree == 0)
                return 1.0;

            if (paramT == 0.0)
                return (index == 0) ? 1.0 : 0.0;

            if (paramT == 1.0)
                return (index == degree) ? 1.0 : 0.0;

            // 使用递归计算
            double value1 = Bernstein(index, degree - 1, paramT);
            double value2 = Bernstein(index - 1, degree - 1, paramT);

            return (1.0 - paramT) * value1 + paramT * value2;
        }

        /// <summary>
        /// 计算所有n次伯恩斯坦多项式的值
        /// NURBS Book第2版第21页
        /// Algorithm A1.3
        /// </summary>
        /// <param name="degree">次数</param>
        /// <param name="paramT">参数t</param>
        /// <returns>多项式值数组</returns>
        public static double[] AllBernstein(int degree, double paramT)
        {
            double[] B = new double[degree + 1];

            B[0] = 1.0;
            double u = paramT;
            double complement = 1.0 - u;

            for (int j = 1; j <= degree; j++)
            {
                double saved = 0.0;
                for (int k = 0; k < j; k++)
                {
                    double temp = B[k];
                    B[k] = saved + complement * temp;
                    saved = u * temp;
                }
                B[j] = saved;
            }

            return B;
        }

        /// <summary>
        /// 使用霍纳法则计算幂基曲面上的点
        /// NURBS Book第2版第36页
        /// Algorithm A1.6
        /// </summary>
        /// <param name="degreeU">U方向次数</param>
        /// <param name="degreeV">V方向次数</param>
        /// <param name="coefficients">系数矩阵</param>
        /// <param name="uv">参数uv</param>
        /// <returns>曲面上的点值</returns>
        public static double Horner(int degreeU, int degreeV, double[,] coefficients, UV uv)
        {
            double[] tempValues = new double[degreeU + 1];

            // 先计算V方向
            for (int i = 0; i <= degreeU; i++)
            {
                double temp = coefficients[i, 0];
                for (int j = 1; j <= degreeV; j++)
                {
                    temp = temp * uv.V + coefficients[i, j];
                }
                tempValues[i] = temp;
            }

            // 再计算U方向
            double result = tempValues[0];
            for (int i = 1; i <= degreeU; i++)
            {
                result = result * uv.U + tempValues[i];
            }

            return result;
        }

        /// <summary>
        /// 获取节点的重复度
        /// NURBS Book第2版第63页
        /// </summary>
        /// <param name="knotVector">节点向量</param>
        /// <param name="paramT">参数t</param>
        /// <returns>重复度</returns>
        public static int GetKnotMultiplicity(double[] knotVector, double paramT)
        {
            int multiplicity = 0;

            for (int i = 0; i < knotVector.Length; i++)
            {
                if (Math.Abs(knotVector[i] - paramT) < Constants.DoubleEpsilon)
                {
                    multiplicity++;
                }
            }

            return multiplicity;
        }

        /// <summary>
        /// 确定参数t所在的节点跨度索引
        /// NURBS Book第2版第68页
        /// Algorithm A2.1
        /// </summary>
        /// <param name="degree">次数</param>
        /// <param name="knotVector">节点向量</param>
        /// <param name="paramT">参数t</param>
        /// <returns>节点跨度索引</returns>
        public static int GetKnotSpanIndex(int degree, double[] knotVector, double paramT)
        {
            int knotCount = knotVector.Length;
            int n = knotCount - degree - 1;

            if (Math.Abs(paramT - knotVector[n]) < Constants.DoubleEpsilon)
            {
                return n - 1;
            }

            // 二分查找
            int low = degree;
            int high = n;
            int mid = (low + high) / 2;

            while (paramT < knotVector[mid] || paramT >= knotVector[mid + 1])
            {
                if (paramT < knotVector[mid])
                {
                    high = mid;
                }
                else
                {
                    low = mid;
                }
                mid = (low + high) / 2;
            }

            return mid;
        }

        /// <summary>
        /// 计算B样条基函数数组
        /// NURBS Book第2版第70页
        /// Algorithm A2.2
        /// </summary>
        /// <param name="spanIndex">节点跨度索引</param>
        /// <param name="degree">次数</param>
        /// <param name="knotVector">节点向量</param>
        /// <param name="paramT">参数t</param>
        /// <param name="basisFunctions">输出的基函数值数组</param>
        public static void BasisFunctions(int spanIndex, int degree, double[] knotVector, double paramT, double[] basisFunctions)
        {
            basisFunctions[0] = 1.0;
            double[] left = new double[degree + 1];
            double[] right = new double[degree + 1];

            for (int j = 1; j <= degree; j++)
            {
                left[j] = paramT - knotVector[spanIndex + 1 - j];
                right[j] = knotVector[spanIndex + j] - paramT;
                double saved = 0.0;

                for (int r = 0; r < j; r++)
                {
                    double temp = basisFunctions[r] / (right[r + 1] + left[j - r]);
                    basisFunctions[r] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }

                basisFunctions[j] = saved;
            }
        }

        /// <summary>
        /// 计算非零基函数及其导数
        /// NURBS Book第2版第72页
        /// Algorithm A2.3
        /// </summary>
        /// <param name="spanIndex">节点跨度索引</param>
        /// <param name="degree">次数</param>
        /// <param name="derivative">导数阶数</param>
        /// <param name="knotVector">节点向量</param>
        /// <param name="paramT">参数t</param>
        /// <returns>基函数值及其导数</returns>
        public static double[][] BasisFunctionsDerivatives(int spanIndex, int degree, int derivative, double[] knotVector, double paramT)
        {
            int du = Math.Min(derivative, degree);
            double[][] derivatives = new double[du + 1][];
            for (int i = 0; i <= du; i++)
            {
                derivatives[i] = new double[degree + 1];
            }

            // 创建并计算ndu表
            double[,] ndu = new double[degree + 1, degree + 1];
            ndu[0, 0] = 1.0;

            double[] left = new double[degree + 1];
            double[] right = new double[degree + 1];

            for (int j = 1; j <= degree; j++)
            {
                left[j] = paramT - knotVector[spanIndex + 1 - j];
                right[j] = knotVector[spanIndex + j] - paramT;
                double saved = 0.0;

                for (int r = 0; r < j; r++)
                {
                    // 对角线上的值
                    ndu[j, r] = right[r + 1] + left[j - r];
                    double temp = ndu[r, j - 1] / ndu[j, r];
                    // 基函数值
                    ndu[r, j] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }

                ndu[j, j] = saved;
            }

            // 复制基函数值
            for (int j = 0; j <= degree; j++)
            {
                derivatives[0][j] = ndu[j, degree];
            }

            // 计算导数
            double[,] a = new double[2, degree + 1];
            // a[0, 0] = 1.0; 实际上不需要

            // 计算导数
            for (int r = 0; r <= degree; r++)
            {
                // 第一行和第二行
                int s1 = 0;
                int s2 = 1;
                a[0, 0] = 1.0;

                // 计算导数
                for (int k = 1; k <= du; k++)
                {
                    double d = 0.0;
                    int rk = r - k;
                    int pk = degree - k;

                    if (r >= k)
                    {
                        a[s2, 0] = a[s1, 0] / ndu[pk + 1, rk];
                        d = a[s2, 0] * ndu[rk, pk];
                    }

                    int j1 = rk >= -1 ? 1 : -rk;
                    int j2 = r - 1 <= pk ? k - 1 : degree - r;

                    for (int j = j1; j <= j2; j++)
                    {
                        a[s2, j] = (a[s1, j] - a[s1, j - 1]) / ndu[pk + 1, rk + j];
                        d += a[s2, j] * ndu[rk + j, pk];
                    }

                    if (r <= pk)
                    {
                        a[s2, k] = -a[s1, k - 1] / ndu[pk + 1, r];
                        d += a[s2, k] * ndu[r, pk];
                    }

                    derivatives[k][r] = d;

                    // 交换行
                    int temp = s1;
                    s1 = s2;
                    s2 = temp;
                }
            }

            // 乘以合适的系数
            int factorial = 1;
            for (int k = 1; k <= du; k++)
            {
                factorial *= k;
                for (int j = 0; j <= degree; j++)
                {
                    derivatives[k][j] *= factorial;
                }
            }

            return derivatives;
        }

        /// <summary>
        /// 计算基函数的一阶导数（优化版本）
        /// BasisFunctionsDerivatives的特殊情况，仅计算一阶导数
        /// </summary>
        /// <param name="spanIndex">节点跨度索引</param>
        /// <param name="degree">次数</param>
        /// <param name="knotVector">节点向量</param>
        /// <param name="paramT">参数t</param>
        /// <param name="derivatives">输出的导数数组</param>
        public static void BasisFunctionsFirstOrderDerivative(int spanIndex, int degree, double[] knotVector, double paramT, double[,] derivatives)
        {
            double[] nders = new double[degree + 1];
            double[] ders = new double[degree + 1];
            double[] N = new double[degree + 1];

            BasisFunctions(spanIndex, degree, knotVector, paramT, N);

            // 复制基函数值到结果的第一行
            for (int j = 0; j <= degree; j++)
            {
                derivatives[0, j] = N[j];
            }

            // 计算一阶导数
            for (int j = 0; j <= degree; j++)
            {
                // 左导数和右导数
                double leftDeriv = 0.0;
                double rightDeriv = 0.0;

                if (j > 0)
                {
                    // 非零左导数
                    leftDeriv = degree / (knotVector[spanIndex + j] - knotVector[spanIndex + j - degree]) * N[j - 1];
                }

                if (j < degree)
                {
                    // 非零右导数
                    rightDeriv = -degree / (knotVector[spanIndex + j + 1] - knotVector[spanIndex + j - degree + 1]) * N[j];
                }

                derivatives[1, j] = leftDeriv + rightDeriv;
            }
        }

        /// <summary>
        /// 计算单个B样条基函数
        /// NURBS Book第2版第74页
        /// Algorithm A2.4
        /// </summary>
        /// <param name="spanIndex">节点跨度索引</param>
        /// <param name="degree">次数</param>
        /// <param name="knotVector">节点向量</param>
        /// <param name="paramT">参数t</param>
        /// <returns>单个基函数值</returns>
        public static double OneBasisFunction(int spanIndex, int degree, double[] knotVector, double paramT)
        {
            // 特殊情况处理
            if ((spanIndex == 0 && Math.Abs(paramT - knotVector[0]) < Constants.DoubleEpsilon) || 
                (spanIndex == knotVector.Length - degree - 2 && Math.Abs(paramT - knotVector[knotVector.Length - 1]) < Constants.DoubleEpsilon))
            {
                return 1.0;
            }

            if (paramT < knotVector[spanIndex] || paramT >= knotVector[spanIndex + degree + 1])
            {
                return 0.0;
            }

            // 存储计算结果的临时数组
            double[] N = new double[degree + 1];

            // 初始化0次基函数
            for (int j = 0; j <= degree; j++)
            {
                if (paramT >= knotVector[spanIndex + j] && paramT < knotVector[spanIndex + j + 1])
                {
                    N[j] = 1.0;
                }
                else
                {
                    N[j] = 0.0;
                }
            }

            // 计算高阶基函数
            for (int k = 1; k <= degree; k++)
            {
                double saved = 0.0;
                if (N[0] != 0.0)
                {
                    saved = ((paramT - knotVector[spanIndex]) * N[0]) / (knotVector[spanIndex + k] - knotVector[spanIndex]);
                }

                for (int j = 0; j < degree - k + 1; j++)
                {
                    double leftKnot = knotVector[spanIndex + j + 1];
                    double rightKnot = knotVector[spanIndex + j + k + 1];
                    if (N[j + 1] == 0.0)
                    {
                        N[j] = saved;
                        saved = 0.0;
                    }
                    else
                    {
                        double temp = N[j + 1] / (rightKnot - leftKnot);
                        N[j] = saved + (rightKnot - paramT) * temp;
                        saved = (paramT - leftKnot) * temp;
                    }
                }
            }

            return N[0];
        }

        /// <summary>
        /// 计算单个基函数及其导数
        /// NURBS Book第2版第76页
        /// Algorithm A2.5
        /// </summary>
        /// <param name="spanIndex">节点跨度索引</param>
        /// <param name="degree">次数</param>
        /// <param name="derivative">导数阶数</param>
        /// <param name="knotVector">节点向量</param>
        /// <param name="paramT">参数t</param>
        /// <returns>基函数值及其导数</returns>
        public static double[] OneBasisFunctionDerivative(int spanIndex, int degree, int derivative, double[] knotVector, double paramT)
        {
            double[] derivatives = new double[derivative + 1];

            // 存储临时结果的二维数组
            double[,] ndu = new double[degree + 1, degree + 1];

            // 初始化0次基函数及其导数（临时结果）
            for (int j = 0; j <= degree; j++)
            {
                if ((paramT >= knotVector[spanIndex + j]) && (paramT < knotVector[spanIndex + j + 1]))
                {
                    ndu[j, 0] = 1.0;
                }
                else
                {
                    ndu[j, 0] = 0.0;
                }
            }

            // 计算三角形表ndu
            for (int k = 1; k <= degree; k++)
            {
                for (int j = 0; j <= degree - k; j++)
                {
                    double leftKnot = knotVector[spanIndex + j + 1];
                    double rightKnot = knotVector[spanIndex + j + k + 1];
                    
                    // 避免除零
                    if (Math.Abs(rightKnot - leftKnot) < Constants.DoubleEpsilon)
                    {
                        ndu[j, k] = 0.0;
                    }
                    else
                    {
                        ndu[j, k] = ndu[j, k - 1] * (paramT - leftKnot) / (rightKnot - leftKnot) + 
                                     ndu[j + 1, k - 1] * (rightKnot - paramT) / (rightKnot - leftKnot);
                    }
                }
            }

            // 基函数值
            derivatives[0] = ndu[0, degree];

            // 计算导数
            for (int k = 1; k <= derivative; k++)
            {
                // 初始化导数公式中的系数
                double[,] a = new double[2, degree + 1];
                
                // 计算k阶导数
                for (int j = 0; j <= k; j++)
                {
                    a[0, j] = 1.0; // 起始值
                }

                for (int jj = 1; jj <= k; jj++)
                {
                    int i = 0;
                    int j = jj;
                    double temp = a[0, 0] / (knotVector[spanIndex + degree + 1 - jj] - knotVector[spanIndex]);
                    a[1, 0] = -temp * degree;

                    for (i = 1; i <= k - jj; i++)
                    {
                        double num1 = knotVector[spanIndex + i];
                        double num2 = knotVector[spanIndex + i + degree + 1 - jj];
                        if (Math.Abs(num2 - num1) < Constants.DoubleEpsilon)
                        {
                            a[1, i] = 0.0;
                        }
                        else
                        {
                            temp = a[0, i] - a[0, i - 1];
                            a[1, i] = temp * degree / (num2 - num1);
                        }
                    }

                    for (i = 0; i < k - jj + 1; i++)
                    {
                        a[0, i] = a[1, i];
                    }
                }

                // 乘以基函数值得到导数
                double factor = degree;
                for (int j = 1; j <= k; j++)
                {
                    factor *= (degree - j + 1);
                }

                derivatives[k] = a[0, 0] * factor;
            }

            return derivatives;
        }

        /// <summary>
        /// 计算所有次数的基函数（从0到degree）
        /// NURBS Book第2版第99页
        /// Algorithm A2.2的修改版
        /// </summary>
        /// <param name="spanIndex">节点跨度索引</param>
        /// <param name="degree">次数</param>
        /// <param name="knotVector">节点向量</param>
        /// <param name="knot">参数</param>
        /// <returns>所有次数的基函数值</returns>
        public static double[][] AllBasisFunctions(int spanIndex, int degree, double[] knotVector, double knot)
        {
            double[][] N = new double[degree + 1][];
            for (int i = 0; i <= degree; i++)
            {
                N[i] = new double[degree + 1];
            }

            N[0][0] = 1.0;
            double[] left = new double[degree + 1];
            double[] right = new double[degree + 1];

            for (int j = 1; j <= degree; j++)
            {
                left[j] = knot - knotVector[spanIndex + 1 - j];
                right[j] = knotVector[spanIndex + j] - knot;
                double saved = 0.0;

                for (int r = 0; r < j; r++)
                {
                    double temp;
                    if (Math.Abs(right[r + 1] + left[j - r]) < Constants.DoubleEpsilon)
                    {
                        temp = 0.0;
                    }
                    else
                    {
                        temp = N[j - 1][r] / (right[r + 1] + left[j - r]);
                    }

                    N[j][r] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }

                N[j][j] = saved;
            }

            return N;
        }

        /// <summary>
        /// 计算p次Bezier矩阵
        /// NURBS Book第2版第269页
        /// Algorithm A6.1
        /// </summary>
        /// <param name="degree">次数</param>
        /// <returns>Bezier矩阵</returns>
        public static double[,] BezierToPowerMatrix(int degree)
        {
            double[,] matrix = new double[degree + 1, degree + 1];

            for (int i = 0; i <= degree; i++)
            {
                double sign = (i % 2 == 0) ? 1.0 : -1.0;

                for (int j = i; j <= degree; j++)
                {
                    // 计算组合数 C(j,i) = j!/(i!*(j-i)!)
                    double comb = 1.0;
                    for (int k = 1; k <= i; k++)
                    {
                        comb *= (j - k + 1.0) / k;
                    }

                    matrix[j, degree - i] = sign * comb;
                    sign = -sign;
                }
            }

            return matrix;
        }

        /// <summary>
        /// 计算p次Bezier矩阵的逆矩阵
        /// NURBS Book第2版第275页
        /// Algorithm A6.2
        /// </summary>
        /// <param name="degree">次数</param>
        /// <param name="matrix">原始矩阵</param>
        /// <returns>逆矩阵</returns>
        public static double[,] PowerToBezierMatrix(int degree, double[,] matrix)
        {
            double[,] inverseMatrix = new double[degree + 1, degree + 1];

            // 计算矩阵的逆
            for (int i = 0; i <= degree; i++)
            {
                // 每一行对应一个Bezier控制点
                double sign = 1.0;

                for (int j = 0; j <= i; j++)
                {
                    // 计算组合数 C(i,j) = i!/(j!*(i-j)!)
                    double comb = 1.0;
                    for (int k = 1; k <= j; k++)
                    {
                        comb *= (i - k + 1.0) / k;
                    }

                    inverseMatrix[i, j] = sign * comb;
                    sign = -sign;
                }
            }

            return inverseMatrix;
        }
    }
} 