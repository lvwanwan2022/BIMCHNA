using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using LNLib.Geometry.Curve;
using LNLib.Geometry.Surface;
using LNLib.Mathematics;

namespace LNLibViewer
{
    /// <summary>
    /// 几何工具类，提供几何对象的生成和转换方法
    /// </summary>
    public static class GeometryHelper
    {
        // 使用LNLib库中的常量
        private static class Constants
        {
            public const double DoubleEpsilon = 1.0e-8;
        }
        
        // 随机数生成器
        private static readonly Random random = new Random();
        
        /// <summary>
        /// 创建随机控制点
        /// </summary>
        public static List<XYZ> CreateRandomControlPoints(int count, bool planar = false, double radius = 1.0)
        {
            List<XYZ> controlPoints = new List<XYZ>();
            for (int i = 0; i < count; i++)
            {
                double angle = 2 * Math.PI * i / count;
                double r = 0.5 + 0.5 * random.NextDouble() * radius;
                
                double x = r * Math.Cos(angle);
                double y = r * Math.Sin(angle);
                double z = planar ? 0 : 0.2 * (random.NextDouble() - 0.5);
                
                controlPoints.Add(new XYZ(x, y, z));
            }
            return controlPoints;
        }
        
        /// <summary>
        /// 创建均匀节点向量
        /// </summary>
        public static List<double> CreateUniformKnotVector(int degree, int controlPointsCount)
        {
            List<double> knotVector = new List<double>();
            int knotCount = controlPointsCount + degree + 1;
            
            // 添加起始重复节点
            for (int i = 0; i <= degree; i++)
            {
                knotVector.Add(0.0);
            }
            
            // 添加中间节点
            int middleKnots = knotCount - 2 * (degree + 1);
            if (middleKnots > 0)
            {
                for (int i = 1; i <= middleKnots; i++)
                {
                    knotVector.Add(i / (double)(middleKnots + 1));
                }
            }
            
            // 添加结束重复节点
            for (int i = 0; i <= degree; i++)
            {
                knotVector.Add(1.0);
            }
            
            return knotVector;
        }
        
        /// <summary>
        /// 创建随机权重值
        /// </summary>
        public static List<double> CreateRandomWeights(int count, double minWeight = 0.5, double maxWeight = 2.0)
        {
            List<double> weights = new List<double>();
            for (int i = 0; i < count; i++)
            {
                weights.Add(minWeight + (maxWeight - minWeight) * random.NextDouble());
            }
            return weights;
        }
        
        /// <summary>
        /// 创建NURBS圆
        /// </summary>
        public static BsplineCurveData<XYZW> CreateCircleNURBS(double radius = 0.5, double centerX = 0, double centerY = 0)
        {
            // 确保半径为正值
            radius = Math.Max(radius, Constants.DoubleEpsilon);
            
            // 使用1/sqrt(2)作为权重，这是标准圆的权重
            double weight = 1.0 / Math.Sqrt(2);
            
            // 确保权重不为零
            if (Math.Abs(weight) < Constants.DoubleEpsilon)
            {
                weight = Constants.DoubleEpsilon;
            }
            
            List<XYZW> controlPoints = new List<XYZW>
            {
                new XYZW(centerX + radius, centerY, 0, 1.0),
                new XYZW(centerX + radius, centerY + radius * weight, 0, weight),
                new XYZW(centerX + radius * weight, centerY + radius, 0, weight),
                new XYZW(centerX, centerY + radius, 0, 1.0),
                new XYZW(centerX - radius * weight, centerY + radius, 0, weight),
                new XYZW(centerX - radius, centerY + radius * weight, 0, weight),
                new XYZW(centerX - radius, centerY, 0, 1.0),
                new XYZW(centerX - radius, centerY - radius * weight, 0, weight),
                new XYZW(centerX - radius * weight, centerY - radius, 0, weight),
                new XYZW(centerX, centerY - radius, 0, 1.0),
                new XYZW(centerX + radius * weight, centerY - radius, 0, weight),
                new XYZW(centerX + radius, centerY - radius * weight, 0, weight),
                new XYZW(centerX + radius, centerY, 0, 1.0)
            };
            
            // 二次NURBS曲线的节点向量
            List<double> knotVector = new List<double>
            {
                0, 0, 0,
                1, 1,
                2, 2,
                3, 3,
                4, 4,
                5, 5, 5
            };
            
            // 归一化节点向量
            for (int i = 0; i < knotVector.Count; i++)
            {
                knotVector[i] /= 5.0;
            }
            
            try
            {
                return new BsplineCurveData<XYZW>(2, controlPoints, knotVector);
            }
            catch (Exception)
            {
                // 如果创建失败，提供一个简单的备选方案（降级处理）
                List<XYZW> simplePoints = new List<XYZW>();
                int segments = 12;
                for (int i = 0; i <= segments; i++)
                {
                    double angle = 2 * Math.PI * i / segments;
                    double x = centerX + radius * Math.Cos(angle);
                    double y = centerY + radius * Math.Sin(angle);
                    simplePoints.Add(new XYZW(x, y, 0, 1.0));
                }
                
                List<double> simpleKnots = CreateUniformKnotVector(2, simplePoints.Count);
                return new BsplineCurveData<XYZW>(2, simplePoints, simpleKnots);
            }
        }
        
