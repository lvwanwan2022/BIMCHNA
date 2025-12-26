using Lv.BIM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Base
{
    public static class GeometryExtension
    {
        public static double RadiansTo0_2PI(this double value)
        {
            double result = value;
            Interval range = new Interval(0, Math.PI * 2);
            if (range.IsValueInInterval(value))
            {
                return value;
            }
            else
            {
                double temp = Math.Atan2(Math.Sin(value), Math.Cos(value));
                return temp < 0 ? temp + Math.PI * 2 : temp;
            }
        }
        public static double RadiansTo0_PI(this double value)
        {
            double result = value;
            Interval range = new Interval(0, Math.PI);
            if (value < 0)
            {
                return -value;
            }
            if (range.IsValueInInterval(value))
            {
                return value;
            }
            else
            {

                return Math.PI * 2 - value;
            }
        }
        public static List<XYZ> SetIntLabel(this List<XYZ> list)
        {
            List<XYZ> result = new List<XYZ>();
            int i = 0;
            foreach(XYZ value in list)
            {
                XYZ temp = value;
                temp.IntLabel = i;
                result.Add(temp);
                i++;
            }
            return result;
        }
    }
}
