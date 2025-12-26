using System;
using LNLib.Mathematics;
using LNLib.Geometry.Primitives;

namespace LNLib.Geometry.Primitives
{
    /// <summary>
    /// 表示三维空间中的椭圆
    /// </summary>
    public class Ellipse
    {
        private XYZ _center;
        private XYZ _normal;
        private XYZ _majorAxis;
        private double _majorRadius;
        private double _minorRadius;

        /// <summary>
        /// 通过中心点、法向量、主轴方向、长轴半径和短轴半径创建椭圆
        /// </summary>
        public Ellipse(XYZ center, XYZ normal, XYZ majorAxis, double majorRadius, double minorRadius)
        {
            if (normal.IsZero())
                throw new ArgumentException("法向量不能为零向量");
            if (majorAxis.IsZero())
                throw new ArgumentException("主轴方向不能为零向量");
            if (Math.Abs(normal.DotProduct(majorAxis)) > Constants.DoubleEpsilon)
                throw new ArgumentException("主轴方向必须垂直于法向量");
            if (majorRadius <= 0)
                throw new ArgumentException("长轴半径必须为正数");
            if (minorRadius <= 0)
                throw new ArgumentException("短轴半径必须为正数");
            if (majorRadius < minorRadius)
                throw new ArgumentException("长轴半径必须大于或等于短轴半径");

            _center = center;
            _normal = normal.Normalize();
            _majorAxis = majorAxis.Normalize();
            _majorRadius = majorRadius;
            _minorRadius = minorRadius;
        }

        /// <summary>
        /// 椭圆中心点
        /// </summary>
        public XYZ Center
        {
            get => _center;
            set => _center = value;
        }

        /// <summary>
        /// 椭圆所在平面的单位法向量
        /// </summary>
        public XYZ Normal
        {
            get => _normal;
            set
            {
                if (value.IsZero())
                    throw new ArgumentException("法向量不能为零向量");
                if (Math.Abs(value.DotProduct(_majorAxis)) > Constants.DoubleEpsilon)
                    throw new ArgumentException("法向量必须垂直于主轴方向");
                _normal = value.Normalize();
            }
        }

        /// <summary>
        /// 椭圆长轴的单位方向向量
        /// </summary>
        public XYZ MajorAxis
        {
            get => _majorAxis;
            set
            {
                if (value.IsZero())
                    throw new ArgumentException("主轴方向不能为零向量");
                if (Math.Abs(value.DotProduct(_normal)) > Constants.DoubleEpsilon)
                    throw new ArgumentException("主轴方向必须垂直于法向量");
                _majorAxis = value.Normalize();
            }
        }

        /// <summary>
        /// 椭圆短轴的单位方向向量
        /// </summary>
        public XYZ MinorAxis => _normal.CrossProduct(_majorAxis);

        /// <summary>
        /// 长轴半径
        /// </summary>
        public double MajorRadius
        {
            get => _majorRadius;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("长轴半径必须为正数");
                if (value < _minorRadius)
                    throw new ArgumentException("长轴半径必须大于或等于短轴半径");
                _majorRadius = value;
            }
        }