        /// <summary>
        /// 创建NURBS椭圆
        /// </summary>
        public static BsplineCurveData<XYZW> CreateEllipseNURBS(double a, double b, double centerX = 0, double centerY = 0)
        {
            // 确保半轴长度为正值
            a = Math.Max(a, Constants.DoubleEpsilon);
            b = Math.Max(b, Constants.DoubleEpsilon);
            
            // 使用1/sqrt(2)作为权重，这是标准圆/椭圆的权重
            double weight = 1.0 / Math.Sqrt(2);
            
            // 确保权重不为零
            if (Math.Abs(weight) < Constants.DoubleEpsilon)
            {
                weight = Constants.DoubleEpsilon;
            }
            
            List<XYZW> controlPoints = new List<XYZW>
            {
                new XYZW(centerX + a, centerY, 0, 1.0),
                new XYZW(centerX + a, centerY + b * weight, 0, weight),
                new XYZW(centerX + a * weight, centerY + b, 0, weight),
                new XYZW(centerX, centerY + b, 0, 1.0),
                new XYZW(centerX - a * weight, centerY + b, 0, weight),
                new XYZW(centerX - a, centerY + b * weight, 0, weight),
                new XYZW(centerX - a, centerY, 0, 1.0),
                new XYZW(centerX - a, centerY - b * weight, 0, weight),
                new XYZW(centerX - a * weight, centerY - b, 0, weight),
                new XYZW(centerX, centerY - b, 0, 1.0),
                new XYZW(centerX + a * weight, centerY - b, 0, weight),
                new XYZW(centerX + a, centerY - b * weight, 0, weight),
                new XYZW(centerX + a, centerY, 0, 1.0)
            };
            
            // 二次NURBS曲线的节点向量
            List<double> knotVector = new List<double>
            {
                0, 0, 0,
                1, 1,
                2, 2,
                3, 3,
                4, 4,
                5, 5, 5
            };
            
            // 归一化节点向量
            for (int i = 0; i < knotVector.Count; i++)
            {
                knotVector[i] /= 5.0;
            }
            
            try
            {
                return new BsplineCurveData<XYZW>(2, controlPoints, knotVector);
            }
            catch (Exception)
            {
                // 如果创建失败，提供一个简单的备选方案（降级处理）
                List<XYZW> simplePoints = new List<XYZW>();
                int segments = 12;
                for (int i = 0; i <= segments; i++)
                {
                    double angle = 2 * Math.PI * i / segments;
                    double x = centerX + a * Math.Cos(angle);
                    double y = centerY + b * Math.Sin(angle);
                    simplePoints.Add(new XYZW(x, y, 0, 1.0));
                }
                
                List<double> simpleKnots = CreateUniformKnotVector(2, simplePoints.Count);
                return new BsplineCurveData<XYZW>(2, simplePoints, simpleKnots);
            }
        }
        
