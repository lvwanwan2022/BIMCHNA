using System;
using System.Collections.Generic;
using LNLib.Mathematics;
using LNLib.Algorithm;

namespace LNLib.Geometry.Curve
{
    /// <summary>
    /// 贝塞尔曲线类
    /// </summary>
    public static class BezierCurve
    {
        /// <summary>
        /// 验证贝塞尔曲线参数的有效性
        /// </summary>
        /// <typeparam name="T">控制点类型</typeparam>
        /// <param name="curve">贝塞尔曲线</param>
        /// <exception cref="ArgumentException">参数无效时抛出异常</exception>
        public static void Check<T>(BezierCurveData<T> curve) where T : class, IWeightable<T>
        {
            int degree = curve.Degree;

            if (degree <= 0)
                throw new ArgumentException("次数必须大于零。", nameof(curve));

            if (curve.Count == 0)
                throw new ArgumentException("控制点列表不能为空。", nameof(curve));

            if (curve.Count != degree + 1)
                throw new ArgumentException($"控制点数量必须等于次数加一。当前次数: {degree}, 控制点数量: {curve.Count}", nameof(curve));
        }

        /// <summary>
        /// 使用伯恩斯坦基函数计算贝塞尔曲线上的点
        /// NURBS Book第2版第22页
        /// Algorithm A1.4
        /// </summary>
        /// <typeparam name="T">控制点类型</typeparam>
        /// <param name="curve">贝塞尔曲线</param>
        /// <param name="paramT">参数t，范围[0,1]</param>
        /// <returns>曲线上的点</returns>
        public static T GetPointOnCurveByBernstein<T>(BezierCurveData<T> curve, double paramT) where T : class, IWeightable<T>, new()
        {
            if (paramT < 0 || paramT > 1)
                throw new ArgumentOutOfRangeException(nameof(paramT), "参数t必须在0到1的范围内。");

            int degree = curve.Degree;
            double[] bernsteinArray = Polynomials.AllBernstein(degree, paramT);
            T result = new T();

            for (int k = 0; k <= degree; k++)
            {
                result = result.Add(curve[k].Multiply(bernsteinArray[k]));
            }

            return result;
        }

        /// <summary>
        /// 使用de Casteljau算法计算贝塞尔曲线上的点
        /// NURBS Book第2版第24页
        /// Algorithm A1.5
        /// </summary>
        /// <typeparam name="T">控制点类型</typeparam>
        /// <param name="curve">贝塞尔曲线</param>
        /// <param name="paramT">参数t，范围[0,1]</param>
        /// <returns>曲线上的点</returns>
        public static T GetPointOnCurveByDeCasteljau<T>(BezierCurveData<T> curve, double paramT) where T : class, IWeightable<T>, new()
        {
            if (paramT < 0 || paramT > 1)
                throw new ArgumentOutOfRangeException(nameof(paramT), "参数t必须在0到1的范围内。");

            int degree = curve.Degree;
            List<T> temp = new List<T>(curve.GetControlPoints());

            for (int k = 1; k <= degree; k++)
            {
                for (int i = 0; i <= degree - k; i++)
                {
                    temp[i] = temp[i].Multiply(1.0 - paramT).Add(temp[i + 1].Multiply(paramT));
                }
            }

            return temp[0];
        }

        /// <summary>
        /// 计算二次有理贝塞尔弧上的点
        /// NURBS Book第2版第291页
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="middlePoint">中间控制点</param>
        /// <param name="endPoint">终点</param>
        /// <param name="paramT">参数t，范围[0,1]</param>
        /// <returns>弧上的点</returns>
        public static XYZ GetPointOnQuadraticArc(XYZW startPoint, XYZW middlePoint, XYZW endPoint, double paramT)
        {
            if (paramT < 0 || paramT > 1)
                throw new ArgumentOutOfRangeException(nameof(paramT), "参数t必须在0到1的范围内。");

            double w0 = startPoint.W;
            XYZ p0 = new XYZ(startPoint.X, startPoint.Y, startPoint.Z);
            
            double w1 = middlePoint.W;
            XYZ p1 = new XYZ(middlePoint.X, middlePoint.Y, middlePoint.Z);
            
            double w2 = endPoint.W;
            XYZ p2 = new XYZ(endPoint.X, endPoint.Y, endPoint.Z);

            double b0 = (1 - paramT) * (1 - paramT) * w0;
            double b1 = 2 * paramT * (1 - paramT) * w1;
            double b2 = paramT * paramT * w2;
            double denominator = b0 + b1 + b2;

            return new XYZ(
                (b0 * p0.X + b1 * p1.X + b2 * p2.X) / denominator,
                (b0 * p0.Y + b1 * p1.Y + b2 * p2.Y) / denominator,
                (b0 * p0.Z + b1 * p1.Z + b2 * p2.Z) / denominator
            );
        }

