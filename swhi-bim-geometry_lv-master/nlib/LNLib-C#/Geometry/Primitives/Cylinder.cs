using System;
using System.Collections.Generic;
using LNLib.Mathematics;

namespace LNLib.Geometry.Primitives
{
    /// <summary>
    /// 表示三维空间中的圆柱体
    /// </summary>
    public class Cylinder
    {
        private XYZ _baseCenter;  // 底面圆心
        private XYZ _axis;        // 轴向（单位向量）
        private double _radius;   // 半径
        private double _height;   // 高度

        /// <summary>
        /// 通过底面圆心、轴向、半径和高度创建圆柱体
        /// </summary>
        /// <param name="baseCenter">底面圆心</param>
        /// <param name="axis">轴向（会被归一化）</param>
        /// <param name="radius">半径</param>
        /// <param name="height">高度</param>
        public Cylinder(XYZ baseCenter, XYZ axis, double radius, double height)
        {
            if (axis.IsZero())
                throw new ArgumentException("轴向向量不能为零向量");

            if (radius <= 0)
                throw new ArgumentException("圆柱体半径必须为正数");

            if (height <= 0)
                throw new ArgumentException("圆柱体高度必须为正数");

            _baseCenter = baseCenter;
            _axis = axis.Normalize();
            _radius = radius;
            _height = height;
        }

        /// <summary>
        /// 底面圆心
        /// </summary>
        public XYZ BaseCenter
        {
            get => _baseCenter;
            set => _baseCenter = value;
        }

        /// <summary>
        /// 轴向（单位向量）
        /// </summary>
        public XYZ Axis
        {
            get => _axis;
            set
            {
                if (value.IsZero())
                    throw new ArgumentException("轴向向量不能为零向量");
                _axis = value.Normalize();
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
                    throw new ArgumentException("圆柱体半径必须为正数");
                _radius = value;
            }
        }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height
        {
            get => _height;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("圆柱体高度必须为正数");
                _height = value;
            }
        }

        /// <summary>
        /// 顶面圆心
        /// </summary>
        public XYZ TopCenter => _baseCenter.Add(_axis.Multiply(_height));

        /// <summary>
        /// 圆柱体中心点
        /// </summary>
        public XYZ Center => _baseCenter.Add(_axis.Multiply(_height / 2.0));

        /// <summary>
        /// 底面圆
        /// </summary>
        public Circle BaseCircle => new Circle(_baseCenter, _axis, _radius);

        /// <summary>
        /// 顶面圆
        /// </summary>
        public Circle TopCircle => new Circle(TopCenter, _axis, _radius);

        /// <summary>
        /// 圆柱体的体积
        /// </summary>
        public double Volume => Math.PI * _radius * _radius * _height;

        /// <summary>
        /// 圆柱体的表面积
        /// </summary>
        public double SurfaceArea => 2 * Math.PI * _radius * (_radius + _height);

        /// <summary>
        /// 计算圆柱面上的参数点
        /// </summary>
        /// <param name="u">角度参数，范围[0, 2π]</param>
        /// <param name="v">高度参数，范围[0, 高度]</param>
        /// <returns>圆柱面上的点</returns>
        public XYZ PointAt(double u, double v)
        {
            // 确保u在[0, 2π]范围内
            u = u % (2 * Math.PI);
            if (u < 0) u += 2 * Math.PI;
            
            // 确保v在[0, 高度]范围内
            v = Math.Max(0, Math.Min(_height, v));
            
            // 创建局部坐标系
            XYZ xAxis, yAxis;
            CreateLocalCoordinateSystem(_axis, out xAxis, out yAxis);
            
            // 计算点坐标
            XYZ point = _baseCenter.Add(_axis.Multiply(v))  // 沿轴向移动
                                  .Add(xAxis.Multiply(_radius * Math.Cos(u)))  // x方向
                                  .Add(yAxis.Multiply(_radius * Math.Sin(u)));  // y方向
            
            return point;
        }

        /// <summary>
        /// 判断点是否在圆柱体内（包括表面）
        /// </summary>
        /// <param name="point">待检测点</param>
        /// <param name="epsilon">容差值</param>
        /// <returns>如果点在圆柱体内部或表面上，则返回true</returns>
        public bool Contains(XYZ point, double epsilon = Constants.DoubleEpsilon)
        {
            // 计算点到轴线的距离
            Line axisLine = Line.CreateByTwoPoints(_baseCenter, TopCenter);
            double distanceToAxis = axisLine.DistanceTo(point);
            
            if (distanceToAxis > _radius + epsilon)
                return false;
            
            // 检查点是否在两个端面之间
            XYZ vectorToPoint = point.Subtract(_baseCenter);
            double projection = vectorToPoint.DotProduct(_axis);
            
            return projection >= -epsilon && projection <= _height + epsilon;
        }

