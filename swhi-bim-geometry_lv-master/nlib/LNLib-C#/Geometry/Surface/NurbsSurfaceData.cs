using System;
using System.Collections.Generic;
using LNLib.Mathematics;

namespace LNLib.Geometry.Surface
{
    /// <summary>
    /// NURBS曲面数据类，表示一个NURBS曲面
    /// </summary>
    public class NurbsSurfaceData
    {
        private int _degreeU;
        private int _degreeV;
        private List<double> _knotVectorU;
        private List<double> _knotVectorV;
        private List<List<XYZW>> _controlPoints;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="degreeU">U方向次数</param>
        /// <param name="degreeV">V方向次数</param>
        /// <param name="knotVectorU">U方向节点向量</param>
        /// <param name="knotVectorV">V方向节点向量</param>
        /// <param name="controlPoints">控制点矩阵</param>
        public NurbsSurfaceData(int degreeU, int degreeV, List<double> knotVectorU, List<double> knotVectorV, List<List<XYZW>> controlPoints)
        {
            _degreeU = degreeU;
            _degreeV = degreeV;
            _knotVectorU = new List<double>(knotVectorU);
            _knotVectorV = new List<double>(knotVectorV);
            _controlPoints = new List<List<XYZW>>();
            foreach (var row in controlPoints)
            {
                _controlPoints.Add(new List<XYZW>(row));
            }
        }

        /// <summary>
        /// 获取U方向次数
        /// </summary>
        public int DegreeU => _degreeU;

        /// <summary>
        /// 获取V方向次数
        /// </summary>
        public int DegreeV => _degreeV;

        /// <summary>
        /// 获取控制点行数
        /// </summary>
        public int RowCount => _controlPoints.Count;

        /// <summary>
        /// 获取控制点列数
        /// </summary>
        public int ColumnCount => _controlPoints.Count > 0 ? _controlPoints[0].Count : 0;

        /// <summary>
        /// 获取控制点
        /// </summary>
        /// <param name="row">行索引</param>
        /// <param name="column">列索引</param>
        /// <returns>控制点</returns>
        public XYZW this[int row, int column] => _controlPoints[row][column];

        /// <summary>
        /// 获取U方向节点向量
        /// </summary>
        /// <returns>U方向节点向量</returns>
        public List<double> GetKnotVectorU() => new List<double>(_knotVectorU);

        /// <summary>
        /// 获取V方向节点向量
        /// </summary>
        /// <returns>V方向节点向量</returns>
        public List<double> GetKnotVectorV() => new List<double>(_knotVectorV);

        /// <summary>
        /// 获取控制点矩阵
        /// </summary>
        /// <returns>控制点矩阵</returns>
        public List<List<XYZW>> GetControlPoints()
        {
            List<List<XYZW>> result = new List<List<XYZW>>();
            foreach (var row in _controlPoints)
            {
                result.Add(new List<XYZW>(row));
            }
            return result;
        }
    }
} 