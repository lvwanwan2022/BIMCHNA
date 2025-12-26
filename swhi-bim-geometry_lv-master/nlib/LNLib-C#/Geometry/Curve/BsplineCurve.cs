using System;
using System.Collections.Generic;
using LNLib.Mathematics;
using LNLib.Algorithm;

namespace LNLib.Geometry.Curve
{
    /// <summary>
    /// B样条曲线数据
    /// </summary>
    /// <typeparam name="T">控制点类型</typeparam>
    public class BsplineCurveData<T> where T : class, IWeightable<T>
    {
        private List<T> _controlPoints;
        private List<double> _knotVector;
        private int _degree;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="degree">次数</param>
        /// <param name="controlPoints">控制点列表</param>
        /// <param name="knotVector">节点向量</param>
        public BsplineCurveData(int degree, List<T> controlPoints, List<double> knotVector)
        {
            _degree = degree;
            _controlPoints = new List<T>(controlPoints);
            _knotVector = new List<double>(knotVector);
        }

        /// <summary>
        /// 获取次数
        /// </summary>
        public int Degree => _degree;

        /// <summary>
        /// 获取控制点数量
        /// </summary>
        public int Count => _controlPoints.Count;

        /// <summary>
        /// 获取控制点
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>控制点</returns>
        public T this[int index] => _controlPoints[index];

        /// <summary>
        /// 获取控制点列表
        /// </summary>
        /// <returns>控制点列表</returns>
        public List<T> GetControlPoints() => new List<T>(_controlPoints);

        /// <summary>
        /// 获取节点向量
        /// </summary>
        /// <returns>节点向量</returns>
        public List<double> GetKnotVector() => new List<double>(_knotVector);
    }

    /// <summary>
    /// B样条曲线类
    /// </summary>
    public static class BsplineCurve
    {
        /// <summary>
        /// 验证B样条曲线参数的有效性
        /// </summary>
        /// <typeparam name="T">控制点类型</typeparam>
        /// <param name="curve">B样条曲线</param>
        /// <exception cref="ArgumentException">参数无效时抛出异常</exception>
        public static void Check<T>(BsplineCurveData<T> curve) where T : class, IWeightable<T>
        {
            int degree = curve.Degree;
            List<double> knotVector = curve.GetKnotVector();
            int controlPointsCount = curve.Count;

            if (degree <= 0)
                throw new ArgumentException("次数必须大于零。", nameof(curve));

            if (knotVector.Count <= 0)
                throw new ArgumentException("节点向量不能为空。", nameof(curve));

            if (!ValidationUtils.IsValidKnotVector(knotVector))
                throw new ArgumentException("节点向量必须是非递减的实数序列。", nameof(curve));

            if (controlPointsCount <= 0)
                throw new ArgumentException("控制点列表不能为空。", nameof(curve));

            if (!ValidationUtils.IsValidBspline(degree, knotVector.Count, controlPointsCount))
                throw new ArgumentException("参数必须满足: m = n + p + 1", nameof(curve));
        }

