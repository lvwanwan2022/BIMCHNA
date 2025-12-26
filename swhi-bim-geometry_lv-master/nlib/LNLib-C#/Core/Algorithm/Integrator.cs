using System;
using System.Collections.Generic;
using LNLib.Mathematics;

namespace LNLib.Algorithm
{
    /// <summary>
    /// 数值积分器
    /// </summary>
    public static class Integrator
    {
        /// <summary>
        /// 用于一元函数积分的委托
        /// </summary>
        /// <param name="x">自变量</param>
        /// <param name="customData">自定义数据</param>
        /// <returns>函数值</returns>
        public delegate double IntegrationFunction(double x, object customData);

        /// <summary>
        /// 用于二元函数积分的委托
        /// </summary>
        /// <param name="u">第一个自变量</param>
        /// <param name="v">第二个自变量</param>
        /// <param name="customData">自定义数据</param>
        /// <returns>函数值</returns>
        public delegate double BinaryIntegrationFunction(double u, double v, object customData);

        /// <summary>
        /// 使用辛普森法则计算一元函数积分
        /// </summary>
        /// <param name="function">待积分函数</param>
        /// <param name="customData">自定义数据</param>
        /// <param name="start">积分下限</param>
        /// <param name="end">积分上限</param>
        /// <returns>积分结果</returns>
        public static double Simpson(IntegrationFunction function, object customData, double start, double end)
        {
            double st = function(start, customData);
            double mt = function((start + end) / 2.0, customData);
            double et = function(end, customData);
            double result = ((end - start) / 6.0) * (st + 4 * mt + et);
            return result;
        }

        /// <summary>
        /// 使用辛普森法则计算二元函数积分
        /// </summary>
        /// <param name="function">待积分函数</param>
        /// <param name="customData">自定义数据</param>
        /// <param name="uStart">第一个变量积分下限</param>
        /// <param name="uEnd">第一个变量积分上限</param>
        /// <param name="vStart">第二个变量积分下限</param>
        /// <param name="vEnd">第二个变量积分上限</param>
        /// <returns>积分结果</returns>
        public static double Simpson(BinaryIntegrationFunction function, object customData, double uStart, double uEnd, double vStart, double vEnd)
        {
            double du = uEnd - uStart;
            double dv = vEnd - vStart;
            double hdu = 0.5 * du;
            double hdv = 0.5 * dv;
            
            // 采样9个点及其权重
            int sampleNumber = 9;
            int patches = 4;
            double[] uvw = new double[]
            {
                uStart,         vStart,         1,
                uStart,         vStart + hdv,   4,
                uStart,         vEnd,           1,
                uStart + hdu,   vStart,         4,
                uStart + hdu,   vStart + hdv,   16,
                uStart + hdu,   vEnd,           4,
                uEnd,           vStart,         1,
                uEnd,           vStart + hdv,   4,
                uEnd,           vEnd,           1,
            };
            
            double sum = 0;
            for(int i = 0; i < sampleNumber; ++i)
            {
                int baseIndex = i * 3;
                double u = uvw[baseIndex];
                double v = uvw[baseIndex + 1];
                double w = uvw[baseIndex + 2];
                double f = function(u, v, customData);
                sum += w * f;
            }

            sum *= du * dv / (sampleNumber * patches);
            return sum;
        }

