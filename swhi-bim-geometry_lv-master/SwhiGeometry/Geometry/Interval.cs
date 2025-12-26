using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
    [Serializable]
    public class Interval
    {
        private double d_s;
        private double d_e;
        public double Start => d_s;
        public double End => d_e;
        public double Length => Math.Abs(d_e - d_s );
        public Interval() {
            d_s = 0d;
            d_e = 1d;
        }

        public Interval(double start, double end)
        {
            d_s = start;
            d_e = end;
        }
        public bool IsValueInInterval(double source)
        {
            bool result = false;
            if (source>=Math.Min(d_s,d_e) && source<=Math.Max(d_s, d_e))
            {
                result = true;
            }
            return result;

        }
        public bool IsForward()
        {
            if (d_s <= d_e)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsBackward()
        {
            if (d_s > d_e)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 是否与端点重合
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool IsValueOnEnd(double source)
        {
            bool result = false;
            if (Math.Abs(source -d_s)<=1E-9 && Math.Abs(source -d_e) <= 1E-9)
            {
                result = true;
            }
            return result;

        }
        public bool IsCrossWithAnother(Interval source)
        {
            if (IsValueInInterval(source.Start) || IsValueInInterval(source.End) ||source.IsValueInInterval(d_s) || source.IsValueInInterval(d_e))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsIncludeAnother(Interval source)
        {
            if (IsValueInInterval(source.Start) && IsValueInInterval(source.End))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override string ToString()
        {
            return base.ToString() + $"[{d_s}, {d_e}]";
        }

        
    }
}
