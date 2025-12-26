using System;
using System.Collections.Generic;
using LNLib.Mathematics;

namespace LNLib.Geometry.Curve
{
    /// <summary>
    /// Bezier曲线描述类
    /// </summary>
    public class BezierCurveDescriptor
    {
        private BezierCurveData<XYZW> _curveData;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="controlPoints">控制点列表</param>
        public BezierCurveDescriptor(IEnumerable<XYZ> controlPoints)
        {
            if (controlPoints == null)
                throw new ArgumentNullException(nameof(controlPoints));

            List<XYZW> weightedPoints = new List<XYZW>();
            foreach (var point in controlPoints)
            {
                weightedPoints.Add(new XYZW(point, 1.0));
            }

            _curveData = new BezierCurveData<XYZW>(weightedPoints.Count - 1, weightedPoints);
            BezierCurve.Check(_curveData);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="controlPoints">带权重的控制点列表</param>
        public BezierCurveDescriptor(IEnumerable<XYZW> controlPoints)
        {
            if (controlPoints == null)
                throw new ArgumentNullException(nameof(controlPoints));

            List<XYZW> weightedPoints = new List<XYZW>(controlPoints);
            _curveData = new BezierCurveData<XYZW>(weightedPoints.Count - 1, weightedPoints);
            BezierCurve.Check(_curveData);
        }

        /// <summary>
        /// 获取曲线的次数
        /// </summary>
        public int Degree => _curveData.Degree;

        /// <summary>
        /// 获取控制点数量
        /// </summary>
        public int ControlPointCount => _curveData.Count;

        /// <summary>
        /// 通过索引获取控制点
        /// </summary>
        /// <param name="index">控制点索引</param>
        /// <returns>指定索引处的控制点</returns>
        public XYZW GetControlPoint(int index)
        {
            if (index < 0 || index >= _curveData.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            return _curveData[index];
        }

        /// <summary>
        /// 获取所有控制点
        /// </summary>
        /// <returns>控制点列表</returns>
        public IReadOnlyList<XYZW> GetControlPoints()
        {
            return _curveData.GetControlPoints();
        }

        /// <summary>
        /// 设置指定索引处的控制点
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="point">新的控制点</param>
        public void SetControlPoint(int index, XYZW point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));
            
            _curveData.SetControlPoint(index, point);
        }

        /// <summary>
        /// 设置指定索引处的控制点
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="point">新的控制点</param>
        /// <param name="weight">权重</param>
        public void SetControlPoint(int index, XYZ point, double weight = 1.0)
        {
            if (point.IsZero())
                throw new ArgumentException("点坐标不能全为零", nameof(point));
            
            if (weight <= 0)
                throw new ArgumentOutOfRangeException(nameof(weight), "权重必须大于零");
            
            _curveData.SetControlPoint(index, new XYZW(point, weight));
        }

        /// <summary>
        /// 在指定索引处插入控制点
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="point">要插入的控制点</param>
        public void InsertControlPoint(int index, XYZW point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));
            