        /// <summary>
        /// 计算点到圆柱体的最短距离
        /// </summary>
        /// <param name="point">参考点</param>
        /// <returns>点到圆柱体的最短距离。如果点在圆柱体内部，则返回0</returns>
        public double DistanceTo(XYZ point)
        {
            // 如果点在圆柱体内部，返回0
            if (Contains(point))
                return 0;
            
            // 计算点到轴线的投影点
            Line axisLine = Line.CreateByTwoPoints(_baseCenter, TopCenter);
            XYZ projectionOnAxis = axisLine.ProjectPoint(point);
            
            // 计算投影点的参数值
            XYZ vectorToProj = projectionOnAxis.Subtract(_baseCenter);
            double tProj = vectorToProj.DotProduct(_axis) / _height;
            
            if (tProj <= 0) // 在底面以下
            {
                // 计算点到底面圆的距离
                return DistanceToCircle(point, _baseCenter, _axis, _radius);
            }
            else if (tProj >= 1) // 在顶面以上
            {
                // 计算点到顶面圆的距离
                return DistanceToCircle(point, TopCenter, _axis, _radius);
            }
            else // 在两个端面之间
            {
                // 计算点到轴线的距离，然后减去半径
                double distanceToAxis = axisLine.DistanceTo(point);
                return distanceToAxis - _radius;
            }
        }

        /// <summary>
        /// 计算点到圆的距离
        /// </summary>
        private double DistanceToCircle(XYZ point, XYZ center, XYZ normal, double radius)
        {
            // 计算点到圆所在平面的投影点
            Plane plane = new Plane(center, normal);
            XYZ projectionOnPlane = plane.ProjectPoint(point);
            
            // 计算投影点到圆心的距离
            double distanceToCenter = projectionOnPlane.Distance(center);
            
            if (distanceToCenter <= radius)
            {
                // 如果投影点在圆内，则距离就是点到平面的距离
                return plane.DistanceTo(point);
            }
            else
            {
                // 计算投影点到圆边缘的最近点
                XYZ direction = projectionOnPlane.Subtract(center).Normalize();
                XYZ pointOnCircle = center.Add(direction.Multiply(radius));
                
                // 返回空间距离
                return point.Distance(pointOnCircle);
            }
        }

        /// <summary>
        /// 计算圆柱体与平面的交线
        /// </summary>
        /// <param name="plane">平面</param>
        /// <returns>交线（椭圆或两条平行线或一条直线），如果不相交则返回null</returns>
        public object? IntersectWithPlane(Plane plane)
        {
            // 计算轴向与平面法向量的夹角
            double dotProduct = _axis.DotProduct(plane.Normal);
            
            if (Math.Abs(dotProduct) <= Constants.DoubleEpsilon)
            {
                // 轴向与平面平行，检查是否相交
                double distanceToBase = plane.DistanceTo(_baseCenter);
                double distanceToTop = plane.DistanceTo(TopCenter);
                
                // 检查圆柱体是否与平面相交
                if (distanceToBase * distanceToTop > 0)
                    return null; // 两个端点在平面同侧，不相交
                
                // 圆柱与平面相切或相交，交线是圆
                // 计算交点的参数值
                double t = distanceToBase / (distanceToBase - distanceToTop);
                XYZ center = _baseCenter.Add(_axis.Multiply(t * _height));
                
                return new Circle(center, _axis, _radius);
            }
            else
            {
                // 轴向与平面不平行，交线是椭圆或两条直线
                // 计算轴线与平面的交点
                Line axisLine = Line.CreateByTwoPoints(_baseCenter, TopCenter);
                XYZ intersectionPoint = axisLine.IntersectWith(plane);
                
                if (intersectionPoint.IsZero())
                    return null; // 轴线与平面不相交（理论上不会发生，因为轴线与平面不平行）
                
                // 判断交点是否在圆柱体范围内
                double t = intersectionPoint.Subtract(_baseCenter).DotProduct(_axis) / _height;
                if (t < 0 || t > 1)
                    return null; // 交点不在圆柱体范围内
                
                // TODO: 这里应该返回椭圆对象，但暂时我们还没有实现椭圆类
                // 所以我们简单地返回交点
                return intersectionPoint;
            }
        }