        /// <summary>
        /// 计算B样条曲线上的点
        /// NURBS Book第2版第82页
        /// Algorithm A3.1
        /// </summary>
        /// <typeparam name="T">控制点类型</typeparam>
        /// <param name="curve">B样条曲线</param>
        /// <param name="paramT">参数t</param>
        /// <returns>曲线上的点</returns>
        public static T GetPointOnCurve<T>(BsplineCurveData<T> curve, double paramT) where T : class, IWeightable<T>, new()
        {
            Check(curve);

            int degree = curve.Degree;
            List<double> knotVector = curve.GetKnotVector();

            if (paramT < knotVector[0] || paramT > knotVector[knotVector.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(paramT), "参数t必须在节点向量的范围内。");

            // 特殊情况处理
            if (Math.Abs(paramT - knotVector[knotVector.Count - 1]) < Constants.DoubleEpsilon)
            {
                return curve[curve.Count - 1];
            }

            int span = Polynomials.GetKnotSpanIndex(degree, knotVector.ToArray(), paramT);
            double[] basisFunctions = new double[degree + 1];
            Polynomials.BasisFunctions(span, degree, knotVector.ToArray(), paramT, basisFunctions);

            T point = new T();
            for (int i = 0; i <= degree; i++)
            {
                point = point.Add(curve[span - degree + i].Multiply(basisFunctions[i]));
            }

            return point;
        }

        /// <summary>
        /// 计算B样条曲线的导数
        /// NURBS Book第2版第93页
        /// Algorithm A3.2
        /// </summary>
        /// <typeparam name="T">控制点类型</typeparam>
        /// <param name="curve">B样条曲线</param>
        /// <param name="derivative">导数阶数</param>
        /// <param name="paramT">参数t</param>
        /// <returns>导数值数组</returns>
        public static List<T> ComputeDerivatives<T>(BsplineCurveData<T> curve, int derivative, double paramT) where T : class, IWeightable<T>, new()
        {
            Check(curve);

            int degree = curve.Degree;
            List<double> knotVector = curve.GetKnotVector();

            if (paramT < knotVector[0] || paramT > knotVector[knotVector.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(paramT), "参数t必须在节点向量的范围内。");

            int du = Math.Min(derivative, degree);
            List<T> result = new List<T>(derivative + 1);
            for (int i = 0; i <= derivative; i++)
            {
                result.Add(new T());
            }

            // 特殊情况处理
            if (Math.Abs(paramT - knotVector[knotVector.Count - 1]) < Constants.DoubleEpsilon)
            {
                int spanBoundary = knotVector.Count - degree - 2;
                double[][] derivativesBasisBoundary = Polynomials.BasisFunctionsDerivatives(spanBoundary, degree, du, knotVector.ToArray(), paramT);

                for (int k = 0; k <= du; k++)
                {
                    for (int j = 0; j <= degree; j++)
                    {
                        result[k] = result[k].Add(curve[spanBoundary - degree + j].Multiply(derivativesBasisBoundary[k][j]));
                    }
                }
                return result;
            }

            int span = Polynomials.GetKnotSpanIndex(degree, knotVector.ToArray(), paramT);
            double[][] derivativesBasis = Polynomials.BasisFunctionsDerivatives(span, degree, du, knotVector.ToArray(), paramT);

            for (int k = 0; k <= du; k++)
            {
                for (int j = 0; j <= degree; j++)
                {
                    result[k] = result[k].Add(curve[span - degree + j].Multiply(derivativesBasis[k][j]));
                }
            }

            return result;
        }

        /// <summary>
        /// 计算B样条曲线导数的控制点
        /// NURBS Book第2版第98页
        /// Algorithm A3.3
        /// </summary>
        /// <typeparam name="T">控制点类型</typeparam>
        /// <param name="curve">B样条曲线</param>
        /// <param name="derivative">导数阶数</param>
        /// <param name="minSpanIndex">最小跨度索引</param>
        /// <param name="maxSpanIndex">最大跨度索引</param>
        /// <returns>导数控制点数组</returns>
        public static List<List<T>> ComputeControlPointsOfDerivatives<T>(BsplineCurveData<T> curve, int derivative, int minSpanIndex, int maxSpanIndex) where T : class, IWeightable<T>, new()
        {
            Check(curve);

            int degree = curve.Degree;
            List<double> knotVector = curve.GetKnotVector();
            List<T> controlPoints = curve.GetControlPoints();

            if (derivative < 1 || derivative > degree)
                throw new ArgumentOutOfRangeException(nameof(derivative), "导数阶数必须在1到次数之间。");

            if (minSpanIndex < 0 || minSpanIndex >= knotVector.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(minSpanIndex), "最小跨度索引无效。");

            if (maxSpanIndex < minSpanIndex || maxSpanIndex >= knotVector.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(maxSpanIndex), "最大跨度索引无效。");

            List<List<T>> result = new List<List<T>>(derivative + 1);
            for (int i = 0; i <= derivative; i++)
            {
                result.Add(new List<T>(controlPoints));
            }

            for (int k = 1; k <= derivative; k++)
            {
                for (int i = minSpanIndex; i <= maxSpanIndex; i++)
                {
                    result[k][i] = result[k - 1][i + 1].Subtract(result[k - 1][i]).Multiply(degree / (knotVector[i + degree + 1] - knotVector[i + 1]));
                }
            }

            return result;
        }

        /// <summary>
        /// 计算B样条曲线的弧长
        /// </summary>
        /// <param name="curve">B样条曲线</param>
        /// <param name="startParam">起始参数</param>
        /// <param name="endParam">结束参数</param>
        /// <param name="tolerance">容差</param>
        /// <returns>弧长</returns>
        public static double GetArcLength(BsplineCurveData<XYZW> curve, double startParam, double endParam, double tolerance = 1e-3)
        {
            Check(curve);

            List<double> knotVector = curve.GetKnotVector();

            if (startParam < knotVector[0] || startParam > knotVector[knotVector.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(startParam), "起始参数t必须在节点向量的范围内。");

            if (endParam < knotVector[0] || endParam > knotVector[knotVector.Count - 1] || endParam < startParam)
                throw new ArgumentOutOfRangeException(nameof(endParam), "结束参数t必须在节点向量的范围内且大于等于起始参数。");

            // 使用辛普森积分计算弧长
            return Integrator.ApplySimpsonRule(
                t =>
                {
                    List<XYZ> derivatives = NurbsCurve.ComputeRationalCurveDerivatives(curve, 1, t);
                    return derivatives[1].Length();
                },
                startParam,
                endParam,
                tolerance
            );
        }

        /// <summary>
        /// 将B样条曲线转换为NurbsCurveData
        /// </summary>
        /// <typeparam name="T">控制点类型</typeparam>
        /// <param name="curve">B样条曲线</param>
        /// <returns>NURBS曲线数据</returns>
        public static BsplineCurveData<XYZW> ConvertToNurbsCurve<T>(BsplineCurveData<T> curve) where T : class, IWeightable<T>, new()
        {
            Check(curve);

            List<XYZW> controlPoints = new List<XYZW>(curve.Count);
            for (int i = 0; i < curve.Count; i++)
            {
                if (curve[i] is XYZ xyz)
                {
                    controlPoints.Add(new XYZW(xyz, 1.0));
                }
                else if (curve[i] is XYZW xyzw)
                {
                    controlPoints.Add(xyzw);
                }
                else
                {
                    throw new ArgumentException("控制点类型必须是XYZ或XYZW。", nameof(curve));
                }
            }

            return new BsplineCurveData<XYZW>(curve.Degree, controlPoints, curve.GetKnotVector());
        }
    }
} 