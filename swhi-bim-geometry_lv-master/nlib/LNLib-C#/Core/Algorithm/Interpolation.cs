using System;
using System.Collections.Generic;
using System.Linq;
using LNLib.Mathematics;

namespace LNLib.Algorithm
{
    /// <summary>
    /// 插值算法类
    /// </summary>
    public static class Interpolation
    {
        /// <summary>
        /// 获取向量qk
        /// </summary>
        private static XYZ Getqk(IList<XYZ> throughPoints, int index)
        {
            return throughPoints[index].Subtract(throughPoints[index - 1]);
        }

        /// <summary>
        /// 获取参数ak
        /// </summary>
        private static double Getak(XYZ qk_1, XYZ qk, XYZ qk1, XYZ qk2)
        {
            return (qk_1.CrossProduct(qk)).Length() / ((qk_1.CrossProduct(qk).Length()) + (qk1.CrossProduct(qk2)).Length());
        }

        /// <summary>
        /// 获取切向量Tk
        /// </summary>
        private static XYZ GetTk(XYZ qk_1, XYZ qk, XYZ qk1, XYZ qk2)
        {
            double ak = Getak(qk_1, qk, qk1, qk2);
            return qk.Multiply(1 - ak).Add(qk1.Multiply(ak)).Normalize();
        }

        /// <summary>
        /// 获取总弦长
        /// </summary>
        /// <param name="throughPoints">通过点</param>
        /// <returns>总弦长</returns>
        public static double GetTotalChordLength(IList<XYZ> throughPoints)
        {
            int n = throughPoints.Count - 1;
            double length = 0.0;
            for (int i = 1; i <= n; i++)
            {
                length += throughPoints[i].Distance(throughPoints[i - 1]);
            }
            return length;
        }

        /// <summary>
        /// 获取弦长参数化
        /// </summary>
        /// <param name="throughPoints">通过点</param>
        /// <returns>参数化结果</returns>
        public static List<double> GetChordParameterization(IList<XYZ> throughPoints)
        {
            int size = throughPoints.Count;
            int n = size - 1;

            List<double> uk = new List<double>(size);
            for (int i = 0; i < size; i++)
            {
                uk.Add(0.0);
            }
            uk[n] = 1.0;

            double d = GetTotalChordLength(throughPoints);
            for (int i = 1; i <= n - 1; i++)
            {
                uk[i] = uk[i - 1] + (throughPoints[i].Distance(throughPoints[i - 1])) / d;
            }
            return uk;
        }

        /// <summary>
        /// 获取向心长度
        /// </summary>
        /// <param name="throughPoints">通过点</param>
        /// <returns>向心长度</returns>
        public static double GetCentripetalLength(IList<XYZ> throughPoints)
        {
            int size = throughPoints.Count;
            int n = size - 1;

            double length = 0.0;
            for (int i = 1; i <= n; i++)
            {
                length += Math.Sqrt(throughPoints[i].Distance(throughPoints[i - 1]));
            }
            return length;
        }

        /// <summary>
        /// 获取向心参数化
        /// </summary>
        /// <param name="throughPoints">通过点</param>
        /// <returns>参数化结果</returns>
        public static List<double> GetCentripetalParameterization(IList<XYZ> throughPoints)
        {
            int size = throughPoints.Count;
            int n = size - 1;

            List<double> uk = new List<double>(size);
            for (int i = 0; i < size; i++)
            {
                uk.Add(0.0);
            }
            uk[n] = 1.0;

            double d = GetCentripetalLength(throughPoints);
            for (int i = 1; i <= n - 1; i++)
            {
                uk[i] = uk[i - 1] + Math.Sqrt(throughPoints[i].Distance(throughPoints[i - 1])) / d;
            }
            return uk;
        }

        /// <summary>
        /// 计算平均节点向量
        /// </summary>
        /// <param name="degree">次数</param>
        /// <param name="params">参数</param>
        /// <returns>节点向量</returns>
        public static List<double> AverageKnotVector(int degree, IList<double> parameters)
        {
            List<double> uk = new List<double>(parameters);
            int size = parameters.Count;
            int n = size - 1;
            int m = n + degree + 1;

            List<double> knotVector = new List<double>(m + 1);
            for (int i = 0; i <= m; i++)
            {
                knotVector.Add(0.0);
            }

            for (int i = m - degree; i <= m; i++)
            {
                knotVector[i] = 1.0;
            }

            for (int j = 1; j <= n - degree; j++)
            {
                double sum = 0.0;
                for (int i = j; i <= j + degree - 1; i++)
                {
                    sum += uk[i];
                }
                knotVector[j + degree] = (1.0 / degree) * sum;
            }
            return knotVector;
        }

        /// <summary>
        /// 计算节点向量
        /// </summary>
        /// <param name="degree">次数</param>
        /// <param name="pointsCount">点数量</param>
        /// <param name="controlPointsCount">控制点数量</param>
        /// <param name="params">参数</param>
        /// <returns>节点向量</returns>
        public static List<double> ComputeKnotVector(int degree, int pointsCount, int controlPointsCount, IList<double> parameters)
        {
            int m = pointsCount - 1;
            int n = controlPointsCount - 1;
            int nn = n + degree + 2;

            List<double> knotVector = new List<double>(nn);
            for (int i = 0; i < nn; i++)
            {
                knotVector.Add(0.0);
            }

            double d = (double)(m + 1) / (double)(n - degree + 1);
            for (int j = 1; j <= n - degree; j++)
            {
                int i = (int)Math.Floor(j * d);
                double alpha = (j * d) - i;
                knotVector[degree + j] = (1.0 - alpha) * parameters[i - 1] + (alpha * parameters[i]);
            }
            for (int i = 0; i < nn; i++)
            {
                if (i <= degree)
                {
                    knotVector[i] = 0.0;
                }
                else if (i >= nn - 1 - degree)
                {
                    knotVector[i] = 1.0;
                }
            }
            return knotVector;
        }