        /// <summary>
        /// 生成圆柱面的网格点
        /// </summary>
        /// <param name="uDivisions">圆周方向的分段数</param>
        /// <param name="vDivisions">高度方向的分段数</param>
        /// <returns>圆柱面上的点集</returns>
        public List<XYZ> GenerateMeshPoints(int uDivisions, int vDivisions)
        {
            if (uDivisions < 3)
                throw new ArgumentException("圆周方向至少需要3段");
            if (vDivisions < 1)
                throw new ArgumentException("高度方向至少需要1段");
            
            List<XYZ> points = new List<XYZ>();
            
            for (int j = 0; j <= vDivisions; j++)
            {
                double v = j * _height / vDivisions;
                
                for (int i = 0; i < uDivisions; i++)
                {
                    double u = i * 2 * Math.PI / uDivisions;
                    points.Add(PointAt(u, v));
                }
            }
            
            return points;
        }

        /// <summary>
        /// 生成圆柱面的多边形网格（不包括端面）
        /// </summary>
        /// <param name="uDivisions">圆周方向的分段数</param>
        /// <param name="vDivisions">高度方向的分段数</param>
        /// <returns>表示圆柱面的多边形集合</returns>
        public List<Polygon> GenerateSideFaces(int uDivisions, int vDivisions)
        {
            List<XYZ> points = GenerateMeshPoints(uDivisions, vDivisions);
            List<Polygon> faces = new List<Polygon>();
            
            // 生成侧面的面片
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
                    
                    faces.Add(new Polygon(new[] { points[index1], points[index2], points[index3], points[index4] }));
                }
            }
            
            return faces;
        }

        /// <summary>
        /// 生成圆柱体的完整多边形网格（包括侧面和端面）
        /// </summary>
        /// <param name="uDivisions">圆周方向的分段数</param>
        /// <param name="vDivisions">高度方向的分段数</param>
        /// <returns>表示完整圆柱体的多边形集合</returns>
        public List<Polygon> GenerateMeshFaces(int uDivisions, int vDivisions)
        {
            List<Polygon> faces = GenerateSideFaces(uDivisions, vDivisions);
            List<XYZ> points = GenerateMeshPoints(uDivisions, vDivisions);
            
            // 添加底面
            List<XYZ> baseVertices = new List<XYZ>();
            for (int i = uDivisions - 1; i >= 0; i--)
            {
                baseVertices.Add(points[i]);
            }
            faces.Add(new Polygon(baseVertices));
            
            // 添加顶面
            List<XYZ> topVertices = new List<XYZ>();
            int topStartIndex = uDivisions * vDivisions;
            for (int i = 0; i < uDivisions; i++)
            {
                topVertices.Add(points[topStartIndex + i]);
            }
            faces.Add(new Polygon(topVertices));
            
            return faces;
        }

        /// <summary>
        /// 变换圆柱体
        /// </summary>
        /// <param name="transform">变换矩阵</param>
        /// <returns>变换后的新圆柱体</returns>
        public Cylinder Transform(Matrix4 transform)
        {
            // 变换底面圆心
            XYZ newBaseCenter = transform.TransformPoint(_baseCenter);
            
            // 变换轴向向量
            XYZ newAxis = transform.TransformVector(_axis);
            
            // 计算半径的缩放
            // 我们需要计算垂直于轴向的方向上的缩放
            XYZ xAxis, yAxis;
            CreateLocalCoordinateSystem(_axis, out xAxis, out yAxis);
            
            XYZ transformedX = transform.TransformVector(xAxis);
            XYZ transformedY = transform.TransformVector(yAxis);
            
            // 使用变换后的X和Y轴的长度的平均值作为半径缩放因子
            double scaleRadius = (transformedX.Length() + transformedY.Length()) / 2.0;
            double newRadius = _radius * scaleRadius;
            
            // 计算高度的缩放
            double scaleHeight = newAxis.Length();
            double newHeight = _height * scaleHeight;
            
            return new Cylinder(newBaseCenter, newAxis, newRadius, newHeight);
        }

        /// <summary>
        /// 创建局部坐标系
        /// </summary>
        private void CreateLocalCoordinateSystem(XYZ zAxis, out XYZ xAxis, out XYZ yAxis)
        {
            // 创建与zAxis垂直的两个轴
            if (Math.Abs(zAxis.X) < Math.Abs(zAxis.Y) && Math.Abs(zAxis.X) < Math.Abs(zAxis.Z))
            {
                xAxis = new XYZ(0, zAxis.Z, -zAxis.Y).Normalize();
            }
            else if (Math.Abs(zAxis.Y) < Math.Abs(zAxis.Z))
            {
                xAxis = new XYZ(zAxis.Z, 0, -zAxis.X).Normalize();
            }
            else
            {
                xAxis = new XYZ(zAxis.Y, -zAxis.X, 0).Normalize();
            }
            
            // 计算yAxis使得三个轴形成右手坐标系
            yAxis = zAxis.CrossProduct(xAxis);
        }
    }
} 