        /// <summary>
        /// 计算二次有理曲线的中间控制点
        /// NURBS Book第2版第392页
        /// 局部有理二次曲线插值
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="startTangent">起点切向量</param>
        /// <param name="endPoint">终点</param>
        /// <param name="endTangent">终点切向量</param>
        /// <param name="controlPoints">输出的控制点</param>
        /// <returns>是否成功计算</returns>
        public static bool ComputerMiddleControlPointsOnQuadraticCurve(
            XYZ startPoint, 
            XYZ startTangent, 
            XYZ endPoint, 
            XYZ endTangent, 
            out List<XYZW> controlPoints)
        {
            controlPoints = new List<XYZW>();
            
            XYZ chord = endPoint.Subtract(startPoint).Normalize();
            XYZ nST = startTangent.Normalize();
            XYZ nET = endTangent.Normalize();

            // 检查特殊情况
            if (nST.IsAlmostEqualTo(chord) || nST.IsAlmostEqualTo(chord.Negative()))
            {
                if (nST.IsAlmostEqualTo(nET) || nST.IsAlmostEqualTo(nET.Negative()))
                {
                    controlPoints.Add(new XYZW(startPoint.Add(endPoint).Multiply(0.5), 1));
                    return true;
                }
            }

            double alf1 = 0.0;
            double alf2 = 0.0;
            XYZ intersectionPoint = new XYZ();
            double weight = 0.0;

            // 计算两条射线的交点
            var intersectionType = IntersectRays(startPoint, nST, endPoint, nET, out alf1, out alf2, out intersectionPoint);
            
            if (intersectionType == CurveCurveIntersectionType.Intersecting)
            {
                if (startPoint.IsAlmostEqualTo(intersectionPoint) || endPoint.IsAlmostEqualTo(intersectionPoint))
                {
                    // 如果交点是端点之一，则使用中点
                    intersectionPoint = startPoint.Add(endPoint).Multiply(0.5);
                    if (ComputeWeightForRationalQuadraticInterpolation(startPoint, intersectionPoint, endPoint, out weight))
                    {
                        controlPoints.Add(new XYZW(intersectionPoint, weight));
                        return true;
                    }
                    return false;
                }

                if (alf1 > 0.0 && alf2 < 0.0)
                {
                    // 相交案例
                    if (ComputeWeightForRationalQuadraticInterpolation(startPoint, intersectionPoint, endPoint, out weight))
                    {
                        controlPoints.Add(new XYZW(intersectionPoint, weight));
                        return true;
                    }
                    return false;
                }
            }

            // 检查直线段情况
            XYZ SE = endPoint.Subtract(startPoint);
            if (SE.Normalize().IsAlmostEqualTo(startTangent) && SE.Normalize().IsAlmostEqualTo(endTangent))
            {
                intersectionPoint = startPoint.Add(endPoint).Multiply(0.5);
                if (ComputeWeightForRationalQuadraticInterpolation(startPoint, intersectionPoint, endPoint, out weight))
                {
                    controlPoints.Add(new XYZW(intersectionPoint, weight));
                    return true;
                }
                return false;
            }

            double gamma1 = 0.0;
            double gamma2 = 0.0;

            // 计算伽马参数
            if (intersectionType != CurveCurveIntersectionType.Intersecting)
            {
                // 如果射线不相交
                gamma1 = 0.5 * SE.Length();
                gamma2 = gamma1;
            }
            else
            {
                // 如果射线相交
                XYZ SR = intersectionPoint.Subtract(startPoint);
                XYZ ER = intersectionPoint.Subtract(endPoint);

                double theta0 = SR.DotProduct(SE) / (SR.Length() * SE.Length());
                double theta1 = ER.DotProduct(SE) / (ER.Length() * SE.Length());

                double alpha = 2.0 / 3.0;
                gamma1 = 0.5 * SE.Length() / (1.0 + alpha * theta1 + (1 - alpha) * theta0);
                gamma2 = 0.5 * SE.Length() / (1.0 + alpha * theta0 + (1 - alpha) * theta1);
            }

            // 计算控制点
            XYZ R1 = startPoint.Add(startTangent.Multiply(gamma1));
            XYZ R2 = endPoint.Subtract(endTangent.Multiply(gamma2));
            XYZ Qk = R1.Multiply(gamma2).Add(R2.Multiply(gamma1)).Multiply(1.0 / (gamma1 + gamma2));

            double weight1 = 0.0;
            if (!ComputeWeightForRationalQuadraticInterpolation(startPoint, R1, Qk, out weight1))
                return false;

            double weight2 = 0.0;
            if (!ComputeWeightForRationalQuadraticInterpolation(Qk, R2, endPoint, out weight2))
                return false;

            controlPoints.Add(new XYZW(R1, weight1));
            controlPoints.Add(new XYZW(Qk, 1.0));
            controlPoints.Add(new XYZW(R2, weight2));

            return true;
        }

