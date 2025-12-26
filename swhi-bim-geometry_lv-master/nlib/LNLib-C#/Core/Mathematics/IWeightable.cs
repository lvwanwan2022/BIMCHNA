namespace LNLib.Mathematics
{
    /// <summary>
    /// 表示可带权重的对象接口
    /// </summary>
    /// <typeparam name="T">具体实现类型</typeparam>
    public interface IWeightable<T> where T : class
    {
        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="other">要添加的对象</param>
        /// <returns>添加结果</returns>
        T Add(T other);

        /// <summary>
        /// 减法操作
        /// </summary>
        /// <param name="other">要减去的对象</param>
        /// <returns>减法结果</returns>
        T Subtract(T other);

        /// <summary>
        /// 乘法操作（标量乘法）
        /// </summary>
        /// <param name="scalar">标量值</param>
        /// <returns>乘法结果</returns>
        T Multiply(double scalar);

        /// <summary>
        /// 除法操作（标量除法）
        /// </summary>
        /// <param name="scalar">标量值</param>
        /// <returns>除法结果</returns>
        T Divide(double scalar);
    }
} 