        /// <summary>
        /// 高斯-勒让德求积法的节点（零点）
        /// </summary>
        public static readonly double[] GaussLegendreAbscissae = new double[]
        {
            -0.0640568928626056260850430826247450385909,
            0.0640568928626056260850430826247450385909,
            -0.1911188674736163091586398207570696318404,
            0.1911188674736163091586398207570696318404,
            -0.3150426796961633743867932913198102407864,
            0.3150426796961633743867932913198102407864,
            -0.4337935076260451384870842319133497124524,
            0.4337935076260451384870842319133497124524,
            -0.5454214713888395356583756172183723700107,
            0.5454214713888395356583756172183723700107,
            -0.6480936519369755692524957869107476266696,
            0.6480936519369755692524957869107476266696,
            -0.7401241915785543642438281030999784255232,
            0.7401241915785543642438281030999784255232,
            -0.8200019859739029219539498726697452080761,
            0.8200019859739029219539498726697452080761,
            -0.8864155270044010342131543419821967550873,
            0.8864155270044010342131543419821967550873,
            -0.9382745520027327585236490017087214496548,
            0.9382745520027327585236490017087214496548,
            -0.9747285559713094981983919930081690617411,
            0.9747285559713094981983919930081690617411,
            -0.9951872199970213601799974097007368118745,
            0.9951872199970213601799974097007368118745,
        };

        /// <summary>
        /// 高斯-勒让德求积法的权重
        /// </summary>
        public static readonly double[] GaussLegendreWeights = new double[]
        {
            0.1279381953467521569740561652246953718517,
            0.1279381953467521569740561652246953718517,
            0.1258374563468282961213753825111836887264,
            0.1258374563468282961213753825111836887264,
            0.121670472927803391204463153476262425607,
            0.121670472927803391204463153476262425607,
            0.1155056680537256013533444839067835598622,
            0.1155056680537256013533444839067835598622,
            0.1074442701159656347825773424466062227946,
            0.1074442701159656347825773424466062227946,
            0.0976186521041138882698806644642471544279,
            0.0976186521041138882698806644642471544279,
            0.086190161531953275917185202983742667185,
            0.086190161531953275917185202983742667185,
            0.0733464814110803057340336152531165181193,
            0.0733464814110803057340336152531165181193,
            0.0592985849154367807463677585001085845412,
            0.0592985849154367807463677585001085845412,
            0.0442774388174198061686027482113382288593,
            0.0442774388174198061686027482113382288593,
            0.0285313886289336631813078159518782864491,
            0.0285313886289336631813078159518782864491,
            0.0123412297999871995468056670700372915759,
            0.0123412297999871995468056670700372915759,
        };

        /// <summary>
        /// 使用高斯-勒让德求积法计算一元函数积分
        /// </summary>
        /// <param name="function">待积分函数</param>
        /// <param name="customData">自定义数据</param>
        /// <param name="start">积分下限</param>
        /// <param name="end">积分上限</param>
        /// <returns>积分结果</returns>
        public static double GaussLegendre(IntegrationFunction function, object customData, double start, double end)
        {
            double halfLength = (end - start) / 2.0;
            double midPoint = (start + end) / 2.0;
            double result = 0;
            
            for (int i = 0; i < GaussLegendreAbscissae.Length; i++)
            {
                double x = midPoint + halfLength * GaussLegendreAbscissae[i];
                result += GaussLegendreWeights[i] * function(x, customData);
            }
            
            result *= halfLength;
            return result;
        }

        /// <summary>
        /// 使用高斯-勒让德求积法计算二元函数积分
        /// </summary>
        /// <param name="function">待积分函数</param>
        /// <param name="customData">自定义数据</param>
        /// <param name="uStart">第一个变量积分下限</param>
        /// <param name="uEnd">第一个变量积分上限</param>
        /// <param name="vStart">第二个变量积分下限</param>
        /// <param name="vEnd">第二个变量积分上限</param>
        /// <returns>积分结果</returns>
        public static double GaussLegendre(BinaryIntegrationFunction function, object customData, double uStart, double uEnd, double vStart, double vEnd)
        {
            double uHalfLength = (uEnd - uStart) / 2.0;
            double uMidPoint = (uStart + uEnd) / 2.0;
            double vHalfLength = (vEnd - vStart) / 2.0;
            double vMidPoint = (vStart + vEnd) / 2.0;
            double result = 0;
            
            for (int i = 0; i < GaussLegendreAbscissae.Length; i++)
            {
                double u = uMidPoint + uHalfLength * GaussLegendreAbscissae[i];
                double wu = GaussLegendreWeights[i];
                
                for (int j = 0; j < GaussLegendreAbscissae.Length; j++)
                {
                    double v = vMidPoint + vHalfLength * GaussLegendreAbscissae[j];
                    double wv = GaussLegendreWeights[j];
                    
                    result += wu * wv * function(u, v, customData);
                }
            }
            
            result *= uHalfLength * vHalfLength;
            return result;
        }

