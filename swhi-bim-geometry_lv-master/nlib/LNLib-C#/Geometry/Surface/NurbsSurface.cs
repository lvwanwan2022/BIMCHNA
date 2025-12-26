using System;
using System.Collections.Generic;
using LNLib.Mathematics;
using LNLib.Algorithm;
using LNLib;

namespace LNLib.Geometry.Surface
{
    /// <summary>
    /// NURBS曲面类
    /// </summary>
    public static class NurbsSurface
    {
        /// <summary>
        /// 验证NURBS曲面参数的有效性
        /// </summary>
        /// <param name="surface">NURBS曲面</param>
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

            if (!ValidationUtils.IsValidNurbs(degreeU, knotVectorU.Count, controlPoints.Count))
                throw new ArgumentException("U方向参数必须满足: m = n + p + 1", nameof(surface));

            if (!ValidationUtils.IsValidNurbs(degreeV, knotVectorV.Count, controlPoints[0].Count))
                throw new ArgumentException("V方向参数必须满足: m = n + p + 1", nameof(surface));
        }

        /// <summary>
        /// 计算NURBS曲面上的点
        /// NURBS Book第2版第134页
        /// Algorithm A4.3
        /// </summary>
        /// <param name="surface">NURBS曲面</param>
        /// <param name="uv">参数点</param>
        /// <returns>曲面上的点</returns>
        public static XYZ GetPointOnSurface(NurbsSurfaceData surface, UV uv)
        {
            XYZW result = BsplineSurface.GetPointOnSurface(surface, uv);
            return result.ToXYZ(1.0);
        }

