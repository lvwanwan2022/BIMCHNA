using System;
using System.Collections.Generic;
using LNLib.Mathematics;
using LNLib.Algorithm;
using LNLib;

namespace LNLib.Geometry.Curve
{
    /// <summary>
    /// NURBS曲线类
    /// </summary>
    public static class NurbsCurve
    {
        /// <summary>
        /// 验证NURBS曲线参数的有效性
        /// </summary>
        /// <param name="curve">NURBS曲线</param>
        /// <exception cref="ArgumentException">参数无效时抛出异常</exception>
        public static void Check(BsplineCurveData<XYZW> curve)
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

            if (!ValidationUtils.IsValidNurbs(degree, knotVector.Count, controlPointsCount))
                throw new ArgumentException("参数必须满足: m = n + p + 1", nameof(curve));
        }

        /// <summary>
        /// 计算NURBS曲线上的点
        /// NURBS Book第2版第124页
        /// Algorithm A4.1
        /// </summary>
        /// <param name="curve">NURBS曲线</param>
        /// <param name="paramT">参数t</param>
        /// <returns>曲线上的点</returns>
        public static XYZ GetPointOnCurve(BsplineCurveData<XYZW> curve, double paramT)
        {
            XYZW weightPoint = BsplineCurve.GetPointOnCurve(curve, paramT);
            return weightPoint.ToXYZ(1.0);
        }

        /// <summary>
        /// 计算NURBS曲线的有理导数
        /// NURBS Book第2版第127页
        /// Algorithm A4.2
        /// </summary>
        /// <param name="curve">NURBS曲线</param>
        /// <param name="derivative">导数阶数</param>
        /// <param name="paramT">参数t</param>
        /// <returns>导数值数组</returns>
        public static List<XYZ> ComputeRationalCurveDerivatives(BsplineCurveData<XYZW> curve, int derivative, double paramT)
        {
            List<XYZW> ders = BsplineCurve.ComputeDerivatives(curve, derivative, paramT);
            List<XYZ> derivatives = new List<XYZ>(derivative + 1);
            List<XYZ> Aders = new List<XYZ>(derivative + 1);
            List<double> wders = new List<double>(derivative + 1);

            for (int i = 0; i <= derivative; i++)
            {
                Aders.Add(ders[i].ToXYZ(1.0));
                wders.Add(ders[i].W);
                derivatives.Add(new XYZ());
            }

            for (int k = 0; k <= derivative; k++)
            {
                XYZ v = Aders[k];
                for (int i = 1; i <= k; i++)
                {
                    v = v.Subtract(MathUtils.Binomial(k, i) * wders[i] * derivatives[k - i]);
                }
                derivatives[k] = v.Multiply(1.0 / wders[0]);
            }

            return derivatives;
        }

        /// <summary>
        /// 判断是否可以计算导数
        /// </summary>
        /// <param name="curve">NURBS曲线</param>
        /// <param name="paramT">参数t</param>
        /// <returns>是否可以计算导数</returns>
        public static bool CanComputeDerivative(BsplineCurveData<XYZW> curve, double paramT)
        {
            Check(curve);

            int degree = curve.Degree;
            List<double> knotVector = curve.GetKnotVector();

            if (paramT < knotVector[0] || paramT > knotVector[knotVector.Count - 1])
                return false;

            int multi = Polynomials.GetKnotMultiplicity(knotVector.ToArray(), paramT);
            return multi <= degree;
        }

        /// <summary>
        /// 计算曲线曲率
        /// </summary>
        /// <param name="curve">NURBS曲线</param>
        /// <param name="paramT">参数t</param>
        /// <returns>曲率</returns>
        public static double Curvature(BsplineCurveData<XYZW> curve, double paramT)
        {
            if (!CanComputeDerivative(curve, paramT))
                throw new ArgumentException("不能在参数t处计算导数。", nameof(paramT));

            List<XYZ> derivatives = ComputeRationalCurveDerivatives(curve, 2, paramT);
            XYZ d1 = derivatives[1];
            XYZ d2 = derivatives[2];

            double len = d1.Length();
            if (MathUtils.IsAlmostEqualTo(len, 0.0))
                return 0.0;

            return d1.CrossProduct(d2).Length() / Math.Pow(len, 3);
        }

