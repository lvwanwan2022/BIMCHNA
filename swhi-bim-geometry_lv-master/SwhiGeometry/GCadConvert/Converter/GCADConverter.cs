using GrxCAD.DatabaseServices;
using GrxCAD.Geometry;
using Lv.BIM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCadConvert.Converter
{
    public static class GCadConverter
    {
        public static Point3d ToPoint3d(this XYZ source)
        {
            
            return new Point3d(source.X, source.Y, source.Z);
        }
        public static XYZ ToXYZ(DBPoint point)
        {
            return point.Position.ToXYZ();
        }
        public static DBPoint ToDBPoint(XYZ point)
        {
            return new DBPoint(point.ToPoint3d());
        }
        public static Point2d ToPoint2d(this UV source)
        {
            return new Point2d(source.U, source.V);
        }
        public static Point3d ToPoint3d(this UV source)
        {
            return new Point3d(source.U, source.V, 0);
        }
        public static Vector3d ToVector3d(this XYZ source)
        {

            return new Vector3d(source.X, source.Y, source.Z);
        }
        public static Vector2d ToVector2d(this UV source)
        {
            return new Vector2d(source.U, source.V);
        }
        public static List<Point3d> ToPoint3d(this List<XYZ> source)
        {
            if (source == null)
            {
                return null;
            }
            List<Point3d> result = new List<Point3d>();
            source.ForEach(a => result.Add(new Point3d(a.X, a.Y, a.Z)));
            return result;
        }
        public static List<Point3d> ToPoint3d(this List<UV> source)
        {
            if (source == null)
            {
                return null;
            }
            List<Point3d> result = new List<Point3d>();
            source.ForEach(a => result.Add(new Point3d(a.X, a.Y, 0)));
            return result;
        }
        public static List<XYZ> ToXYZ(this List<UV> source)
        {
            if (source == null)
            {
                return null;
            }
            List<XYZ> result = new List<XYZ>();
            source.ForEach(a => result.Add(new XYZ(a.X, a.Y, 0)));
            return result;
        }
        public static List<Point2d> ToPoint2d(this List<UV> source)
        {
            if (source == null)
            {
                return null;
            }
            List<Point2d> result = new List<Point2d>();
            source.ForEach(a => result.Add(new Point2d(a.U, a.V)));
            return result;
        }
        //public static List<Point2f> ToPoint2f(this List<UV> source)
        //{
        //    if (source == null)
        //    {
        //        return null;
        //    }
        //    List<Point2f> result = new List<Point2f>();
        //    source.ForEach(a => result.Add(new Point2f(a.U, a.V)));
        //    return result;
        //}
        public static XYZ ToXYZ(this Point3d source)
        {
            if (source == null)
            {
                return null;
            }
            return new XYZ(source.X, source.Y, source.Z);
        }
        public static UV ToUV(this Point3d source)
        {
            if (source == null)
            {
                return null;
            }
            return new UV(source.X, source.Y);
        }
        public static UV ToUV(this Point2d source)
        {
            if (source == null)
            {
                return null;
            }
            return new UV(source.X, source.Y);
        }
        //public static UV ToUV(this Point2f source)
        //{
        //    if (source == null)
        //    {
        //        return null;
        //    }
        //    return new UV(source.X, source.Y);
        //}
        public static XYZ ToXYZ(this Vector3d source)
        {
            if (source == null)
            {
                return null;
            }
            return new XYZ(source.X, source.Y, source.Z);
        }
        public static UV ToUV(this Vector2d source)
        {
            if (source == null)
            {
                return null;
            }
            return new UV(source.X, source.Y);
        }
        public static List<XYZ> ToXYZ(this List<Point3d> source)
        {
            if (source == null)
            {
                return null;
            }
            List<XYZ> result = new List<XYZ>();
            source.ForEach(a => result.Add(new XYZ(a.X, a.Y, a.Z)));
            return result;
        }
        public static List<UV> ToUV(this List<Point3d> source)
        {
            if (source == null)
            {
                return null;
            }
            List<UV> result = new List<UV>();
            source.ForEach(a => result.Add(new UV(a.X, a.Y)));
            return result;
        }
        public static List<UV> ToUV(this List<Point2d> source)
        {
            if (source == null)
            {
                return null;
            }
            List<UV> result = new List<UV>();
            source.ForEach(a => result.Add(new UV(a.X, a.Y)));
            return result;
        }
        //public static List<UV> ToUV(this List<Point2f> source)
        //{
        //    if (source == null)
        //    {
        //        return null;
        //    }
        //    List<UV> result = new List<UV>();
        //    source.ForEach(a => result.Add(new UV(a.X, a.Y)));
        //    return result;
        //}
        public static Line ToLineGCAD(this LineXYZ source)
        {
            
            return new Line(ToPoint3d(source.StartPoint), ToPoint3d(source.EndPoint));
        }
        public static Line3d ToLine3dGCAD(this LineXYZ source)
        {
            if (source == null)
            {
                return null;
            }
            return new Line3d(ToPoint3d(source.StartPoint), ToPoint3d(source.EndPoint));
        }
        public static LineSegment3d ToLineSeg3dGCAD(this LineXYZ source)
        {
            if (source == null)
            {
                return null;
            }
            return new LineSegment3d(ToPoint3d(source.StartPoint), ToPoint3d(source.EndPoint));
        }
        public static LineSegment2d ToLineSeg3dGCAD(this LineUV source)
        {
            if (source == null)
            {
                return null;
            }
            return new LineSegment2d(ToPoint2d(source.StartPoint), ToPoint2d(source.EndPoint));
        }
        public static LineXYZ ToLineXYZ(this Line3d source)
        {
            if (source == null)
            {
                return null;
            }
            return new LineXYZ(ToXYZ(source.StartPoint), ToXYZ(source.EndPoint));
        }
       
        public static LineXYZ ToLineXYZ(this Line source)
        {
            if (source == null)
            {
                return null;
            }
            Point3d start = source.StartPoint;
            Point3d end = source.EndPoint;
            return new LineXYZ(ToXYZ(start), ToXYZ(end));
        }
        public static LineXYZ ToLineXYZ(this LineSegment3d source)
        {
            if (source == null)
            {
                return null;
            }
            Point3d start = source.StartPoint;
            Point3d end = source.EndPoint;
            return new LineXYZ(ToXYZ(start), ToXYZ(end));
        }
        public static LineUV ToLineUV(this Line2d source)
        {
            if (source == null)
            {
                return null;
            }
            return new LineUV(ToUV(source.StartPoint), ToUV(source.EndPoint));
        }
        public static LineUV ToLineUV(this LineSegment2d source)
        {
            if (source == null)
            {
                return null;
            }
            return new LineUV(ToUV(source.StartPoint), ToUV(source.EndPoint));
        }
        public static LineUV ToLineUV(this Line source)
        {
            if (source == null)
            {
                return null;
            }
            Point3d start = source.StartPoint;
            Point3d end = source.EndPoint;
            return new LineUV(ToUV(start), ToUV(end));
        }
        public static PolylineXYZ ToPolylineXYZ(this Polyline source)
        {
            if (source == null)
            {
                return null;
            }
            int num = source.NumberOfVertices;
            List<Point3d> pos = new List<Point3d>();
            for(int i = 0; i < num; i++)
            {
                pos.Add(source.GetPoint3dAt(i));
            }
            if (source.Closed)
            {
                pos.RemoveAt(pos.Count - 1);
            }
            List<XYZ> xyzs = ToXYZ(pos);
            return new PolylineXYZ(xyzs, source.Closed);
        }
        public static PolylineUV ToPolylineUV(this Polyline source)
        {
            if (source == null)
            {
                return null;
            }
            int num = source.NumberOfVertices;
            List<Point3d> pos = new List<Point3d>();
            for (int i = 0; i < num; i++)
            {
                pos.Add(source.GetPoint3dAt(i));
            }
            if (source.Closed)
            {
                pos.RemoveAt(pos.Count - 1);
            }
            List<UV> xyzs = ToUV(pos);
            return new PolylineUV(xyzs, source.Closed);
        }
        public static PolylineXYZ ToPolylineXYZ(this PolylineCurve3d source)
        {
            
            if (source == null)
            {
                return null;
            }
            int num = source.NumberOfFitPoints;
            List<Point3d> pos = new List<Point3d>();
            for (int i = 0; i < num; i++)
            {
                pos.Add(source.FitPointAt(i));
            }
            if (source.IsClosed())
            {
                pos.RemoveAt(pos.Count - 1);
            }
            List<XYZ> xyzs = ToXYZ(pos);
            return new PolylineXYZ(xyzs, source.IsClosed());

            
        }
        public static PolylineUV ToPolylineUV(this PolylineCurve2d source)
        {
            if (source == null)
            {
                return null;
            }
            int num = source.NumberOfFitPoints;
            List<Point2d> pos = new List<Point2d>();
            for (int i = 0; i < num; i++)
            {
                pos.Add(source.FitPointAt(i));
            }
            if (source.IsClosed())
            {
                pos.RemoveAt(pos.Count - 1);
            }
            List<UV> xyzs = ToUV(pos);
            return new PolylineUV(xyzs, source.IsClosed());
        }
        public static PolylineCurve3d ToPolylineCurve3dGCAD(this PolylineXYZ source)
        {
            if (source == null)
            {
                return null;
            }
            List<XYZ> xyzs = source.Points;
            if (source.IsClosed)
            {
                xyzs.Add(source.StartPoint);
            }
            List<Point3d> pos = ToPoint3d(xyzs);
            Point3dCollection aa = new Point3dCollection(pos.ToArray());
            PolylineCurve3d result = new PolylineCurve3d(aa);
            return result;
        }
        public static PolylineCurve2d ToPolylineCurveGCAD(this PolylineUV source)
        {
            if (source == null)
            {
                return null;
            }
            List<UV> xyzs = source.Points;
            if (source.IsClosed)
            {
                xyzs.Add(source.StartPoint);
            }
            List<Point2d> pos = ToPoint2d(xyzs);
            Point2dCollection aa = new Point2dCollection(pos.ToArray());
            //Polyline2d result = new Polyline2d(Poly3dType.SimplePoly,pos,0,source.IsClosed,0,0,);
            PolylineCurve2d result = new PolylineCurve2d(aa);
            //Polyline2d cc = result as Polyline2d;
            return result;
        }
        public static PolylineXYZ ToPolylineXYZ(Rectangle3d rectangle)
        {
            var vertices = new List<Point3d>() { rectangle.LowerLeft, rectangle.LowerRight, rectangle.UpperRight ,rectangle.UpperLeft};
            return new PolylineXYZ(vertices.ToXYZ(),true);
        }
        public static ArcXYZ ToArcXYZ(this Arc source)
        {
            if (source == null)
            {
                return null;
            }
            Point3d start = source.StartPoint;
            Point3d mid = source.GetPointAtDist(source.Length/2);
            Point3d end = source.EndPoint;
            return new ArcXYZ(ToXYZ(start), ToXYZ(end), ToXYZ(mid));
        }
        public static ArcUV ToArcXYZ(this CircularArc2d source)
        {
            if (source == null)
            {
                return null;
            }
            Point2d start = source.StartPoint;
            Point2d mid = source.EvaluatePoint(0.5);
            Point2d end = source.EndPoint;
            return new ArcUV(ToUV(start), ToUV(end), ToUV(mid));
        }
        public static CircularArc3d ToArcCurveGCAD(this ArcXYZ source)
        {
            if (source == null)
            {
                return null;
            }
            XYZ start = source.StartPoint;
            XYZ mid = source.MidPoint;
            XYZ end = source.EndPoint;
            return new CircularArc3d(ToPoint3d(start), ToPoint3d(mid), ToPoint3d(end));
        }
        public static CircularArc2d ToArcCurveGCAD(this ArcUV source)
        {
            if (source == null)
            {
                return null;
            }
            UV start = source.StartPoint;
            UV mid = source.MidPoint;
            UV end = source.EndPoint;
            return new CircularArc2d(ToPoint2d(start), ToPoint2d(mid), ToPoint2d(end));
        }
        public static Arc ToArcGCAD(this ArcXYZ source)
        {

            XYZ center = source.Center;
            XYZ mid = source.MidPoint;
            XYZ end = source.EndPoint;
            return new Arc(ToPoint3d(center),source.Radius,source.StartAngle,source.EndAngle);
        }
        public static PlaneXYZ ToPlaneXYZ(this Plane plane)
        {
            PlaneSurface aa = new PlaneSurface();


            if (plane == null)
            {
                return null;
            }
            XYZ origin = plane.PointOnPlane.ToXYZ();
            XYZ xv = plane.GetCoordinateSystem().Xaxis.ToXYZ();
            XYZ yv = plane.GetCoordinateSystem().Yaxis.ToXYZ();
            return new PlaneXYZ(origin, xv, yv);

        }
        public static Plane ToPlaneGCAD(this PlaneXYZ plane)
        {

            Point3d origin = plane.Origin.ToPoint3d();
            Vector3d xv = plane.Xaxis.ToVector3d();
            Vector3d yv = plane.Yaxis.ToVector3d();
            return new Plane(origin, xv, yv);
        }
        /**
       public static PolycurveXYZ ToPolyCurveXYZ(this PolylineCurve3d poly)
       {
           if (poly == null)
           {
               return null;
           }
           PolycurveXYZ result = new PolycurveXYZ();

           int segcount = poly.nu;
           for (int i = 0; i < segcount; i++)
           {
               GCAD.Geometry.Curve seg = poly.SegmentCurve(i);
               //以下代码容易出错,两个顶点的polyline比较特殊
               if (seg.IsLinear())
               {

                   LineCurve li = new LineCurve(seg.PointAtStart, seg.PointAtEnd);
                   result.Append(li.ToLineXYZ());
               }
               else if (seg.IsArc())
               {
                   ArcCurve li = seg as ArcCurve;
                   result.Append(li.ToArcXYZ());
               }
               else if (seg.IsPolyline() && !seg.IsLinear())
               {
                   PolylineCurve li = seg as PolylineCurve;
                   result.Append(li.ToPolylineXYZ());
               }

           }
           result.IsClosed = poly.IsClosed;
           return result;
       }

      
       public static PolyCurve ToPolyCurveGCAD(this PolycurveXYZ poly)
       {
           
           if (poly == null)
           {
               return null;
           }
           PolyCurve result = new PolyCurve();
           List<ICurveXYZ> list = poly.Segments;
           foreach (ICurveXYZ seg in list)
           {

               if (seg.IsLine())
               {
                   LineXYZ li = seg as LineXYZ;
                   result.Append(li.ToLine());
               }
               else if (seg.IsArc())
               {
                   ArcXYZ li = seg as ArcXYZ;
                   result.Append(li.ToArcGCAD());
               }
               else if (seg.IsPolyline())
               {
                   PolylineXYZ li = seg as PolylineXYZ;
                   PolylineCurve licu = li.ToPolylineCurveGCAD();
                   result.Append(licu);
               }
           }

           return result;

       }

      
       
       public static GrxCAD.DatabaseServices.Curve ToCurveGCAD(this ICurveXYZ curve)
       {
           if (curve == null)
           {
               return null;
           }
           if (curve.IsArc())
           {
               ArcXYZ arc = curve as ArcXYZ;
               return arc.ToArcGCAD();
           }
           else if (curve.IsLine())
           {
               LineXYZ li = curve as LineXYZ;
               return li.ToLine();
           }
           else if (curve.IsPolyline())
           {
               PolylineXYZ li = curve as PolylineXYZ;
               return (Polyline)li.ToPolylineCurve3dGCAD();
           }
           else if (curve.IsPolyCurve())
           {
               PolycurveXYZ li = curve as PolycurveXYZ;
               return li.ToPolyCurveGCAD();
           }
           else
           {
               throw new NotImplementedException();
           }
       }
       public static ICurveXYZ ToCurveXYZ(this GrxCAD.DatabaseServices.Curve curve)
       {
           if (curve == null)
           {
               return null;
           }
           if (curve.IsArc())
           {
               ArcCurve arc = curve as ArcCurve;
               return arc.ToArcXYZ();
           }
           else if (curve.IsLinear())
           {
               LineXYZ li = new LineXYZ(curve.PointAtStart.ToXYZ(), curve.PointAtEnd.ToXYZ());
               return li;
           }
           else if (curve.IsPolyline() && !curve.IsLinear())
           {
               PolylineCurve li = curve as PolylineCurve;
               return li.ToPolylineXYZ();
           }
           else if (curve.GetType().Name == "PolyCurve")
           {
               PolyCurve li = curve as PolyCurve;
               return li.ToPolyCurveXYZ();
           }
           else
           {
               throw new NotImplementedException();
           }
       }
       public static MeshFaceXYZ ToMeshFaceXYZ(this GCAD.Geometry.MeshFace source)
       {
           
           MeshFaceXYZ li = new MeshFaceXYZ();
           li.I1 = source.A;
           li.I2 = source.B;
           li.I3 = source.C;
           li.I4 = source.D;
           return li;
       }
       public static MeshFace ToMeshFaceGCAD(this MeshFaceXYZ source)
       {
           MeshFace li = new MeshFace();
           li.A = source.I1;
           li.B = source.I2;
           li.C = source.I3;
           li.D = source.I4;
           return li;
       }
       public static List<MeshFaceXYZ> ToMeshFaceXYZList(this MeshFaceList source)
       {
           List<MeshFaceXYZ> list = new List<MeshFaceXYZ>();
           List<MeshFace> faces = source.ToList();
           foreach (MeshFace face in faces)
           {
               list.Add(ToMeshFaceXYZ(face));
           }
           return list;
       }
       public static List<MeshFaceXYZ> ToMeshFaceXYZList(this List<MeshFace> source)
       {
           List<MeshFaceXYZ> list = new List<MeshFaceXYZ>();
           List<MeshFace> faces = source;
           foreach (MeshFace face in faces)
           {
               list.Add(ToMeshFaceXYZ(face));
           }
           return list;
       }
       public static List<MeshFace> ToMeshFaceListGCAD(this List<MeshFaceXYZ> source)
       {
           List<MeshFace> faces = new List<MeshFace>();
           source.ForEach(a => faces.Add(a.ToMeshFaceGCAD()));
           return faces;
       }
       public static MeshXYZ ToMeshXYZ(this GCAD.Geometry.Mesh source)
       {
           MeshXYZ mesh = new MeshXYZ();
           List<Point3d> points = new List<Point3d>();
           points = source.Vertices.ToPoint3dArray().ToList();
           mesh.Vertices = points.ToXYZ();
           // MeshFaceList faces= source.Faces;
           List<MeshFace> faces = source.Faces.ToList();
           mesh.Faces = faces.ToMeshFaceXYZList();
           mesh.TextureCoordinates = source.TextureCoordinates.ToList().ToUV();
           mesh.Colors = source.VertexColors.ToList();
           return mesh;
       }
       public static Mesh ToMeshGCAD(this MeshXYZ source)
       {
           Mesh mesh = new Mesh();
           List<Point3d> points = source.Vertices.ToPoint3d();
           mesh.Vertices.AddVertices(points);
           mesh.VertexColors.SetColors(source.Colors.ToArray());
           mesh.TextureCoordinates.SetTextureCoordinates(source.TextureCoordinates.ToPoint2f().ToArray());
           mesh.Faces.AddFaces(source.Faces.ToMeshFaceListGCAD());
           /// 
           //MeshNgon ngon = MeshNgon.Create(mesh.faces.GetRange(i + 1, n), faceIndices);
           //mesh.Ngons.AddNgon(ngon);
           mesh.Faces.CullDegenerateFaces();
           ///
           return mesh;
       }
       **/
    }
}