        /// <summary>
        /// 计算有理二次插值的权重
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="middleControlPoint">中间控制点</param>
        /// <param name="endPoint">终点</param>
        /// <param name="weight">权重</param>
        /// <returns>是否成功</returns>
        public static bool ComputerWeightForRationalQuadraticInterpolation(XYZ startPoint, XYZ middleControlPoint, XYZ endPoint, out double weight)
        {
            weight = 0.0;
            
            XYZ SM = middleControlPoint.Subtract(startPoint);
            XYZ EM = middleControlPoint.Subtract(endPoint);
            XYZ SE = endPoint.Subtract(startPoint);

            if (SM.Normalize().IsAlmostEqualTo(EM.Normalize()) ||
                SM.Normalize().IsAlmostEqualTo(EM.Normalize().Negative()))
            {
                weight = 1.0;
                return true;
            }

            if (Math.Abs(SM.Length() - EM.Length()) < Constants.DoubleEpsilon)
            {
                weight = SM.DotProduct(SE) / (SM.Length() * SE.Length());
                return true;
            }

            XYZ M = startPoint.Add(endPoint).Multiply(0.5);
            XYZ MR = middleControlPoint.Subtract(M);
            double ratio = SM.Length() / SE.Length();
            double frac = 1.0 / (ratio + 1.0);
            XYZ D = endPoint.Add(EM.Multiply(frac));
            XYZ SD = D.Subtract(startPoint);
            XYZ S1;

            double alf1 = 0.0;
            double alf2 = 0.0;

            var type = Intersection.ComputeRays(startPoint, SD.Normalize(), M, MR.Normalize(), out alf1, out alf2, out S1);
            if (type != CurveCurveIntersectionType.Intersecting)
            {
                return false;
            }

            ratio = EM.Length() / SE.Length();
            frac = 1.0 / (ratio + 1.0);
            D = startPoint.Add(SM.Multiply(frac));
            XYZ ED = D.Subtract(endPoint);
            XYZ S2;
            type = Intersection.ComputeRays(endPoint, ED.Normalize(), M, MR.Normalize(), out alf1, out alf2, out S2);
            if (type != CurveCurveIntersectionType.Intersecting)
            {
                return false;
            }

            XYZ S = S1.Add(S2).Multiply(0.5);
            XYZ MS = S.Subtract(M);
            double s = MS.Length() / MR.Length();
            weight = s / (1.0 - s);
            return true;
        }

        /// <summary>
        /// 获取曲面网格参数化
        /// </summary>
        /// <param name="throughPoints">通过点</param>
        /// <param name="paramsU">U方向参数</param>
        /// <param name="paramsV">V方向参数</param>
        /// <returns>是否成功</returns>
        public static bool GetSurfaceMeshParameterization(IList<IList<XYZ>> throughPoints, out List<double> paramsU, out List<double> paramsV)
        {
            int n = throughPoints.Count;
            int m = throughPoints[0].Count;

            double[] cds = new double[Math.Max(n, m)];
            paramsU = new List<double>(n);
            paramsV = new List<double>(m);

            for (int i = 0; i < n; i++)
            {
                paramsU.Add(0.0);
            }

            for (int i = 0; i < m; i++)
            {
                paramsV.Add(0.0);
            }

            int num = m;
            
            for (int l = 0; l < m; l++)
            {
                double total = 0.0;
                for (int k = 1; k < n; k++)
                {
                    cds[k] = throughPoints[k][l].Distance(throughPoints[k - 1][l]);
                    total += cds[k];
                }

                if (Math.Abs(total) < Constants.DoubleEpsilon)
                {
                    num--;
                }
                else
                {
                    double d = 0.0;
                    for (int k = 1; k < n; k++)
                    {
                        d += cds[k];
                        paramsU[k] = paramsU[k] + d / total;
                    }
                }
            }
            if (num == 0)
            {
                return false;
            }

            for (int k = 1; k < n - 1; k++)
            {
                paramsU[k] = paramsU[k] / num;
            }
            paramsU[n - 1] = 1.0;

            num = n;
            
            for (int k = 0; k < n; k++)
            {
                double total = 0.0;
                for (int l = 1; l < m; l++)
                {
                    cds[l] = throughPoints[k][l].Distance(throughPoints[k][l - 1]);
                    total += cds[l];
                }

                if (Math.Abs(total) < Constants.DoubleEpsilon)
                {
                    num--;
                }
                else
                {
                    double d = 0.0;
                    for (int l = 1; l < m; l++)
                    {
                        d += cds[l];
                        paramsV[l] = paramsV[l] + d / total;
                    }
                }
            }
            if (num == 0)
            {
                return false;
            }

            for (int l = 1; l < m - 1; l++)
            {
                paramsV[l] = paramsV[l] / num;
            }
            paramsV[m - 1] = 1.0;

            return true;
        }
    }
} 