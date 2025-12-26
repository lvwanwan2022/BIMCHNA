using System;

namespace LNLib.Mathematics
{
    /// <summary>
    /// 表示数值区间[min, max]
    /// </summary>
    public struct Interval
    {
        private double _min;
        private double _max;

        /// <summary>
        /// 创建区间[min, max]
        /// </summary>
        public Interval(double min, double max)
        {
            if (min > max)
                throw new ArgumentException("最小值不能大于最大值");
            _min = min;
            _max = max;
        }

        /// <summary>
        /// 区间最小值
        /// </summary>
        public double Min
        {
            get => _min;
            set
            {
                if (value > _max)
                    throw new ArgumentException("最小值不能大于最大值");
                _min = value;
            }
        }

        /// <summary>
        /// 区间最大值
        /// </summary>
        public double Max
        {
            get => _max;
            set
            {
                if (value < _min)
                    throw new ArgumentException("最大值不能小于最小值");
                _max = value;
            }
        }

        /// <summary>
        /// 区间长度
        /// </summary>
        public double Length => _max - _min;

        /// <summary>
        /// 区间中点
        /// </summary>
        public double Middle => (_min + _max) / 2.0;

        /// <summary>
        /// 判断区间是否为空
        /// </summary>
        public bool IsEmpty => Math.Abs(_max - _min) < Constants.DoubleEpsilon;

        /// <summary>
        /// 判断值是否在区间内
        /// </summary>
        public bool Contains(double value, double epsilon = Constants.DoubleEpsilon)
        {
            return value >= _min - epsilon && value <= _max + epsilon;
        }

        /// <summary>
        /// 判断区间是否包含另一个区间
        /// </summary>
        public bool Contains(Interval other, double epsilon = Constants.DoubleEpsilon)
        {
            return other._min >= _min - epsilon && other._max <= _max + epsilon;
        }

        /// <summary>
        /// 判断两个区间是否相交
        /// </summary>
        public bool Intersects(Interval other, double epsilon = Constants.DoubleEpsilon)
        {
            return !(_max + epsilon < other._min || other._max + epsilon < _min);
        }

        /// <summary>
        /// 获取与另一个区间的交集
        /// </summary>
        public Interval Intersect(Interval other)
        {
            if (!Intersects(other))
                throw new ArgumentException("区间不相交");
            return new Interval(Math.Max(_min, other._min), Math.Min(_max, other._max));
        }

        /// <summary>
        /// 获取与另一个区间的并集
        /// </summary>
        public Interval Union(Interval other)
        {
            return new Interval(Math.Min(_min, other._min), Math.Max(_max, other._max));
        }

        /// <summary>
        /// 扩展区间
        /// </summary>
        public void Extend(double value)
        {
            _min = Math.Min(_min, value);
            _max = Math.Max(_max, value);
        }

        /// <summary>
        /// 将值限制在区间内
        /// </summary>
        public double Clamp(double value)
        {
            if (value < _min) return _min;
            if (value > _max) return _max;
            return value;
        }

        /// <summary>
        /// 将[0,1]区间映射到当前区间
        /// </summary>
        public double ParameterAt(double t)
        {
            return (1.0 - t) * _min + t * _max;
        }

        /// <summary>
        /// 将值映射到[0,1]区间
        /// </summary>
        public double NormalizedParameter(double value)
        {
            if (IsEmpty) return 0.0;
            return (value - _min) / Length;
        }

        public override string ToString()
        {
            return $"[{_min}, {_max}]";
        }

        public static bool operator ==(Interval a, Interval b)
        {
            return Math.Abs(a._min - b._min) < Constants.DoubleEpsilon && 
                   Math.Abs(a._max - b._max) < Constants.DoubleEpsilon;
        }

        public static bool operator !=(Interval a, Interval b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is Interval interval)
                return this == interval;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_min, _max);
        }
    }
} 