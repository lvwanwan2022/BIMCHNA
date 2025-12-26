using System;
using LNLib.Mathematics;

namespace LNLib.Geometry.Primitives
{
    /// <summary>
    /// 表示三维空间中的圆
    /// </summary>
    public class Circle
    {
        private XYZ _center;
        private XYZ _normal;
        private double _radius;
        private XYZ _xAxis;

        /// <summary>
        /// 通过圆心、法向量和半径创建圆
        /// </summary>
        public Circle(XYZ center, XYZ normal, double radius)
        {
            if (normal.IsZero())
                throw new ArgumentException("法向量不能为零向量");
            if (radius <= 0)
                throw new ArgumentException("半径必须为正数");

            _center = center;
            _normal = normal.Normalize();
            _radius = radius;
            _xAxis = XYZ.CreateRandomOrthogonal(_normal);
        }

        /// <summary>
        /// 通过三点创建圆
        /// </summary>
        public static Circle CreateByThreePoints(XYZ p1, XYZ p2, XYZ p3)
        {
            // 计算法向量
            XYZ v1 = p2.Subtract(p1);
            XYZ v2 = p3.Subtract(p1);
            XYZ normal = v1.CrossProduct(v2);
            if (normal.IsZero())
                throw new ArgumentException("三点共线，无法创建圆");

            // 计算圆心
            double a = v1.SqrLength();
            double b = v1.DotProduct(v2);
            double c = v2.SqrLength();
            double d = 2 * (a * c - b * b);
            if (Math.Abs(d) < Constants.DoubleEpsilon)
                throw new ArgumentException("三点共线，无法创建圆");

            double u = (c * (a - b) + b * (b - c)) / d;
            double v = (a * (c - b) + b * (b - a)) / d;
            XYZ center = p1.Add(v1.Multiply(u)).Add(v2.Multiply(v));

            // 计算半径
            double radius = center.Distance(p1);
            return new Circle(center, normal, radius);
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
        /// 圆的局部坐标系X轴
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
        /// 圆的局部坐标系Y轴
        /// </summary>
        public XYZ YAxis => _normal.CrossProduct(_xAxis);

        /// <summary>
        /// 圆的周长
        /// </summary>
        public double Circumference => 2.0 * Constants.Pi * _radius;

        /// <summary>
        /// 圆的面积
        /// </summary>
        public double Area => Constants.Pi * _radius * _radius;

        /// <summary>
        /// 获取参数t对应的点，t在[0,1]区间表示一周
        /// </summary>
        public XYZ PointAt(double t)
        {
            double angle = t * Constants.TwoPi;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return _center.Add(_xAxis.Multiply(_radius * cos)).Add(YAxis.Multiply(_radius * sin));
        }

        /// <summary>
        /// 计算点到圆的最短距离
        /// </summary>
        public double DistanceTo(XYZ point)
        {
            // 先将点投影到圆所在平面
            Plane plane = new Plane(_center, _normal);
            XYZ projection = plane.ProjectPoint(point);
            
            // 计算投影点到圆心的距离
            double distance = projection.Distance(_center);
            
            // 计算投影点到圆周的距离
            double radialDistance = Math.Abs(distance - _radius);
            
            // 计算点到投影点的距离
            double heightDistance = point.Distance(projection);
            
            // 返回两个距离的平方和的平方根
            return Math.Sqrt(radialDistance * radialDistance + heightDistance * heightDistance);
        }

        /// <summary>
        /// 判断点是否在圆上
        /// </summary>
        public bool Contains(XYZ point, double epsilon = Constants.DoubleEpsilon)
        {
            return DistanceTo(point) < epsilon;
        }

        /// <summary>
        /// 变换圆
        /// </summary>
        public Circle Transform(Matrix4 transform)
        {
            XYZ newCenter = transform.TransformPoint(_center);
            XYZ newNormal = transform.TransformVector(_normal);
            XYZ newXAxis = transform.TransformVector(_xAxis);
            
            // 计算缩放因子
            double scale = newXAxis.Length();
            double newRadius = _radius * scale;

            Circle result = new Circle(newCenter, newNormal, newRadius);
            result.XAxis = newXAxis;
            return result;
        }
    }
} 