        /// <summary>
        /// 创建NURBS曲面
        /// </summary>
        public static NurbsSurfaceData CreateNurbsSurface(int uDegree, int vDegree, int uCount, int vCount)
        {
            // 创建控制点网格
            List<List<XYZW>> controlPoints = new List<List<XYZW>>();
            
            for (int i = 0; i < uCount; i++)
            {
                List<XYZW> row = new List<XYZW>();
                double u = i / (double)(uCount - 1);
                
                for (int j = 0; j < vCount; j++)
                {
                    double v = j / (double)(vCount - 1);
                    double x = (u - 0.5) * 2.0;
                    double y = (v - 0.5) * 2.0;
                    
                    // 创建波浪形状的曲面
                    double z = 0.2 * Math.Sin(2 * Math.PI * u) * Math.Sin(2 * Math.PI * v);
                    
                    row.Add(new XYZW(x, y, z, 1.0));
                }
                controlPoints.Add(row);
            }
            
            // 创建均匀节点向量
            List<double> knotVectorU = CreateUniformKnotVector(uDegree, uCount);
            List<double> knotVectorV = CreateUniformKnotVector(vDegree, vCount);
            
            return new NurbsSurfaceData(uDegree, vDegree, knotVectorU, knotVectorV, controlPoints);
        }
        
        /// <summary>
        /// 创建球体网格
        /// </summary>
        public static MeshGeometry3D CreateSphere(double radius, int thetaDiv, int phiDiv)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            
            // 创建顶点
            for (int j = 0; j <= phiDiv; j++)
            {
                double phi = j * Math.PI / phiDiv;
                double sinPhi = Math.Sin(phi);
                double cosPhi = Math.Cos(phi);
                
                for (int i = 0; i <= thetaDiv; i++)
                {
                    double theta = i * 2 * Math.PI / thetaDiv;
                    double sinTheta = Math.Sin(theta);
                    double cosTheta = Math.Cos(theta);
                    
                    double x = radius * sinPhi * cosTheta;
                    double y = radius * sinPhi * sinTheta;
                    double z = radius * cosPhi;
                    
                    mesh.Positions.Add(new Point3D(x, y, z));
                    mesh.TextureCoordinates.Add(new System.Windows.Point((double)i / thetaDiv, (double)j / phiDiv));
                }
            }
            
            // 创建三角形
            for (int j = 0; j < phiDiv; j++)
            {
                int offset = (thetaDiv + 1) * j;
                
                for (int i = 0; i < thetaDiv; i++)
                {
                    int a = offset + i;
                    int b = offset + i + 1;
                    int c = offset + i + thetaDiv + 1;
                    int d = offset + i + thetaDiv + 2;
                    
                    mesh.TriangleIndices.Add(a);
                    mesh.TriangleIndices.Add(c);
                    mesh.TriangleIndices.Add(b);
                    
                    mesh.TriangleIndices.Add(b);
                    mesh.TriangleIndices.Add(c);
                    mesh.TriangleIndices.Add(d);
                }
            }
            
            return mesh;
        }
        
        /// <summary>
        /// 创建圆柱体网格
        /// </summary>
        public static MeshGeometry3D CreateCylinder(double radius, double height, int thetaDiv, int heightDiv)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            
            // 创建顶点
            for (int j = 0; j <= heightDiv; j++)
            {
                double h = j * height / heightDiv - height / 2;
                
                for (int i = 0; i <= thetaDiv; i++)
                {
                    double theta = i * 2 * Math.PI / thetaDiv;
                    double sinTheta = Math.Sin(theta);
                    double cosTheta = Math.Cos(theta);
                    
                    double x = radius * cosTheta;
                    double y = radius * sinTheta;
                    double z = h;
                    
                    mesh.Positions.Add(new Point3D(x, y, z));
                    mesh.TextureCoordinates.Add(new System.Windows.Point((double)i / thetaDiv, (double)j / heightDiv));
                }
            }
            
