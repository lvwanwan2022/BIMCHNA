using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
    public static class CurveTypeEncoding
    {
        public const double Arc = 0;
        public const double Circle = 1;
        public const double Curve = 2;
        public const double Ellipse = 3;
        public const double Line = 4;
        public const double Polyline = 5;
        public const double PolyCurve = 6;
    }
    public static class CurveArrayEncodingExtensions
    {

        public static List<double> ToArray(List<ICurve> curves)
        {
            var list = new List<double>();
            foreach (var curve in curves)
            {
                switch (curve)
                {
                    case ArcXYZ a: list.AddRange(a.ToList()); break;
                    case CircleXYZ c: list.AddRange(c.ToList()); break;
                    case Curve c: list.AddRange(c.ToList()); break;
                    case EllipseXYZ e: list.AddRange(e.ToList()); break;
                    case LineXYZ l: list.AddRange(l.ToList()); break;
                    case PolycurveXYZ p: list.AddRange(p.ToList()); break;
                    case PolylineXYZ p: list.AddRange(p.ToList()); break;
                    default: throw new Exception($"Unkown curve type: {curve.GetType()}.");
                }
            }

            return list;
        }
        public static List<double> ToArray(List<ICurveXYZ> curves)
        {
            var list = new List<double>();
            foreach (var curve in curves)
            {
                switch (curve)
                {
                    case ArcXYZ a: list.AddRange(a.ToList()); break;
                    case CircleXYZ c: list.AddRange(c.ToList()); break;
                    case Curve c: list.AddRange(c.ToList()); break;
                    case EllipseXYZ e: list.AddRange(e.ToList()); break;
                    case LineXYZ l: list.AddRange(l.ToList()); break;
                    case PolycurveXYZ p: list.AddRange(p.ToList()); break;
                    case PolylineXYZ p: list.AddRange(p.ToList()); break;
                    default: throw new Exception($"Unkown curve type: {curve.GetType()}.");
                }
            }

            return list;
        }
        public static List<double> ToArray(List<ICurveUV> curves)
        {
            var list = new List<double>();
            foreach (var curve in curves)
            {
                switch (curve)
                {
                    case ArcUV a: list.AddRange(a.ToList()); break;
                    case CircleUV c: list.AddRange(c.ToList()); break;
                    case Curve c: list.AddRange(c.ToList()); break;
                    case EllipseUV e: list.AddRange(e.ToList()); break;
                    case LineUV l: list.AddRange(l.ToList()); break;
                    case PolycurveUV p: list.AddRange(p.ToList()); break;
                    case PolylineUV p: list.AddRange(p.ToList()); break;
                    default: throw new Exception($"Unkown curve type: {curve.GetType()}.");
                }
            }

            return list;
        }
        public static List<ICurve> FromArray(List<double> list)
        {
            var curves = new List<ICurve>();
            if (list.Count == 0) return curves;
            var done = false;
            var currentIndex = 0;

            while (!done)
            {
                var itemLength = (int)list[currentIndex];
                var item = list.GetRange(currentIndex, itemLength + 1);

                switch (item[1])
                {
                    case CurveTypeEncoding.Arc: curves.Add(ArcXYZ.FromList(item)); break;
                    case CurveTypeEncoding.Circle: curves.Add(CircleXYZ.FromList(item)); break;
                    case CurveTypeEncoding.Curve: curves.Add(Curve.FromList(item)); break;
                    case CurveTypeEncoding.Ellipse: curves.Add(EllipseXYZ.FromList(item)); break;
                    case CurveTypeEncoding.Line: curves.Add(LineXYZ.FromList(item)); break;
                    case CurveTypeEncoding.Polyline: curves.Add(PolylineXYZ.FromList(item)); break;
                   // case CurveTypeEncoding.PolyCurve: curves.Add(Polycurve.FromList(item)); break;
                }

                currentIndex += itemLength + 1;
                done = currentIndex >= list.Count;
            }
            return curves;
        }

    }
}