        /// <summary>
        /// 短轴半径
        /// </summary>
        public double MinorRadius
        {
            get => _minorRadius;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("短轴半径必须为正数");
                if (_majorRadius < value)
                    throw new ArgumentException("短轴半径必须小于或等于长轴半径");
                _minorRadius = value;
            }
        }

        /// <summary>
        /// 椭圆的离心率
        /// </summary>
        public double Eccentricity
        {
            get
            {
                if (Math.Abs(_majorRadius - _minorRadius) < Constants.DoubleEpsilon)
                    return 0.0; // 圆的离心率为0
                
                double e = Math.Sqrt(1.0 - (_minorRadius * _minorRadius) / (_majorRadius * _majorRadius));
                return e;
            }
        }

        /// <summary>
        /// 椭圆的周长（近似值）
        /// </summary>
        public double Circumference
        {
            get
            {
                double a = _majorRadius;
                double b = _minorRadius;
                
                // 使用Ramanujan近似公式
                double h = Math.Pow(a - b, 2) / Math.Pow(a + b, 2);
                return Constants.Pi * (a + b) * (1 + (3 * h) / (10 + Math.Sqrt(4 - 3 * h)));
            }
        }

        /// <summary>
        /// 椭圆的面积
        /// </summary>
        public double Area => Constants.Pi * _majorRadius * _minorRadius;

        /// <summary>
        /// 获取参数t对应的点，t在[0,1]区间表示一周
        /// </summary>
        public XYZ PointAt(double t)
        {
            double angle = t * Constants.TwoPi;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return _center.Add(_majorAxis.Multiply(_majorRadius * cos)).Add(MinorAxis.Multiply(_minorRadius * sin));
        }

        /// <summary>
        /// 获取参数t对应的切向量，t在[0,1]区间表示一周
        /// </summary>
        public XYZ TangentAt(double t)
        {
            double angle = t * Constants.TwoPi;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            XYZ result = _majorAxis.Multiply(-_majorRadius * sin).Add(MinorAxis.Multiply(_minorRadius * cos));
            return result.Normalize();
        }

        /// <summary>
        /// 计算点到椭圆的最短距离（近似值）
        /// </summary>
        public double DistanceTo(XYZ point)
        {
            // 先将点投影到椭圆所在平面
            Plane plane = new Plane(_center, _normal);
            XYZ projection = plane.ProjectPoint(point);
            
            // 计算投影点相对于椭圆中心的坐标
            XYZ v = projection.Subtract(_center);
            
            // 计算投影点在椭圆局部坐标系下的坐标
            double x = v.DotProduct(_majorAxis);
            double y = v.DotProduct(MinorAxis);
            
            // 计算投影点到椭圆的距离（迭代求解）
            double distance = DistanceToEllipse(x, y);
            
            // 计算点到投影点的距离
            double heightDistance = point.Distance(projection);
            
            // 返回总距离
            return Math.Sqrt(distance * distance + heightDistance * heightDistance);
        }

        /// <summary>
        /// 计算平面上点(x,y)到椭圆的最短距离（迭代求解）
        /// </summary>
        private double DistanceToEllipse(double x, double y)
        {
            // 特殊情况：点在椭圆中心
            if (Math.Abs(x) < Constants.DoubleEpsilon && Math.Abs(y) < Constants.DoubleEpsilon)
                return Math.Min(_majorRadius, _minorRadius);
            
            // 特殊情况：点在椭圆上
            double value = (x * x) / (_majorRadius * _majorRadius) + (y * y) / (_minorRadius * _minorRadius);
            if (Math.Abs(value - 1.0) < Constants.DoubleEpsilon)
                return 0.0;
            
            // 椭圆参数方程: (a*cos(t), b*sin(t))
            // 点到椭圆上的点的距离的平方: (x-a*cos(t))^2 + (y-b*sin(t))^2
            // 最小值对应的t满足: a*x*sin(t) + b*y*cos(t) = 0
            
            // 使用牛顿迭代法求解
            double a = _majorRadius;
            double b = _minorRadius;
            double phi = Math.Atan2(a * y, b * x);
            
            // 迭代求解
            int maxIter = 10;
            double tol = 1e-8;
            for (int i = 0; i < maxIter; i++)
            {
                double cos_phi_iter = Math.Cos(phi);
                double sin_phi_iter = Math.Sin(phi);
                
                double f = a * x * sin_phi_iter - b * y * cos_phi_iter;
                double df = a * x * cos_phi_iter + b * y * sin_phi_iter;
                
                double delta = f / df;
                phi -= delta;
                
                if (Math.Abs(delta) < tol)
                    break;
            }
            
            // 计算最近点
            double cos_phi = Math.Cos(phi);
            double sin_phi = Math.Sin(phi);
            double closest_x = a * cos_phi;
            double closest_y = b * sin_phi;
            
            // 计算距离
            double dx = x - closest_x;
            double dy = y - closest_y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 判断点是否在椭圆上
        /// </summary>
        public bool Contains(XYZ point, double epsilon = Constants.DoubleEpsilon)
        {
            return DistanceTo(point) < epsilon;
        }

        /// <summary>
        /// 变换椭圆
        /// </summary>
        public Ellipse Transform(Matrix4 transform)
        {
            XYZ newCenter = transform.TransformPoint(_center);
            XYZ newNormal = transform.TransformVector(_normal);
            XYZ newMajorAxis = transform.TransformVector(_majorAxis);
            
            // 计算变换后的半轴长度
            double scaleA = newMajorAxis.Length();
            double scaleB = transform.TransformVector(MinorAxis).Length();
            
            double newMajorRadius = _majorRadius * scaleA;
            double newMinorRadius = _minorRadius * scaleB;
            
            // 确保长轴半径大于短轴半径
            if (newMajorRadius < newMinorRadius)
            {
                double temp = newMajorRadius;
                newMajorRadius = newMinorRadius;
                newMinorRadius = temp;
                
                newMajorAxis = transform.TransformVector(MinorAxis).Normalize();
            }
            else
            {
                newMajorAxis = newMajorAxis.Normalize();
            }
            
            return new Ellipse(newCenter, newNormal, newMajorAxis, newMajorRadius, newMinorRadius);
        }
    }
} 