            _curveData.InsertControlPoint(index, point);
        }

        /// <summary>
        /// 在指定索引处插入控制点
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="point">要插入的控制点</param>
        /// <param name="weight">权重</param>
        public void InsertControlPoint(int index, XYZ point, double weight = 1.0)
        {
            if (point.IsZero())
                throw new ArgumentException("点坐标不能全为零", nameof(point));
            
            if (weight <= 0)
                throw new ArgumentOutOfRangeException(nameof(weight), "权重必须大于零");
            
            _curveData.InsertControlPoint(index, new XYZW(point, weight));
        }

        /// <summary>
        /// 移除指定索引处的控制点
        /// </summary>
        /// <param name="index">要移除的控制点索引</param>
        public void RemoveControlPoint(int index)
        {
            _curveData.RemoveControlPoint(index);
        }

        /// <summary>
        /// 使用伯恩斯坦基函数计算曲线上的点
        /// </summary>
        /// <param name="paramT">参数t，范围[0,1]</param>
        /// <returns>曲线上的点</returns>
        public XYZ GetPointOnCurveByBernstein(double paramT)
        {
            if (paramT < 0 || paramT > 1)
                throw new ArgumentOutOfRangeException(nameof(paramT), "参数t必须在0到1的范围内。");
            
            XYZW result = BezierCurve.GetPointOnCurveByBernstein(_curveData, paramT);
            return result.ProjectToXYZ();
        }

        /// <summary>
        /// 使用de Casteljau算法计算曲线上的点
        /// </summary>
        /// <param name="paramT">参数t，范围[0,1]</param>
        /// <returns>曲线上的点</returns>
        public XYZ GetPointOnCurveByDeCasteljau(double paramT)
        {
            if (paramT < 0 || paramT > 1)
                throw new ArgumentOutOfRangeException(nameof(paramT), "参数t必须在0到1的范围内。");
            
            XYZW result = BezierCurve.GetPointOnCurveByDeCasteljau(_curveData, paramT);
            return result.ProjectToXYZ();
        }

        /// <summary>
        /// 获取曲线上的点
        /// </summary>
        /// <param name="paramT">参数t，范围[0,1]</param>
        /// <returns>曲线上的点</returns>
        public XYZ GetPointOnCurve(double paramT)
        {
            // 默认使用de Casteljau算法，因为它在数值上更稳定
            return GetPointOnCurveByDeCasteljau(paramT);
        }

        /// <summary>
        /// 获取曲线上的多个点
        /// </summary>
        /// <param name="pointCount">要计算的点数量</param>
        /// <returns>曲线上的点列表</returns>
        public List<XYZ> GetPointsOnCurve(int pointCount)
        {
            if (pointCount <= 1)
                throw new ArgumentOutOfRangeException(nameof(pointCount), "点数量必须大于1");
            
            List<XYZ> points = new List<XYZ>();
            
            double step = 1.0 / (pointCount - 1);
            for (int i = 0; i < pointCount; i++)
            {
                double paramT = i * step;
                if (i == pointCount - 1) paramT = 1.0; // 确保最后一个点精确在t=1
                
                points.Add(GetPointOnCurve(paramT));
            }
            
            return points;
        }

        /// <summary>
        /// 划分曲线为两部分
        /// </summary>
        /// <param name="paramT">划分参数，范围[0,1]</param>
        /// <returns>划分后的两条曲线</returns>
        public (BezierCurveDescriptor left, BezierCurveDescriptor right) Split(double paramT)
        {
            if (paramT < 0 || paramT > 1)
                throw new ArgumentOutOfRangeException(nameof(paramT), "参数t必须在0到1的范围内。");

            if (Math.Abs(paramT) < Constants.DoubleEpsilon)
            {
                // 如果t接近0，则左侧为空，右侧为原始曲线
                BezierCurveDescriptor left = new BezierCurveDescriptor(new[] { new XYZW(_curveData[0].WX, _curveData[0].WY, _curveData[0].WZ, _curveData[0].W) });
                BezierCurveDescriptor right = new BezierCurveDescriptor(GetControlPoints());
                return (left, right);
            }
            
            if (Math.Abs(paramT - 1) < Constants.DoubleEpsilon)
            {
                // 如果t接近1，则左侧为原始曲线，右侧为空
                BezierCurveDescriptor left = new BezierCurveDescriptor(GetControlPoints());
                BezierCurveDescriptor right = new BezierCurveDescriptor(new[] { new XYZW(_curveData[Degree].WX, _curveData[Degree].WY, _curveData[Degree].WZ, _curveData[Degree].W) });
                return (left, right);
            }

            int n = Degree;
            List<XYZW[]> q = new List<XYZW[]>();
            
            // 初始化q[0]为原始控制点
            q.Add(new XYZW[n + 1]);
            for (int i = 0; i <= n; i++)
            {
                q[0][i] = _curveData[i];
            }
            
            // 计算中间点
            for (int k = 1; k <= n; k++)
            {
                q.Add(new XYZW[n - k + 1]);
                for (int i = 0; i <= n - k; i++)
                {
                    q[k][i] = q[k - 1][i].Multiply(1 - paramT).Add(q[k - 1][i + 1].Multiply(paramT));
                }
            }
            
            // 构造左侧曲线的控制点
            List<XYZW> leftControlPoints = new List<XYZW>();
            for (int i = 0; i <= n; i++)
            {
                leftControlPoints.Add(q[i][0]);
            }
            
            // 构造右侧曲线的控制点
            List<XYZW> rightControlPoints = new List<XYZW>();
            for (int i = 0; i <= n; i++)
            {
                rightControlPoints.Add(q[n - i][i]);
            }
            
            return (new BezierCurveDescriptor(leftControlPoints), new BezierCurveDescriptor(rightControlPoints));
        }

        /// <summary>
        /// 计算曲线上参数t处的导数
        /// </summary>
        /// <param name="paramT">参数t，范围[0,1]</param>
        /// <returns>曲线在该点的导数向量</returns>
        public XYZ GetDerivative(double paramT)
        {
            if (paramT < 0 || paramT > 1)
                throw new ArgumentOutOfRangeException(nameof(paramT), "参数t必须在0到1的范围内。");
            
            if (Degree < 1)
                return new XYZ(); // 0次曲线的导数为零向量
            
            // 计算一阶导数曲线的控制点
            List<XYZW> derivativeControlPoints = new List<XYZW>();
            for (int i = 0; i < Degree; i++)
            {
                XYZW p0 = _curveData[i];
                XYZW p1 = _curveData[i + 1];
                
                // 计算导数曲线的控制点：n*(P[i+1] - P[i])
                XYZW dp = p1.Subtract(p0).Multiply(Degree);
                derivativeControlPoints.Add(dp);
            }
            
            // 创建导数曲线
            BezierCurveData<XYZW> derivativeCurve = new BezierCurveData<XYZW>(derivativeControlPoints.Count - 1, derivativeControlPoints);
            
            // 计算导数曲线上的点
            XYZW result = BezierCurve.GetPointOnCurveByBernstein(derivativeCurve, paramT);
            return result.ProjectToXYZ();
        }

        /// <summary>
        /// 计算曲线上参数t处的曲率
        /// </summary>
        /// <param name="paramT">参数t，范围[0,1]</param>
        /// <returns>曲线在该点的曲率</returns>
        public double GetCurvature(double paramT)
        {
            if (paramT < 0 || paramT > 1)
                throw new ArgumentOutOfRangeException(nameof(paramT), "参数t必须在0到1的范围内。");
            
            if (Degree < 2)
                return 0.0; // 0次或1次曲线的曲率为零
            
            // 计算一阶导数
            XYZ firstDerivative = GetDerivative(paramT);
            double firstLength = firstDerivative.Length();
            
            if (firstLength < Constants.DoubleEpsilon)
                return 0.0; // 如果一阶导数为零，则曲率为零
            
            // 计算一阶导数曲线的控制点
            List<XYZW> firstDerivativeControlPoints = new List<XYZW>();
            for (int i = 0; i < Degree; i++)
            {
                XYZW p0 = _curveData[i];
                XYZW p1 = _curveData[i + 1];
                
                // 计算导数曲线的控制点：n*(P[i+1] - P[i])
                XYZW dp = p1.Subtract(p0).Multiply(Degree);
                firstDerivativeControlPoints.Add(dp);
            }
            
            // 计算二阶导数曲线的控制点
            List<XYZW> secondDerivativeControlPoints = new List<XYZW>();
            for (int i = 0; i < Degree - 1; i++)
            {
                XYZW p0 = firstDerivativeControlPoints[i];
                XYZW p1 = firstDerivativeControlPoints[i + 1];
                
                // 计算导数曲线的控制点：(n-1)*(P[i+1] - P[i])
                XYZW dp = p1.Subtract(p0).Multiply(Degree - 1);
                secondDerivativeControlPoints.Add(dp);
            }
            
            // 创建二阶导数曲线
            BezierCurveData<XYZW> secondDerivativeCurve = new BezierCurveData<XYZW>(secondDerivativeControlPoints.Count - 1, secondDerivativeControlPoints);
            
            // 计算二阶导数曲线上的点
            XYZW secondDerivativeWeighted = BezierCurve.GetPointOnCurveByBernstein(secondDerivativeCurve, paramT);
            XYZ secondDerivative = secondDerivativeWeighted.ProjectToXYZ();
            
            // 计算曲率
            XYZ crossProduct = firstDerivative.CrossProduct(secondDerivative);
            double numerator = crossProduct.Length();
            double denominator = Math.Pow(firstLength, 3);
            
            return numerator / denominator;
        }

        /// <summary>
        /// 创建一个表示直线段的Bezier曲线
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">终点</param>
        /// <returns>表示直线段的Bezier曲线</returns>
        public static BezierCurveDescriptor CreateLine(XYZ startPoint, XYZ endPoint)
        {
            if (startPoint.IsZero())
                throw new ArgumentException("起点坐标不能全为零", nameof(startPoint));
            
            if (endPoint.IsZero())
                throw new ArgumentException("终点坐标不能全为零", nameof(endPoint));
            
            List<XYZ> controlPoints = new List<XYZ> { startPoint, endPoint };
            return new BezierCurveDescriptor(controlPoints);
        }

        /// <summary>
        /// 创建一个表示圆弧的Bezier曲线
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">终点</param>
        /// <param name="bulge">凸度（弧高与弦长之比的一半）</param>
        /// <returns>表示圆弧的Bezier曲线</returns>
        public static BezierCurveDescriptor CreateArc(XYZ startPoint, XYZ endPoint, double bulge)
        {
            if (startPoint.IsZero())
                throw new ArgumentException("起点坐标不能全为零", nameof(startPoint));
            
            if (endPoint.IsZero())
                throw new ArgumentException("终点坐标不能全为零", nameof(endPoint));
            
            if (Math.Abs(bulge) < Constants.DoubleEpsilon)
                return CreateLine(startPoint, endPoint);
            
            // 计算弦向量和弦长
            XYZ chord = endPoint.Subtract(startPoint);
            double chordLength = chord.Length();
            
            if (chordLength < Constants.DoubleEpsilon)
                throw new ArgumentException("起点和终点不能重合");
            
            // 计算弧高
            double sagitta = Math.Abs(bulge) * chordLength / 2.0;
            
            // 计算法向量
            XYZ normalizedChord = chord.Multiply(1.0 / chordLength);
            XYZ normal;
            
            if (bulge > 0)
            {
                // 计算垂直于弦的向量
                normal = new XYZ(-normalizedChord.Y, normalizedChord.X, 0);
            }
            else
            {
                // 计算垂直于弦的向量，方向相反
                normal = new XYZ(normalizedChord.Y, -normalizedChord.X, 0);
            }
            
            // 计算中间控制点
            XYZ midChord = startPoint.Add(chord.Multiply(0.5));
            XYZ midPoint = midChord.Add(normal.Multiply(sagitta));
            
            // 计算权重
            double theta = 4.0 * Math.Abs(bulge) / (1.0 + 4.0 * bulge * bulge);
            double weight = Math.Cos(theta / 2.0);
            
            // 创建控制点
            XYZW p0 = new XYZW(startPoint, 1.0);
            XYZW p1 = new XYZW(midPoint, weight);
            XYZW p2 = new XYZW(endPoint, 1.0);
            
            return new BezierCurveDescriptor(new[] { p0, p1, p2 });
        }
    }
} 