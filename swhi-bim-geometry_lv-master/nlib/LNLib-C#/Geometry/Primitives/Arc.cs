using System;
using LNLib.Mathematics;
using LNLib.Geometry.Primitives;

namespace LNLib.Geometry.Primitives
{
    /// <summary>
    /// 表示三维空间中的圆弧
    /// </summary>
    public class Arc
    {
        private XYZ _center;
        private XYZ _normal;
        private double _radius;
        private XYZ _xAxis;
        private double _startAngle;
        private double _endAngle;

        /// <summary>
        /// 通过圆心、法向量、半径、起始角度和终止角度创建圆弧
        /// </summary>
        /// <param name="center">圆心</param>
        /// <param name="normal">法向量</param>
        /// <param name="radius">半径</param>
        /// <param name="startAngle">起始角度（弧度）</param>
        /// <param name="endAngle">终止角度（弧度）</param>
        public Arc(XYZ center, XYZ normal, double radius, double startAngle, double endAngle)
        {
            if (normal.IsZero())
                throw new ArgumentException("法向量不能为零向量");
            if (radius <= 0)
                throw new ArgumentException("半径必须为正数");

            _center = center;
            _normal = normal.Normalize();
            _radius = radius;
            _xAxis = XYZ.CreateRandomOrthogonal(_normal);
            _startAngle = startAngle;
            _endAngle = endAngle;

            // 确保结束角度大于起始角度
            while (_endAngle <= _startAngle)
                _endAngle += Constants.TwoPi;
        }

        /// <summary>
        /// 通过三点创建圆弧
        /// </summary>
        public static Arc CreateByThreePoints(XYZ p1, XYZ p2, XYZ p3)
        {
            // 创建圆
            Circle circle = Circle.CreateByThreePoints(p1, p2, p3);
            
            // 计算三点对应的角度
            XYZ center = circle.Center;
            XYZ normal = circle.Normal;
            XYZ xAxis = circle.XAxis;
            XYZ yAxis = circle.YAxis;
            
            // 计算向量
            XYZ v1 = p1.Subtract(center);
            XYZ v2 = p2.Subtract(center);
            XYZ v3 = p3.Subtract(center);
            
            // 计算向量在xy平面的投影
            double x1 = v1.DotProduct(xAxis);
            double y1 = v1.DotProduct(yAxis);
            double x2 = v2.DotProduct(xAxis);
            double y2 = v2.DotProduct(yAxis);
            double x3 = v3.DotProduct(xAxis);
            double y3 = v3.DotProduct(yAxis);
            
            // 计算角度
            double angle1 = Math.Atan2(y1, x1);
            double angle2 = Math.Atan2(y2, x2);
            double angle3 = Math.Atan2(y3, x3);
            
            // 规范化角度
            if (angle1 < 0) angle1 += Constants.TwoPi;
            if (angle2 < 0) angle2 += Constants.TwoPi;
            if (angle3 < 0) angle3 += Constants.TwoPi;
            
            // 确定起始和终止角度
            double startAngle, endAngle;
            
            // 判断三点的顺序
            double cross = (x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1);
            
            if (cross > 0)
            {
                // 逆时针顺序
                if (angle3 < angle1 && angle3 < angle2)
                    angle3 += Constants.TwoPi;
                if (angle2 < angle1)
                    angle2 += Constants.TwoPi;
                
                if (angle1 <= angle2 && angle2 <= angle3)
                {
                    startAngle = angle1;
                    endAngle = angle3;
                }
                else if (angle2 <= angle3 && angle3 <= angle1)
                {
                    startAngle = angle2;
                    endAngle = angle1;
                }
                else
                {
                    startAngle = angle3;
                    endAngle = angle2;
                }
            }
            else
            {
                // 顺时针顺序
                if (angle1 < angle3 && angle1 < angle2)
                    angle1 += Constants.TwoPi;
                if (angle2 < angle3)
                    angle2 += Constants.TwoPi;
                
                if (angle3 <= angle2 && angle2 <= angle1)
                {
                    startAngle = angle3;
                    endAngle = angle1;
                }
                else if (angle2 <= angle1 && angle1 <= angle3)
                {
                    startAngle = angle2;
                    endAngle = angle3;
                }
                else
                {
                    startAngle = angle1;
                    endAngle = angle2;
                }
            }
            
            return new Arc(center, normal, circle.Radius, startAngle, endAngle);
        }

        /// <summary>
        /// 圆心
        /// </summary>
        public XYZ Center
        {
            get => _center;
            set => _center = value;
        }

        /// <summary>
        /// 单位法向量
        /// </summary>
        public XYZ Normal
        {
            get => _normal;
            set
            {
                if (value.IsZero())
                    throw new ArgumentException("法向量不能为零向量");
                _normal = value.Normalize();
                _xAxis = XYZ.CreateRandomOrthogonal(_normal);
            }
        }

        /// <summary>
        /// 半径
        /// </summary>
        public double Radius
        {
            get => _radius;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("半径必须为正数");
                _radius = value;
            }
        }

