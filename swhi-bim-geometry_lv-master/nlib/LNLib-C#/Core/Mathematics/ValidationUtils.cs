using System;
using System.Collections.Generic;
using System.Linq;

namespace LNLib.Mathematics
{
    /// <summary>
    /// 验证工具类
    /// </summary>
    public static class ValidationUtils
    {
        /// <summary>
        /// 验证贝塞尔曲线参数
        /// </summary>
        /// <param name="degree">次数</param>
        /// <param name="controlPointsCount">控制点数量</param>
        /// <returns>是否有效</returns>
        public static bool IsValidBezier(int degree, int controlPointsCount)
        {
            return controlPointsCount == degree + 1;
        }

        /// <summary>
        /// The NURBS Book 2nd Edition Page50
        /// 验证节点向量是否是非递减实数序列
        /// </summary>
        /// <param name="knotVector">节点向量</param>
        /// <returns>是否有效</returns>
        public static bool IsValidKnotVector(IList<double> knotVector)
        {
            for (int i = 0; i < knotVector.Count - 1; i++)
            {
                if (knotVector[i] > knotVector[i + 1])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 验证B样条曲线参数
        /// </summary>
        /// <param name="degree">次数</param>
        /// <param name="knotVectorCount">节点向量数量</param>
        /// <param name="controlPointsCount">控制点数量</param>
        /// <returns>是否有效</returns>
        public static bool IsValidBspline(int degree, int knotVectorCount, int controlPointsCount)
        {
            return (knotVectorCount - 1) == (controlPointsCount - 1) + degree + 1;
        }

        /// <summary>
        /// 验证NURBS曲线参数
        /// </summary>
        /// <param name="degree">次数</param>
        /// <param name="knotVectorCount">节点向量数量</param>
        /// <param name="weightedControlPointsCount">带权重控制点数量</param>
        /// <returns>是否有效</returns>
        public static bool IsValidNurbs(int degree, int knotVectorCount, int weightedControlPointsCount)
        {
            return (knotVectorCount - 1) == (weightedControlPointsCount - 1) + degree + 1;
        }

        /// <summary>
        /// 验证降阶操作是否有效
        /// </summary>
        /// <param name="degree">次数</param>
        /// <returns>是否有效</returns>
        public static bool IsValidDegreeReduction(int degree)
        {
            return degree > 1;
        }

        /// <summary>
        /// The NURBS Book 2nd Edition Page185
        /// TOL = dWmin / (1+abs(Pmax))
        /// 计算曲线修改容差
        /// </summary>
        /// <param name="controlPoints">控制点列表</param>
        /// <returns>修改容差</returns>
        public static double ComputeCurveModifyTolerance(IList<XYZW> controlPoints)
        {
            double minWeight = 1.0;
            double maxDistance = 0.0;

            int size = controlPoints.Count;
            for (int i = 0; i < size; i++)
            {
                XYZW temp = controlPoints[i];
                minWeight = Math.Min(minWeight, temp.W);
                maxDistance = Math.Max(maxDistance, temp.ToXYZ(1.0).Length());
            }

            return Constants.DistanceEpsilon * minWeight / (1 + Math.Abs(maxDistance));
        }
    }
} 