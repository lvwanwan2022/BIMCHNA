using System;
using System.Collections.Generic;
using LNLib.Mathematics;
using LNLib.Algorithm;

namespace LNLib.Geometry.Surface
{
    /// <summary>
    /// B样条曲面类
    /// </summary>
    public static class BsplineSurface
    {
        /// <summary>
        /// 验证B样条曲面参数的有效性
        /// </summary>
        /// <param name="surface">B样条曲面</param>
        /// <exception cref="ArgumentException">参数无效时抛出异常</exception>
        public static void Check(NurbsSurfaceData surface)
        {
            int degreeU = surface.DegreeU;
            int degreeV = surface.DegreeV;
            List<double> knotVectorU = surface.GetKnotVectorU();
            List<double> knotVectorV = surface.GetKnotVectorV();
            List<List<XYZW>> controlPoints = surface.GetControlPoints();

            if (degreeU <= 0)
                throw new ArgumentException("U方向次数必须大于零。", nameof(surface));

            if (degreeV <= 0)
                throw new ArgumentException("V方向次数必须大于零。", nameof(surface));

            if (knotVectorU.Count <= 0)
                throw new ArgumentException("U方向节点向量不能为空。", nameof(surface));

            if (!ValidationUtils.IsValidKnotVector(knotVectorU))
                throw new ArgumentException("U方向节点向量必须是非递减的实数序列。", nameof(surface));

            if (knotVectorV.Count <= 0)
                throw new ArgumentException("V方向节点向量不能为空。", nameof(surface));

            if (!ValidationUtils.IsValidKnotVector(knotVectorV))
                throw new ArgumentException("V方向节点向量必须是非递减的实数序列。", nameof(surface));

            if (controlPoints.Count <= 0)
                throw new ArgumentException("控制点列表不能为空。", nameof(surface));

            if (!ValidationUtils.IsValidBspline(degreeU, knotVectorU.Count, controlPoints.Count))
                throw new ArgumentException("U方向参数必须满足: m = n + p + 1", nameof(surface));

            if (!ValidationUtils.IsValidBspline(degreeV, knotVectorV.Count, controlPoints[0].Count))
                throw new ArgumentException("V方向参数必须满足: m = n + p + 1", nameof(surface));
        }

