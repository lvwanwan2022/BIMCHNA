using Lv.BIM.Base;
using Lv.BIM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry3d
{
    public class TriangleFace
    {
        private XYZ p1;
        private XYZ p2;
        private XYZ p3;
        public XYZ P1 => p1;
        public XYZ P2 => p2;
        public XYZ P3 => p3;
        public LineXYZ E1 => new LineXYZ(p1, p2);
        public LineXYZ E2 => new LineXYZ(p2, p3);
        public LineXYZ E3 => new LineXYZ(p3, p1);
        public XYZ A => p1;
        public XYZ B => p2;
        public XYZ C => p3;
        public XYZ Center => GetCenter();
        public double Area => GetArea();
        public double Radius=> GetRadius();
        public bool IsValide { get; set; } = true;
        public TriangleFace(XYZ point1, XYZ point2, XYZ point3)
        {
            p1 = point1;
            p2 = point2;
            p3 = point3;
        }
        public XYZ CenterZ0()
        {
            double x1 = A.X;
            double y1 = A.Y;
            double x2 = B.X;
            double y2 = B.Y;
            double x3 = C.X;
            double y3 = C.Y;
            double x0 = ((y2 - y1) * (y3 * y3 - y1 * y1 + x3 * x3 - x1 * x1) - (y3 - y1) * (y2 * y2 - y1 * y1 + x2 * x2 - x1 * x1)) / (2.0 * (x3 - x1) * (y2 - y1) - 2.0 * (x2 - x1) * (y3 - y1));
            double y0 = ((x2 - x1) * (x3 * x3 - x1 * x1 + y3 * y3 - y1 * y1) - (x3 - x1) * (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1)) / (2.0 * (y3 - y1) * (x2 - x1) - 2.0 * (y2 - y1) * (x3 - x1));
            //double r = Math.Sqrt((x0 - x1) * (x0 - x1) + (y0 - y1) * (y0 - y1));
            return new XYZ(x0, y0, 0);
        }
        /// <summary>
        /// 外接圆圆心
        /// </summary>
        /// <returns></returns>
        public XYZ GetCenter()
        {
            CircleXYZ yuan = CircleXYZ.CreateBy3Point(p1, p2, p3);
            return yuan.Center;
        }
        public double RadiusZ0()
        {
            double x1 = A.X;
            double y1 = A.Y;
            double x2 = B.X;
            double y2 = B.Y;
            double x3 = C.X;
            double y3 = C.Y;
            double x0 = ((y2 - y1) * (y3 * y3 - y1 * y1 + x3 * x3 - x1 * x1) - (y3 - y1) * (y2 * y2 - y1 * y1 + x2 * x2 - x1 * x1)) / (2.0 * (x3 - x1) * (y2 - y1) - 2.0 * (x2 - x1) * (y3 - y1));
            double y0 = ((x2 - x1) * (x3 * x3 - x1 * x1 + y3 * y3 - y1 * y1) - (x3 - x1) * (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1)) / (2.0 * (y3 - y1) * (x2 - x1) - 2.0 * (y2 - y1) * (x3 - x1));
            double r = Math.Sqrt((x0 - x1) * (x0 - x1) + (y0 - y1) * (y0 - y1));
            return r;
        }
        public double GetRadius()
        {
            CircleXYZ yuan = CircleXYZ.CreateBy3Point(p1, p2, p3);
            return yuan.Radius;
        }
        public double AreaZ0()
        {
            double x1 = A.X;
            double y1 = A.Y;
            double x2 = B.X;
            double y2 = B.Y;
            double x3 = C.X;
            double y3 = C.Y;
            double s = Math.Abs((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1)) / 2.0;
            return s;
        }
        public double GetArea()
        {
            XYZ dir12 = p2 - p1;
            XYZ dir23 = p3 - p2;
            double s = dir12.CrossProduct(dir23).Length;
            return s;
        }
        /// <summary>
        /// 判断点是否与三角形顶点重合
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsContain(XYZ point)
        {
            if (p1.IsAlmostEqualTo(point) || p2.IsAlmostEqualTo(point) || p3.IsAlmostEqualTo(point))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断点是否在边上,false时edgeIndex=-1
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsEdgeContain(XYZ point,out int edgeIndex)
        {
            if (E1.IsPointOnCurve(point)) 
            {                
                edgeIndex = 0;
                return true;
            }
            if (E2.IsPointOnCurve(point)) 
            {                
                edgeIndex =1;
                return true;
            }

            if( E3.IsPointOnCurve(point))
            {
                edgeIndex = 2;
                return true;
            }
            edgeIndex = -1;
            return false;
        }
        public bool IsPointInFace(XYZ point)
        {
            TriangleFace t1 = new TriangleFace(p1, p2, point);
            TriangleFace t2 = new TriangleFace(p2, p3, point);
            TriangleFace t3 = new TriangleFace(p1, p3, point);
            double a1 = t1.Area;
            double a2 = t2.Area;
            double a3 = t3.Area;
            if(Math.Abs(a1 + a2 + a3 - Area) <= 1E-6)
            {
                return true;
            }

            return false;
        }
        public bool IsPointXYInFaceXY(XYZ point)
        {
            TriangleFace t = new TriangleFace(p1.XY, p2.XY, p3.XY);
            TriangleFace t1 = new TriangleFace(p1.XY, p2.XY, point.XY);
            TriangleFace t2 = new TriangleFace(p2.XY, p3.XY, point.XY);
            TriangleFace t3 = new TriangleFace(p1.XY, p3.XY, point.XY);
            double a0 = t.Area;
            double a1 = t1.Area;
            double a2 = t2.Area;
            double a3 = t3.Area;
            if (Math.Abs(a1 + a2 + a3 - a0) <= 1E-6)
            {
                return true;
            }

            return false;
        }
        //判断两个三角形是否重合一条边
        public bool IsEdgeCoincide(TriangleFace triangle)
        {
            bool result = false;
            if (IsContain(triangle.p1))
            {
                if (IsContain(triangle.p2) && !IsContain(triangle.p3))
                {
                    return true;
                }
                if (IsContain(triangle.p3) && !IsContain(triangle.p2))
                {
                    return true;
                }
            }
            if (IsContain(triangle.p2))
            {
                if (IsContain(triangle.p1) && !IsContain(triangle.p3))
                {
                    return true;
                }
                if (IsContain(triangle.p3) && !IsContain(triangle.p1))
                {
                    return true;
                }
            }
            if (IsContain(triangle.p3))
            {
                if (IsContain(triangle.p2) && !IsContain(triangle.p1))
                {
                    return true;
                }
                if (IsContain(triangle.p1) && !IsContain(triangle.p2))
                {
                    return true;
                }
            }
            return result;
        }
        public XYZ Normal
        {
            get
            {
                XYZ a = P2 - P1;
                XYZ b = P3 - P2;
                return (a.CrossProduct(b)).Normalize();
            }
        }
        public bool IsDirectwithZaxis()
        {
            if (Normal.DotProduct(XYZ.BasisZ) < 0)
            {
                return false;
            }
            return true;
        }
        public void Reverse()
        {
            var temp = p2;
            p2 = p3;
            p3 = temp;
        }
        public double Angle1=>Math.PI-((E1.AngleTo(E2)).RadiansTo0_PI());
        public double Angle2=> Math.PI - ((E2.AngleTo(E3)).RadiansTo0_PI());
        public double Angle3 => Math.PI - ((E3.AngleTo(E1)).RadiansTo0_PI());
        public double MinAngle => Math.Min(Math.Min(Angle1, Angle2), Angle3);
        public double EdgeLength1 => p1.DistanceTo(p2);
        public double EdgeLength2 => p2.DistanceTo(p3);
        public double EdgeLength3 => p3.DistanceTo(p1);
        public double MaxEdgeLength => Math.Max(Math.Max(EdgeLength1, EdgeLength2), EdgeLength3);
        public List<TriangleFace> SplitWith1pointInner(XYZ point)
        {
            List<TriangleFace> result = new List<TriangleFace>();
            result.Add(new TriangleFace(p1,p2,point));
            result.Add(new TriangleFace(p2,p3,point));
            result.Add(new TriangleFace(p3,p1,point));
            return result;
        }
        public List<TriangleFace> SplitWith1pointOnEdge(XYZ point)
        {
            
            List<TriangleFace> result = new List<TriangleFace>();
            if (IsContain(point)) 
            {
                result.Add(this);
            }
            else
            {
                if (E1.IsPointOnCurve(point))
                {
                    result.Add(new TriangleFace(p1, point, p3));
                    result.Add(new TriangleFace(point, p2, p3));
                }
                else if (E2.IsPointOnCurve(point))
                {
                    result.Add(new TriangleFace(p1, p2, point));
                    result.Add(new TriangleFace(p1, point, p3));
                }
                else
                {
                    result.Add(new TriangleFace(p1, p2, point));
                    result.Add(new TriangleFace(point, p2, p3));
                }
            }
            return result;
        }
        public List<TriangleFace> SplitWith1point(XYZ point)
        {
            List<TriangleFace> result=new List<TriangleFace>();
            if (IsPointXYInFaceXY(point))
            {
                if (IsEdgeContain(point,out int index))
                {
                    result=SplitWith1pointOnEdge(point);
                }
                else
                {
                    result=SplitWith1pointInner(point);
                }
                return result;
            }
            throw new Exception("点不在三角形内");
        }
        public List<TriangleFace> SplitWithPointsInner(List<XYZ> points)
        {            
            //List<TriangleFace> result = new List<TriangleFace>();
            List<TriangleFace> temp = new List<TriangleFace>();
            temp.Add(this);
            foreach (XYZ point in points)
            {
                List<TriangleFace> poSplitResultTemp = new List<TriangleFace>();
                foreach (TriangleFace triangleFace in temp)
                {
                    if (triangleFace.IsPointXYInFaceXY(point))
                    {
                        poSplitResultTemp.AddRange(triangleFace.SplitWith1point(point));
                    }
                    else
                    {
                        poSplitResultTemp.Add(triangleFace);
                    }
                }
                temp = poSplitResultTemp;
            }
            return temp;
                 
        }
       

    }
}