        /// <summary>
        /// 生成切比雪夫级数
        /// </summary>
        /// <param name="size">级数大小</param>
        /// <returns>切比雪夫级数</returns>
        public static double[] ChebyshevSeries(int size)
        {
            double[] series = new double[size];

            int lenw = series.Length - 1;
            int j, k, l, m;
            double cos2, sin1, sin2, hl;

            cos2 = 0;
            sin1 = 1;
            sin2 = 1;
            hl = 0.5;
            k = lenw;
            l = 2;
            while (l < k - l - 1) 
            {
                series[0] = hl * 0.5;
                for (j = 1; j <= l; j++) 
                {
                    series[j] = hl / (1 - 4 * j * j);
                }
                series[l] *= 0.5;
                DFCT(l, 0.5 * cos2, sin1, series);
                cos2 = Math.Sqrt(2 + cos2);
                sin1 /= cos2;
                sin2 /= 2 + cos2;
                series[k] = sin2;
                series[k - 1] = series[0];
                series[k - 2] = series[l];
                k -= 3;
                m = l;
                while (m > 1) 
                {
                    m >>= 1;
                    for (j = m; j <= l - m; j += (m << 1)) 
                    {
                        series[k] = series[j];
                        k--;
                    }
                }
                hl *= 0.5;
                l *= 2;
            }
            return series;
        }

        /// <summary>
        /// 离散余弦变换辅助方法
        /// </summary>
        private static void DFCT(int n, double fac, double fsq, double[] a)
        {
            int i, ii, j, n1, n2;
            double alpha, beta, zeta, z, c, s, co, si;

            for (i = 1; i <= n / 2; i++) 
            {
                j = n - i;
                a[j + 1] = a[i];
                a[i] = 0;
            }
            a[n / 2 + 1] = 0;
            FFT(n, a);
            alpha = fac;
            beta = fsq;
            zeta = 2 * beta;
            n1 = n + 1;
            n2 = n + 2;
            c = a[1] / 2;
            s = a[2] / 2;
            a[1] = c;
            a[2] = s;
            for (i = 2; i <= n / 2; i++) 
            {
                ii = n2 - i;
                z = a[ii];
                a[ii] = a[i] - z;
                a[i] += z;
                c = Math.Cos(alpha);
                s = Math.Sin(alpha);
                co = a[ii + 1] * c - a[i + 1] * s;
                si = a[ii + 1] * s + a[i + 1] * c;
                a[i + 1] = si;
                a[ii + 1] = co;
                alpha += beta;
                beta += zeta;
            }
            a[n / 2 + 2] = 0;
        }

        /// <summary>
        /// 快速傅里叶变换辅助方法
        /// </summary>
        private static void FFT(int n, double[] a)
        {
            int i, j, k, m;
            double co, si, re, im, temp;

            m = n / 2;
            j = m;
            for (i = 1; i <= n - 2; i++) 
            {
                if (i < j) 
                {
                    temp = a[j + 1];
                    a[j + 1] = a[i + 1];
                    a[i + 1] = temp;
                }
                k = m;
                while (k <= j) 
                {
                    j -= k;
                    k /= 2;
                }
                j += k;
            }
            k = 1;
            while (k < n) 
            {
                for (i = 0; i < n; i += 2 * k) 
                {
                    for (j = 0; j < k; j++) 
                    {
                        co = Math.Cos(Math.PI * j / k);
                        si = Math.Sin(Math.PI * j / k);
                        re = a[i + j + k + 1] * co + a[i + j + k + 2] * si;
                        im = a[i + j + k + 2] * co - a[i + j + k + 1] * si;
                        a[i + j + k + 1] = a[i + j + 1] - re;
                        a[i + j + k + 2] = a[i + j + 2] - im;
                        a[i + j + 1] += re;
                        a[i + j + 2] += im;
                    }
                }
                k *= 2;
            }
        }

