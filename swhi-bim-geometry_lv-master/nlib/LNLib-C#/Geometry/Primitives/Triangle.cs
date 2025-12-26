using System;
using LNLib.Mathematics;

namespace LNLib.Geometry.Primitives
{
    /// <summary>
    /// 表示三维空间中的三角形
    /// </summary>
    public class Triangle
    {
        private XYZ _p1;
        private XYZ _p2;
        private XYZ _p3;

        /// <summary>
        /// 通过三个顶点创建三角形
        /// </summary>
        public Triangle(XYZ p1, XYZ p2, XYZ p3)
        {
            XYZ v1 = p2.Subtract(p1);
            XYZ v2 = p3.Subtract(p1);
            XYZ cross = v1.CrossProduct(v2);
            
            if (cross.IsZero())
                throw new ArgumentException("三点共线，无法创建三角形");
            
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;
        }

        /// <summary>
        /// 三角形顶点1
        /// </summary>
        public XYZ P1
        {
            get => _p1;
            set
            {
                XYZ v1 = _p2.Subtract(value);
                XYZ v2 = _p3.Subtract(value);
                XYZ cross = v1.CrossProduct(v2);
                
                if (cross.IsZero())
                    throw new ArgumentException("三点共线，无法构成三角形");
                
                _p1 = value;
            }
        }

        /// <summary>
        /// 三角形顶点2
        /// </summary>
        public XYZ P2
        {
            get => _p2;
            set
            {
                XYZ v1 = value.Subtract(_p1);
                XYZ v2 = _p3.Subtract(_p1);
                XYZ cross = v1.CrossProduct(v2);
                
                if (cross.IsZero())
                    throw new ArgumentException("三点共线，无法构成三角形");
                
                _p2 = value;
            }
        }

        /// <summary>
        /// 三角形顶点3
        /// </summary>
        public XYZ P3
        {
            get => _p3;
            set
            {
                XYZ v1 = _p2.Subtract(_p1);
                XYZ v2 = value.Subtract(_p1);
                XYZ cross = v1.CrossProduct(v2);
                
                if (cross.IsZero())
                    throw new ArgumentException("三点共线，无法构成三角形");
                
                _p3 = value;
            }
        }

        /// <summary>
        /// 三角形的周长
        /// </summary>
        public double Perimeter => _p1.Distance(_p2) + _p2.Distance(_p3) + _p3.Distance(_p1);

        /// <summary>
        /// 三角形的面积
        /// </summary>
        public double Area
        {
            get
            {
                XYZ v1 = _p2.Subtract(_p1);
                XYZ v2 = _p3.Subtract(_p1);
                return 0.5 * v1.CrossProduct(v2).Length();
            }
        }

        /// <summary>
        /// 三角形的法向量
        /// </summary>
        public XYZ Normal
        {
            get
            {
                XYZ v1 = _p2.Subtract(_p1);
                XYZ v2 = _p3.Subtract(_p1);
                return v1.CrossProduct(v2).Normalize();
            }
        }

        /// <summary>
        /// 三角形的质心
        /// </summary>
        public XYZ Centroid => new XYZ(
            (_p1.X + _p2.X + _p3.X) / 3.0,
            (_p1.Y + _p2.Y + _p3.Y) / 3.0,
            (_p1.Z + _p2.Z + _p3.Z) / 3.0);

        /// <summary>
        /// 获取参数(u,v)对应的点，u和v都在[0,1]区间
        /// 使用重心坐标：P = (1-u-v)*P1 + u*P2 + v*P3
        /// </summary>
        public XYZ PointAt(double u, double v)
        {
            if (u < 0 || v < 0 || u + v > 1)
                throw new ArgumentException("参数u和v必须都大于等于0且u+v小于等于1");
            
            double w = 1 - u - v;
            return _p1.Multiply(w).Add(_p2.Multiply(u)).Add(_p3.Multiply(v));
        }

        /// <summary>
        /// 计算点到三角形的最短距离
        /// </summary>
        public double DistanceTo(XYZ point)
        {
            // 先检查点是否在三角形所在平面上
            Plane plane = new Plane(_p1, Normal);
            double distanceToPlane = plane.DistanceTo(point);
            
            // 计算点在三角形平面上的投影
            XYZ projection = plane.ProjectPoint(point);
            
            // 检查投影点是否在三角形内
            if (Contains(projection))
                return distanceToPlane;
            
            // 投影点不在三角形内，计算到最近边的距离
            Line edge1 = Line.CreateByTwoPoints(_p1, _p2);
            Line edge2 = Line.CreateByTwoPoints(_p2, _p3);
            Line edge3 = Line.CreateByTwoPoints(_p3, _p1);
            
            double distance1 = edge1.DistanceTo(projection);
            double distance2 = edge2.DistanceTo(projection);
            double distance3 = edge3.DistanceTo(projection);
            
            double minDistance = Math.Min(distance1, Math.Min(distance2, distance3));
            return Math.Sqrt(minDistance * minDistance + distanceToPlane * distanceToPlane);
        }

        /// <summary>
        /// 判断点是否在三角形上（包括边缘）
        /// </summary>
        public bool Contains(XYZ point, double epsilon = Constants.DoubleEpsilon)
        {
            // 先检查点是否在三角形所在平面上
            Plane plane = new Plane(_p1, Normal);
            if (!plane.Contains(point, epsilon))
                return false;
            
            // 使用重心坐标判断点是否在三角形内
            XYZ v0 = _p2.Subtract(_p1);
            XYZ v1 = _p3.Subtract(_p1);
            XYZ v2 = point.Subtract(_p1);
            
            double d00 = v0.DotProduct(v0);
            double d01 = v0.DotProduct(v1);
            double d11 = v1.DotProduct(v1);
            double d20 = v2.DotProduct(v0);
            double d21 = v2.DotProduct(v1);
            
            double denom = d00 * d11 - d01 * d01;
            if (Math.Abs(denom) < Constants.DoubleEpsilon)
                return false;
            
            double u = (d11 * d20 - d01 * d21) / denom;
            double v = (d00 * d21 - d01 * d20) / denom;
            double w = 1 - u - v;
            
            // 考虑数值误差
            return u >= -epsilon && v >= -epsilon && w >= -epsilon;
        }

        /// <summary>
        /// 计算与直线的交点
        /// </summary>
        public XYZ? IntersectWithLine(Line line)
        {
            Plane plane = new Plane(_p1, Normal);
            
            // 检查直线是否与平面平行
            if (line.IsParallelTo(plane))
                return null;
            
            // 计算直线与平面的交点
            XYZ intersection = line.IntersectWith(plane);
            
            // 检查交点是否在三角形内
            if (Contains(intersection))
                return intersection;
            
            return null;
        }

        /// <summary>
        /// 计算与射线的交点
        /// </summary>
        public XYZ? IntersectWithRay(XYZ origin, XYZ direction)
        {
            Line line = new Line(origin, direction);
            XYZ? intersection = IntersectWithLine(line);
            
            if (intersection == null)
                return null;
            
            // 检查交点是否在射线的正方向
            XYZ v = intersection.Value.Subtract(origin);
            if (v.DotProduct(direction) < 0)
                return null;
            
            return intersection;
        }

        /// <summary>
        /// 变换三角形
        /// </summary>
        public Triangle Transform(Matrix4 transform)
        {
            XYZ newP1 = transform.TransformPoint(_p1);
            XYZ newP2 = transform.TransformPoint(_p2);
            XYZ newP3 = transform.TransformPoint(_p3);
            return new Triangle(newP1, newP2, newP3);
        }
    }
} 