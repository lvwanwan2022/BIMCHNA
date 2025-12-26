using System;
using System.Collections.Generic;
using LNLib.Mathematics;

namespace LNLib.Geometry.Primitives
{
    /// <summary>
    /// 表示三维空间中的长方体
    /// </summary>
    public class Box
    {
        private XYZ _center;  // 中心点
        private XYZ _xAxis;   // X轴方向（归一化）
        private XYZ _yAxis;   // Y轴方向（归一化）
        private XYZ _zAxis;   // Z轴方向（归一化）
        private double _xLength;  // X方向尺寸
        private double _yLength;  // Y方向尺寸
        private double _zLength;  // Z方向尺寸

        /// <summary>
        /// 通过中心点、三个轴向和尺寸创建长方体
        /// </summary>
        public Box(XYZ center, XYZ xAxis, XYZ yAxis, XYZ zAxis, double xLength, double yLength, double zLength)
        {
            if (xAxis.IsZero() || yAxis.IsZero() || zAxis.IsZero())
                throw new ArgumentException("轴向向量不能为零向量");

            if (xLength <= 0 || yLength <= 0 || zLength <= 0)
                throw new ArgumentException("长方体的尺寸必须为正数");

            double xyDot = xAxis.Normalize().DotProduct(yAxis.Normalize());
            double yzDot = yAxis.Normalize().DotProduct(zAxis.Normalize());
            double zxDot = zAxis.Normalize().DotProduct(xAxis.Normalize());

            if (Math.Abs(xyDot) > Constants.DoubleEpsilon || 
                Math.Abs(yzDot) > Constants.DoubleEpsilon || 
                Math.Abs(zxDot) > Constants.DoubleEpsilon)
                throw new ArgumentException("轴向向量必须相互正交");

            _center = center;
            _xAxis = xAxis.Normalize();
            _yAxis = yAxis.Normalize();
            _zAxis = zAxis.Normalize();
            _xLength = xLength;
            _yLength = yLength;
            _zLength = zLength;
        }

        /// <summary>
        /// 通过两个对角点创建轴向长方体（轴向与全局坐标系平行）
        /// </summary>
        public static Box CreateAligned(XYZ p1, XYZ p2)
        {
            // 确保p1的坐标小于p2的坐标
            double minX = Math.Min(p1.X, p2.X);
            double minY = Math.Min(p1.Y, p2.Y);
            double minZ = Math.Min(p1.Z, p2.Z);
            double maxX = Math.Max(p1.X, p2.X);
            double maxY = Math.Max(p1.Y, p2.Y);
            double maxZ = Math.Max(p1.Z, p2.Z);

            // 检查有效性
            if (Math.Abs(maxX - minX) < Constants.DoubleEpsilon ||
                Math.Abs(maxY - minY) < Constants.DoubleEpsilon ||
                Math.Abs(maxZ - minZ) < Constants.DoubleEpsilon)
                throw new ArgumentException("无法创建厚度为零的长方体");

            // 计算中心点和尺寸
            XYZ center = new XYZ(
                (minX + maxX) / 2.0,
                (minY + maxY) / 2.0,
                (minZ + maxZ) / 2.0);

            double xLength = maxX - minX;
            double yLength = maxY - minY;
            double zLength = maxZ - minZ;

            // 创建标准轴向
            XYZ xAxis = new XYZ(1, 0, 0);
            XYZ yAxis = new XYZ(0, 1, 0);
            XYZ zAxis = new XYZ(0, 0, 1);

            return new Box(center, xAxis, yAxis, zAxis, xLength, yLength, zLength);
        }

        /// <summary>
        /// 通过包围盒创建轴向长方体
        /// </summary>
        public static Box CreateFromBoundingBox(BoundingBox boundingBox)
        {
            return CreateAligned(boundingBox.Min, boundingBox.Max);
        }

        /// <summary>
        /// 长方体的中心点
        /// </summary>
        public XYZ Center
        {
            get => _center;
            set => _center = value;
        }

        /// <summary>
        /// X轴方向（单位向量）
        /// </summary>
        public XYZ XAxis
        {
            get => _xAxis;
            set
            {
                if (value.IsZero())
                    throw new ArgumentException("轴向向量不能为零向量");

                XYZ normalized = value.Normalize();
                
                // 检查正交性
                if (Math.Abs(normalized.DotProduct(_yAxis)) > Constants.DoubleEpsilon ||
                    Math.Abs(normalized.DotProduct(_zAxis)) > Constants.DoubleEpsilon)
                    throw new ArgumentException("轴向向量必须相互正交");

                _xAxis = normalized;
            }
        }