        /// <summary>
        /// 计算B样条曲面上的点
        /// NURBS Book第2版第111页
        /// Algorithm A3.5
        /// </summary>
        /// <param name="surface">B样条曲面</param>
        /// <param name="uv">参数点</param>
        /// <returns>曲面上的点</returns>
        public static XYZW GetPointOnSurface(NurbsSurfaceData surface, UV uv)
        {
            Check(surface);

            int degreeU = surface.DegreeU;
            int degreeV = surface.DegreeV;
            List<double> knotVectorU = surface.GetKnotVectorU();
            List<double> knotVectorV = surface.GetKnotVectorV();
            List<List<XYZW>> controlPoints = surface.GetControlPoints();

            double u = uv.U;
            double v = uv.V;

            if (u < knotVectorU[0] || u > knotVectorU[knotVectorU.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(uv), "U参数必须在U方向节点向量的范围内。");

            if (v < knotVectorV[0] || v > knotVectorV[knotVectorV.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(uv), "V参数必须在V方向节点向量的范围内。");

            // 特殊情况处理，如果参数点正好位于右边界上
            if (Math.Abs(u - knotVectorU[knotVectorU.Count - 1]) < Constants.DoubleEpsilon &&
                Math.Abs(v - knotVectorV[knotVectorV.Count - 1]) < Constants.DoubleEpsilon)
            {
                return controlPoints[controlPoints.Count - 1][controlPoints[0].Count - 1];
            }
            else if (Math.Abs(u - knotVectorU[knotVectorU.Count - 1]) < Constants.DoubleEpsilon)
            {
                int spanVBoundary = Polynomials.GetKnotSpanIndex(degreeV, knotVectorV.ToArray(), v);
                double[] basisFunctionsVBoundary = new double[degreeV + 1];
                Polynomials.BasisFunctions(spanVBoundary, degreeV, knotVectorV.ToArray(), v, basisFunctionsVBoundary);

                XYZW point = new XYZW();
                for (int j = 0; j <= degreeV; j++)
                {
                    point = point.Add(controlPoints[controlPoints.Count - 1][spanVBoundary - degreeV + j].Multiply(basisFunctionsVBoundary[j]));
                }
                return point;
            }
            else if (Math.Abs(v - knotVectorV[knotVectorV.Count - 1]) < Constants.DoubleEpsilon)
            {
                int spanUBoundary = Polynomials.GetKnotSpanIndex(degreeU, knotVectorU.ToArray(), u);
                double[] basisFunctionsUBoundary = new double[degreeU + 1];
                Polynomials.BasisFunctions(spanUBoundary, degreeU, knotVectorU.ToArray(), u, basisFunctionsUBoundary);

                XYZW point = new XYZW();
                for (int i = 0; i <= degreeU; i++)
                {
                    point = point.Add(controlPoints[spanUBoundary - degreeU + i][controlPoints[0].Count - 1].Multiply(basisFunctionsUBoundary[i]));
                }
                return point;
            }

            // 一般情况处理
            int spanU = Polynomials.GetKnotSpanIndex(degreeU, knotVectorU.ToArray(), u);
            int spanV = Polynomials.GetKnotSpanIndex(degreeV, knotVectorV.ToArray(), v);
            double[] basisFunctionsU = new double[degreeU + 1];
            double[] basisFunctionsV = new double[degreeV + 1];
            Polynomials.BasisFunctions(spanU, degreeU, knotVectorU.ToArray(), u, basisFunctionsU);
            Polynomials.BasisFunctions(spanV, degreeV, knotVectorV.ToArray(), v, basisFunctionsV);

            List<XYZW> temp = new List<XYZW>(degreeU + 1);
            for (int i = 0; i <= degreeU; i++)
            {
                XYZW point = new XYZW();
                for (int j = 0; j <= degreeV; j++)
                {
                    point = point.Add(controlPoints[spanU - degreeU + i][spanV - degreeV + j].Multiply(basisFunctionsV[j]));
                }
                temp.Add(point);
            }

            XYZW result = new XYZW();
            for (int i = 0; i <= degreeU; i++)
            {
                result = result.Add(temp[i].Multiply(basisFunctionsU[i]));
            }

            return result;
        }

        /// <summary>
        /// 计算B样条曲面的导数
        /// NURBS Book第2版第114页
        /// Algorithm A3.6
        /// </summary>
        /// <param name="surface">B样条曲面</param>
        /// <param name="derivative">导数阶数</param>
        /// <param name="uv">参数点</param>
        /// <returns>导数值数组</returns>
        public static List<List<XYZW>> ComputeDerivatives(NurbsSurfaceData surface, int derivative, UV uv)
        {
            Check(surface);

            int degreeU = surface.DegreeU;
            int degreeV = surface.DegreeV;
            List<double> knotVectorU = surface.GetKnotVectorU();
            List<double> knotVectorV = surface.GetKnotVectorV();
            List<List<XYZW>> controlPoints = surface.GetControlPoints();

            double u = uv.U;
            double v = uv.V;

            if (u < knotVectorU[0] || u > knotVectorU[knotVectorU.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(uv), "U参数必须在U方向节点向量的范围内。");

            if (v < knotVectorV[0] || v > knotVectorV[knotVectorV.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(uv), "V参数必须在V方向节点向量的范围内。");

            // 初始化结果数组
            List<List<XYZW>> result = new List<List<XYZW>>();
            for (int i = 0; i <= derivative; i++)
            {
                List<XYZW> row = new List<XYZW>();
                for (int j = 0; j <= derivative - i; j++)
                {
                    row.Add(new XYZW());
                }
                result.Add(row);
            }

            int du = Math.Min(derivative, degreeU);
            int dv = Math.Min(derivative, degreeV);

            // 计算U方向和V方向的导数基函数
            int spanU = Polynomials.GetKnotSpanIndex(degreeU, knotVectorU.ToArray(), u);
            int spanV = Polynomials.GetKnotSpanIndex(degreeV, knotVectorV.ToArray(), v);
            double[][] dersU = Polynomials.BasisFunctionsDerivatives(spanU, degreeU, du, knotVectorU.ToArray(), u);
            double[][] dersV = Polynomials.BasisFunctionsDerivatives(spanV, degreeV, dv, knotVectorV.ToArray(), v);

            // 计算导数
            for (int k = 0; k <= du; k++)
            {
                List<XYZW> temp = new List<XYZW>();
                for (int s = 0; s <= degreeV; s++)
                {
                    XYZW point = new XYZW();
                    for (int r = 0; r <= degreeU; r++)
                    {
                        int uIndex = spanU - degreeU + r;
                        int vIndex = spanV - degreeV + s;
                        if (uIndex < 0 || uIndex >= controlPoints.Count || vIndex < 0 || vIndex >= controlPoints[0].Count)
                            continue;
                        point = point.Add(controlPoints[uIndex][vIndex].Multiply(dersU[k][r]));
                    }
                    temp.Add(point);
                }

                int dd = Math.Min(derivative - k, dv);
                for (int l = 0; l <= dd; l++)
                {
                    XYZW point = new XYZW();
                    for (int s = 0; s <= degreeV; s++)
                    {
                        point = point.Add(temp[s].Multiply(dersV[l][s]));
                    }
                    result[k][l] = point;
                }
            }

            return result;
        }

        /// <summary>
        /// 计算B样条曲面的一阶导数
        /// 这是ComputeDerivatives方法的优化版本，仅针对一阶导数
        /// </summary>
        /// <param name="surface">B样条曲面</param>
        /// <param name="uv">参数点</param>
        /// <returns>一阶导数结果矩阵</returns>
        public static XYZW[,] ComputeFirstOrderDerivative(NurbsSurfaceData surface, UV uv)
        {
            Check(surface);

            int degreeU = surface.DegreeU;
            int degreeV = surface.DegreeV;
            List<double> knotVectorU = surface.GetKnotVectorU();
            List<double> knotVectorV = surface.GetKnotVectorV();
            List<List<XYZW>> controlPoints = surface.GetControlPoints();

            double u = uv.U;
            double v = uv.V;

            if (u < knotVectorU[0] || u > knotVectorU[knotVectorU.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(uv), "U参数必须在U方向节点向量的范围内。");

            if (v < knotVectorV[0] || v > knotVectorV[knotVectorV.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(uv), "V参数必须在V方向节点向量的范围内。");

            // 初始化结果数组
            XYZW[,] result = new XYZW[2, 2];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    result[i, j] = new XYZW();
                }
            }

            // 计算U方向和V方向的导数基函数
            int spanU = Polynomials.GetKnotSpanIndex(degreeU, knotVectorU.ToArray(), u);
            int spanV = Polynomials.GetKnotSpanIndex(degreeV, knotVectorV.ToArray(), v);
            double[][] dersU = Polynomials.BasisFunctionsDerivatives(spanU, degreeU, 1, knotVectorU.ToArray(), u);
            double[][] dersV = Polynomials.BasisFunctionsDerivatives(spanV, degreeV, 1, knotVectorV.ToArray(), v);

            // 计算S(u,v)
            for (int i = 0; i <= degreeU; i++)
            {
                for (int j = 0; j <= degreeV; j++)
                {
                    XYZW point = controlPoints[spanU - degreeU + i][spanV - degreeV + j];
                    result[0, 0] = result[0, 0].Add(point.Multiply(dersU[0][i] * dersV[0][j]));
                }
            }

            // 计算Su(u,v)
            for (int i = 0; i <= degreeU; i++)
            {
                XYZW temp = new XYZW();
                for (int j = 0; j <= degreeV; j++)
                {
                    temp = temp.Add(controlPoints[spanU - degreeU + i][spanV - degreeV + j].Multiply(dersV[0][j]));
                }
                result[1, 0] = result[1, 0].Add(temp.Multiply(dersU[1][i]));
            }

            // 计算Sv(u,v)
            for (int j = 0; j <= degreeV; j++)
            {
                XYZW temp = new XYZW();
                for (int i = 0; i <= degreeU; i++)
                {
                    temp = temp.Add(controlPoints[spanU - degreeU + i][spanV - degreeV + j].Multiply(dersU[0][i]));
                }
                result[0, 1] = result[0, 1].Add(temp.Multiply(dersV[1][j]));
            }

            // 计算Suv(u,v)
            for (int i = 0; i <= degreeU; i++)
            {
                for (int j = 0; j <= degreeV; j++)
                {
                    XYZW point = controlPoints[spanU - degreeU + i][spanV - degreeV + j];
                    result[1, 1] = result[1, 1].Add(point.Multiply(dersU[1][i] * dersV[1][j]));
                }
            }

            return result;
        }
    }
} 