        /// <summary>
        /// 计算二次有理插值的权重
        /// </summary>
        private static bool ComputeWeightForRationalQuadraticInterpolation(XYZ p0, XYZ p1, XYZ p2, out double weight)
        {
            weight = 0.0;

            // 计算点的向量
            XYZ v01 = p1.Subtract(p0);
            XYZ v12 = p2.Subtract(p1);
            XYZ v02 = p2.Subtract(p0);

            // 计算距离
            double d01 = v01.Length();
            double d12 = v12.Length();
            double d02 = v02.Length();

            // 检查共线性
            if (Math.Abs(d01 + d12 - d02) < Constants.DoubleEpsilon)
            {
                // 点在一条直线上
                weight = 1.0;
                return true;
            }

            // 计算夹角余弦值
            double cosTheta = v01.DotProduct(v12) / (d01 * d12);
            
            // 计算权重
            weight = Math.Sqrt((1.0 + cosTheta) / 2.0);
            
            return true;
        }

        /// <summary>
        /// 计算两条射线的交点
        /// </summary>
        private static CurveCurveIntersectionType IntersectRays(
            XYZ p1, XYZ dir1, 
            XYZ p2, XYZ dir2, 
            out double alpha1, 
            out double alpha2, 
            out XYZ intersectionPoint)
        {
            alpha1 = 0.0;
            alpha2 = 0.0;
            intersectionPoint = new XYZ();

            // 归一化方向向量
            XYZ d1 = dir1.Normalize();
            XYZ d2 = dir2.Normalize();

            // 检查是否平行
            XYZ cross = d1.CrossProduct(d2);
            double crossLength = cross.Length();
            
            if (crossLength < Constants.DoubleEpsilon)
            {
                // 检查是否共线
                XYZ v = p2.Subtract(p1);
                XYZ crossV = v.CrossProduct(d1);
                
                if (crossV.Length() < Constants.DoubleEpsilon)
                {
                    // 共线
                    alpha1 = v.DotProduct(d1);
                    alpha2 = 0.0;
                    intersectionPoint = p1.Add(d1.Multiply(alpha1));
                    
                    return CurveCurveIntersectionType.Coincident;
                }
                
                // 平行不共线
                return CurveCurveIntersectionType.Parallel;
            }

            // 计算两条直线间的距离
            XYZ v0 = p2.Subtract(p1);
            XYZ n = cross.Normalize();
            double dist = Math.Abs(v0.DotProduct(n));
            
            if (dist > Constants.DoubleEpsilon)
            {
                // 异面直线
                return CurveCurveIntersectionType.Skew;
            }

            // 相交直线，计算交点
            XYZ v2 = d1.CrossProduct(d2);
            XYZ v3 = v0.CrossProduct(d2);
            
            if (v2.Length() < Constants.DoubleEpsilon)
            {
                return CurveCurveIntersectionType.Skew;
            }
            
            alpha1 = v3.DotProduct(v2) / v2.DotProduct(v2);
            intersectionPoint = p1.Add(d1.Multiply(alpha1));
            
            XYZ v4 = v0.CrossProduct(d1);
            alpha2 = v4.DotProduct(v2) / v2.DotProduct(v2);
            
            return CurveCurveIntersectionType.Intersecting;
        }
    }

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
} 