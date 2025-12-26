using System;

namespace LNLib.Mathematics
{
    /// <summary>
    /// 表示三维轴向包围盒
    /// </summary>
    public class BoundingBox
    {
        private XYZ _min;
        private XYZ _max;

        /// <summary>
        /// 创建包围盒
        /// </summary>
        public BoundingBox(XYZ min, XYZ max)
        {
            if (min.X > max.X || min.Y > max.Y || min.Z > max.Z)
                throw new ArgumentException("最小点的坐标不能大于最大点的坐标");
            _min = min;
            _max = max;
        }

        /// <summary>
        /// 最小点
        /// </summary>
        public XYZ Min
        {
            get => _min;
            set
            {
                if (value.X > _max.X || value.Y > _max.Y || value.Z > _max.Z)
                    throw new ArgumentException("最小点的坐标不能大于最大点的坐标");
                _min = value;
            }
        }

        /// <summary>
        /// 最大点
        /// </summary>
        public XYZ Max
        {
            get => _max;
            set
            {
                if (_min.X > value.X || _min.Y > value.Y || _min.Z > value.Z)
                    throw new ArgumentException("最大点的坐标不能小于最小点的坐标");
                _max = value;
            }
        }

        /// <summary>
        /// 中心点
        /// </summary>
        public XYZ Center => new XYZ(
            (_min.X + _max.X) / 2.0,
            (_min.Y + _max.Y) / 2.0,
            (_min.Z + _max.Z) / 2.0);

        /// <summary>
        /// 对角线向量
        /// </summary>
        public XYZ Diagonal => _max.Subtract(_min);

        /// <summary>
        /// X方向区间
        /// </summary>
        public Interval IntervalX => new Interval(_min.X, _max.X);

        /// <summary>
        /// Y方向区间
        /// </summary>
        public Interval IntervalY => new Interval(_min.Y, _max.Y);

        /// <summary>
        /// Z方向区间
        /// </summary>
        public Interval IntervalZ => new Interval(_min.Z, _max.Z);

        /// <summary>
        /// 判断包围盒是否为空
        /// </summary>
        public bool IsEmpty => Diagonal.IsZero();

        /// <summary>
        /// 判断点是否在包围盒内
        /// </summary>
        public bool Contains(XYZ point, double epsilon = Constants.DoubleEpsilon)
        {
            return point.X >= _min.X - epsilon && point.X <= _max.X + epsilon &&
                   point.Y >= _min.Y - epsilon && point.Y <= _max.Y + epsilon &&
                   point.Z >= _min.Z - epsilon && point.Z <= _max.Z + epsilon;
        }

        /// <summary>
        /// 判断包围盒是否包含另一个包围盒
        /// </summary>
        public bool Contains(BoundingBox other, double epsilon = Constants.DoubleEpsilon)
        {
            return other._min.X >= _min.X - epsilon && other._max.X <= _max.X + epsilon &&
                   other._min.Y >= _min.Y - epsilon && other._max.Y <= _max.Y + epsilon &&
                   other._min.Z >= _min.Z - epsilon && other._max.Z <= _max.Z + epsilon;
        }

        /// <summary>
        /// 判断两个包围盒是否相交
        /// </summary>
        public bool Intersects(BoundingBox other, double epsilon = Constants.DoubleEpsilon)
        {
            return !(_max.X + epsilon < other._min.X || other._max.X + epsilon < _min.X ||
                    _max.Y + epsilon < other._min.Y || other._max.Y + epsilon < _min.Y ||
                    _max.Z + epsilon < other._min.Z || other._max.Z + epsilon < _min.Z);
        }

        /// <summary>
        /// 获取与另一个包围盒的交集
        /// </summary>
        public BoundingBox Intersect(BoundingBox other)
        {
            if (!Intersects(other))
                throw new ArgumentException("包围盒不相交");

            return new BoundingBox(
                new XYZ(
                    Math.Max(_min.X, other._min.X),
                    Math.Max(_min.Y, other._min.Y),
                    Math.Max(_min.Z, other._min.Z)),
                new XYZ(
                    Math.Min(_max.X, other._max.X),
                    Math.Min(_max.Y, other._max.Y),
                    Math.Min(_max.Z, other._max.Z)));
        }

        /// <summary>
        /// 获取与另一个包围盒的并集
        /// </summary>
        public BoundingBox Union(BoundingBox other)
        {
            return new BoundingBox(
                new XYZ(
                    Math.Min(_min.X, other._min.X),
                    Math.Min(_min.Y, other._min.Y),
                    Math.Min(_min.Z, other._min.Z)),
                new XYZ(
                    Math.Max(_max.X, other._max.X),
                    Math.Max(_max.Y, other._max.Y),
                    Math.Max(_max.Z, other._max.Z)));
        }

        /// <summary>
        /// 扩展包围盒以包含指定点
        /// </summary>
        public void Extend(XYZ point)
        {
            _min = new XYZ(
                Math.Min(_min.X, point.X),
                Math.Min(_min.Y, point.Y),
                Math.Min(_min.Z, point.Z));
            _max = new XYZ(
                Math.Max(_max.X, point.X),
                Math.Max(_max.Y, point.Y),
                Math.Max(_max.Z, point.Z));
        }

        /// <summary>
        /// 按指定距离扩展包围盒
        /// </summary>
        public void Inflate(double delta)
        {
            _min = new XYZ(_min.X - delta, _min.Y - delta, _min.Z - delta);
            _max = new XYZ(_max.X + delta, _max.Y + delta, _max.Z + delta);
        }

        /// <summary>
        /// 变换包围盒
        /// </summary>
        public BoundingBox Transform(Matrix4 transform)
        {
            XYZ[] corners = new XYZ[8];
            corners[0] = new XYZ(_min.X, _min.Y, _min.Z);
            corners[1] = new XYZ(_min.X, _min.Y, _max.Z);
            corners[2] = new XYZ(_min.X, _max.Y, _min.Z);
            corners[3] = new XYZ(_min.X, _max.Y, _max.Z);
            corners[4] = new XYZ(_max.X, _min.Y, _min.Z);
            corners[5] = new XYZ(_max.X, _min.Y, _max.Z);
            corners[6] = new XYZ(_max.X, _max.Y, _min.Z);
            corners[7] = new XYZ(_max.X, _max.Y, _max.Z);

            XYZ newMin = transform.TransformPoint(corners[0]);
            XYZ newMax = newMin;

            for (int i = 1; i < 8; i++)
            {
                XYZ point = transform.TransformPoint(corners[i]);
                newMin = new XYZ(
                    Math.Min(newMin.X, point.X),
                    Math.Min(newMin.Y, point.Y),
                    Math.Min(newMin.Z, point.Z));
                newMax = new XYZ(
                    Math.Max(newMax.X, point.X),
                    Math.Max(newMax.Y, point.Y),
                    Math.Max(newMax.Z, point.Z));
            }

            return new BoundingBox(newMin, newMax);
        }
    }
} 