        /// <summary>
        /// 计算曲线扭率
        /// </summary>
        /// <param name="curve">NURBS曲线</param>
        /// <param name="paramT">参数t</param>
        /// <returns>扭率</returns>
        public static double Torsion(BsplineCurveData<XYZW> curve, double paramT)
        {
            if (!CanComputeDerivative(curve, paramT))
                throw new ArgumentException("不能在参数t处计算导数。", nameof(paramT));

            List<XYZ> derivatives = ComputeRationalCurveDerivatives(curve, 3, paramT);
            XYZ d1 = derivatives[1];
            XYZ d2 = derivatives[2];
            XYZ d3 = derivatives[3];

            XYZ c = d1.CrossProduct(d2);
            double lenC = c.Length();

            if (MathUtils.IsAlmostEqualTo(lenC, 0.0))
                return 0.0;

            return c.DotProduct(d3) / lenC / lenC;
        }

        /// <summary>
        /// 插入节点
        /// NURBS Book第2版第151页
        /// Algorithm A5.1
        /// </summary>
        /// <param name="curve">NURBS曲线</param>
        /// <param name="insertKnot">插入的节点</param>
        /// <param name="times">插入次数</param>
        /// <param name="result">结果曲线</param>
        /// <returns>插入节点的跨度索引</returns>
        public static int InsertKnot(BsplineCurveData<XYZW> curve, double insertKnot, int times, out BsplineCurveData<XYZW> result)
        {
            Check(curve);

            int degree = curve.Degree;
            List<double> knotVector = curve.GetKnotVector();
            List<XYZW> controlPoints = curve.GetControlPoints();

            if (times <= 0)
                throw new ArgumentException("插入次数必须大于零。", nameof(times));

            if (insertKnot < knotVector[0] || insertKnot > knotVector[knotVector.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(insertKnot), "插入节点必须在节点向量的范围内。");

            int multi = Polynomials.GetKnotMultiplicity(knotVector.ToArray(), insertKnot);
            if (multi + times > degree)
                throw new ArgumentException($"插入次数必须满足: 重复度 + 插入次数 <= 次数。当前重复度: {multi}, 插入次数: {times}, 次数: {degree}", nameof(times));

            int span = Polynomials.GetKnotSpanIndex(degree, knotVector.ToArray(), insertKnot);
            List<XYZW> newControlPoints = new List<XYZW>(controlPoints.Count + times);
            for (int i = 0; i <= span - degree; i++)
            {
                newControlPoints.Add(controlPoints[i]);
            }

            List<XYZW> tempPoints = new List<XYZW>(degree + 1);
            for (int i = 0; i <= degree; i++)
            {
                tempPoints.Add(controlPoints[span - degree + i]);
            }

            List<double> newKnotVector = new List<double>(knotVector.Count + times);
            for (int i = 0; i <= span; i++)
            {
                newKnotVector.Add(knotVector[i]);
            }

            for (int i = 1; i <= times; i++)
            {
                newKnotVector.Add(insertKnot);
            }

            for (int i = span + 1; i < knotVector.Count; i++)
            {
                newKnotVector.Add(knotVector[i]);
            }

            double[] alpha = new double[degree - multi + 1];
            for (int j = 1; j <= degree - multi; j++)
            {
                alpha[j] = (insertKnot - knotVector[span + 1 - j]) / (knotVector[span + j] - knotVector[span + 1 - j]);
            }

            int L = 0;
            for (int j = 1; j <= times; j++)
            {
                L = span - degree + j;
                for (int i = 0; i <= degree - j - multi; i++)
                {
                    XYZW temp = tempPoints[i].Multiply(alpha[i + 1]).Add(tempPoints[i + 1].Multiply(1.0 - alpha[i + 1]));
                    tempPoints[i] = temp;
                }
                newControlPoints.Add(tempPoints[0]);
                tempPoints.RemoveAt(0);
            }

            for (int i = L + 1; i < controlPoints.Count; i++)
            {
                newControlPoints.Add(controlPoints[i]);
            }

            result = new BsplineCurveData<XYZW>(degree, newControlPoints, newKnotVector);
            return span;
        }

        /// <summary>
        /// 使用角切法计算NURBS曲线上的点
        /// NURBS Book第2版第155页
        /// Algorithm A5.2
        /// </summary>
        /// <param name="curve">NURBS曲线</param>
        /// <param name="paramT">参数t</param>
        /// <returns>曲线上的点</returns>
        public static XYZ GetPointOnCurveByCornerCut(BsplineCurveData<XYZW> curve, double paramT)
        {
            Check(curve);

            BsplineCurveData<XYZW> tempCurve;
            InsertKnot(curve, paramT, curve.Degree, out tempCurve);
            int span = Polynomials.GetKnotSpanIndex(tempCurve.Degree, tempCurve.GetKnotVector().ToArray(), paramT);
            
            return tempCurve[span].ToXYZ(1.0);
        }

        /// <summary>
        /// 均匀细分NURBS曲线
        /// </summary>
        /// <param name="curve">NURBS曲线</param>
        /// <param name="tessellatedPoints">细分后的点</param>
        /// <param name="correspondingKnots">对应的节点</param>
        public static void EquallyTessellate(BsplineCurveData<XYZW> curve, out List<XYZ> tessellatedPoints, out List<double> correspondingKnots)
        {
            Check(curve);

            int degree = curve.Degree;
            List<double> knotVector = curve.GetKnotVector();
            
            tessellatedPoints = new List<XYZ>();
            correspondingKnots = new List<double>();

            // 获取唯一节点值
            List<double> uniqueKv = new List<double>();
            foreach (double knot in knotVector)
            {
                if (!uniqueKv.Contains(knot))
                {
                    uniqueKv.Add(knot);
                }
            }

            int size = uniqueKv.Count;
            int num = 100; // 每段的点数

            for (int i = 0; i < size - 1; i++)
            {
                double currentU = uniqueKv[i];
                double nextU = uniqueKv[i + 1];

                double step = (nextU - currentU) / (num - 1);
                for (int j = 0; j < num; j++)
                {
                    double u = currentU + step * j;
                    correspondingKnots.Add(u);
                    tessellatedPoints.Add(GetPointOnCurve(curve, u));
                }
            }
        }

        /// <summary>
        /// 判断曲线是否闭合
        /// </summary>
        /// <param name="curve">NURBS曲线</param>
        /// <returns>是否闭合</returns>
        public static bool IsClosed(BsplineCurveData<XYZW> curve)
        {
            Check(curve);

            List<double> knotVector = curve.GetKnotVector();
            double first = knotVector[0];
            double end = knotVector[knotVector.Count - 1];

            XYZ startPoint = GetPointOnCurve(curve, first);
            XYZ endPoint = GetPointOnCurve(curve, end);

            // 检查起点和终点是否重合
            double distance = startPoint.Distance(endPoint);
            if (MathUtils.IsAlmostEqualTo(distance, 0.0))
                return true;

            // 检查控制点是否构成闭合回路
            List<XYZW> controlPoints = curve.GetControlPoints();
            int n = controlPoints.Count - 1;
            XYZ last = controlPoints[n].ToXYZ(1.0);

            bool flag = false;
            int index = 0;
            for (int i = 0; i < n; i++)
            {
                XYZ current = controlPoints[i].ToXYZ(1.0);
                if (last.IsAlmostEqualTo(current))
                {
                    index = i;
                    flag = true;
                    break;
                }
            }

            if (!flag) return false;
            if (index == 0) return true;
            
            for (int i = index; i >= 0; i--)
            {
                XYZ current = controlPoints[i].ToXYZ(1.0);
                XYZ another = controlPoints[n - index + i].ToXYZ(1.0);
                if (!another.IsAlmostEqualTo(current))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断NURBS曲线是否线性
        /// </summary>
        /// <param name="curve">NURBS曲线</param>
        /// <returns>是否线性</returns>
        public static bool IsLinear(BsplineCurveData<XYZW> curve)
        {
            Check(curve);

            List<XYZW> controlPoints = curve.GetControlPoints();
            List<double> knotVector = curve.GetKnotVector();

            int size = controlPoints.Count;
            if (size == 2)
                return true;
                
            XYZ start = controlPoints[0].ToXYZ(1.0);
            XYZ end = controlPoints[size - 1].ToXYZ(1.0);
            
            if (start.Subtract(end).IsAlmostEqualTo(new XYZ(0, 0, 0)))
                return false;
                
            XYZ dir = start.Subtract(end).Normalize();
            
            // 检查所有控制点是否共线
            for (int i = 0; i < size - 1; i++)
            {
                XYZ current = controlPoints[i].ToXYZ(1.0);
                XYZ next = controlPoints[i + 1].ToXYZ(1.0);
                if (!current.Subtract(next).Normalize().IsAlmostEqualTo(dir))
                {
                    return false;
                }
            }

            // 检查内部节点对应的点是否在直线上
            Dictionary<double, int> map = KnotVectorUtils.GetInternalKnotMultiplicityMap(knotVector);
            foreach (var item in map)
            {
                double u = item.Key;
                XYZ cp = GetPointOnCurve(curve, u);
                XYZ cp2s = cp.Subtract(start).Normalize();
                XYZ cp2e = cp.Subtract(end).Normalize();

                if (!cp2s.IsAlmostEqualTo(dir))
                    return false;
                if (!cp2e.IsAlmostEqualTo(dir.Negate()))
                    return false;
            }
            
            return true;
        }
    }
} 