using System;
using LNLib.Mathematics;

namespace LNLib.Algorithm
{
    /// <summary>
    /// 曲线相交类型
    /// </summary>
    public enum CurveCurveIntersectionType
    {
        /// <summary>
        /// 相交
        /// </summary>
        Intersecting = 0,
        
        /// <summary>
        /// 平行
        /// </summary>
        Parallel = 1,
        
        /// <summary>
        /// 重合
        /// </summary>
        Coincident = 2,
        
        /// <summary>
        /// 异面
        /// </summary>
        Skew = 3
    }

    /// <summary>
    /// 直线与平面相交类型
    /// </summary>
    public enum LinePlaneIntersectionType
    {
        /// <summary>
        /// 相交
        /// </summary>
        Intersecting = 0,
        
        /// <summary>
        /// 平行
        /// </summary>
        Parallel = 1,
        
        /// <summary>
        /// 在平面上
        /// </summary>
        On = 2
    }

    /// <summary>
    /// 相交计算工具类
    /// </summary>
    public static class Intersection
    {
        /// <summary>
        /// 计算两条射线的交点
        /// </summary>
        /// <param name="point0">第一条射线的起点</param>
        /// <param name="vector0">第一条射线的方向向量</param>
        /// <param name="point1">第二条射线的起点</param>
        /// <param name="vector1">第二条射线的方向向量</param>
        /// <param name="param0">第一条射线的参数</param>
        /// <param name="param1">第二条射线的参数</param>
        /// <param name="intersectPoint">交点</param>
        /// <returns>相交类型</returns>
        public static CurveCurveIntersectionType ComputeRays(XYZ point0, XYZ vector0, XYZ point1, XYZ vector1, out double param0, out double param1, out XYZ intersectPoint)
        {
            param0 = 0.0;
            param1 = 0.0;
            intersectPoint = new XYZ();

            if (vector0.IsZero())
                throw new ArgumentException("向量不能为零向量", nameof(vector0));
            
            if (vector1.IsZero())
                throw new ArgumentException("向量不能为零向量", nameof(vector1));

            XYZ v0 = vector0;
            XYZ v1 = vector1;

            XYZ cross = v0.CrossProduct(v1);
            XYZ diff = point1.Subtract(point0);
            XYZ coinCross = diff.CrossProduct(v1);

            if (cross.IsZero())
            {
                if (coinCross.IsZero())
                {
                    return CurveCurveIntersectionType.Coincident;
                }
                else
                {
                    return CurveCurveIntersectionType.Parallel;
                }
            }

            double crossLength = cross.Length();
            double squareLength = crossLength * crossLength;

            XYZ pd1Cross = diff.CrossProduct(vector1);
            double pd1Dot = pd1Cross.DotProduct(cross);
            param0 = pd1Dot / squareLength;

            XYZ pd2Cross = diff.CrossProduct(vector0);
            double pd2Dot = pd2Cross.DotProduct(cross);
            param1 = pd2Dot / squareLength;

            XYZ rayP0 = point0.Add(vector0.Multiply(param0));
            XYZ rayP1 = point1.Add(vector1.Multiply(param1));

            if (rayP0.IsAlmostEqualTo(rayP1))
            {
                intersectPoint = rayP0;
                return CurveCurveIntersectionType.Intersecting;
            }
            return CurveCurveIntersectionType.Skew;
        }

        /// <summary>
        /// 计算直线与平面的交点
        /// </summary>
        /// <param name="normal">平面法向量</param>
        /// <param name="pointOnPlane">平面上的点</param>
        /// <param name="pointOnLine">直线上的点</param>
        /// <param name="lineDirection">直线方向向量</param>
        /// <param name="intersectPoint">交点</param>
        /// <returns>相交类型</returns>
        public static LinePlaneIntersectionType ComputeLineAndPlane(XYZ normal, XYZ pointOnPlane, XYZ pointOnLine, XYZ lineDirection, out XYZ intersectPoint)
        {
            intersectPoint = new XYZ();
            
            XYZ planeNormal = normal.Normalize();
            XYZ lineDirectionNormal = lineDirection.Normalize();
            XYZ P2L = pointOnLine.Subtract(pointOnPlane);
            XYZ P2LNormal = P2L.Normalize();
            double dot = P2LNormal.DotProduct(planeNormal);
            
            if (Math.Abs(dot) < Constants.DoubleEpsilon)
            {
                return LinePlaneIntersectionType.On;
            }
            
            double angle = planeNormal.AngleTo(lineDirectionNormal);
            if (Math.Abs(angle - Constants.Pi / 2) < Constants.DoubleEpsilon)
            {
                return LinePlaneIntersectionType.Parallel;
            }
            
            double d = -P2L.DotProduct(planeNormal) / lineDirection.DotProduct(planeNormal);
            intersectPoint = lineDirectionNormal.Multiply(d).Add(pointOnLine);
            return LinePlaneIntersectionType.Intersecting;
        }
    }
} 