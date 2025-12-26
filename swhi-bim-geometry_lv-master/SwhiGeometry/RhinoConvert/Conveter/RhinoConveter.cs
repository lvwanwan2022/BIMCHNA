using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lv.BIM.Geometry;
using PolylineRhino = Rhino.Geometry.Polyline;
using Rhino.Geometry.Collections;

namespace RhinoConvert
{
    public static class RhinoConverter
    {
        public static Point3d ToPoint3d(this XYZ source)
        {
            
            return new Point3d(source.X, source.Y, source.Z);
        }
        public static Point2d ToPoint2d(this UV source)
        {
            return new Point2d(source.U, source.V);
        }
        public static Point3d ToPoint3d(this UV source)
        {
            return new Point3d(source.U, source.V,0);
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
        public static List<Point2f> ToPoint2f(this List<UV> source)
        {
            if (source == null)
            {
                return null;
            }
            List<Point2f> result = new List<Point2f>();
            source.ForEach(a => result.Add(new Point2f(a.U, a.V)));
            return result;
        }
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
        public static UV ToUV(this Point2f source)
        {
            if (source == null)
            {
                return null;
            }
            return new UV(source.X, source.Y);
        }
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
        public static List<UV> ToUV(this List<Point2f> source)
        {
            if (source == null)
            {
                return null;
            }
            List<UV> result = new List<UV>();
            source.ForEach(a => result.Add(new UV(a.X, a.Y)));
            return result;
        }
        public static Line ToLine(this LineXYZ source)
        {
           
            return new Line(ToPoint3d( source.StartPoint), ToPoint3d(source.EndPoint));
        }
        public static LineCurve ToLineCurve(this LineXYZ source)
        {
            if (source == null)
            {
                return null;
            }
            return new LineCurve(new Line(ToPoint3d(source.StartPoint), ToPoint3d(source.EndPoint)));
        }
        public static LineXYZ ToLineXYZ(this LineCurve source)
        {
            if (source == null)
            {
                return null;
            }
            return new LineXYZ(ToXYZ(source.PointAtStart), ToXYZ(source.PointAtEnd));
        }
        public static LineXYZ ToLineXYZ(this Line source)
        {
            if (source == null)
            {
                return null;
            }
            Point3d start = source.PointAt(0);
            Point3d end = source.PointAt(1);
            return new LineXYZ(ToXYZ(start), ToXYZ(end));
        }
        public static LineUV ToLineUV(this LineCurve source)
        {
            if (source == null)
            {
                return null;
            }
            return new LineUV(ToUV(source.PointAtStart), ToUV(source.PointAtEnd));
        }
        public static LineUV ToLineUV(this Line source)
        {
            if (source == null)
            {
                return null;
            }
            Point3d start = source.PointAt(0);
            Point3d end = source.PointAt(1);
            return new LineUV(ToUV(start), ToUV(end));
        }
        public static PolylineXYZ ToPolylineXYZ(this PolylineRhino source)
        {
            if (source == null)
            {
                return null;
            }
            List<Point3d> pos = source.ToList();
            if (source.IsClosed)
            {
                pos.RemoveAt(pos.Count - 1);
            }
            List<XYZ> xyzs = ToXYZ(pos);
            return new PolylineXYZ(xyzs, source.IsClosed);
        }
        public static PolylineUV ToPolylineUV(this PolylineRhino source)
        {
            if (source == null)
            {
                return null;
            }
            List<Point3d> pos = source.ToList();
            if (source.IsClosed)
            {
                pos.RemoveAt(pos.Count - 1);
            }
            List<UV> xyzs = ToUV(pos);
            return new PolylineUV(xyzs, source.IsClosed);
        }
        public static PolylineXYZ ToPolylineXYZ(this PolylineCurve source)
        {
            if (source == null)
            {
                return null;
            }
            Polyline poly = source.ToPolyline();
           
            return poly.ToPolylineXYZ();
        }
        public static PolylineUV ToPolylineUV(this PolylineCurve source)
        {
            if (source == null)
            {
                return null;
            }
            Polyline poly = source.ToPolyline();

            return poly.ToPolylineUV();
        }
        public static Polyline ToPolylineRhino(this PolylineXYZ source)
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
            PolylineRhino result = new PolylineRhino(pos);
            
            return result;
        }
        public static Polyline ToPolylineRhino(this PolylineUV source)
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
            List<Point3d> pos = ToPoint3d(xyzs);
            PolylineRhino result = new PolylineRhino(pos);

