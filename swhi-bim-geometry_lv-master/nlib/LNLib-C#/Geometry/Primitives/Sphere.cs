using System;
using System.Collections.Generic;
using LNLib.Mathematics;

namespace LNLib.Geometry.Primitives
{
    /// <summary>
    /// 表示三维空间中的球体
    /// </summary>
    public class Sphere
    {
        private XYZ _center;
        private double _radius;

        /// <summary>
        /// 通过中心点和半径创建球体
        /// </summary>
        /// <param name="center">球心</param>
        /// <param name="radius">半径</param>
        public Sphere(XYZ center, double radius)
        {
            if (radius <= 0)
                throw new ArgumentException("球体半径必须为正数");
            
            _center = center;
            _radius = radius;
        }

        /// <summary>
        /// 球心
        /// </summary>
        public XYZ Center
        {
            get => _center;
            set => _center = value;
        }

        /// <summary>
        /// 球体半径
        /// </summary>
        public double Radius
        {
            get => _radius;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("球体半径必须为正数");
                _radius = value;
            }
        }

        /// <summary>
        /// 球体直径
        /// </summary>
        public double Diameter
        {
            get => 2 * _radius;
            set => Radius = value / 2;
        }

        /// <summary>
        /// 球体表面积
        /// </summary>
        public double SurfaceArea => 4 * Math.PI * _radius * _radius;

        /// <summary>
        /// 球体体积
        /// </summary>
        public double Volume => (4.0 / 3.0) * Math.PI * _radius * _radius * _radius;

        /// <summary>
        /// 判断点是否在球体内（包括表面）
        /// </summary>
        /// <param name="point">待检测点</param>
        /// <param name="epsilon">容差值</param>
        /// <returns>如果点在球体内部或表面上，则返回true</returns>
        public bool Contains(XYZ point, double epsilon = Constants.DoubleEpsilon)
        {
            double distanceSquared = _center.DistanceSquared(point);
            return distanceSquared <= (_radius + epsilon) * (_radius + epsilon);
        }

        /// <summary>
        /// 计算点到球体的最短距离
        /// </summary>
        /// <param name="point">参考点</param>
        /// <returns>点到球体的最短距离。如果点在球体内部，则返回0</returns>
        public double DistanceTo(XYZ point)
        {
            double distance = _center.Distance(point);
            
            // 如果点在球体内部，返回0
            if (distance <= _radius)
                return 0;
            
            // 否则返回点到球面的距离
            return distance - _radius;
        }

        /// <summary>
        /// 计算球体上参数点的坐标
        /// </summary>
        /// <param name="u">经度参数，范围[0, 2π]</param>
        /// <param name="v">纬度参数，范围[0, π]</param>
        /// <returns>球面上的点</returns>
        public XYZ PointAt(double u, double v)
        {
            // 确保u在[0, 2π]范围内
            u = u % (2 * Math.PI);
            if (u < 0) u += 2 * Math.PI;
            
            // 确保v在[0, π]范围内
            v = Math.Max(0, Math.Min(Math.PI, v));
            
            double sinV = Math.Sin(v);
            double x = _radius * Math.Cos(u) * sinV;
            double y = _radius * Math.Sin(u) * sinV;
            double z = _radius * Math.Cos(v);
            
            return _center.Add(new XYZ(x, y, z));
        }
        
        /// <summary>
        /// 计算点在球面上的参数值
        /// </summary>
        /// <param name="point">球面上的点</param>
        /// <returns>表示(u,v)参数的元组，如果点不在球面上则返回null</returns>
        public (double u, double v)? ParameterAt(XYZ point)
        {
            // 检查点是否在球面上
            if (Math.Abs(_center.Distance(point) - _radius) > Constants.DoubleEpsilon)
                return null;
            
            // 转换为相对于球心的坐标
            XYZ relative = point.Subtract(_center);
            
            // 计算球面参数
            double v = Math.Acos(relative.Z / _radius);
            double u = Math.Atan2(relative.Y, relative.X);
            
            // 确保u在[0, 2π]范围内
            if (u < 0) u += 2 * Math.PI;
            
            return (u, v);
        }

        /// <summary>
        /// 计算球面上点的法向量
        /// </summary>
        /// <param name="point">球面上的点</param>
        /// <returns>法向量（单位向量）</returns>
        public XYZ NormalAt(XYZ point)
        {
            // 检查点是否在球面上
            double distance = _center.Distance(point);
            if (Math.Abs(distance - _radius) > Constants.DoubleEpsilon)
                throw new ArgumentException("点不在球面上");
            
            // 从球心指向点的向量即为法向量
            return point.Subtract(_center).Normalize();
        }

