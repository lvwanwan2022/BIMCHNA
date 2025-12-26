using System;
using LNLib.Mathematics;

namespace LNLib.Geometry.Primitives
{
    /// <summary>
    /// 表示三维平面
    /// </summary>
    public class Plane
    {
        private XYZ _origin;
        private XYZ _normal;

        /// <summary>
        /// 通过点和法向量创建平面
        /// </summary>
        public Plane(XYZ origin, XYZ normal)
        {
            if (normal.IsZero())
                throw new ArgumentException("法向量不能为零向量");
            _origin = origin;
            _normal = normal.Normalize();
        }

        /// <summary>
        /// 通过三点创建平面
        /// </summary>
        public static Plane CreateByThreePoints(XYZ p1, XYZ p2, XYZ p3)
        {
            XYZ v1 = p2.Subtract(p1);
            XYZ v2 = p3.Subtract(p1);
            XYZ normal = v1.CrossProduct(v2);
            if (normal.IsZero())
                throw new ArgumentException("三点共线，无法创建平面");
            return new Plane(p1, normal);
        }

        /// <summary>
        /// 平面上的点
        /// </summary>
        public XYZ Origin
        {
            get => _origin;
            set => _origin = value;
        }

        /// <summary>
        /// 平面的单位法向量
        /// </summary>
        public XYZ Normal
        {
            get => _normal;
            set
            {
                if (value.IsZero())
                    throw new ArgumentException("法向量不能为零向量");
                _normal = value.Normalize();
            }
        }

        /// <summary>
        /// 计算点到平面的有符号距离
        /// </summary>
        public double SignedDistanceTo(XYZ point)
        {
            return _normal.DotProduct(point.Subtract(_origin));
        }

        /// <summary>
        /// 计算点到平面的距离
        /// </summary>
        public double DistanceTo(XYZ point)
        {
            return Math.Abs(SignedDistanceTo(point));
        }

        /// <summary>
        /// 判断点是否在平面上
        /// </summary>
        public bool Contains(XYZ point, double epsilon = Constants.DoubleEpsilon)
        {
            return Math.Abs(SignedDistanceTo(point)) < epsilon;
        }

        /// <summary>
        /// 获取点在平面上的投影
        /// </summary>
        public XYZ ProjectPoint(XYZ point)
        {
            double distance = SignedDistanceTo(point);
            return point.Subtract(_normal.Multiply(distance));
        }

        /// <summary>
        /// 获取向量在平面上的投影
        /// </summary>
        public XYZ ProjectVector(XYZ vector)
        {
            return vector.Subtract(_normal.Multiply(_normal.DotProduct(vector)));
        }

        /// <summary>
        /// 判断平面是否与另一个平面平行
        /// </summary>
        public bool IsParallelTo(Plane other, double epsilon = Constants.DoubleEpsilon)
        {
            return Math.Abs(Math.Abs(_normal.DotProduct(other._normal)) - 1.0) < epsilon;
        }

        /// <summary>
        /// 判断平面是否与另一个平面垂直
        /// </summary>
        public bool IsPerpendicularTo(Plane other, double epsilon = Constants.DoubleEpsilon)
        {
            return Math.Abs(_normal.DotProduct(other._normal)) < epsilon;
        }

        /// <summary>
        /// 获取与另一个平面的交线方向
        /// </summary>
        public XYZ IntersectionDirection(Plane other)
        {
            XYZ direction = _normal.CrossProduct(other._normal);
            if (direction.IsZero())
                throw new ArgumentException("平面平行或重合");
            return direction.Normalize();
        }

        /// <summary>
        /// 获取与另一个平面的交线上的点
        /// </summary>
        public XYZ IntersectionPoint(Plane other)
        {
            XYZ direction = IntersectionDirection(other);
            double d1 = -_normal.DotProduct(_origin);
            double d2 = -other._normal.DotProduct(other._origin);
            double det = _normal.X * other._normal.Y - _normal.Y * other._normal.X;

            if (Math.Abs(det) > Constants.DoubleEpsilon)
            {
                double x = (other._normal.Y * d1 - _normal.Y * d2) / det;
                double y = (_normal.X * d2 - other._normal.X * d1) / det;
                return new XYZ(x, y, 0);
            }

            det = _normal.X * other._normal.Z - _normal.Z * other._normal.X;
            if (Math.Abs(det) > Constants.DoubleEpsilon)
            {
                double x = (other._normal.Z * d1 - _normal.Z * d2) / det;
                double z = (_normal.X * d2 - other._normal.X * d1) / det;
                return new XYZ(x, 0, z);
            }

            det = _normal.Y * other._normal.Z - _normal.Z * other._normal.Y;
            if (Math.Abs(det) > Constants.DoubleEpsilon)
            {
                double y = (other._normal.Z * d1 - _normal.Z * d2) / det;
                double z = (_normal.Y * d2 - other._normal.Y * d1) / det;
                return new XYZ(0, y, z);
            }

            throw new ArgumentException("平面平行或重合");
        }

        /// <summary>
        /// 变换平面
        /// </summary>
        public Plane Transform(Matrix4 transform)
        {
            XYZ newOrigin = transform.TransformPoint(_origin);
            XYZ newNormal = transform.TransformVector(_normal);
            return new Plane(newOrigin, newNormal);
        }
    }
} 