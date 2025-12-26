using System;
using System.Collections.Generic;
using System.Linq;
using LNLib.Mathematics;

namespace LNLib.Algorithm
{
    /// <summary>
    /// 节点向量工具类
    /// </summary>
    public static class KnotVectorUtils
    {
        /// <summary>
        /// 递归插入中点节点
        /// </summary>
        private static void InsertMidKnotCore(List<double> uniqueKnotVector, List<double> insert, int limitNumber)
        {
            if (insert.Count == limitNumber)
            {
                return;
            }
            else
            {
                double standard = Constants.DoubleEpsilon;
                int index = -1;
                for (int i = 0; i < uniqueKnotVector.Count - 1; i++)
                {
                    double delta = uniqueKnotVector[i + 1] - uniqueKnotVector[i];
                    if (delta > standard)
                    {
                        standard = delta;
                        index = i;
                    }
                }
                double current = uniqueKnotVector[index] + standard / 2.0;
                uniqueKnotVector.Add(current);
                uniqueKnotVector.Sort();
                insert.Add(current);

                InsertMidKnotCore(uniqueKnotVector, insert, limitNumber);
            }
        }

        /// <summary>
        /// 获取连续性
        /// </summary>
        /// <param name="degree">次数</param>
        /// <param name="knotVector">节点向量</param>
        /// <param name="knot">节点</param>
        /// <returns>连续性</returns>
        public static int GetContinuity(int degree, IList<double> knotVector, double knot)
        {
            if (degree <= 0)
                throw new ArgumentException("次数必须大于零", nameof(degree));
                
            int multi = Polynomials.GetKnotMultiplicity(knotVector.ToArray(), knot);
            return degree - multi;
        }

        /// <summary>
        /// 重新缩放节点向量
        /// </summary>
        /// <param name="knotVector">原始节点向量</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>缩放后的节点向量</returns>
        public static List<double> Rescale(IList<double> knotVector, double min, double max)
        {
            double originMin = knotVector[0];
            double originMax = knotVector[knotVector.Count - 1];
            double k = (max - min) / (originMax - originMin);

            int size = knotVector.Count;
            List<double> result = new List<double>(size);
            for (int i = 0; i < size; i++)
            {
                result.Add((k * knotVector[i] - originMin) + min);
            }
            return result;
        }

