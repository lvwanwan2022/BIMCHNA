using Lv.BIM.Base;
using Lv.BIM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry3d
{
    public class QuadFace
    {
        private XYZ p1;
        private XYZ p2;
        private XYZ p3;
        private XYZ p4;
        public XYZ P1 => p1;
        public XYZ P2 => p2;
        public XYZ P3 => p3;
        public XYZ P4 => p4;
        public LineXYZ E1=>new LineXYZ(p1,p2);
        public LineXYZ E2=>new LineXYZ(p2,p3);
        public LineXYZ E3=>new LineXYZ(p3,p4);
        public LineXYZ E4=>new LineXYZ(p4,p1);
        public XYZ A => p1;
        public XYZ B => p2;
        public XYZ C => p3;
        public XYZ D => p4;
        public bool IsValide { get; set; } = true;
        public bool IsOnePlane { get { 
                XYZ dir12 = p2 - p1;
                XYZ dir23 = p3 - p2;
                XYZ dir34 = p4-p3;
                XYZ normal1 = dir12.CrossProduct(dir23);
                XYZ normal2 = dir23.CrossProduct(dir34);
                if (normal1.IsParallelTo(normal2))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                    } }
        
        
        public XYZ Normal
        {
            get
            {
                XYZ a = P2 - P1;
                XYZ b = P3 - P2;
                if (IsOnePlane)
                {
                    return (a.CrossProduct(b)).Normalize();
                }
                return null;

            }
        }
        public QuadFace(XYZ point1, XYZ point2, XYZ point3,XYZ point4)
        {           
                p1 = point1;
                p2 = point2;
                p3 = point3;
                p4 = point4;
        }
        public static QuadFace CreateBy4PointsNoOrder(XYZ point1, XYZ point2, XYZ point3, XYZ point4)
        {

            bool isOnePlane=false;
            XYZ dir12 = point2 - point1;
            XYZ dir23 = point3 - point2;
            XYZ dir34 = point4 - point3;
            XYZ normal1 = dir12.CrossProduct(dir23);
            XYZ normal2 = dir23.CrossProduct(dir34);
            if (normal1.IsParallelTo(normal2))
            {
                isOnePlane= true;
            }
            if (isOnePlane)
            {
                LineXYZ line13 = new LineXYZ(point1, point3);
                LineXYZ line24=new LineXYZ(point2, point4);
                if(line13.IsIntersected(line24))
                {
                    return new QuadFace(point1, point2, point3, point4);
                }
                LineXYZ line14 = new LineXYZ(point1, point4);
                LineXYZ line23=new LineXYZ(point2, point3);
                if (line14.IsIntersected(line23))
                {
                    return new QuadFace(point1, point2, point4, point3);
                }
                LineXYZ line12 = new LineXYZ(point1, point2);
                LineXYZ line34=new LineXYZ(point3, point4);
                if (line12.IsIntersected(line34))
                {
                    return new QuadFace(point1, point3, point2, point4);
                }
            }            
                return new QuadFace(point1, point2, point3, point4);
           



        }
        public XYZ Center()
        {
            double x1 = A.X;
            double y1 = A.Y;
            double x2 = B.X;
            double y2 = B.Y;
            double x3 = C.X;
            double y3 = C.Y;

            double x0 = (A.X+B.X+C.X+D.X)/4;
            double y0 = (A.Y + B.Y + C.Y + D.Y) / 4;
            double z0 = (A.Z + B.Z + C.Z + D.Z) / 4;            
            return new XYZ(x0, y0, z0);
        }
        public double Radius()
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
        public double Area()
        {
                TriangleFace aa = new TriangleFace(p1, p2, p3);
                TriangleFace bb=new TriangleFace(p1, p3, p4);
                return aa.AreaZ0() + bb.AreaZ0();
        }
        /// <summary>
        /// 判断是否为凸多边形
        /// </summary>
        public bool IsConvex(out int ConcavePointIndex)
        {
            if (IsOnePlane)
            {
                TriangleFace t1 = new TriangleFace(p1, p2, p3);
                if (t1.IsPointInFace(p4))
                {
                    ConcavePointIndex = 3;
                    return false;
                }
                TriangleFace t2 = new TriangleFace(p2, p3, p4);
                if (t1.IsPointInFace(p1))
                {
                    ConcavePointIndex = 0;
                    return false;
                }
                TriangleFace t3 = new TriangleFace(p3, p4, p1);
                if (t1.IsPointInFace(p2))
                {
                    ConcavePointIndex = 1;
                    return false;
                }
                TriangleFace t4 = new TriangleFace(p1, p2, p4);
                if (t1.IsPointInFace(p3))
                {
                    ConcavePointIndex = 2;
                    return false;
                }
            }
            ConcavePointIndex = -1;
            return true;
        }
        /// <summary>
        /// 是否包含顶点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsContain(XYZ point)
        {
            if (p1.IsAlmostEqualTo(point) || p2.IsAlmostEqualTo(point) || p3.IsAlmostEqualTo(point) || p4.IsAlmostEqualTo(point))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否包含边Edge
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool IsContain(LineXYZ line)
        {

            if (E1.IsOverLoaded(line) || E2.IsOverLoaded(line) || E3.IsOverLoaded(line) || E4.IsOverLoaded(line))
            {
                return true;
            }
            return false;
        }

        //判断两个三角形是否重合一条边
        public bool IsEdgeCoincide(QuadFace quad)
        {
            bool result = false;
            if (IsContain(quad.E1))
            {
                if (!IsContain(quad.E2) && !IsContain(quad.E3) && !IsContain(quad.E4))
                {
                    return true;
                }
                
            }
            if (IsContain(quad.E2))
            {
                if (!IsContain(quad.E1) && !IsContain(quad.E3) && !IsContain(quad.E4))
                {
                    return true;
                }

            }
            if (IsContain(quad.E3))
            {
                if (!IsContain(quad.E1) && !IsContain(quad.E2) && !IsContain(quad.E4))
                {
                    return true;
                }

            }
            if (IsContain(quad.E4))
            {
                if (!IsContain(quad.E2) && !IsContain(quad.E3) && !IsContain(quad.E1))
                {
                    return true;
                }

            }
            return result;
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
            p2 = p4;
            p4 = temp;
        }
        public Pair<TriangleFace> ToTriangleFace()
        {
            bool convex = IsConvex(out int concavpo);
            if (convex)
            {
                //最小角最大
                //新增边长度最小
                double l24 = p2.DistanceTo(p4);
                double l13 = p1.DistanceTo(p3);
                if (l24 < l13)
                {
                    TriangleFace t1 = new TriangleFace(p1, p2, p4);
                    TriangleFace t2 = new TriangleFace(p2, p3, p4);
                    return new Pair<TriangleFace>(t1, t2);
                }
                else
                {
                    TriangleFace t1 = new TriangleFace(p1, p2, p3);
                    TriangleFace t2 = new TriangleFace(p3, p4, p1);
                    return new Pair<TriangleFace>(t1, t2);
                }

            }
            else
            {
                if (concavpo == 0 || concavpo == 2)
                {
                    TriangleFace t1 = new TriangleFace(p1, p2, p3);
                    TriangleFace t2 = new TriangleFace(p3, p4, p1);
                    return new Pair<TriangleFace>(t1, t2);
                }
                else
                {
                    TriangleFace t1 = new TriangleFace(p2, p3, p4);
                    TriangleFace t2 = new TriangleFace( p1, p2,p4);
                    return new Pair<TriangleFace>(t1, t2);
                }
                
            }
            
        }
    }
}
