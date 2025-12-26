using Lv.BIM.Geometry;
using System;


namespace CanalSystem.BaseTools
{
    public class ArcHelper
    {
        private XYZ p_p0;

        private XYZ p_p2;

        private XYZ p_p4;
        private double arc_radius;

        public XYZ P0 => p_p0;
        public XYZ P2 => p_p2;
        public XYZ P4 => p_p4;
        public LineXYZ l02 => new LineXYZ(p_p0, p_p2);
        public LineXYZ l24 => new LineXYZ(p_p2, p_p4);
        //切点1
        public XYZ p_p1 => l02.GetClosestPoint(Center);
        public XYZ P1 => p_p1;
        //切点2
        public XYZ p_p3 => l24.GetClosestPoint(Center);
        public XYZ P3 => p_p3;
        public ArcXYZ qiehu => new ArcXYZ(p_p1, V02, p_p3);
        public double Radius => arc_radius;
        public XYZ V02 { get { return (p_p2 - p_p0); } }
        public XYZ V24 { get { return (p_p4 - p_p2); } }

        public XYZ Normal { get { return V02.CrossProduct( V24); } }
        public double Sita
        {
            get
            {
                return V02.AngleTo( -V24);
            }
        }
        public XYZ p2o
        {
            get
            {
                XYZ aa = V24;
                aa.RotateByAxisAndDegree(Normal,Sita / 2);
                return aa;
            }
        }
        public double lenp2o { get { return arc_radius / Math.Sin(Sita / 2); } }
        public XYZ Center
        {
            get
            {
                return p_p2 + p2o / p2o.Length * lenp2o;
            }
        }

        public PlaneXYZ plane
        {
            get
            {
                return new PlaneXYZ(p_p0, Normal);
            }
        }


        public ArcHelper(XYZ startPoint, XYZ intersectPoint, XYZ endPoint, double r)
        {
            p_p0 = startPoint;
            p_p2 = intersectPoint;
            p_p4 = endPoint;
            arc_radius = r;
        }
        public ArcHelper(ArcXYZ arc)
        {
            double sa = (arc.EndAngle - arc.StartAngle) / 2;
            double len = arc.Radius * Math.Tan(sa);
            XYZ point = new XYZ();
            XYZ v1 = arc.StartPoint - arc.Center;
            XYZ v2 = arc.EndPoint - arc.Center;
            XYZ ax = v1.CrossProduct( v2);
            //plane = new Plane(arc.StartPoint, ax);
            v1.RotateByAxisAndDegree(ax,Math.PI / 2);
            v1.Unitize();
            point = arc.StartPoint + v1 * len;
            p_p0 = arc.StartPoint;
            p_p2 = point;
            p_p4 = arc.EndPoint;
            arc_radius = arc.Radius;
        }
        
    }
}