        /// <summary>
        /// Y轴方向（单位向量）
        /// </summary>
        public XYZ YAxis
        {
            get => _yAxis;
            set
            {
                if (value.IsZero())
                    throw new ArgumentException("轴向向量不能为零向量");

                XYZ normalized = value.Normalize();
                
                // 检查正交性
                if (Math.Abs(normalized.DotProduct(_xAxis)) > Constants.DoubleEpsilon ||
                    Math.Abs(normalized.DotProduct(_zAxis)) > Constants.DoubleEpsilon)
                    throw new ArgumentException("轴向向量必须相互正交");

                _yAxis = normalized;
            }
        }

        /// <summary>
        /// Z轴方向（单位向量）
        /// </summary>
        public XYZ ZAxis
        {
            get => _zAxis;
            set
            {
                if (value.IsZero())
                    throw new ArgumentException("轴向向量不能为零向量");

                XYZ normalized = value.Normalize();
                
                // 检查正交性
                if (Math.Abs(normalized.DotProduct(_xAxis)) > Constants.DoubleEpsilon ||
                    Math.Abs(normalized.DotProduct(_yAxis)) > Constants.DoubleEpsilon)
                    throw new ArgumentException("轴向向量必须相互正交");

                _zAxis = normalized;
            }
        }

        /// <summary>
        /// X方向尺寸
        /// </summary>
        public double XLength
        {
            get => _xLength;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("长方体的尺寸必须为正数");
                _xLength = value;
            }
        }

