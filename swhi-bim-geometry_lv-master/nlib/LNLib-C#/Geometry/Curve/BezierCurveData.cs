using System;
using System.Collections.Generic;
using LNLib.Mathematics;

namespace LNLib.Geometry.Curve
{
    /// <summary>
    /// 表示Bezier曲线的数据结构
    /// </summary>
    /// <typeparam name="T">控制点的类型，必须实现IWeightable接口</typeparam>
    public class BezierCurveData<T> where T : class, IWeightable<T>
    {
        private List<T> _controlPoints;
        private int _degree;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="degree">次数</param>
        /// <param name="controlPoints">控制点列表</param>
        public BezierCurveData(int degree, List<T> controlPoints)
        {
            _degree = degree;
            _controlPoints = new List<T>(controlPoints);
            
            if (_controlPoints.Count < 2)
                throw new ArgumentException("Bezier曲线至少需要2个控制点", nameof(controlPoints));
        }

        /// <summary>
        /// 获取次数
        /// </summary>
        public int Degree => _degree;

        /// <summary>
        /// 获取控制点数量
        /// </summary>
        public int Count => _controlPoints.Count;

        /// <summary>
        /// 获取控制点
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>控制点</returns>
        public T this[int index] => _controlPoints[index];

        /// <summary>
        /// 获取控制点列表
        /// </summary>
        /// <returns>控制点列表</returns>
        public List<T> GetControlPoints() => new List<T>(_controlPoints);

        /// <summary>
        /// 在指定位置插入控制点
        /// </summary>
        /// <param name="index">插入位置</param>
        /// <param name="controlPoint">要插入的控制点</param>
        public void InsertControlPoint(int index, T controlPoint)
        {
            if (index < 0 || index > _controlPoints.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            if (controlPoint == null)
                throw new ArgumentNullException(nameof(controlPoint));
            
            _controlPoints.Insert(index, controlPoint);
        }

        /// <summary>
        /// 添加控制点到末尾
        /// </summary>
        /// <param name="controlPoint">要添加的控制点</param>
        public void AddControlPoint(T controlPoint)
        {
            if (controlPoint == null)
                throw new ArgumentNullException(nameof(controlPoint));
            
            _controlPoints.Add(controlPoint);
        }

        /// <summary>
        /// 移除指定索引处的控制点
        /// </summary>
        /// <param name="index">要移除的控制点索引</param>
        public void RemoveControlPoint(int index)
        {
            if (index < 0 || index >= _controlPoints.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            if (_controlPoints.Count <= 2)
                throw new InvalidOperationException("Bezier曲线至少需要2个控制点");
            
            _controlPoints.RemoveAt(index);
        }

        /// <summary>
        /// 设置指定索引处的控制点
        /// </summary>
        /// <param name="index">控制点索引</param>
        /// <param name="controlPoint">新的控制点</param>
        public void SetControlPoint(int index, T controlPoint)
        {
            if (index < 0 || index >= _controlPoints.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            if (controlPoint == null)
                throw new ArgumentNullException(nameof(controlPoint));
            
            _controlPoints[index] = controlPoint;
        }

        /// <summary>
        /// 清除所有控制点
        /// </summary>
        public void Clear()
        {
            _controlPoints.Clear();
        }
    }
} 