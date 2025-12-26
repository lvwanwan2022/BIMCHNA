using System;
using System.Collections.Generic;
using LNLib.Mathematics;

namespace LNLib.Geometry.Primitives
{
    /// <summary>
    /// 表示三维空间中的圆锥体
    /// </summary>
    public class Cone
    {
        private XYZ _baseCenter;  // 底面圆心
        private XYZ _axis;        // 轴向（单位向量）
        private double _radius;   // 底面半径
        private double _height;   // 高度

        /// <summary>
        /// 通过底面圆心、轴向、半径和高度创建圆锥体
        /// </summary>
        /// <param name="baseCenter">底面圆心</param>
        /// <param name="axis">轴向（会被归一化）</param>
        /// <param name="radius">底面半径</param>
        /// <param name="height">高度</param>
        public Cone(XYZ baseCenter, XYZ axis, double radius, double height)
        {
            if (axis.IsZero())
                throw new ArgumentException("轴向向量不能为零向量");

            if (radius <= 0)
                throw new ArgumentException("圆锥体半径必须为正数");

            if (height <= 0)
                throw new ArgumentException("圆锥体高度必须为正数");

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
        /// 底面半径
        /// </summary>
        public double Radius
        {
            get => _radius;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("圆锥体半径必须为正数");
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
                    throw new ArgumentException("圆锥体高度必须为正数");
                _height = value;
            }
        }

        /// <summary>
        /// 顶点
        /// </summary>
        public XYZ Apex => _baseCenter.Add(_axis.Multiply(_height));

        /// <summary>
        /// 圆锥体中心点（质心位置）
        /// </summary>
        public XYZ Center => _baseCenter.Add(_axis.Multiply(_height / 4.0));

        /// <summary>
        /// 底面圆
        /// </summary>
        public Circle BaseCircle => new Circle(_baseCenter, _axis, _radius);

        /// <summary>
        /// 圆锥体的体积
        /// </summary>
        public double Volume => Math.PI * _radius * _radius * _height / 3.0;

        /// <summary>
        /// 圆锥体的表面积（包括底面）
        /// </summary>
        public double SurfaceArea
        {
            get
            {
                double slantHeight = Math.Sqrt(_radius * _radius + _height * _height);
                double lateralArea = Math.PI * _radius * slantHeight;
                double baseArea = Math.PI * _radius * _radius;
                return lateralArea + baseArea;
            }
        }

        /// <summary>
        /// 斜高（从顶点到底面圆周的距离）
        /// </summary>
        public double SlantHeight => Math.Sqrt(_radius * _radius + _height * _height);

        /// <summary>
        /// 半顶角（圆锥的张角的一半）
        /// </summary>
        public double SemiApexAngle => Math.Atan(_radius / _height);

        /// <summary>
        /// 计算圆锥面上的参数点
        /// </summary>
        /// <param name="u">角度参数，范围[0, 2π]</param>
        /// <param name="v">高度参数，范围[0, 1]，其中0表示底面圆周，1表示顶点</param>
        /// <returns>圆锥面上的点</returns>
        public XYZ PointAt(double u, double v)
        {
            // 确保u在[0, 2π]范围内
            u = u % (2 * Math.PI);
            if (u < 0) u += 2 * Math.PI;
            
            // 确保v在[0, 1]范围内
            v = Math.Max(0, Math.Min(1, v));
            
            // 创建局部坐标系
            XYZ xAxis, yAxis;
            CreateLocalCoordinateSystem(_axis, out xAxis, out yAxis);
            
            // 计算当前高度的半径
            double currentRadius = _radius * (1 - v);
            
            // 计算点坐标
            XYZ point = _baseCenter.Add(_axis.Multiply(v * _height))  // 沿轴向移动
                                  .Add(xAxis.Multiply(currentRadius * Math.Cos(u)))  // x方向
                                  .Add(yAxis.Multiply(currentRadius * Math.Sin(u)));  // y方向
            
            return point;
        }

        /// <summary>
        /// 判断点是否在圆锥体内（包括表面）
        /// </summary>
        /// <param name="point">待检测点</param>
        /// <param name="epsilon">容差值</param>
        /// <returns>如果点在圆锥体内部或表面上，则返回true</returns>
        public bool Contains(XYZ point, double epsilon = Constants.DoubleEpsilon)
        {
            // 检查点是否在底面以上且在顶点以下
            XYZ vectorToPoint = point.Subtract(_baseCenter);
            double projection = vectorToPoint.DotProduct(_axis);
            
            if (projection < -epsilon || projection > _height + epsilon)
                return false;
            
            // 计算点到轴的垂直距离
            double distanceToAxis = vectorToPoint.CrossProduct(_axis).Length();
            
            // 计算当前高度的允许半径
            double currentRadius = _radius * (1 - projection / _height);
            
            return distanceToAxis <= currentRadius + epsilon;
        }

