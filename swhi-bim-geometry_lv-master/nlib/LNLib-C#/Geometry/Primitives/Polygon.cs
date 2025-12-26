using System;
using System.Collections.Generic;
using System.Linq;
using LNLib.Mathematics;

namespace LNLib.Geometry.Primitives
{
    /// <summary>
    /// 表示三维空间中的多边形
    /// </summary>
    public class Polygon
    {
        private List<XYZ> _vertices;
        
        /// <summary>
        /// 通过顶点集合创建多边形
        /// </summary>
        public Polygon(IEnumerable<XYZ> vertices)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));
            
            List<XYZ> points = vertices.ToList();
            if (points.Count < 3)
                throw new ArgumentException("多边形至少需要3个顶点");
            
            // 检查所有点是否共面
            if (points.Count > 3)
            {
                Plane plane = new Plane(points[0], CalculateNormal(points));
                
                foreach (XYZ p in points)
                {
                    if (Math.Abs(plane.DistanceTo(p)) > Constants.DoubleEpsilon)
                        throw new ArgumentException("所有顶点必须在同一平面上。", nameof(vertices));
                }
            }
            
            _vertices = new List<XYZ>(points);
        }
        
        /// <summary>
        /// 获取或设置多边形的顶点集合
        /// </summary>
        public IReadOnlyList<XYZ> Vertices => _vertices.AsReadOnly();
        
        /// <summary>
        /// 获取多边形的顶点数量
        /// </summary>
        public int VertexCount => _vertices.Count;
        
        /// <summary>
        /// 获取指定索引的顶点
        /// </summary>
        public XYZ this[int index]
        {
            get
            {
                if (index < 0 || index >= _vertices.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _vertices[index];
            }
            set
            {
                if (index < 0 || index >= _vertices.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                
                // 备份原始顶点
                XYZ oldVertex = _vertices[index];
                _vertices[index] = value;
                
                // 检查是否仍然共面
                try
                {
                    Plane plane = new Plane(_vertices[0], CalculateNormal(_vertices));
                    
                    foreach (XYZ p in _vertices)
                    {
                        if (Math.Abs(plane.DistanceTo(p)) > Constants.DoubleEpsilon)
                            throw new ArgumentException("修改后的多边形顶点必须共面。", nameof(value));
                    }
                }
                catch
                {
                    // 恢复原始顶点
                    _vertices[index] = oldVertex;
                    throw;
                }
            }
        }
        
        /// <summary>
        /// 添加顶点
        /// </summary>
        public void AddVertex(XYZ vertex)
        {
            if (_vertices.Count >= 3)
            {
                // 检查新顶点是否在多边形平面上
                Plane plane = new Plane(_vertices[0], CalculateNormal(_vertices));
                
                if (Math.Abs(plane.DistanceTo(vertex)) > Constants.DoubleEpsilon)
                    throw new ArgumentException("新顶点必须在多边形平面上。", nameof(vertex));
            }
            
            _vertices.Add(vertex);
        }
        
        /// <summary>
        /// 插入顶点
        /// </summary>
        public void InsertVertex(int index, XYZ vertex)
        {
            if (index < 0 || index > _vertices.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            if (_vertices.Count >= 3)
            {
                // 检查新顶点是否在多边形平面上
                Plane plane = new Plane(_vertices[0], CalculateNormal(_vertices));
                
                if (Math.Abs(plane.DistanceTo(vertex)) > Constants.DoubleEpsilon)
                    throw new ArgumentException("新顶点必须在多边形平面上。", nameof(vertex));
            }
            
            _vertices.Insert(index, vertex);
        }
        
        /// <summary>
        /// 移除指定索引的顶点
        /// </summary>
        public void RemoveVertex(int index)
        {
            if (index < 0 || index >= _vertices.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            if (_vertices.Count <= 3)
                throw new InvalidOperationException("多边形必须至少有3个顶点");
            
            _vertices.RemoveAt(index);
        }
        
        /// <summary>
        /// 获取多边形的法向量
        /// </summary>
        public XYZ GetNormal()
        {
            if (_vertices.Count < 3)
                throw new InvalidOperationException("多边形必须至少有3个顶点才能计算法向量");
            
            return CalculateNormal(_vertices);
        }
        
        /// <summary>
        /// 计算多边形的周长
        /// </summary>
        public double Perimeter
        {
            get
            {
                double perimeter = 0;
                for (int i = 0; i < _vertices.Count; i++)
                {
                    int nextIndex = (i + 1) % _vertices.Count;
                    perimeter += _vertices[i].Distance(_vertices[nextIndex]);
                }
                return perimeter;
            }
        }
        
        /// <summary>
        /// 计算多边形的面积
        /// </summary>
        public double Area
        {
            get
            {
                if (_vertices.Count < 3)
                    return 0;
                
                // 使用三角剖分计算面积
                XYZ p0 = _vertices[0];
                double area = 0;
                
                for (int i = 1; i < _vertices.Count - 1; i++)
                {
                    Triangle triangle = new Triangle(p0, _vertices[i], _vertices[i + 1]);
                    area += triangle.Area;
                }
                
                return area;
            }
        }
        
        /// <summary>
        /// 计算多边形的质心
        /// </summary>
        public XYZ Centroid
        {
            get
            {
                if (_vertices.Count < 3)
                    throw new InvalidOperationException("多边形必须至少有3个顶点才能计算质心");
                
                // 三角形剖分加权平均法
                XYZ p0 = _vertices[0];
                double totalArea = 0;
                XYZ weightedSum = new XYZ();
                
                for (int i = 1; i < _vertices.Count - 1; i++)
                {
                    Triangle triangle = new Triangle(p0, _vertices[i], _vertices[i + 1]);
                    double area = triangle.Area;
                    
                    if (area > Constants.DoubleEpsilon)
                    {
                        totalArea += area;
                        weightedSum = weightedSum.Add(triangle.Centroid.Multiply(area));
                    }
                }
                
                if (totalArea < Constants.DoubleEpsilon)
                    return p0; // 退化为点
                
                return weightedSum.Multiply(1.0 / totalArea);
            }
        }
        
        /// <summary>
        /// 判断多边形是否为凸多边形
        /// </summary>
        public bool IsConvex
        {
            get
            {
                if (_vertices.Count < 3)
                    return false;
                
                if (_vertices.Count == 3)
                    return true; // 三角形一定是凸的
                
                XYZ normal = GetNormal();
                
                for (int i = 0; i < _vertices.Count; i++)
                {
                    int j = (i + 1) % _vertices.Count;
                    int k = (i + 2) % _vertices.Count;
                    
                    XYZ edge1 = _vertices[j].Subtract(_vertices[i]);
                    XYZ edge2 = _vertices[k].Subtract(_vertices[j]);
                    
                    XYZ cross = edge1.CrossProduct(edge2);
                    
                    // 如果叉积与法向量方向相反，则为凹多边形
                    if (cross.DotProduct(normal) < 0)
                        return false;
                }
                
                return true;
            }
        }
        
        /// <summary>
        /// 判断点是否在多边形内（包括边界）
        /// </summary>
        public bool Contains(XYZ point, double epsilon = Constants.DoubleEpsilon)
        {
            if (_vertices.Count < 3)
                return false;
            
            // 检查点是否在多边形平面上
            Plane plane = new Plane(_vertices[0], GetNormal());
            if (!plane.Contains(point, epsilon))
                return false;
            
            // 射线法判断点是否在多边形内
            // 首先将点投影到多边形平面上
            XYZ projection = plane.ProjectPoint(point);
            
            // 创建局部坐标系
            XYZ origin = _vertices[0];
            XYZ xAxis = _vertices[1].Subtract(origin).Normalize();
            XYZ zAxis = GetNormal();
            XYZ yAxis = zAxis.CrossProduct(xAxis);
            
            // 将多边形顶点转换到局部坐标系
            List<UV> local2dVertices = new List<UV>();
            foreach (XYZ vertex in _vertices)
            {
                XYZ v = vertex.Subtract(origin);
                double x = v.DotProduct(xAxis);
                double y = v.DotProduct(yAxis);
                local2dVertices.Add(new UV(x, y));
            }
            
            // 将投影点转换到局部坐标系
            XYZ v0 = projection.Subtract(origin);
            double x0 = v0.DotProduct(xAxis);
            double y0 = v0.DotProduct(yAxis);
            UV localPoint = new UV(x0, y0);
            
            // 使用射线法（Ray Casting Algorithm）
            bool inside = false;
            for (int i = 0, j = local2dVertices.Count - 1; i < local2dVertices.Count; j = i++)
            {
                UV vi = local2dVertices[i];
                UV vj = local2dVertices[j];
                
                // 检查点是否在边上
                if (PointOnSegment(localPoint, vi, vj, epsilon))
                    return true;
                
                // 射线法判断内外
                if (((vi.V > localPoint.V) != (vj.V > localPoint.V)) &&
                    (localPoint.U < (vj.U - vi.U) * (localPoint.V - vi.V) / (vj.V - vi.V) + vi.U))
                {
                    inside = !inside;
                }
            }
            
            return inside;
        }
        
        /// <summary>
        /// 计算点到多边形的最短距离
        /// </summary>
        public double DistanceTo(XYZ point)
        {
            if (_vertices.Count < 3)
                throw new InvalidOperationException("多边形必须至少有3个顶点");
            
            // 先检查点是否在多边形平面上
            Plane plane = new Plane(_vertices[0], GetNormal());
            double distanceToPlane = plane.DistanceTo(point);
            
            // 计算点在多边形平面上的投影
            XYZ projection = plane.ProjectPoint(point);
            
            // 检查投影点是否在多边形内
            if (Contains(projection))
                return distanceToPlane;
            
            // 投影点不在多边形内，计算到最近边的距离
            double minDistance = double.MaxValue;
            
            for (int i = 0; i < _vertices.Count; i++)
            {
                int j = (i + 1) % _vertices.Count;
                Line edge = Line.CreateByTwoPoints(_vertices[i], _vertices[j]);
                double distance = edge.DistanceTo(projection);
                minDistance = Math.Min(minDistance, distance);
            }
            
            return Math.Sqrt(minDistance * minDistance + distanceToPlane * distanceToPlane);
        }
        
        /// <summary>
        /// 将多边形三角剖分为三角形集合
        /// </summary>
        public List<Triangle> Triangulate()
        {
            if (_vertices.Count < 3)
                throw new InvalidOperationException("多边形必须至少有3个顶点才能进行三角剖分");
            
            List<Triangle> triangles = new List<Triangle>();
            
            // 简单的扇形三角剖分（仅适用于凸多边形）
            if (IsConvex)
            {
                XYZ p0 = _vertices[0];
                
                for (int i = 1; i < _vertices.Count - 1; i++)
                {
                    triangles.Add(new Triangle(p0, _vertices[i], _vertices[i + 1]));
                }
                
                return triangles;
            }
            
            // 耳切法（Ear Clipping）三角剖分（适用于简单多边形）
            List<int> indices = Enumerable.Range(0, _vertices.Count).ToList();
            
            while (indices.Count > 3)
            {
                bool earFound = false;
                
                for (int i = 0; i < indices.Count; i++)
                {
                    int prev = (i + indices.Count - 1) % indices.Count;
                    int curr = i;
                    int next = (i + 1) % indices.Count;
                    
                    int prevIndex = indices[prev];
                    int currIndex = indices[curr];
                    int nextIndex = indices[next];
                    
                    XYZ v1 = _vertices[prevIndex];
                    XYZ v2 = _vertices[currIndex];
                    XYZ v3 = _vertices[nextIndex];
                    
                    // 检查是否形成凸角
                    XYZ edge1 = v2.Subtract(v1);
                    XYZ edge2 = v3.Subtract(v2);
                    XYZ cross = edge1.CrossProduct(edge2);
                    
                    if (cross.DotProduct(GetNormal()) <= 0)
                        continue; // 不是凸角
                    
                    // 检查三角形内是否有其他顶点
                    bool isEar = true;
                    Triangle triangle = new Triangle(v1, v2, v3);
                    
                    for (int j = 0; j < indices.Count; j++)
                    {
                        if (j == prev || j == curr || j == next)
                            continue;
                        
                        if (triangle.Contains(_vertices[indices[j]]))
                        {
                            isEar = false;
                            break;
                        }
                    }
                    
                    if (isEar)
                    {
                        triangles.Add(triangle);
                        indices.RemoveAt(curr);
                        earFound = true;
                        break;
                    }
                }
                
                if (!earFound)
                {
                    // 如果没有找到耳朵，可能是由于数值误差或非简单多边形
                    // 在这种情况下，使用简单的扇形三角剖分作为备选
                    XYZ p0 = _vertices[indices[0]];
                    
                    for (int i = 1; i < indices.Count - 1; i++)
                    {
                        triangles.Add(new Triangle(p0, _vertices[indices[i]], _vertices[indices[i + 1]]));
                    }
                    
                    break;
                }
            }
            
            // 添加最后一个三角形
            if (indices.Count == 3)
            {
                triangles.Add(new Triangle(_vertices[indices[0]], _vertices[indices[1]], _vertices[indices[2]]));
            }
            
            return triangles;
        }
        
        /// <summary>
        /// 变换多边形
        /// </summary>
        public Polygon Transform(Matrix4 transform)
        {
            List<XYZ> transformedVertices = new List<XYZ>();
            
            foreach (XYZ vertex in _vertices)
            {
                transformedVertices.Add(transform.TransformPoint(vertex));
            }
            
            return new Polygon(transformedVertices);
        }
        
        /// <summary>
        /// 计算给定顶点集合的法向量
        /// </summary>
        private static XYZ CalculateNormal(List<XYZ> vertices)
        {
            if (vertices.Count < 3)
                throw new ArgumentException("至少需要3个顶点才能计算法向量");
            
            // 使用Newell方法计算多边形法向量
            XYZ normal = new XYZ();
            
            for (int i = 0; i < vertices.Count; i++)
            {
                int j = (i + 1) % vertices.Count;
                XYZ v1 = vertices[i];
                XYZ v2 = vertices[j];
                
                normal = new XYZ(
                    normal.X + (v1.Y - v2.Y) * (v1.Z + v2.Z),
                    normal.Y + (v1.Z - v2.Z) * (v1.X + v2.X),
                    normal.Z + (v1.X - v2.X) * (v1.Y + v2.Y)
                );
            }
            
            if (normal.IsZero())
                throw new ArgumentException("无法计算多边形法向量，顶点可能共线");
            
            return normal.Normalize();
        }
        
        /// <summary>
        /// 判断点是否在线段上
        /// </summary>
        private static bool PointOnSegment(UV p, UV q, UV r, double epsilon)
        {
            if (Math.Abs((q.U - p.U) * (r.V - p.V) - (q.V - p.V) * (r.U - p.U)) > epsilon)
                return false;
            
            return (p.U >= Math.Min(q.U, r.U) - epsilon && p.U <= Math.Max(q.U, r.U) + epsilon &&
                    p.V >= Math.Min(q.V, r.V) - epsilon && p.V <= Math.Max(q.V, r.V) + epsilon);
        }
    }
} 