        /// <summary>
        /// 获取切割球体的平面
        /// </summary>
        /// <param name="point">球面上的点</param>
        /// <returns>过该点且与球面相切的平面</returns>
        public Plane GetTangentPlane(XYZ point)
        {
            XYZ normal = NormalAt(point);
            return new Plane(point, normal);
        }

        /// <summary>
        /// 计算球体与直线的交点
        /// </summary>
        /// <param name="line">直线</param>
        /// <returns>交点集合，可能包含0、1或2个点</returns>
        public List<XYZ> IntersectWithLine(Line line)
        {
            List<XYZ> intersections = new List<XYZ>();
            
            XYZ rayDirection = line.Direction;
            XYZ linePoint = line.Origin;
            
            // 计算球心到直线的距离
            XYZ centerToLine = linePoint.Subtract(_center);
            
            // 计算二次方程的系数
            double a = rayDirection.DotProduct(rayDirection); // 应该等于1，如果方向是单位向量
            double b = 2 * centerToLine.DotProduct(rayDirection);
            double c = centerToLine.DotProduct(centerToLine) - _radius * _radius;
            
            // 计算判别式
            double discriminant = b * b - 4 * a * c;
            
            if (discriminant < -Constants.DoubleEpsilon)
            {
                // 没有交点
                return intersections;
            }
            
            if (Math.Abs(discriminant) <= Constants.DoubleEpsilon)
            {
                // 一个交点（相切）
                double t = -b / (2 * a);
                intersections.Add(linePoint.Add(rayDirection.Multiply(t)));
                return intersections;
            }
            
            // 两个交点
            double sqrtDiscriminant = Math.Sqrt(discriminant);
            double t1 = (-b - sqrtDiscriminant) / (2 * a);
            double t2 = (-b + sqrtDiscriminant) / (2 * a);
            
            intersections.Add(linePoint.Add(rayDirection.Multiply(t1)));
            intersections.Add(linePoint.Add(rayDirection.Multiply(t2)));
            
            return intersections;
        }

        /// <summary>
        /// 计算球体与射线的交点
        /// </summary>
        /// <param name="ray">射线</param>
        /// <returns>交点集合，可能包含0、1或2个点</returns>
        public List<XYZ> IntersectWithRay(Line ray)
        {
            List<XYZ> lineIntersections = IntersectWithLine(ray);
            List<XYZ> rayIntersections = new List<XYZ>();
            
            // 过滤出射线正方向上的交点
            XYZ rayOrigin = ray.Origin;
            XYZ rayDirection = ray.Direction;
            
            foreach (XYZ point in lineIntersections)
            {
                XYZ v = point.Subtract(rayOrigin);
                if (v.DotProduct(rayDirection) >= -Constants.DoubleEpsilon)
                {
                    rayIntersections.Add(point);
                }
            }
            
            return rayIntersections;
        }

        /// <summary>
        /// 计算球体与平面的交线（圆）
        /// </summary>
        /// <param name="plane">平面</param>
        /// <returns>交线圆，如果不相交则返回null</returns>
        public Circle? IntersectWithPlane(Plane plane)
        {
            // 计算球心到平面的距离
            double distance = plane.DistanceTo(_center);
            
            // 如果距离大于半径，则不相交
            if (distance > _radius + Constants.DoubleEpsilon)
                return null;
            
            // 如果距离等于半径，则只有一个点相交
            if (Math.Abs(distance - _radius) <= Constants.DoubleEpsilon)
            {
                // 在这里我们返回一个半径为0的圆（退化为点）
                XYZ point = _center.Add(plane.Normal.Multiply(-distance));
                return new Circle(point, plane.Normal, 0);
            }
            
            // 计算球心在平面上的投影点
            XYZ projectionCenter = plane.ProjectPoint(_center);
            
            // 计算交圆的半径 (使用毕达哥拉斯定理)
            double circleRadius = Math.Sqrt(_radius * _radius - distance * distance);
            
            // 创建交圆
            return new Circle(projectionCenter, plane.Normal, circleRadius);
        }