        /// <summary>
        /// 计算NURBS曲面的有理导数
        /// NURBS Book第2版第137页
        /// Algorithm A4.4
        /// </summary>
        /// <param name="surface">NURBS曲面</param>
        /// <param name="derivative">导数阶数</param>
        /// <param name="uv">参数点</param>
        /// <returns>导数值数组</returns>
        public static List<List<XYZ>> ComputeRationalSurfaceDerivatives(NurbsSurfaceData surface, int derivative, UV uv)
        {
            if (derivative <= 0)
                throw new ArgumentException("导数阶数必须大于零。", nameof(derivative));

            List<double> knotVectorU = surface.GetKnotVectorU();
            List<double> knotVectorV = surface.GetKnotVectorV();

            if (uv.U < knotVectorU[0] || uv.U > knotVectorU[knotVectorU.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(uv), "U参数必须在U方向节点向量的范围内。");

            if (uv.V < knotVectorV[0] || uv.V > knotVectorV[knotVectorV.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(uv), "V参数必须在V方向节点向量的范围内。");

            List<List<XYZ>> derivatives = new List<List<XYZ>>();
            for (int i = 0; i <= derivative; i++)
            {
                List<XYZ> row = new List<XYZ>();
                for (int j = 0; j <= derivative - i; j++)
                {
                    row.Add(new XYZ());
                }
                derivatives.Add(row);
            }

            List<List<XYZW>> ders = BsplineSurface.ComputeDerivatives(surface, derivative, uv);
            List<List<XYZ>> Aders = new List<List<XYZ>>();
            List<List<double>> wders = new List<List<double>>();

            for (int i = 0; i <= derivative; i++)
            {
                List<XYZ> rowA = new List<XYZ>();
                List<double> rowW = new List<double>();
                for (int j = 0; j <= derivative - i; j++)
                {
                    rowA.Add(ders[i][j].ToXYZ(1.0));
                    rowW.Add(ders[i][j].W);
                }
                Aders.Add(rowA);
                wders.Add(rowW);
            }

            for (int k = 0; k <= derivative; k++)
            {
                for (int l = 0; l <= derivative - k; l++)
                {
                    XYZ v = Aders[k][l];
                    for (int j = 1; j <= l; j++)
                    {
                        v = v.Subtract(MathUtils.Binomial(l, j) * wders[0][j] * derivatives[k][l - j]);
                    }

                    for (int i = 1; i <= k; i++)
                    {
                        v = v.Subtract(MathUtils.Binomial(k, i) * wders[i][0] * derivatives[k - i][l]);

                        XYZ v2 = new XYZ();
                        for (int j = 1; j <= l; j++)
                        {
                            v2 = v2.Add(MathUtils.Binomial(l, j) * wders[i][j] * derivatives[k - i][l - j]);
                        }
                        v = v.Subtract(MathUtils.Binomial(k, i) * v2);
                    }
                    derivatives[k][l] = v.Multiply(1.0 / wders[0][0]);
                }
            }
            return derivatives;
        }

        /// <summary>
        /// 计算NURBS曲面的一阶有理导数
        /// 这是ComputeRationalSurfaceDerivatives方法的优化版本，仅针对一阶导数
        /// </summary>
        /// <param name="surface">NURBS曲面</param>
        /// <param name="uv">参数点</param>
        /// <param name="S">曲面上的点</param>
        /// <param name="Su">U方向导数</param>
        /// <param name="Sv">V方向导数</param>
        public static void ComputeRationalSurfaceFirstOrderDerivative(NurbsSurfaceData surface, UV uv, out XYZ S, out XYZ Su, out XYZ Sv)
        {
            List<double> knotVectorU = surface.GetKnotVectorU();
            List<double> knotVectorV = surface.GetKnotVectorV();

            if (uv.U < knotVectorU[0] || uv.U > knotVectorU[knotVectorU.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(uv), "U参数必须在U方向节点向量的范围内。");

            if (uv.V < knotVectorV[0] || uv.V > knotVectorV[knotVectorV.Count - 1])
                throw new ArgumentOutOfRangeException(nameof(uv), "V参数必须在V方向节点向量的范围内。");

            XYZW[,] ders = BsplineSurface.ComputeFirstOrderDerivative(surface, uv);

            XYZ[,] Aders = new XYZ[2, 2];
            double[,] wders = new double[2, 2];

            Aders[0, 0] = ders[0, 0].ToXYZ(1.0);
            wders[0, 0] = ders[0, 0].W;
            Aders[0, 1] = ders[0, 1].ToXYZ(1.0);
            wders[0, 1] = ders[0, 1].W;

            Aders[1, 0] = ders[1, 0].ToXYZ(1.0);
            wders[1, 0] = ders[1, 0].W;
            Aders[1, 1] = ders[1, 1].ToXYZ(1.0);
            wders[1, 1] = ders[1, 1].W;

            double invW = 1.0 / wders[0, 0];
            S = Aders[0, 0].Multiply(invW);
            XYZ v = Aders[0, 1];
            v = v.Subtract(S.Multiply(wders[0, 1]));
            Sv = v.Multiply(invW);
            v = Aders[1, 0];
            v = v.Subtract(S.Multiply(wders[1, 0]));
            Su = v.Multiply(invW);
        }

        /// <summary>
        /// 计算曲面法向量
        /// </summary>
        /// <param name="surface">NURBS曲面</param>
        /// <param name="uv">参数点</param>
        /// <returns>法向量</returns>
        public static XYZ Normal(NurbsSurfaceData surface, UV uv)
        {
            ComputeRationalSurfaceFirstOrderDerivative(surface, uv, out XYZ _, out XYZ Su, out XYZ Sv);
            return Su.CrossProduct(Sv).Normalize();
        }

        /// <summary>
        /// 计算曲面曲率
        /// </summary>
        /// <param name="surface">NURBS曲面</param>
        /// <param name="curvature">曲率类型</param>
        /// <param name="uv">参数点</param>
        /// <returns>曲率值</returns>
        public static double Curvature(NurbsSurfaceData surface, SurfaceCurvature curvature, UV uv)
        {
            List<List<XYZ>> derivatives = ComputeRationalSurfaceDerivatives(surface, 2, uv);
            XYZ Su = derivatives[1][0];
            XYZ Sv = derivatives[0][1];
            XYZ Suu = derivatives[2][0];
            XYZ Svv = derivatives[0][2];
            XYZ Suv = derivatives[1][1];

            XYZ normal = Su.CrossProduct(Sv);
            double normLength = normal.Length();
            if (MathUtils.IsAlmostEqualTo(normLength, 0.0))
                return 0.0;

            normal = normal.Multiply(1.0 / normLength);

            // 计算第一基本形式系数
            double E = Su.DotProduct(Su);
            double F = Su.DotProduct(Sv);
            double G = Sv.DotProduct(Sv);

            // 计算第二基本形式系数
            double L = Suu.DotProduct(normal);
            double M = Suv.DotProduct(normal);
            double N = Svv.DotProduct(normal);

            // 计算主曲率
            if (curvature == SurfaceCurvature.Maximum || curvature == SurfaceCurvature.Minimum ||
                curvature == SurfaceCurvature.Mean || curvature == SurfaceCurvature.Gauss)
            {
                double EG_FF = E * G - F * F;
                if (MathUtils.IsAlmostEqualTo(EG_FF, 0.0))
                    return 0.0;

                double A = (L * G - 2 * M * F + N * E) / (2 * EG_FF);
                double B = Math.Sqrt(Math.Pow(L * G - N * E, 2) + 4 * Math.Pow(M * F - L * G / 2 - N * E / 2, 2)) / (2 * EG_FF);
                double k1 = A + B;
                double k2 = A - B;

                switch (curvature)
                {
                    case SurfaceCurvature.Maximum:
                        return Math.Max(k1, k2);
                    case SurfaceCurvature.Minimum:
                        return Math.Min(k1, k2);
                    case SurfaceCurvature.Mean:
                        return (k1 + k2) / 2;
                    case SurfaceCurvature.Gauss:
                        return k1 * k2;
                    default:
                        throw new ArgumentException("未支持的曲率类型", nameof(curvature));
                }
            }
            else if (curvature == SurfaceCurvature.Abs)
            {
                // 曲面在该点的全曲率 = 第二基本形式的模
                return Math.Sqrt(L * L + 2 * M * M + N * N);
            }
            else if (curvature == SurfaceCurvature.Rms)
            {
                // 曲面在该点的RMS曲率 = 第二基本形式与第一基本形式之比的均方根
                double EG_FF = E * G - F * F;
                if (MathUtils.IsAlmostEqualTo(EG_FF, 0.0))
                    return 0.0;

                return Math.Sqrt((L * L + 2 * M * M + N * N) / EG_FF);
            }
            else
            {
                throw new ArgumentException("未支持的曲率类型", nameof(curvature));
            }
        }
    }

    /// <summary>
    /// 曲面曲率类型
    /// </summary>
    public enum SurfaceCurvature
    {
        /// <summary>
        /// 最大主曲率
        /// </summary>
        Maximum = 0,
        
        /// <summary>
        /// 最小主曲率
        /// </summary>
        Minimum = 1,
        
        /// <summary>
        /// 高斯曲率
        /// </summary>
        Gauss = 2,
        
        /// <summary>
        /// 平均曲率
        /// </summary>
        Mean = 3,
        
        /// <summary>
        /// 绝对曲率
        /// </summary>
        Abs = 4,
        
        /// <summary>
        /// 均方根曲率
        /// </summary>
        Rms = 5
    }

    /// <summary>
    /// 曲面方向
    /// </summary>
    public enum SurfaceDirection
    {
        /// <summary>
        /// 所有方向
        /// </summary>
        All = 0,
        
        /// <summary>
        /// U方向
        /// </summary>
        UDirection = 1,
        
        /// <summary>
        /// V方向
        /// </summary>
        VDirection = 2
    }
} 