            return result;
        }
        public static PolylineCurve ToPolylineCurveRhino(this PolylineXYZ source)
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
            PolylineRhino result = new PolylineRhino(pos);

            return new PolylineCurve(result);
        }
        public static ArcXYZ ToArcXYZ(this Arc source)
        {
            if (source == null)
            {
                return null;
            }
            Point3d start = source.StartPoint;
            Point3d mid = source.MidPoint;
            Point3d end = source.EndPoint;
            return new ArcXYZ(ToXYZ(start), ToXYZ(end), ToXYZ(mid));
        }
        public static ArcXYZ ToArcXYZ(this ArcCurve source)
        {
            if (source == null)
            {
                return null;
            }
            Point3d start = source.PointAtStart;
            Point3d mid = source.PointAtLength(source.GetLength()/2);
            Point3d end = source.PointAtEnd;
            return new ArcXYZ(ToXYZ(start), ToXYZ(end), ToXYZ(mid));
        }
        public static ArcCurve ToArcCurveRhino(this ArcXYZ source)
        {
            if (source == null)
            {
                return null;
            }
            XYZ start = source.StartPoint;
            XYZ mid = source.MidPoint;
            XYZ end = source.EndPoint;
            return new ArcCurve(new Arc( ToPoint3d(start), ToPoint3d(mid), ToPoint3d(end)));
        }
        public static Arc ToArcRhino(this ArcXYZ source)
        {
            
            XYZ start = source.StartPoint;
            XYZ mid = source.MidPoint;
            XYZ end = source.EndPoint;
            return new Arc(ToPoint3d(start), ToPoint3d(mid), ToPoint3d(end));
        }
        public static PolycurveXYZ ToPolyCurveXYZ(this PolyCurve poly)
        {
            if (poly == null)
            {
                return null;
            }
            PolycurveXYZ result = new PolycurveXYZ();
           
            int segcount = poly.SegmentCount;
            for(int i = 0; i < segcount; i++)
            {
                Rhino.Geometry.Curve seg = poly.SegmentCurve(i);
                //以下代码容易出错,两个顶点的polyline比较特殊
                if (seg.IsLinear())
                {

                    LineCurve li = new LineCurve(seg.PointAtStart,seg.PointAtEnd);
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
            return result ;
        }

        public static PolyCurve ToPolyCurveRhino(this PolycurveXYZ poly)
        {
            if (poly == null)
            {
                return null;
            }
            PolyCurve result = new PolyCurve();
            List<ICurveXYZ> list = poly.Segments;
            foreach(ICurveXYZ seg in list)
            {
                
                if (seg.IsLine())
                {
                    LineXYZ li = seg as LineXYZ;
                    result.Append(li.ToLine());
                }
                else if (seg.IsArc())
                {
                    ArcXYZ li = seg as ArcXYZ;
                    result.Append(li.ToArcRhino());
                }
                else if (seg.IsPolyline())
                {
                    PolylineXYZ li = seg as PolylineXYZ;
                    PolylineCurve licu = li.ToPolylineCurveRhino();
                    result.Append(licu);
                }
            }

            return result;

        }

        public static Transform ToTransformRhino(this TransformXYZ transform)
        {
            
            Transform result = new Transform();
            for(int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    result[i, j] = transform[i, j];
                }
            }
            return result;
        }
        public static TransformXYZ ToTransformXYZ(this Transform transform)
        {
            if (transform == null)
            {
                return null;
            }
            TransformXYZ result = new TransformXYZ();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i, j] = transform[i, j];
                }
            }
            return result;
        }
        public static PlaneXYZ ToPlaneXYZ(this Plane plane)
        {
            if (plane == null)
            {
                return null;
            }
            XYZ origin = plane.Origin.ToXYZ();
            XYZ xv = plane.XAxis.ToXYZ();
            XYZ yv = plane.YAxis.ToXYZ();
            return new PlaneXYZ(origin,xv,yv);

        }
        public static Plane ToPlaneRhino(this PlaneXYZ plane)
        {
            
            Point3d origin = plane.Origin.ToPoint3d();
            Vector3d xv = plane.Xaxis.ToVector3d();
            Vector3d yv = plane.Yaxis.ToVector3d();
            return new Plane(origin, xv, yv);
        }
        public static Rhino.Geometry.Curve ToCurveRhino(this ICurveXYZ curve)
        {
            if (curve == null)
            {
                return null;
            }
            if (curve.IsArc())
            {
                ArcXYZ arc = curve as ArcXYZ;
                return new ArcCurve( arc.ToArcRhino());
            }
            else if (curve.IsLine())
            {
                LineXYZ li = curve as LineXYZ;
                return new LineCurve(li.ToLine());
            }
            else if (curve.IsPolyline())
            {
                PolylineXYZ  li = curve as PolylineXYZ;
                return li.ToPolylineCurveRhino();
            }
            else if (curve.IsPolyCurve())
            {
                PolycurveXYZ li = curve as PolycurveXYZ;
                return li.ToPolyCurveRhino();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public static ICurveXYZ ToCurveXYZ(this Rhino.Geometry.Curve curve)
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
                LineXYZ li = new LineXYZ(curve.PointAtStart.ToXYZ(),curve.PointAtEnd.ToXYZ());
                return li;
            }
            else if (curve.IsPolyline() && !curve.IsLinear())
            {
                PolylineCurve li = curve as PolylineCurve;
                return li.ToPolylineXYZ();
            }
            else if (curve.GetType().Name=="PolyCurve")
            {
                PolyCurve li = curve as PolyCurve;
                return li.ToPolyCurveXYZ();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public static MeshFaceXYZ ToMeshFaceXYZ(this Rhino.Geometry.MeshFace source)
        {
            MeshFaceXYZ li = new MeshFaceXYZ();
            li.I1 = source.A;
            li.I2 = source.B;
            li.I3 = source.C;
            li.I4 = source.D;
            return li;
        }
        public static MeshFace ToMeshFaceRhino(this MeshFaceXYZ source)
        {
            MeshFace li = new MeshFace();
            li.A=source.I1 ;
           li .B=source.I2 ;
           li .C=source.I3 ;
           li .D= source.I4 ;
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
        public static List<MeshFace> ToMeshFaceListRhino(this List<MeshFaceXYZ> source)
        {
            List<MeshFace> faces = new List<MeshFace> ();
                source.ForEach(a=> faces.Add( a.ToMeshFaceRhino()));
            return faces;
        }
        public static MeshXYZ ToMeshXYZ(this Rhino.Geometry.Mesh source)
        {
            MeshXYZ mesh = new MeshXYZ();
            List<Point3d> points = new List<Point3d>();
            points = source.Vertices.ToPoint3dArray().ToList();
            mesh.Vertices = points.ToXYZ();
           // MeshFaceList faces= source.Faces;
            List<MeshFace> faces=source.Faces.ToList();
            mesh.Faces=faces.ToMeshFaceXYZList();
            mesh.TextureCoordinates = source.TextureCoordinates.ToList().ToUV();
            mesh.Colors = source.VertexColors.ToList();
            return mesh;
        }
        public static Mesh ToMeshRhino(this MeshXYZ source)
        {
            Mesh mesh = new Mesh();
            List<Point3d> points =  source.Vertices.ToPoint3d();
            mesh.Vertices.AddVertices(points);
            mesh.VertexColors.SetColors(source.Colors.ToArray());
            mesh.TextureCoordinates.SetTextureCoordinates(source.TextureCoordinates.ToPoint2f().ToArray());
            mesh.Faces.AddFaces(source.Faces.ToMeshFaceListRhino());
            /// 
            //MeshNgon ngon = MeshNgon.Create(mesh.faces.GetRange(i + 1, n), faceIndices);
            //mesh.Ngons.AddNgon(ngon);
            mesh.Faces.CullDegenerateFaces();
            ///
            return mesh;
        }
    }
}