        /// <summary>
        /// 圆弧的局部坐标系X轴
        /// </summary>
        public XYZ XAxis
        {
            get => _xAxis;
            set
            {
                if (value.IsZero())
                    throw new ArgumentException("X轴不能为零向量");
                if (Math.Abs(value.DotProduct(_normal)) > Constants.DoubleEpsilon)
                    throw new ArgumentException("X轴必须垂直于法向量");
                _xAxis = value.Normalize();
            }
        }

        /// <summary>
        /// 圆弧的局部坐标系Y轴
        /// </summary>
        public XYZ YAxis => _normal.CrossProduct(_xAxis);

        /// <summary>
        /// 起始角度（弧度）
        /// </summary>
        public double StartAngle
        {
            get => _startAngle;
            set
            {
                _startAngle = value;
                
                // 确保结束角度大于起始角度
                while (_endAngle <= _startAngle)
                    _endAngle += Constants.TwoPi;
            }
        }

        /// <summary>
        /// 终止角度（弧度）
        /// </summary>
        public double EndAngle
        {
            get => _endAngle;
            set
            {
                _endAngle = value;
                
                // 确保结束角度大于起始角度
                while (_endAngle <= _startAngle)
                    _endAngle += Constants.TwoPi;
            }
        }

        /// <summary>
        /// 圆弧的角度范围（弧度）
        /// </summary>
        public double AngleRange => _endAngle - _startAngle;

        /// <summary>
        /// 圆弧长度
        /// </summary>
        public double Length => _radius * AngleRange;

        /// <summary>
        /// 起点
        /// </summary>
        public XYZ StartPoint => PointAt(0);

        /// <summary>
        /// 终点
        /// </summary>
        public XYZ EndPoint => PointAt(1);

        /// <summary>
        /// 获取参数t对应的点，t在[0,1]区间表示从起点到终点
        /// </summary>
        public XYZ PointAt(double t)
        {
            double angle = _startAngle + t * AngleRange;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return _center.Add(_xAxis.Multiply(_radius * cos)).Add(YAxis.Multiply(_radius * sin));
        }

        /// <summary>
        /// 获取参数t对应的切向量，t在[0,1]区间表示从起点到终点
        /// </summary>
        public XYZ TangentAt(double t)
        {
            double angle = _startAngle + t * AngleRange;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return YAxis.Multiply(cos).Subtract(_xAxis.Multiply(sin));
        }

        /// <summary>
        /// 计算点到圆弧的最短距离
        /// </summary>
        public double DistanceTo(XYZ point)
        {
            // 先将点投影到圆弧所在平面
            Plane plane = new Plane(_center, _normal);
            XYZ projection = plane.ProjectPoint(point);
            
            // 计算投影点到圆心的方向向量
            XYZ radialVector = projection.Subtract(_center);
            if (radialVector.IsZero())
            {
                // 投影点在圆心，返回半径
                return _radius;
            }
            
            // 计算投影点的角度
            double x = radialVector.DotProduct(_xAxis);
            double y = radialVector.DotProduct(YAxis);
            double angle = Math.Atan2(y, x);
            if (angle < 0) angle += Constants.TwoPi;
            
            // 检查角度是否在圆弧范围内
            bool isInRange = false;
            double normalizedStartAngle = _startAngle;
            double normalizedEndAngle = _endAngle;
            
            while (normalizedStartAngle > Constants.TwoPi)
            {
                normalizedStartAngle -= Constants.TwoPi;
                normalizedEndAngle -= Constants.TwoPi;
            }
            
            if (normalizedEndAngle <= Constants.TwoPi)
            {
                isInRange = (angle >= normalizedStartAngle && angle <= normalizedEndAngle);
            }
            else
            {
                isInRange = (angle >= normalizedStartAngle || angle <= (normalizedEndAngle - Constants.TwoPi));
            }
            
            if (isInRange)
            {
                // 角度在圆弧范围内，计算径向距离
                double radialDistance = Math.Abs(radialVector.Length() - _radius);
                double heightDistance = point.Distance(projection);
                return Math.Sqrt(radialDistance * radialDistance + heightDistance * heightDistance);
            }
            else
            {
                // 角度不在圆弧范围内，计算到端点的距离
                double distanceToStart = point.Distance(StartPoint);
                double distanceToEnd = point.Distance(EndPoint);
                return Math.Min(distanceToStart, distanceToEnd);
            }
        }

        /// <summary>
        /// 判断点是否在圆弧上
        /// </summary>
        public bool Contains(XYZ point, double epsilon = Constants.DoubleEpsilon)
        {
            return DistanceTo(point) < epsilon;
        }

        /// <summary>
        /// 变换圆弧
        /// </summary>
        public Arc Transform(Matrix4 transform)
        {
            XYZ newCenter = transform.TransformPoint(_center);
            XYZ newNormal = transform.TransformVector(_normal);
            XYZ newXAxis = transform.TransformVector(_xAxis);
            
            // 计算缩放因子
            double scale = newXAxis.Length();
            double newRadius = _radius * scale;

            Arc result = new Arc(newCenter, newNormal, newRadius, _startAngle, _endAngle);
            result.XAxis = newXAxis;
            return result;
        }
    }
} 