        /// <summary>
        /// 计算点到圆锥体的最短距离
        /// </summary>
        /// <param name="point">参考点</param>
        /// <returns>点到圆锥体的最短距离。如果点在圆锥体内部，则返回0</returns>
        public double DistanceTo(XYZ point)
        {
            // 如果点在圆锥体内部，返回0
            if (Contains(point))
                return 0;
            
            // 计算点到轴线的投影
            Line axisLine = Line.CreateByTwoPoints(_baseCenter, Apex);
            XYZ vectorToPoint = point.Subtract(_baseCenter);
            double projection = vectorToPoint.DotProduct(_axis);
            
            if (projection <= 0)
            {
                // 点在底面以下
                // 计算点到底面圆的距离
                Plane basePlane = new Plane(_baseCenter, _axis);
                XYZ projectionOnBase = basePlane.ProjectPoint(point);
                
                double distanceToCenter = projectionOnBase.Distance(_baseCenter);
                if (distanceToCenter <= _radius)
                {
                    // 点的投影在底面圆内
                    return basePlane.DistanceTo(point);
                }
                else
                {
                    // 点的投影在底面圆外
                    XYZ direction = projectionOnBase.Subtract(_baseCenter).Normalize();
                    XYZ pointOnCircle = _baseCenter.Add(direction.Multiply(_radius));
                    return point.Distance(pointOnCircle);
                }
            }
            else if (projection >= _height)
            {
                // 点在顶点以上
                return point.Distance(Apex);
            }
            else
            {
                // 点在底面和顶点之间，但在圆锥外部
                // 计算当前高度的半径
                double currentRadius = _radius * (1 - projection / _height);
                
                // 计算点到轴线的垂直距离
                XYZ projectionOnAxis = _baseCenter.Add(_axis.Multiply(projection));
                double distanceToAxis = point.Distance(projectionOnAxis);
                
                // 距离是点到当前高度圆的距离
                return distanceToAxis - currentRadius;
            }
        }

        /// <summary>
        /// 生成圆锥面的网格点
        /// </summary>
        /// <param name="uDivisions">圆周方向的分段数</param>
        /// <param name="vDivisions">高度方向的分段数</param>
        /// <returns>圆锥面上的点集</returns>
        public List<XYZ> GenerateMeshPoints(int uDivisions, int vDivisions)
        {
            if (uDivisions < 3)
                throw new ArgumentException("圆周方向至少需要3段");
            if (vDivisions < 1)
                throw new ArgumentException("高度方向至少需要1段");
            
            List<XYZ> points = new List<XYZ>();
            
            // 添加底面圆周上的点
            for (int i = 0; i < uDivisions; i++)
            {
                double u = i * 2 * Math.PI / uDivisions;
                points.Add(PointAt(u, 0));
            }
            
            // 添加侧面上的点
            for (int j = 1; j < vDivisions; j++)
            {
                double v = j * 1.0 / vDivisions;
                
                for (int i = 0; i < uDivisions; i++)
                {
                    double u = i * 2 * Math.PI / uDivisions;
                    points.Add(PointAt(u, v));
                }
            }
            
            // 添加顶点
            points.Add(Apex);
            
            return points;
        }

        /// <summary>
        /// 生成圆锥面的多边形网格（不包括底面）
        /// </summary>
        /// <param name="uDivisions">圆周方向的分段数</param>
        /// <param name="vDivisions">高度方向的分段数</param>
        /// <returns>表示圆锥面的多边形集合</returns>
        public List<Polygon> GenerateSideFaces(int uDivisions, int vDivisions)
        {
            List<XYZ> points = GenerateMeshPoints(uDivisions, vDivisions);
            List<Polygon> faces = new List<Polygon>();
            
            // 生成侧面的面片
            for (int j = 0; j < vDivisions - 1; j++)
            {
                for (int i = 0; i < uDivisions; i++)
                {
                    int i1 = i;
                    int i2 = (i + 1) % uDivisions;
                    
                    int index1 = j * uDivisions + i1;
                    int index2 = j * uDivisions + i2;
                    
                    if (j < vDivisions - 2)
                    {
                        // 生成普通四边形面片
                        int index3 = (j + 1) * uDivisions + i2;
                        int index4 = (j + 1) * uDivisions + i1;
                        
                        faces.Add(new Polygon(new[] { points[index1], points[index2], points[index3], points[index4] }));
                    }
                    else
                    {
                        // 生成连接到顶点的三角形面片
                        int apexIndex = points.Count - 1;
                        
                        faces.Add(new Polygon(new[] { points[index1], points[index2], points[apexIndex] }));
                    }
                }
            }
            
            return faces;
        }

        /// <summary>
        /// 生成圆锥体的完整多边形网格（包括侧面和底面）
        /// </summary>
        /// <param name="uDivisions">圆周方向的分段数</param>
        /// <param name="vDivisions">高度方向的分段数</param>
        /// <returns>表示完整圆锥体的多边形集合</returns>
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
            
            return faces;
        }

        /// <summary>
        /// 变换圆锥体
        /// </summary>
        /// <param name="transform">变换矩阵</param>
        /// <returns>变换后的新圆锥体</returns>
        public Cone Transform(Matrix4 transform)
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
            
            return new Cone(newBaseCenter, newAxis, newRadius, newHeight);
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