        /// <summary>
        /// 使用Clenshaw-Curtis求积法计算一元函数积分
        /// </summary>
        /// <param name="function">待积分函数</param>
        /// <param name="customData">自定义数据</param>
        /// <param name="start">积分下限</param>
        /// <param name="end">积分上限</param>
        /// <param name="series">切比雪夫级数</param>
        /// <param name="epsilon">精度</param>
        /// <returns>积分结果</returns>
        public static double ClenshawCurtisQuadrature(IntegrationFunction function, object customData, double start, double end, double[] series, double epsilon = 1.0e-10)
        {
            double integration;
            int j, k, l;
            double err, esf, eref, erefh, hh, ir, iback, irback, ba, ss, x, y, fx, errir;
            int lenw = series.Length - 1;
            esf = 10;
            ba = 0.5 * (end - start);
            ss = 2 * series[lenw];
            x = ba * series[lenw];
            series[0] = 0.5 * function(start, customData);
            series[3] = 0.5 * function(end, customData);
            series[2] = function(start + x, customData);
            series[4] = function(end - x, customData);
            series[1] = function(start + ba, customData);
            eref = 0.5 * (Math.Abs(series[0]) + Math.Abs(series[1]) + Math.Abs(series[2]) + Math.Abs(series[3]) + Math.Abs(series[4]));
            series[0] += series[3];
            series[2] += series[4];
            ir = series[0] + series[1] + series[2];
            integration = series[0] * series[lenw - 1] + series[1] * series[lenw - 2] + series[2] * series[lenw - 3];
            erefh = eref * Math.Sqrt(epsilon);
            eref *= epsilon;
            hh = 0.25;
            l = 2;
            k = lenw - 5;
            do {
                iback = integration;
                irback = ir;
                x = ba * series[k + 1];
                y = 0;
                integration = series[0] * series[k];
                for (j = 1; j <= l; j++) {
                    x += y;
                    y += ss * (ba - x);
                    fx = function(start + x, customData) + function(end - x, customData);
                    ir += fx;
                    integration += series[j] * series[k - j] + fx * series[k - j - l];
                    series[j + l] = fx;
                }
                ss = 2 * series[k + 1];
                err = esf * l * Math.Abs(integration - iback);
                hh *= 0.25;
                errir = hh * Math.Abs(ir - 2 * irback);
                l *= 2;
                k -= l + 2;
            } while ((err > erefh || errir > eref) && k > 4 * l);
            integration *= end - start;
            if (err > erefh || errir > eref)
            {
                err *= -Math.Abs(end - start);
            }
            else
            {
                err = eref * Math.Abs(end - start);
            }
            return integration;
        }

        /// <summary>
        /// 使用辛普森规则进行数值积分
        /// </summary>
        /// <param name="function">要积分的函数</param>
        /// <param name="lowerBound">积分下限</param>
        /// <param name="upperBound">积分上限</param>
        /// <param name="tolerance">容差</param>
        /// <returns>积分结果</returns>
        public static double ApplySimpsonRule(Func<double, double> function, double lowerBound, double upperBound, double tolerance = 1e-6)
        {
            if (lowerBound == upperBound)
                return 0.0;

            // 确保积分边界是有序的
            if (lowerBound > upperBound)
            {
                double temp = lowerBound;
                lowerBound = upperBound;
                upperBound = temp;
            }

            // 初始步长
            double h = (upperBound - lowerBound);
            double middle = (lowerBound + upperBound) / 2.0;

            // 2点Simpson公式的结果
            double s2 = (h / 6.0) * (function(lowerBound) + 4.0 * function(middle) + function(upperBound));

            // 递归求解
            return AdaptiveSimpson(function, lowerBound, upperBound, s2, tolerance);
        }

