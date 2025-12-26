using System;

namespace LNLib.Mathematics
{
    /// <summary>
    /// 表示4x4矩阵
    /// </summary>
    public struct Matrix4
    {
        private double[,] _matrix;

        /// <summary>
        /// 创建单位矩阵
        /// </summary>
        public Matrix4()
        {
            _matrix = new double[4, 4];
            for (int i = 0; i < 4; i++)
                _matrix[i, i] = 1.0;
        }

        /// <summary>
        /// 从16个元素创建矩阵
        /// </summary>
        public Matrix4(
            double m11, double m12, double m13, double m14,
            double m21, double m22, double m23, double m24,
            double m31, double m32, double m33, double m34,
            double m41, double m42, double m43, double m44)
        {
            _matrix = new double[4, 4] {
                { m11, m12, m13, m14 },
                { m21, m22, m23, m24 },
                { m31, m32, m33, m34 },
                { m41, m42, m43, m44 }
            };
        }

        /// <summary>
        /// 获取或设置指定位置的元素
        /// </summary>
        public double this[int row, int col]
        {
            get
            {
                if (row < 0 || row > 3 || col < 0 || col > 3)
                    throw new ArgumentOutOfRangeException();
                return _matrix[row, col];
            }
            set
            {
                if (row < 0 || row > 3 || col < 0 || col > 3)
                    throw new ArgumentOutOfRangeException();
                _matrix[row, col] = value;
            }
        }

        /// <summary>
        /// 创建平移矩阵
        /// </summary>
        public static Matrix4 CreateTranslation(XYZ translation)
        {
            return new Matrix4(
                1, 0, 0, translation.X,
                0, 1, 0, translation.Y,
                0, 0, 1, translation.Z,
                0, 0, 0, 1);
        }

        /// <summary>
        /// 创建缩放矩阵
        /// </summary>
        public static Matrix4 CreateScale(double scale)
        {
            return new Matrix4(
                scale, 0, 0, 0,
                0, scale, 0, 0,
                0, 0, scale, 0,
                0, 0, 0, 1);
        }

        /// <summary>
        /// 创建绕X轴旋转的矩阵
        /// </summary>
        public static Matrix4 CreateRotationX(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Matrix4(
                1, 0, 0, 0,
                0, cos, -sin, 0,
                0, sin, cos, 0,
                0, 0, 0, 1);
        }

        /// <summary>
        /// 创建绕Y轴旋转的矩阵
        /// </summary>
        public static Matrix4 CreateRotationY(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Matrix4(
                cos, 0, sin, 0,
                0, 1, 0, 0,
                -sin, 0, cos, 0,
                0, 0, 0, 1);
        }

        /// <summary>
        /// 创建绕Z轴旋转的矩阵
        /// </summary>
        public static Matrix4 CreateRotationZ(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Matrix4(
                cos, -sin, 0, 0,
                sin, cos, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        public Matrix4 Multiply(Matrix4 other)
        {
            Matrix4 result = new Matrix4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        result[i, j] += this[i, k] * other[k, j];
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 转置矩阵
        /// </summary>
        public Matrix4 Transpose()
        {
            Matrix4 result = new Matrix4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i, j] = this[j, i];
                }
            }
            return result;
        }

        /// <summary>
        /// 变换点
        /// </summary>
        public XYZ TransformPoint(XYZ point)
        {
            return new XYZ(
                point.X * this[0, 0] + point.Y * this[0, 1] + point.Z * this[0, 2] + this[0, 3],
                point.X * this[1, 0] + point.Y * this[1, 1] + point.Z * this[1, 2] + this[1, 3],
                point.X * this[2, 0] + point.Y * this[2, 1] + point.Z * this[2, 2] + this[2, 3]);
        }

        /// <summary>
        /// 变换向量
        /// </summary>
        public XYZ TransformVector(XYZ vector)
        {
            return new XYZ(
                vector.X * this[0, 0] + vector.Y * this[0, 1] + vector.Z * this[0, 2],
                vector.X * this[1, 0] + vector.Y * this[1, 1] + vector.Z * this[1, 2],
                vector.X * this[2, 0] + vector.Y * this[2, 1] + vector.Z * this[2, 2]);
        }

        public static Matrix4 operator *(Matrix4 a, Matrix4 b) => a.Multiply(b);
    }
} 