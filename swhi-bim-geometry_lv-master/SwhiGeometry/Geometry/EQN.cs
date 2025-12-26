using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{


    /// <summary>
    /// 构造+-(aX+b)^c类
    /// 常数项方程请将a设为0，c设为1
    /// </summary>
    [Serializable]
    public class EQN1d
    {
        
        private double a;//Slope,斜率
        private double b;//intercept,截距
        /// <summary>
        /// c应为大于等于1的正整数
        /// </summary>
        private int c;//pow,次方
        private bool is_positive = true;
        private double d => is_positive ? 1 : -1;
#nullable enable
        private Interval? i_domain;
        public double A => a;
        public double B => b;
        public int C => c;
        public double FeatureValue => -b / a;
        public int Cishu => a == 0 ? 0 : c;
        public Interval? Domain=>i_domain;
        public EQN1d Derivative { get {
               if (Cishu == 0)
                { return new EQN1d(0, 0 , 1,is_positive); }
               else if (Cishu == 1)
                { return new EQN1d(0, a, 1, is_positive); }
                else
                {
                    double temp = Math.Pow(a, 1 / (c - 1));
                    return new EQN1d(a/temp, b/temp, c - 1, is_positive); }
            } }
        public EQN1d()
        {
            a = 1;
            b = 0;
            c = 1;
            is_positive = true;
            i_domain = null;
        }

        /// <summary>
        /// 构造d(aX+b)^c类
        /// </summary>
        /// <param name="a">Slope,斜率a</param>
        /// <param name="b">intercept,截距</param>
        /// <param name="cValue">pow, 次方</param>
        /// <param name="isPositive">是否为正值</param>
        public EQN1d(double aValue, double bValue, int cValue, bool isPositive = true, Interval? doMain =null)
        {
           
            if (cValue <1)
            {
                throw new Exception("次数不能为小于1");
            }
            a = aValue;
            b = bValue;
            c = cValue;
            is_positive= isPositive;
            i_domain = doMain;
        }

        public double this[double idx] { get { return GetResultValue(idx); } }
        public static EQN1d Create1dLinearFunctionBy_k_b(double k_Slope,double b_intercept,Interval? domain=null)
        {
            return new EQN1d(k_Slope, b_intercept, 1,true,domain);
        }
        public double GetResultValue(double source)
        {
            return Math.Pow(a * source + b, c)*d ;
        }
      
    
        public (double x1,double x2) SolveByValue(double resultValue,out bool hasTwoRoot)
        {
            double result1 = resultValue;
            double result2 = resultValue;
            hasTwoRoot = false;
            if (Cishu == 0)
            {
                return (result1,0);
            }
            else
            {
                
                double item = resultValue;
                    if (Cishu % 2 == 0 && item * d < 0)
                    {
                        throw new Exception("方程无实数解");
                    }
                    else if (Cishu == 2)
                    {
                    result1=(Math.Pow(item * d, 1.0 / c) - b) / a;
                    result2 = (-Math.Pow(item * d, 1.0 / c) - b) / a;
                    hasTwoRoot = true;
                    }
                    else
                    {
                        result1=(Math.Pow(item * d, 1.0 / c) - b) / a;
                    }
                return (result1,result2);
            }
        }
        public static EQN1d operator -(EQN1d source)
        {
            if (null == source)
            {
                throw new Exception("空值错误");
            }
            return source.Negate();
        }
        public EQN1d Negate()
        {
            return new EQN1d(a,b,c,false);
        }

        public void Deconstruct(out double a1, out double b1, out double c1,out double d1)
        {
            a1 = this.a;
            b1 = this.b;
            c1 = this.c;
            d1 = this.d;
        }

        //求导数
       public EQN1d GetDerivative()
        {
            if (Cishu == 0)
            { return new EQN1d(0, 0, 1, is_positive); }
            else if (Cishu == 1)
            { return new EQN1d(0, a, 1, is_positive); }
            else
            {
                double temp = Math.Pow(a, 1 / (c - 1));
                return new EQN1d(a / temp, b / temp, c - 1, is_positive);
            }
        }
        public EQN1d GetDerivative(int times = 2)
        {
            if (times > Cishu)
            {
                throw new Exception("求导次数超出方程次数");
            }
            int i = 1;
            EQN1d dao1 = GetDerivative();
            while (i<times){
                EQN1d temp = dao1.GetDerivative();
                dao1 = temp;
                i++;
            }
            return dao1;
        }
        //求极限
        //public double GetLimitValue()
        //{

        //}
    }
    public class EQN2d
    {
        private EQN1d f_u;
        private EQN1d f_v;
        private double d_constant;
        public EQN1d FU => f_u;
        public EQN1d FV => f_v;
        private double? Constant=>d_constant;
        //public FeatureValue
        public UV FeatureUV => new UV(f_u.FeatureValue,f_v.FeatureValue);
        public EQN2d Derivative => new EQN2d(-f_v.Derivative,f_u.Derivative);
        public int Cishu => Math.Max(f_u.Cishu,f_v.Cishu);
        public bool Has0Ci => f_u.Cishu == 0 || f_v.Cishu == 0;
        public double this[UV source] { get { return GetResultValue(source); } }
        public EQN2d()
        {
            f_u = new EQN1d(1,0,1);
            f_v = new EQN1d(1, 0, 1);
            d_constant = 0;
        }
        public EQN2d(EQN1d fx,EQN1d fy,double constant=0)
        {
            f_u = fx;
            f_v = fy;
            d_constant = constant;
        }
        /// <summary>
        /// y=kx+b即kx+b-y=0
        /// </summary>
        /// <param name="kSlope"></param>
        /// <param name="b_intercept"></param>
        /// <returns></returns>
        public static EQN2d Create2dLinearFunction(double kSlope,double bIntercept,Interval? xdomain=null, Interval? ydomain = null)
        {
           EQN1d fx = EQN1d.Create1dLinearFunctionBy_k_b(kSlope, bIntercept,xdomain);
           EQN1d fy= EQN1d.Create1dLinearFunctionBy_k_b(-1, 0,ydomain);
            return new EQN2d(fx, fy);
        }
        public static EQN2d Create2dLinearFunction(XLineUV lineSource, Interval? xdomain = null, Interval? ydomain = null)
        {
            double kSlope = lineSource.Vector.V / (lineSource.Vector.U);
            double bIntercept = lineSource.BasePoint.V - lineSource.BasePoint.U * kSlope;
            EQN1d fx = EQN1d.Create1dLinearFunctionBy_k_b(kSlope, bIntercept, xdomain);
            EQN1d fy = EQN1d.Create1dLinearFunctionBy_k_b(-1, 0, ydomain);
            return new EQN2d(fx, fy);
        }
        public static EQN2d Create2dLinearFunction(LineUV lineSource)
        {
            double kSlope = lineSource.Direction.V / (lineSource.Direction.U);
            double bIntercept = lineSource.StartPoint.V - lineSource.StartPoint.U * kSlope;
            EQN1d fx = EQN1d.Create1dLinearFunctionBy_k_b(kSlope, bIntercept, new Interval(lineSource.StartPoint.U, lineSource.EndPoint.U));
            EQN1d fy = EQN1d.Create1dLinearFunctionBy_k_b(-1, 0);
            return new EQN2d(fx, fy);
        }
        public static EQN2d Create2dCircleFunction(UV center,double radius=0)
        {
            double u = center.U;
            double v = center.V;
            EQN1d fx = new EQN1d(1,-u,2);
            EQN1d fy = new EQN1d(1, -v, 2); ;
            return new EQN2d(fx, fy,-radius*radius);
        }
        public static EQN2d Create2dCircleFunction(CircleUV source)
        {
            UV center = source.Center;
            double radius = source.Radius;
            return EQN2d.Create2dCircleFunction( center,radius);
        }
        //2d椭圆方程
        //2d抛物线方程
        //
        public double GetResultValue(UV source)
        {
            double u = source.U;
            double v = source.V;
            return f_u.GetResultValue(u)+f_v.GetResultValue(v)+d_constant;
        }
        public double GetResultValue(double  x,double y)
        {
            double u = x;
            double v = y;
            return f_u[u] + f_v[v] + d_constant;
        }
        public (double x1,double x2) GetXResultByYValue(double y, out bool has2root)
        {
            return f_u.SolveByValue(-d_constant-(f_v[y]),out has2root);
        }
        public (double x1, double x2) GetYResultByXValue(double x, out bool has2root)
        {
            return f_v.SolveByValue(-d_constant - (f_u[x]), out has2root);
        }
        public UV GetClosetPoint(UV source)
        {
            UV result = new UV();

            return result;
        }
        public List<UV> SolveWithAnother(EQN2d sourceEQN, double initialValue=0, int iterationTimes=100,double tolerance=1E-5)
        {
            List<UV> result = new List<UV>();
           Interval? xInterval = FU.Domain;
            EQN2d fxy1=this;
            EQN2d fxy2=sourceEQN;
            if (this.Cishu<=sourceEQN.Cishu)//第1个方程有0次项，那么第一个方程为1号方程，第二个为2号方程
            {
                fxy1 = this;
                fxy2 = sourceEQN;
            }
            else //第2个方程有0次项，将第2个方程序号设为为1号
            {
                fxy1 = sourceEQN;
                fxy2 = this;
            }
           EQN1d fx1 =fxy1. FU;
            EQN1d fy1 = fxy1.FV;
            EQN1d fx2 = fxy2.FU;
            EQN1d fy2 = fxy2.FV;
            var (a1, b1, c1, d1) = fx1;
            var (a2, b2, c2, d2) = fy1;
            var (a3, b3, c3, d3) = fx2;
            var (a4, b4, c4, d4) = fy2;
            //二元1次方程求解
            if (fxy1.Cishu == 1 && fxy2.Cishu == 1)
            {
                double y0 = ((b1 + b2) * a3 - (b3 + b4) * a1) / (a1 * a4 - a2 * a3);
                 (double x0, double x1) = fx1.SolveByValue(-d_constant - fy1[y0],out bool has2root);
                result.Add(new UV(x0, y0));

            }
            else if (fxy1.Has0Ci)//如果第1个方程有0次项
            {
                if (fx1.Cishu == 0)//x项为常数
                {
                    (double y1, double y2) = fy1.SolveByValue(-fxy1.d_constant - fx1[0], out bool has2root);
                    if (has2root)
                    {
                        (double x2a, double x2b) = fx2.SolveByValue(-fxy2.d_constant - fy2[y1], out bool has2roota);
                        result.Add(new UV(x2a, y1));
                        if (has2roota)
                        {
                            result.Add(new UV(x2b, y1));
                        }
                        (double x2c,double x2d) = fx2.SolveByValue(-fxy2.d_constant - fy2[y2], out bool has2rootb);
                        result.Add(new UV(x2c, -y1));
                        if (has2rootb)
                        {
                            result.Add(new UV(x2d, y2));
                        }
                    }
                    else
                    {
                        (double x2a,double x2b) = fx2.SolveByValue(-fxy2.d_constant - fy2[y1], out bool has2rootc);
                        result.Add(new UV(x2a, y1));
                        if (has2rootc)
                        {
                            result.Add(new UV(x2b, y1));
                        }
                    }
                       
                    
                }
                else////y项为常数
                {
                    (double x1,double x2) = fx1.SolveByValue(-fy1[0] - fxy1.d_constant, out bool has2root);
                    if (has2root)
                    {
                        (double y2a,double y2b) = fy2.SolveByValue(-fx2[x1] - fxy2.d_constant, out bool has2roota);
                        result.Add(new UV(x1, y2a));
                        if (has2roota)
                        {
                            result.Add(new UV(x1, y2b));
                        }
                        (double y2c,double y2d) = fy2.SolveByValue(-fx2[x2] - fxy2.d_constant, out bool has2rootb);
                        result.Add(new UV(x2, y2c));
                        if (has2rootb)
                        {
                            result.Add(new UV(x2, y2d));
                        }
                    }
                    else
                    {
                        (double y2a,double y2b) = fy2.SolveByValue(-fx2[x1] - fxy2.d_constant, out bool has2rootc);
                        result.Add(new UV(x1, y2a));
                        if (has2rootc)
                        {                            
                            result.Add(new UV(x1, y2b));
                        }

                    }
                    

                    
                }
            }
            else
            {
                throw new Exception("not implitment"); 
            }
            return result;
            //else if (fxy1.Cishu == 1)
            //{

                //十等分试算法
                //    for (int j = 0; j < 11; j++)
                //{
                //    //牛顿迭代法
                //    double xresult = xInterval.Start + xInterval.Length / 10 * j;
                //    double x0 = xresult;
                //    for (int i = 0; i < iterationTimes; i++)
                //    {
                //        double y0 = GetYResultByXValue(x0);
                //        double y1 = sourceEQN.GetYResultByXValue(x0);
                //        double jieguo = Math.Abs(y0 - y1);
                //        if (jieguo < 1E-4)
                //        {
                //            xresult = x1;
                //            break;
                //        }
                //        x0 = x1;
                //    }
                //    UV xy = new UV(xresult, GetYResultByXValue(xresult));
                //    result.Add(xy);
            //}
        }


            //if (xInterval != null)
            //{
            //    //十等分试算法
            //    for (int j = 0; j < 11; j++)
            //    {
            //        //牛顿迭代法
            //        double xresult = xInterval.Start + xInterval.Length / 10 * j;
            //        double x0 = xresult;
            //        for (int i = 0; i < iterationTimes; i++)
            //        {
            //            double y0 = GetYResultByXValue(x0);
            //            double y1 = sourceEQN.GetYResultByXValue(x0);
            //            double jieguo = Math.Abs(y0 - y1);
            //            if (jieguo < 1E-4)
            //            {
            //                xresult = x1;
            //                break;
            //            }
            //            x0 = x1;
            //        }
            //        UV xy = new UV(xresult, GetYResultByXValue(xresult));
            //        result.Add(xy);
            //    }
            //}
            //else
            //{
            //    //牛顿迭代法
            //    double xresult = initialValue;
            //    double x0 = xresult;
            //    for (int i = 0; i < iterationTimes; i++)
            //    {
            //        double y0 = GetYResultByXValue(x0);
            //        double x1 = sourceEQN.GetXResultByYValue(y0);
            //        double jieguo = Math.Abs(x0 - x1);
            //        if (jieguo < 1E-4)
            //        {
            //            xresult = x1;
            //            break;
            //        }
            //        if(i== iterationTimes-1)
            //        {
            //            throw new Exception("该初始值试算不出结果");
            //        }
            //        x0 = x1;
            //    }

            //    UV xy = new UV(xresult, GetYResultByXValue(xresult));
            //    result.Add(xy);
            //}



           // return result;
        }

}