            // 创建圆柱侧面的三角形
            for (int j = 0; j < heightDiv; j++)
            {
                int offset = (thetaDiv + 1) * j;
                
                for (int i = 0; i < thetaDiv; i++)
                {
                    int a = offset + i;
                    int b = offset + i + 1;
                    int c = offset + i + thetaDiv + 1;
                    int d = offset + i + thetaDiv + 2;
                    
                    mesh.TriangleIndices.Add(a);
                    mesh.TriangleIndices.Add(c);
                    mesh.TriangleIndices.Add(b);
                    
                    mesh.TriangleIndices.Add(b);
                    mesh.TriangleIndices.Add(c);
                    mesh.TriangleIndices.Add(d);
                }
            }
            
            // 创建顶面和底面
            int topCenter = mesh.Positions.Count;
            mesh.Positions.Add(new Point3D(0, 0, height / 2));
            mesh.TextureCoordinates.Add(new System.Windows.Point(0.5, 0.5));
            
            int bottomCenter = mesh.Positions.Count;
            mesh.Positions.Add(new Point3D(0, 0, -height / 2));
            mesh.TextureCoordinates.Add(new System.Windows.Point(0.5, 0.5));
            
            // 顶面三角形
            for (int i = 0; i < thetaDiv; i++)
            {
                int a = i;
                int b = i + 1;
                
                mesh.TriangleIndices.Add(topCenter);
                mesh.TriangleIndices.Add(a);
                mesh.TriangleIndices.Add(b);
            }
            
            // 底面三角形
            int bottomOffset = thetaDiv * heightDiv;
            for (int i = 0; i < thetaDiv; i++)
            {
                int a = bottomOffset + i;
                int b = bottomOffset + i + 1;
                
                mesh.TriangleIndices.Add(bottomCenter);
                mesh.TriangleIndices.Add(b);
                mesh.TriangleIndices.Add(a);
            }
            
            return mesh;
        }
        
        /// <summary>
        /// 创建NURBS曲面网格
        /// </summary>
        public static MeshGeometry3D CreateNurbsSurfaceMesh(NurbsSurfaceData surfaceData, int uDiv, int vDiv)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            
            // 获取节点向量范围
            List<double> knotVectorU = surfaceData.GetKnotVectorU();
            List<double> knotVectorV = surfaceData.GetKnotVectorV();
            
            double uStart = knotVectorU[0];
            double uEnd = knotVectorU[knotVectorU.Count - 1];
            double vStart = knotVectorV[0];
            double vEnd = knotVectorV[knotVectorV.Count - 1];
            
            // 创建网格顶点
            for (int j = 0; j <= vDiv; j++)
            {
                double v = vStart + j * (vEnd - vStart) / vDiv;
                
                for (int i = 0; i <= uDiv; i++)
                {
                    double u = uStart + i * (uEnd - uStart) / uDiv;
                    XYZ point = NurbsSurface.GetPointOnSurface(surfaceData, new UV(u, v));
                    
                    mesh.Positions.Add(new Point3D(point.X, point.Y, point.Z));
                    mesh.TextureCoordinates.Add(new System.Windows.Point((double)i / uDiv, (double)j / vDiv));
                }
            }
            
            // 创建三角形
            for (int j = 0; j < vDiv; j++)
            {
                int offset = (uDiv + 1) * j;
                
                for (int i = 0; i < uDiv; i++)
                {
                    int a = offset + i;
                    int b = offset + i + 1;
                    int c = offset + i + uDiv + 1;
                    int d = offset + i + uDiv + 2;
                    
                    mesh.TriangleIndices.Add(a);
                    mesh.TriangleIndices.Add(c);
                    mesh.TriangleIndices.Add(b);
                    
                    mesh.TriangleIndices.Add(b);
                    mesh.TriangleIndices.Add(c);
                    mesh.TriangleIndices.Add(d);
                }
            }
            
            return mesh;
        }
        
        /// <summary>
        /// XYZW点转换为Point3D
        /// </summary>
        public static Point3D ToPoint3D(this XYZ point)
        {
            return new Point3D(point.X, point.Y, point.Z);
        }

        /// <summary>
        /// XYZW点转换为Point3D
        /// </summary>
        public static Point3D ToPoint3D(this XYZW point)
        {
            return new Point3D(point.X / point.W, point.Y / point.W, point.Z / point.W);
        }
    }
} 