        /// <summary>
        /// 计算球体与另一个球体的交线（圆）
        /// </summary>
        /// <param name="other">另一个球体</param>
        /// <returns>交线圆，如果不相交则返回null</returns>
        public Circle? IntersectWithSphere(Sphere other)
        {
            // 计算两个球心的距离
            double centerDistance = _center.Distance(other._center);
            
            // 检查是否有交点
            double sumRadii = _radius + other._radius;
            double diffRadii = Math.Abs(_radius - other._radius);
            
            // 如果两球心距离大于半径之和，则不相交
            if (centerDistance > sumRadii + Constants.DoubleEpsilon)
                return null;
            
            // 如果一个球体包含另一个球体，则不相交
            if (centerDistance < diffRadii - Constants.DoubleEpsilon)
                return null;
            
            // 如果两个球体相切，则只有一个点相交
            if (Math.Abs(centerDistance - sumRadii) <= Constants.DoubleEpsilon || 
                Math.Abs(centerDistance - diffRadii) <= Constants.DoubleEpsilon)
            {
                // 计算相切点
                XYZ direction = other._center.Subtract(_center).Normalize();
                XYZ point;
                
                if (Math.Abs(centerDistance - sumRadii) <= Constants.DoubleEpsilon)
                    point = _center.Add(direction.Multiply(_radius));
                else
                    point = _center.Add(direction.Multiply(_radius * Math.Sign(_radius - other._radius)));
                
                // 返回一个半径为0的圆（退化为点）
                return new Circle(point, direction, 0);
            }
            
            // 计算交圆的平面
            // 使用球心连线的垂直平分面
            double d = (_radius * _radius - other._radius * other._radius + centerDistance * centerDistance) / (2 * centerDistance);
            XYZ centerDirection = other._center.Subtract(_center).Normalize();
            XYZ planeCenter = _center.Add(centerDirection.Multiply(d));
            
            // 计算交圆的半径
            double r = Math.Sqrt(_radius * _radius - d * d);
            
            // 创建交圆
            return new Circle(planeCenter, centerDirection, r);
        }

        /// <summary>
        /// 变换球体
        /// </summary>
        /// <param name="transform">变换矩阵</param>
        /// <returns>变换后的新球体</returns>
        public Sphere Transform(Matrix4 transform)
        {
            // 变换球心
            XYZ newCenter = transform.TransformPoint(_center);
            
            // 计算半径的缩放
            // 注意：如果变换不是均匀缩放，可能会导致球体变为椭球体
            // 这里我们使用最大缩放因子作为新半径的依据
            XYZ scaleVector = transform.TransformVector(new XYZ(1, 0, 0));
            double scaleX = scaleVector.Length();
            
            scaleVector = transform.TransformVector(new XYZ(0, 1, 0));
            double scaleY = scaleVector.Length();
            
            scaleVector = transform.TransformVector(new XYZ(0, 0, 1));
            double scaleZ = scaleVector.Length();
            
            double maxScale = Math.Max(Math.Max(scaleX, scaleY), scaleZ);
            double newRadius = _radius * maxScale;
            
            return new Sphere(newCenter, newRadius);
        }

        /// <summary>
        /// 生成球面上的网格点
        /// </summary>
        /// <param name="uDivisions">经度方向的分段数</param>
        /// <param name="vDivisions">纬度方向的分段数</param>
        /// <returns>球面上的点集</returns>
        public List<XYZ> GenerateMeshPoints(int uDivisions, int vDivisions)
        {
            if (uDivisions < 3 || vDivisions < 2)
                throw new ArgumentException("分段数不足，经度至少需要3段，纬度至少需要2段");
            
            List<XYZ> points = new List<XYZ>();
            
            for (int j = 0; j <= vDivisions; j++)
            {
                double v = j * Math.PI / vDivisions;
                
                for (int i = 0; i < uDivisions; i++)
                {
                    double u = i * 2 * Math.PI / uDivisions;
                    points.Add(PointAt(u, v));
                }
            }
            
            return points;
        }

        /// <summary>
        /// 生成球面的多边形网格
        /// </summary>
        /// <param name="uDivisions">经度方向的分段数</param>
        /// <param name="vDivisions">纬度方向的分段数</param>
        /// <returns>表示球面的多边形集合</returns>
        public List<Polygon> GenerateMeshFaces(int uDivisions, int vDivisions)
        {
            if (uDivisions < 3 || vDivisions < 2)
                throw new ArgumentException("分段数不足，经度至少需要3段，纬度至少需要2段");
            
            List<XYZ> points = GenerateMeshPoints(uDivisions, vDivisions);
            List<Polygon> faces = new List<Polygon>();
            
            // 生成面片
            for (int j = 0; j < vDivisions; j++)
            {
                for (int i = 0; i < uDivisions; i++)
                {
                    int i1 = i;
                    int i2 = (i + 1) % uDivisions;
                    
                    int index1 = j * uDivisions + i1;
                    int index2 = j * uDivisions + i2;
                    int index3 = (j + 1) * uDivisions + i2;
                    int index4 = (j + 1) * uDivisions + i1;
                    
                    // 针对两极附近的退化情况特殊处理
                    if (j == 0) // 南极
                    {
                        faces.Add(new Polygon(new[] { points[index1], points[index2], points[index3] }));
                    }
                    else if (j == vDivisions - 1) // 北极
                    {
                        faces.Add(new Polygon(new[] { points[index1], points[index2], points[index4] }));
                    }
                    else // 中间区域，使用四边形
                    {
                        faces.Add(new Polygon(new[] { points[index1], points[index2], points[index3], points[index4] }));
                    }
                }
            }
            
            return faces;
        }
    }
} 