        /// <summary>
        /// Y方向尺寸
        /// </summary>
        public double YLength
        {
            get => _yLength;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("长方体的尺寸必须为正数");
                _yLength = value;
            }
        }

        /// <summary>
        /// Z方向尺寸
        /// </summary>
        public double ZLength
        {
            get => _zLength;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("长方体的尺寸必须为正数");
                _zLength = value;
            }
        }

        /// <summary>
        /// 长方体的体积
        /// </summary>
        public double Volume => _xLength * _yLength * _zLength;

        /// <summary>
        /// 长方体的表面积
        /// </summary>
        public double SurfaceArea => 2 * (_xLength * _yLength + _yLength * _zLength + _zLength * _xLength);

        /// <summary>
        /// 获取长方体的8个角点
        /// </summary>
        public XYZ[] GetCorners()
        {
            XYZ[] corners = new XYZ[8];

            // 计算半长
            double halfX = _xLength / 2.0;
            double halfY = _yLength / 2.0;
            double halfZ = _zLength / 2.0;

            // 计算角点
            corners[0] = _center.Add(_xAxis.Multiply(-halfX)).Add(_yAxis.Multiply(-halfY)).Add(_zAxis.Multiply(-halfZ));
            corners[1] = _center.Add(_xAxis.Multiply(halfX)).Add(_yAxis.Multiply(-halfY)).Add(_zAxis.Multiply(-halfZ));
            corners[2] = _center.Add(_xAxis.Multiply(halfX)).Add(_yAxis.Multiply(halfY)).Add(_zAxis.Multiply(-halfZ));
            corners[3] = _center.Add(_xAxis.Multiply(-halfX)).Add(_yAxis.Multiply(halfY)).Add(_zAxis.Multiply(-halfZ));
            corners[4] = _center.Add(_xAxis.Multiply(-halfX)).Add(_yAxis.Multiply(-halfY)).Add(_zAxis.Multiply(halfZ));
            corners[5] = _center.Add(_xAxis.Multiply(halfX)).Add(_yAxis.Multiply(-halfY)).Add(_zAxis.Multiply(halfZ));
            corners[6] = _center.Add(_xAxis.Multiply(halfX)).Add(_yAxis.Multiply(halfY)).Add(_zAxis.Multiply(halfZ));
            corners[7] = _center.Add(_xAxis.Multiply(-halfX)).Add(_yAxis.Multiply(halfY)).Add(_zAxis.Multiply(halfZ));

            return corners;
        }

        /// <summary>
        /// 获取长方体的6个面
        /// </summary>
        public List<Polygon> GetFaces()
        {
            XYZ[] corners = GetCorners();
            List<Polygon> faces = new List<Polygon>();

            // 底面 (Z-)
            faces.Add(new Polygon(new[] { corners[0], corners[1], corners[2], corners[3] }));
            // 顶面 (Z+)
            faces.Add(new Polygon(new[] { corners[4], corners[7], corners[6], corners[5] }));
            // 前面 (Y-)
            faces.Add(new Polygon(new[] { corners[0], corners[4], corners[5], corners[1] }));
            // 后面 (Y+)
            faces.Add(new Polygon(new[] { corners[3], corners[2], corners[6], corners[7] }));
            // 左面 (X-)
            faces.Add(new Polygon(new[] { corners[0], corners[3], corners[7], corners[4] }));
            // 右面 (X+)
            faces.Add(new Polygon(new[] { corners[1], corners[5], corners[6], corners[2] }));

            return faces;
        }

        /// <summary>
        /// 获取长方体的12条边
        /// </summary>
        public List<Line> GetEdges()
        {
            XYZ[] corners = GetCorners();
            List<Line> edges = new List<Line>();

            // 底面边
            edges.Add(Line.CreateByTwoPoints(corners[0], corners[1]));
            edges.Add(Line.CreateByTwoPoints(corners[1], corners[2]));
            edges.Add(Line.CreateByTwoPoints(corners[2], corners[3]));
            edges.Add(Line.CreateByTwoPoints(corners[3], corners[0]));

            // 顶面边
            edges.Add(Line.CreateByTwoPoints(corners[4], corners[5]));
            edges.Add(Line.CreateByTwoPoints(corners[5], corners[6]));
            edges.Add(Line.CreateByTwoPoints(corners[6], corners[7]));
            edges.Add(Line.CreateByTwoPoints(corners[7], corners[4]));

            // 竖向边
            edges.Add(Line.CreateByTwoPoints(corners[0], corners[4]));
            edges.Add(Line.CreateByTwoPoints(corners[1], corners[5]));
            edges.Add(Line.CreateByTwoPoints(corners[2], corners[6]));
            edges.Add(Line.CreateByTwoPoints(corners[3], corners[7]));

            return edges;
        }

        /// <summary>
        /// 判断点是否在长方体内（包括表面）
        /// </summary>
        public bool Contains(XYZ point, double epsilon = Constants.DoubleEpsilon)
        {
            // 将点转换到长方体的局部坐标系
            XYZ localPoint = PointToLocal(point);

            // 检查局部坐标是否在[-halfLength, halfLength]范围内
            double halfX = _xLength / 2.0 + epsilon;
            double halfY = _yLength / 2.0 + epsilon;
            double halfZ = _zLength / 2.0 + epsilon;

            return localPoint.X >= -halfX && localPoint.X <= halfX &&
                   localPoint.Y >= -halfY && localPoint.Y <= halfY &&
                   localPoint.Z >= -halfZ && localPoint.Z <= halfZ;
        }

        /// <summary>
        /// 计算点到长方体的最短距离
        /// </summary>
        public double DistanceTo(XYZ point)
        {
            // 将点转换到长方体的局部坐标系
            XYZ localPoint = PointToLocal(point);

            // 计算点到长方体表面的最短距离
            double halfX = _xLength / 2.0;
            double halfY = _yLength / 2.0;
            double halfZ = _zLength / 2.0;

            // 计算各个方向的距离
            double dx = Math.Max(0, Math.Max(-localPoint.X - halfX, localPoint.X - halfX));
            double dy = Math.Max(0, Math.Max(-localPoint.Y - halfY, localPoint.Y - halfY));
            double dz = Math.Max(0, Math.Max(-localPoint.Z - halfZ, localPoint.Z - halfZ));

            // 计算总距离
            if (dx == 0 && dy == 0 && dz == 0)
                return 0; // 点在长方体内部或表面上

            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// 计算长方体与另一个长方体的交集
        /// </summary>
        public Box? IntersectWith(Box other)
        {
            // 目前仅实现两个轴向相同的长方体求交
            if (!AreAxesParallel(other))
                return null; // 轴向不同，暂不支持求交

            // 获取两个长方体的角点
            XYZ[] corners1 = GetCorners();
            XYZ[] corners2 = other.GetCorners();

            // 计算局部坐标下的包围盒
            double minX1 = double.MaxValue, minY1 = double.MaxValue, minZ1 = double.MaxValue;
            double maxX1 = double.MinValue, maxY1 = double.MinValue, maxZ1 = double.MinValue;

            foreach (var corner in corners1)
            {
                XYZ local = PointToLocal(corner);
                minX1 = Math.Min(minX1, local.X);
                minY1 = Math.Min(minY1, local.Y);
                minZ1 = Math.Min(minZ1, local.Z);
                maxX1 = Math.Max(maxX1, local.X);
                maxY1 = Math.Max(maxY1, local.Y);
                maxZ1 = Math.Max(maxZ1, local.Z);
            }

            double minX2 = double.MaxValue, minY2 = double.MaxValue, minZ2 = double.MaxValue;
            double maxX2 = double.MinValue, maxY2 = double.MinValue, maxZ2 = double.MinValue;

            foreach (var corner in corners2)
            {
                XYZ local = PointToLocal(corner);
                minX2 = Math.Min(minX2, local.X);
                minY2 = Math.Min(minY2, local.Y);
                minZ2 = Math.Min(minZ2, local.Z);
                maxX2 = Math.Max(maxX2, local.X);
                maxY2 = Math.Max(maxY2, local.Y);
                maxZ2 = Math.Max(maxZ2, local.Z);
            }

            // 计算交集
            double intersectMinX = Math.Max(minX1, minX2);
            double intersectMinY = Math.Max(minY1, minY2);
            double intersectMinZ = Math.Max(minZ1, minZ2);
            double intersectMaxX = Math.Min(maxX1, maxX2);
            double intersectMaxY = Math.Min(maxY1, maxY2);
            double intersectMaxZ = Math.Min(maxZ1, maxZ2);

            // 检查是否有交集
            if (intersectMinX > intersectMaxX || intersectMinY > intersectMaxY || intersectMinZ > intersectMaxZ)
                return null; // 无交集

            // 计算交集长方体的中心和尺寸
            XYZ center = LocalToPoint(new XYZ(
                (intersectMinX + intersectMaxX) / 2.0,
                (intersectMinY + intersectMaxY) / 2.0,
                (intersectMinZ + intersectMaxZ) / 2.0));

            double xLength = intersectMaxX - intersectMinX;
            double yLength = intersectMaxY - intersectMinY;
            double zLength = intersectMaxZ - intersectMinZ;

            return new Box(center, _xAxis, _yAxis, _zAxis, xLength, yLength, zLength);
        }

        /// <summary>
        /// 变换长方体
        /// </summary>
        public Box Transform(Matrix4 transform)
        {
            XYZ newCenter = transform.TransformPoint(_center);
            XYZ newXAxis = transform.TransformVector(_xAxis);
            XYZ newYAxis = transform.TransformVector(_yAxis);
            XYZ newZAxis = transform.TransformVector(_zAxis);

            double scaleX = newXAxis.Length();
            double scaleY = newYAxis.Length();
            double scaleZ = newZAxis.Length();

            return new Box(
                newCenter,
                newXAxis.Normalize(),
                newYAxis.Normalize(),
                newZAxis.Normalize(),
                _xLength * scaleX,
                _yLength * scaleY,
                _zLength * scaleZ);
        }

        /// <summary>
        /// 将全局坐标点转换为长方体局部坐标系下的点
        /// </summary>
        private XYZ PointToLocal(XYZ point)
        {
            XYZ v = point.Subtract(_center);
            return new XYZ(
                v.DotProduct(_xAxis),
                v.DotProduct(_yAxis),
                v.DotProduct(_zAxis));
        }

        /// <summary>
        /// 将长方体局部坐标系下的点转换为全局坐标点
        /// </summary>
        private XYZ LocalToPoint(XYZ localPoint)
        {
            return _center
                .Add(_xAxis.Multiply(localPoint.X))
                .Add(_yAxis.Multiply(localPoint.Y))
                .Add(_zAxis.Multiply(localPoint.Z));
        }

        /// <summary>
        /// 检查两个长方体的轴向是否平行
        /// </summary>
        private bool AreAxesParallel(Box other)
        {
            return (Math.Abs(Math.Abs(_xAxis.DotProduct(other._xAxis)) - 1.0) < Constants.DoubleEpsilon &&
                    Math.Abs(Math.Abs(_yAxis.DotProduct(other._yAxis)) - 1.0) < Constants.DoubleEpsilon &&
                    Math.Abs(Math.Abs(_zAxis.DotProduct(other._zAxis)) - 1.0) < Constants.DoubleEpsilon) ||
                   (Math.Abs(Math.Abs(_xAxis.DotProduct(other._yAxis)) - 1.0) < Constants.DoubleEpsilon &&
                    Math.Abs(Math.Abs(_yAxis.DotProduct(other._zAxis)) - 1.0) < Constants.DoubleEpsilon &&
                    Math.Abs(Math.Abs(_zAxis.DotProduct(other._xAxis)) - 1.0) < Constants.DoubleEpsilon) ||
                   (Math.Abs(Math.Abs(_xAxis.DotProduct(other._zAxis)) - 1.0) < Constants.DoubleEpsilon &&
                    Math.Abs(Math.Abs(_yAxis.DotProduct(other._xAxis)) - 1.0) < Constants.DoubleEpsilon &&
                    Math.Abs(Math.Abs(_zAxis.DotProduct(other._yAxis)) - 1.0) < Constants.DoubleEpsilon);
        }
    }
} 