        /// <summary>
        /// 获取插入的节点元素
        /// </summary>
        /// <param name="degree">次数</param>
        /// <param name="knotVector">节点向量</param>
        /// <param name="startParam">起始参数</param>
        /// <param name="endParam">结束参数</param>
        /// <returns>插入的节点元素</returns>
        public static List<double> GetInsertedKnotElement(int degree, IList<double> knotVector, double startParam, double endParam)
        {
            if (degree < 0)
                throw new ArgumentException("次数必须大于等于零", nameof(degree));
                
            if (knotVector.Count <= 0)
                throw new ArgumentException("节点向量大小必须大于零", nameof(knotVector));
                
            if (!ValidationUtils.IsValidKnotVector(knotVector))
                throw new ArgumentException("节点向量必须是非递减的实数序列", nameof(knotVector));

            List<double> result = new List<double>();
            int startMulti = Polynomials.GetKnotMultiplicity(knotVector.ToArray(), startParam);
            if (startMulti < degree)
            {
                for (int i = 0; i < degree - startMulti; i++)
                {
                    result.Add(startParam);
                }
            }

            int endMulti = Polynomials.GetKnotMultiplicity(knotVector.ToArray(), endParam);
            if (endMulti < degree)
            {
                for (int i = 0; i < degree - endMulti; i++)
                {
                    result.Add(endParam);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取节点重复度映射
        /// </summary>
        /// <param name="knotVector">节点向量</param>
        /// <returns>节点重复度映射</returns>
        public static Dictionary<double, int> GetKnotMultiplicityMap(IList<double> knotVector)
        {
            Dictionary<double, int> result = new Dictionary<double, int>();

            for (int i = 0; i < knotVector.Count; i++)
            {
                double knot = knotVector[i];
                if (!result.ContainsKey(knot))
                {
                    int multi = Polynomials.GetKnotMultiplicity(knotVector.ToArray(), knotVector[i]);
                    result.Add(knotVector[i], multi);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取内部节点重复度映射
        /// </summary>
        /// <param name="knotVector">节点向量</param>
        /// <returns>内部节点重复度映射</returns>
        public static Dictionary<double, int> GetInternalKnotMultiplicityMap(IList<double> knotVector)
        {
            Dictionary<double, int> result = GetKnotMultiplicityMap(knotVector);
            if (result.Count > 0)
            {
                // 移除第一个元素和最后一个元素
                double firstKey = result.Keys.First();
                double lastKey = result.Keys.Last();
                result.Remove(firstKey);
                result.Remove(lastKey);
            }
            return result;
        }

        /// <summary>
        /// 获取插入的节点元素
        /// </summary>
        /// <param name="knotVector0">节点向量0</param>
        /// <param name="knotVector1">节点向量1</param>
        /// <param name="insertElements0">输出的插入元素0</param>
        /// <param name="insertElements1">输出的插入元素1</param>
        public static void GetInsertedKnotElement(IList<double> knotVector0, IList<double> knotVector1, out List<double> insertElements0, out List<double> insertElements1)
        {
            insertElements0 = new List<double>();
            insertElements1 = new List<double>();
            
            Dictionary<double, int> map0 = GetKnotMultiplicityMap(knotVector0);
            Dictionary<double, int> map1 = GetKnotMultiplicityMap(knotVector1);

            foreach (var kvp in map0)
            {
                double key0 = kvp.Key;
                int count0 = kvp.Value;

                if (!map1.TryGetValue(key0, out int count1))
                {
                    for (int i = 0; i < count0; i++)
                    {
                        insertElements1.Add(key0);
                    }
                }
                else
                {
                    if (count0 > count1)
                    {
                        int times = count0 - count1;
                        for (int j = 0; j < times; j++)
                        {
                            insertElements1.Add(key0);
                        }
                    }
                    else
                    {
                        int times = count1 - count0;
                        for (int j = 0; j < times; j++)
                        {
                            insertElements0.Add(key0);
                        }
                    }
                }
            }

            foreach (var kvp in map1)
            {
                double key1 = kvp.Key;
                int count1 = kvp.Value;

                if (!map0.ContainsKey(key1))
                {
                    for (int i = 0; i < count1; i++)
                    {
                        insertElements0.Add(key1);
                    }
                }
            }

            insertElements0.Sort();
            insertElements1.Sort();
        }

        /// <summary>
        /// 获取插入的节点元素列表
        /// </summary>
        /// <param name="knotVectors">节点向量列表</param>
        /// <returns>插入的节点元素列表</returns>
        public static List<List<double>> GetInsertedKnotElements(IList<IList<double>> knotVectors)
        {
            Dictionary<double, int> finalMap = new Dictionary<double, int>();
            for (int i = 0; i < knotVectors.Count; i++)
            {
                var kv = knotVectors[i];
                Dictionary<double, int> map = GetKnotMultiplicityMap(kv);
                foreach (var kvp in map)
                {
                    double key = kvp.Key;
                    int count = kvp.Value;
                    if (!finalMap.TryGetValue(key, out int currentCount))
                    {
                        finalMap.Add(key, count);
                    }
                    else
                    {
                        if (currentCount < count)
                        {
                            finalMap[key] = count;
                        }
                    }
                }
            }

            List<List<double>> result = new List<List<double>>();
            for (int i = 0; i < knotVectors.Count; i++)
            {
                var kv = knotVectors[i];
                Dictionary<double, int> map = GetKnotMultiplicityMap(kv);

                List<double> insertElements = new List<double>();
                foreach (var kvp in finalMap)
                {
                    double key = kvp.Key;
                    int count = kvp.Value;

                    if (!map.TryGetValue(key, out int currentCount))
                    {
                        for (int j = 0; j < count; j++)
                        {
                            insertElements.Add(key);
                        }
                    }
                    else
                    {
                        int times = count - currentCount;
                        for (int j = 0; j < times; j++)
                        {
                            insertElements.Add(key);
                        }
                    }
                }
                result.Add(insertElements);
            }
            return result;
        }

        /// <summary>
        /// 获取插入的节点元素
        /// </summary>
        /// <param name="insertKnotsNumber">插入节点数量</param>
        /// <param name="knotVector">节点向量</param>
        /// <returns>插入的节点元素</returns>
        public static List<double> GetInsertedKnotElements(int insertKnotsNumber, IList<double> knotVector)
        {
            List<double> uniqueKnots = new List<double>();
            foreach (var knot in knotVector)
            {
                if (!uniqueKnots.Contains(knot))
                {
                    uniqueKnots.Add(knot);
                }
            }

            List<double> result = new List<double>();
            InsertMidKnotCore(uniqueKnots, result, insertKnotsNumber);
            
            if (result.Count < insertKnotsNumber)
            {
                int actualInsert = result.Count;
                int gapNumber = insertKnotsNumber - actualInsert;
                
                // 剩余的节点随机平均分配
                for (int i = 0; i < uniqueKnots.Count - 1; i++)
                {
                    double a = uniqueKnots[i];
                    double b = uniqueKnots[i + 1];
                    
                    if (Math.Abs(b - a) <= Constants.DoubleEpsilon)
                        continue;
                        
                    int insertNumber = gapNumber / (uniqueKnots.Count - 1);
                    if (i == uniqueKnots.Count - 2)
                    {
                        insertNumber = gapNumber - (uniqueKnots.Count - 2) * (gapNumber / (uniqueKnots.Count - 1));
                    }
                    
                    if (insertNumber > 0)
                    {
                        double step = (b - a) / (insertNumber + 1);
                        for (int j = 1; j <= insertNumber; j++)
                        {
                            result.Add(a + j * step);
                        }
                    }
                }
            }
            
            result.Sort();
            return result;
        }

        /// <summary>
        /// 获取均匀分布的插入节点元素
        /// </summary>
        /// <param name="insertKnotsNumber">插入节点数量</param>
        /// <param name="start">起始参数</param>
        /// <param name="end">结束参数</param>
        /// <returns>插入的节点元素</returns>
        public static List<double> GetUniformInsertKnotElements(int insertKnotsNumber, double start, double end)
        {
            if (insertKnotsNumber <= 0)
                return new List<double>();
                
            List<double> result = new List<double>();
            double step = (end - start) / (insertKnotsNumber + 1);
            for (int i = 1; i <= insertKnotsNumber; i++)
            {
                result.Add(start + i * step);
            }
            return result;
        }
        
        /// <summary>
        /// 获取区间
        /// </summary>
        /// <param name="knotSpanIndex">节点跨度索引</param>
        /// <param name="knotVector">节点向量</param>
        /// <returns>区间</returns>
        public static Interval GetKnotInterval(int knotSpanIndex, IList<double> knotVector)
        {
            return new Interval(knotVector[knotSpanIndex], knotVector[knotSpanIndex + 1]);
        }
    }
} 