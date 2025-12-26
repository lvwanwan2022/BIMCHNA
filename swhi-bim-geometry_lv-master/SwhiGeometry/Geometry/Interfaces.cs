using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
    public interface IPoint
    {
        //public double DistanceTo();
        public bool IsXYZ();
        public bool IsUV();
        public bool IsRAB();
        public bool IsRS();

    }
    public interface ICurve
    {

        public double Length { get; }
    }
    public interface ICurveXYZ : ICurve, ITransformable
    {
        public XYZ TangentAtStart { get; }
        public XYZ TangentAtEnd { get; }
        public XYZ StartPoint { get; }
        public XYZ EndPoint { get; }
        public bool IsLine();
        public bool IsArc();
        public bool IsPolyline();
        public bool IsPolyCurve();
        public XYZ GetPointAtDist(double dist);
        public double GetDistAtPoint(XYZ point);
        public bool IsPointOnCurve(XYZ source);
        public XYZ TangentAt(double dist);
        public XYZ GetClosestPointWithinLine(XYZ testPoint, out double distance);
        public bool ClosestPoint(XYZ testPoint, out double t);
        //public ICurveXYZ OffsetBy2Direction(XYZ startDirection, XYZ endDirection, double offsetDis);


    }
    public interface ICurveUV : ICurve, ITransformable
    {
        public UV TangentAtStart { get; }
        public UV TangentAtEnd { get; }
        public UV StartPoint { get; }
        public UV EndPoint { get; }
        public bool IsLine();
        public bool IsArc();
        public bool IsPolyline();
        public bool IsPolyCurve();
        public UV GetPointAtDist(double dist);
        public double GetDistAtPoint(UV point);
        public bool IsPointOnCurve(UV source);
        public UV GetClosestPointWithinLine(UV testPoint, out double distance);
        public bool ClosestPoint(UV testPoint, out double t);
        //public ICurveUV OffsetBy2Direction(UV startDirection, UV endDirection, double offsetDis);


    }
    public interface IHasArea
    {
        double Area { get; }
    }
    public interface IParameterizable
    {
    }
    public static class IParameterExtension
        {
        public static  object GetProperty<T>(this T t, string propName) where T : IParameterizable
        {
            return typeof(T).GetProperty(propName).GetValue(t, null);
        }

        public static void ChangeProperty<T>(this T t, string propName, object value)
        {
            t.GetType().GetProperty(propName).SetValue(t, value);
        }
    }

    public interface IHasVolume
    {
        double Volume { get; }
    }
    /// <summary>
    /// Interface for transformable objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITransformable<T> where T : ITransformable<T>
    {
        bool TransformTo(TransformXYZ transform, out T transformed);
    }

    /// <summary>
    /// Interface for transformable objects where the type may not be known on convert (eg ICurve implementations)
    /// </summary>
    public interface ITransformable
    {
        bool TransformTo(TransformXYZ transform, out ITransformable transformed);
    }

}
