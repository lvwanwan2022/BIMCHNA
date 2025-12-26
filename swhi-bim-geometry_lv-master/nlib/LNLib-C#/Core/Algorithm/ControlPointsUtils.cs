using System;
using System.Collections.Generic;
using LNLib.Mathematics;

namespace LNLib.Algorithm
{
    /// <summary>
    /// 控制点操作工具类
    /// </summary>
    public static class ControlPointsUtils
    {
        /// <summary>
        /// 将带权重的控制点转换为普通三维点
        /// </summary>
        /// <param name="weightedControlPoints">带权重的控制点列表</param>
        /// <returns>三维点列表</returns>
        public static List<XYZ> ToXYZ(IList<XYZW> weightedControlPoints)
        {
            List<XYZ> result = new List<XYZ>();
            for (int i = 0; i < weightedControlPoints.Count; i++)
            {
                result.Add(weightedControlPoints[i].ProjectToXYZ());
            }
            return result;
        }

        /// <summary>
        /// 将二维带权重的控制点数组转换为普通三维点二维数组
        /// </summary>
        /// <param name="points">二维带权重的点数组</param>
        /// <returns>三维点二维数组</returns>
        public static List<List<XYZ>> ToXYZ(IList<IList<XYZW>> points)
        {
            int row = points.Count;
            int column = points[0].Count;

            List<List<XYZ>> result = new List<List<XYZ>>(row);
            for (int i = 0; i < row; i++)
            {
                List<XYZ> rowPoints = new List<XYZ>(column);
                for (int j = 0; j < column; j++)
                {
                    rowPoints.Add(points[i][j].ProjectToXYZ());
                }
                result.Add(rowPoints);
            }
            return result;
        }

        /// <summary>
        /// 将二维三维点数组转换为带权重的点二维数组
        /// </summary>
        /// <param name="points">三维点二维数组</param>
        /// <returns>带权重的点二维数组</returns>
        public static List<List<XYZW>> ToXYZW(IList<IList<XYZ>> points)
        {
            int row = points.Count;
            int column = points[0].Count;

            List<List<XYZW>> result = new List<List<XYZW>>(row);
            for (int i = 0; i < row; i++)
            {
                List<XYZW> rowPoints = new List<XYZW>(column);
                for (int j = 0; j < column; j++)
                {
                    rowPoints.Add(new XYZW(points[i][j], 1.0));
                }
                result.Add(rowPoints);
            }
            return result;
        }

        /// <summary>
        /// 将二维带权重的点数组与系数矩阵相乘
        /// </summary>
        /// <param name="points">二维带权重的点数组</param>
        /// <param name="coefficient">系数矩阵</param>
        /// <returns>结果矩阵</returns>
        public static List<List<XYZW>> Multiply(IList<IList<XYZW>> points, IList<IList<double>> coefficient)
        {
            int m = points.Count;
            int n = points[0].Count;
            int p = coefficient[0].Count;

            List<List<XYZW>> result = new List<List<XYZW>>(m);
            for (int i = 0; i < m; i++)
            {
                List<XYZW> rowPoints = new List<XYZW>(p);
                for (int j = 0; j < p; j++)
                {
                    XYZW sum = new XYZW();
                    for (int k = 0; k < n; k++)
                    {
                        sum = sum.Add(points[i][k].Multiply(coefficient[k][j]));
                    }
                    rowPoints.Add(sum);
                }
                result.Add(rowPoints);
            }
            return result;
        }

        /// <summary>
        /// 将系数矩阵与二维带权重的点数组相乘
        /// </summary>
        /// <param name="coefficient">系数矩阵</param>
        /// <param name="points">二维带权重的点数组</param>
        /// <returns>结果矩阵</returns>
        public static List<List<XYZW>> Multiply(IList<IList<double>> coefficient, IList<IList<XYZW>> points)
        {
            int m = coefficient.Count;
            int n = coefficient[0].Count;
            int p = points[0].Count;

            List<List<XYZW>> result = new List<List<XYZW>>(m);
            for (int i = 0; i < m; i++)
            {
                List<XYZW> rowPoints = new List<XYZW>(p);
                for (int j = 0; j < p; j++)
                {
                    XYZW sum = new XYZW();
                    for (int k = 0; k < n; k++)
                    {
                        sum = sum.Add(points[k][j].Multiply(coefficient[i][k]));
                    }
                    rowPoints.Add(sum);
                }
                result.Add(rowPoints);
            }
            return result;
        }
    }
} 