        /// <summary>
        /// 自适应Simpson积分递归实现
        /// </summary>
        /// <param name="function">要积分的函数</param>
        /// <param name="a">积分下限</param>
        /// <param name="b">积分上限</param>
        /// <param name="wholeS">整体区间的Simpson积分估计值</param>
        /// <param name="tolerance">容差</param>
        /// <returns>积分结果</returns>
        private static double AdaptiveSimpson(Func<double, double> function, double a, double b, double wholeS, double tolerance)
        {
            double mid = (a + b) / 2.0;
            double h = (b - a) / 2.0;
            
            double leftMid = (a + mid) / 2.0;
            double rightMid = (mid + b) / 2.0;
            
            // 计算左右子区间的Simpson积分值
            double leftS = (h / 6.0) * (function(a) + 4.0 * function(leftMid) + function(mid));
            double rightS = (h / 6.0) * (function(mid) + 4.0 * function(rightMid) + function(b));
            
            // 子区间积分和
            double sum = leftS + rightS;
            
            // 如果子区间和与整体区间积分的差小于容差，返回更精确的结果
            if (Math.Abs(sum - wholeS) <= 15.0 * tolerance)
            {
                return sum + (sum - wholeS) / 15.0; // 外推公式提高精度
            }
            
            // 否则递归计算左右子区间
            return AdaptiveSimpson(function, a, mid, leftS, tolerance / 2.0) + 
                   AdaptiveSimpson(function, mid, b, rightS, tolerance / 2.0);
        }
        
        /// <summary>
        /// 使用高斯积分法计算定积分
        /// </summary>
        /// <param name="function">要积分的函数</param>
        /// <param name="lowerBound">积分下限</param>
        /// <param name="upperBound">积分上限</param>
        /// <param name="points">高斯点数量</param>
        /// <returns>积分结果</returns>
        public static double ApplyGaussIntegration(Func<double, double> function, double lowerBound, double upperBound, int points = 5)
        {
            if (lowerBound == upperBound)
                return 0.0;

            // 确保积分边界是有序的
            if (lowerBound > upperBound)
            {
                double temp = lowerBound;
                lowerBound = upperBound;
                upperBound = temp;
            }

            // 定义高斯点和权重
            double[] abscissas;
            double[] weights;

            switch (points)
            {
                case 2:
                    abscissas = new double[] { -0.5773502691896257, 0.5773502691896257 };
                    weights = new double[] { 1.0, 1.0 };
                    break;
                case 3:
                    abscissas = new double[] { -0.7745966692414834, 0.0, 0.7745966692414834 };
                    weights = new double[] { 0.5555555555555556, 0.8888888888888888, 0.5555555555555556 };
                    break;
                case 4:
                    abscissas = new double[] { -0.8611363115940526, -0.3399810435848563, 0.3399810435848563, 0.8611363115940526 };
                    weights = new double[] { 0.3478548451374538, 0.6521451548625461, 0.6521451548625461, 0.3478548451374538 };
                    break;
                case 5:
                    abscissas = new double[] { -0.9061798459386640, -0.5384693101056831, 0.0, 0.5384693101056831, 0.9061798459386640 };
                    weights = new double[] { 0.2369268850561891, 0.4786286704993665, 0.5688888888888889, 0.4786286704993665, 0.2369268850561891 };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(points), "高斯点数量必须在2到5之间。");
            }

            double halfLength = (upperBound - lowerBound) / 2.0;
            double midPoint = (upperBound + lowerBound) / 2.0;
            double result = 0.0;

            for (int i = 0; i < points; i++)
            {
                // 将[-1,1]映射到[a,b]
                double x = midPoint + halfLength * abscissas[i];
                result += weights[i] * function(x);
            }

            return result * halfLength;
        }
    }
} 