using System;
using LNLib.Mathematics;

namespace LNLib.Geometry.Primitives
{
    /// <summary>
    /// 表示三维直线
    /// </summary>
    public class Line
    {
        private XYZ _origin;
        private XYZ _direction;

        /// <summary>
        /// 通过点和方向创建直线
        /// </summary>
        public Line(XYZ origin, XYZ direction)
        {
            if (direction.IsZero())
                throw new ArgumentException("方向向量不能为零向量");
            _origin = origin;
            _direction = direction.Normalize();
        }

        /// <summary>
        /// 通过两点创建直线
        /// </summary>
        public static Line CreateByTwoPoints(XYZ p1, XYZ p2)
        {
            XYZ direction = p2.Subtract(p1);
            if (direction.IsZero())
                throw new ArgumentException("两点重合，无法创建直线");
            return new Line(p1, direction);
        }

        /// <summary>
        /// 直线上的点
        /// </summary>
        public XYZ Origin
        {
            get => _origin;
            set => _origin = value;
        }

        /// <summary>
        /// 直线的单位方向向量
        /// </summary>
        public XYZ Direction
        {
            get => _direction;
            set
            {
                if (value.IsZero())
                    throw new ArgumentException("方向向量不能为零向量");
                _direction = value.Normalize();
            }
        }

        /// <summary>
        /// 获取直线上参数t对应的点
        /// </summary>
        public XYZ PointAt(double t)
        {
            return _origin.Add(_direction.Multiply(t));
        }

        /// <summary>
        /// 计算点到直线的距离
        /// </summary>
        public double DistanceTo(XYZ point)
        {
            XYZ v = point.Subtract(_origin);
            XYZ projection = _direction.Multiply(_direction.DotProduct(v));
            return v.Subtract(projection).Length();
        }

        /// <summary>
        /// 计算点在直线上的投影点
        /// </summary>
        public XYZ ProjectPoint(XYZ point)
        {
            XYZ v = point.Subtract(_origin);
            double t = _direction.DotProduct(v);
            return PointAt(t);
        }

        /// <summary>
        /// 判断点是否在直线上
        /// </summary>
        public bool Contains(XYZ point, double epsilon = Constants.DoubleEpsilon)
        {
            return DistanceTo(point) < epsilon;
        }

        /// <summary>
        /// 判断直线是否与另一条直线平行
        /// </summary>
        public bool IsParallelTo(Line other, double epsilon = Constants.DoubleEpsilon)
        {
            return Math.Abs(Math.Abs(_direction.DotProduct(other._direction)) - 1.0) < epsilon;
        }

        /// <summary>
        /// 判断直线是否与另一条直线垂直
        /// </summary>
        public bool IsPerpendicularTo(Line other, double epsilon = Constants.DoubleEpsilon)
        {
            return Math.Abs(_direction.DotProduct(other._direction)) < epsilon;
        }

        /// <summary>
        /// 判断直线是否与平面平行
        /// </summary>
        public bool IsParallelTo(Plane plane, double epsilon = Constants.DoubleEpsilon)
        {
            return Math.Abs(_direction.DotProduct(plane.Normal)) < epsilon;
        }

        /// <summary>
        /// 判断直线是否与平面垂直
        /// </summary>
        public bool IsPerpendicularTo(Plane plane, double epsilon = Constants.DoubleEpsilon)
        {
            return Math.Abs(Math.Abs(_direction.DotProduct(plane.Normal)) - 1.0) < epsilon;
        }

        /// <summary>
        /// 计算与另一条直线的最短距离
        /// </summary>
        public double DistanceTo(Line other)
        {
            XYZ n = _direction.CrossProduct(other._direction);
            if (n.IsZero())
            {
                // 平行或重合
                return DistanceTo(other._origin);
            }
            XYZ v = other._origin.Subtract(_origin);
            return Math.Abs(v.DotProduct(n)) / n.Length();
        }

        /// <summary>
        /// 计算与平面的交点
        /// </summary>
        public XYZ IntersectWith(Plane plane)
        {
            if (IsParallelTo(plane))
                throw new ArgumentException("直线与平面平行或在平面内");

            double d = plane.Normal.DotProduct(_direction);
            double t = -plane.SignedDistanceTo(_origin) / d;
            return PointAt(t);
        }

        /// <summary>
        /// 变换直线
        /// </summary>
        public Line Transform(Matrix4 transform)
        {
            XYZ newOrigin = transform.TransformPoint(_origin);
            XYZ newDirection = transform.TransformVector(_direction);
            return new Line(newOrigin, newDirection);
        }
    }
} 