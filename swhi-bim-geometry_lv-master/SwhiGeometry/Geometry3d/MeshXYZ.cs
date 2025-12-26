using Lv.BIM.Base;
using Lv.BIM.Geometry3d;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace Lv.BIM.Geometry
{
    public class MeshXYZ : Base, IHasVolume, IHasArea, ITransformable
    {

        public List<XYZ> Vertices { get; set; } = new List<XYZ>();

        public List<MeshFaceXYZ> Faces { get; set; } = new List<MeshFaceXYZ>();

        /// <summary> Vertex colors as ARGB <see cref="int"/>s</summary>

        public List<Color> Colors { get; set; } = new List<Color>();
        public int FaceCount => Faces.Count;
        public int VerticeCount => Vertices.Count;
        public List<LineXYZ> BoundaryLines => GetBoundary();
        public List<LineXYZ> GetBoundary()
        {
            PolylineXYZ poly = new PolylineXYZ();
            List<LineXYZ> lines = new List<LineXYZ>();
            //string=minInt_maxInt,int表示出现次数
            Dictionary<string, int> edgesUsedTimes = new Dictionary<string, int>();
            foreach (MeshFaceXYZ mf in Faces)
            {
                int a = mf.A;
                int b = mf.B;
                int c = mf.C;
                int d = mf.D;
                List<string> keys = new List<string>();
                if (mf.IsTriangle)
                {
                    keys.Add(Math.Min(a, b) + "_" + Math.Max(a, b));
                    keys.Add(Math.Min(b, c) + "_" + Math.Max(b, c));
                    keys.Add(Math.Min(c, a) + "_" + Math.Max(c, a));
                }

                if (mf.IsQuadrangular)
                {
                    keys.Add(Math.Min(a, b) + "_" + Math.Max(a, b));
                    keys.Add(Math.Min(b, c) + "_" + Math.Max(b, c));
                    keys.Add(Math.Min(c, d) + "_" + Math.Max(c, d));
                    keys.Add(Math.Min(a, d) + "_" + Math.Max(a, d));
                }
                foreach (string key in keys)
                {
                    if (edgesUsedTimes.ContainsKey(key))
                    {
                        edgesUsedTimes[key]++;
                    }
                    else
                    {
                        edgesUsedTimes.Add(key, 1);
                    }
                }
            }
            foreach (var dic in edgesUsedTimes.Keys)
            {
                int num = edgesUsedTimes[dic];

                if (num == 1)
                {
                    string[] temp = dic.Split('_');
                    int startindex = int.Parse(temp[0]);
                    int endindex = int.Parse(temp[1]);
                    if (!Vertices[startindex].IsAlmostEqualTo(Vertices[endindex]))
                    {
                        lines.Add(new LineXYZ(Vertices[startindex], Vertices[endindex]));
                    }

                }
            }
            return lines;
        }
        public List<UV> TextureCoordinates { get; set; } = new List<UV>();
        public int VerticesCount => Vertices.Count;
        public int TextureCoordinatesCount => TextureCoordinates.Count;
        public Box bbox { get; set; }

        public double Area { get; }

        public double Volume { get; }
        public MeshFaceXYZ this[int idx] => Faces[idx];

        public MeshXYZ()
        {

        }

        public MeshXYZ(List<XYZ> vertices, List<MeshFaceXYZ> faces, List<Color> colors = null, List<UV> texture_coords = null)
        {
            this.Vertices = vertices;
            this.Faces = faces;
            this.Colors = colors ?? this.Colors;
            this.TextureCoordinates = texture_coords ?? this.TextureCoordinates;
            SetIntLabel();
        }
        public MeshXYZ(XYZ[] vertices, MeshFaceXYZ[] faces, Color[] colors = null, UV[] texture_coords = null)
        : this(
          vertices.ToList(),
          faces.ToList(),
          colors?.ToList(),
          texture_coords?.ToList()
        )
        { }
        public MeshXYZ(List<TriangleFace> trianglelist)
        {
            List<XYZ> verticestemp=new List<XYZ>();
            foreach (var face in trianglelist)
            {
               int flag= verticestemp.FindIndex(a => a.IsAlmostEqualTo(face.P1));
                if (flag == -1)
                {
                    verticestemp.Add(face.P1);
                }
                int flag1 = verticestemp.FindIndex(a => a.IsAlmostEqualTo(face.P2));
                if (flag1 == -1)
                {
                    verticestemp.Add(face.P2);
                }
                int flag2 = verticestemp.FindIndex(a => a.IsAlmostEqualTo(face.P3));
                if (flag2 == -1)
                {
                    verticestemp.Add(face.P3);
                }
            }
            verticestemp.SetIntLabel();
            Vertices = verticestemp;
            foreach (var face in trianglelist)
            { 
                Faces.Add(ToMeshFace(face));
            }

            }
        public void SetIntLabel()
        {
            int i = 0;
            foreach (XYZ po in Vertices)
            {
                Vertices[i].IntLabel = i;
                i++;
            }
            i = 0;
            foreach (MeshFaceXYZ face in Faces)
            {
                Faces[i].IntLabel = i;
                i++;
            }
        }

        public List<XYZ> GetPointXYList()
        {
            List<XYZ> points = new List<XYZ>();
            Vertices.ForEach(a => points.Add(new XYZ(a.X, a.Y, 0)));
            int i = 0;
            points.ForEach(a => a.IntLabel = i++) ;
            return points;
        }
        public LineXYZ GetFaceEdge1(int index)
        {
            if (index >= FaceCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            MeshFaceXYZ face = Faces[index];
            XYZ start = Vertices[face.A];
            XYZ end = Vertices[face.B];
            return new LineXYZ(start,end);
        }
        public LineXYZ GetFaceEdge2(int index)
        {
            if (index >= FaceCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            MeshFaceXYZ face = Faces[index];
            XYZ start = Vertices[face.B];
            XYZ end = Vertices[face.C];
            return new LineXYZ(start, end);
        }
        public LineXYZ GetFaceEdge3(int index)
        {
            if (index >= FaceCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            MeshFaceXYZ face = Faces[index];
            XYZ start = Vertices[face.C];
            XYZ end;
            if (face.IsTriangle)
            {
                 end= Vertices[face.A];
            }
            else
            {
                end = Vertices[face.D];
            }
            
            return new LineXYZ(start, end);
        }
        public LineXYZ GetFaceEdge4(int index)
        {
            if (index >= FaceCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            MeshFaceXYZ face = Faces[index];
            XYZ start = Vertices[face.D];
            XYZ end;
            if (face.IsTriangle)
            {
                return null;
            }
            else
            {
                end = Vertices[face.A];
            }

            return new LineXYZ(start, end);
        }
        public XYZ GetFacePointA(int index)
        {
            if (index >= FaceCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            MeshFaceXYZ face = Faces[index];
            return Vertices[face.A];
        }
        public XYZ GetFacePointB(int index)
        {
            if (index >= FaceCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            MeshFaceXYZ face = Faces[index];
            return Vertices[face.B];
        }
        public XYZ GetFacePointC(int index)
        {
            if (index >= FaceCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            MeshFaceXYZ face = Faces[index];
            return Vertices[face.C];
        }
        public XYZ GetFacePointD(int index)
        {
            if (index >= FaceCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            MeshFaceXYZ face = Faces[index];
            return Vertices[face.D];
        }
        public XYZ GetFaceCenter(int index)
        {
            if (index >= FaceCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            MeshFaceXYZ face = Faces[index];
            if (face.IsTriangle)
            {
                return (Vertices[face.A] + Vertices[face.B] + Vertices[face.C]) / 3;
            }
            else
            {
                return (Vertices[face.A] + Vertices[face.B] + Vertices[face.C] + Vertices[face.D])/4;
            }            
        }
        public double FaceDistanceTo(int i,XYZ point)
        {
            double dist = 0;
            MeshFaceXYZ face= Faces[i];
            XYZ a=GetFacePointA(i);
            XYZ b=GetFacePointB(i);
            XYZ c=GetFacePointC(i);
            XYZ d=GetFacePointD(i);
            if (face.IsTriangle)
            {
                dist = (a.DistanceTo(point) + b.DistanceTo(point) + c.DistanceTo(point)) / 3;
            }
            else
            {
                dist = (a.DistanceTo(point) + b.DistanceTo(point) + c.DistanceTo(point) + d.DistanceTo(point)) / 4;
            }            

            return dist;
        }
        public double FaceXYDistanceTo(int i, XYZ point)
        {
            double dist = 0;
            MeshFaceXYZ face = Faces[i];
            UV po = point.XYToUV();
            UV a = GetFacePointA(i).XYToUV();
            UV b = GetFacePointB(i).XYToUV();
            UV c = GetFacePointC(i).XYToUV();
            UV d = GetFacePointD(i).XYToUV();
            if (face.IsTriangle)
            {
                dist = (a.DistanceTo(po) + b.DistanceTo(po) + c.DistanceTo(po)) / 3;
            }
            else
            {
                dist = (a.DistanceTo(po) + b.DistanceTo(po) + c.DistanceTo(po) + d.DistanceTo(po)) / 4;
            }

            return dist;
        }

        /// <summary>
        /// Gets a vertex as a <see cref="XYZ"/> by <paramref name="index"/>
        /// </summary>
        /// <param name="index">The index of the vertex</param>
        /// <returns>Vertex as a <see cref="XYZ"/></returns>
        public XYZ GetPoint(int index)
        {
            return Vertices[index];
        }

        public MeshXYZ[] SplitByXLine(XLineXYZ line)
        {
            var result = new List<MeshXYZ>();

            return result.ToArray();
        }
        public MeshXYZ[] SplitByPolyline(PolylineXYZ polyline)
        {
            var result = new List<MeshXYZ>();

            return result.ToArray();
        }
        /// <summary>
        /// 沿Z轴方向投影
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public XYZ ProjectToMesh(XYZ point)
        {            
            List<double> dists = new List<double>();
            for(int i = 0; i < FaceCount; i++)
            {
                double dist = FaceXYDistanceTo(i,point);
                dists.Add(dist);
            }
            double mindist = dists.Min();
            int minindex=dists.FindIndex(dist => dist ==mindist);
            return new XYZ(point.X, point.Y, GetFaceCenter(minindex).Z);
        }
        public MeshXYZ JoinMesh(MeshXYZ mesh)
        {
            MeshXYZ result = new MeshXYZ();

            return result;

        }

        /// <summary>
        /// Gets a texture coordinate as a <see cref="ValueTuple{T1,T2}"/> by <paramref name="index"/>
        /// </summary>
        /// <param name="index">The index of the texture coordinate</param>
        /// <returns>Texture coordinate as a <see cref="ValueTuple{T1,T2}"/></returns>
        public UV GetTextureCoordinate(int index)
        {
            return TextureCoordinates[index];
        }

        /// <summary>
        /// If not already so, this method will align <see cref="MeshXYZ.Vertices"/>
        /// such that a vertex and its corresponding texture coordinates have the same index.
        /// This alignment is what is expected by most applications.<br/>
        /// </summary>
        /// <remarks>
        /// If the calling application expects 
        /// <code>vertices.count == textureCoordinates.count</code>
        /// Then this method should be called by the <c>MeshToNative</c> method before parsing <see cref="MeshXYZ.Vertices"/> and <see cref="MeshXYZ.Faces"/>
        /// to ensure compatibility with geometry originating from applications that map <see cref="MeshXYZ.Vertices"/> to <see cref="MeshXYZ.TextureCoordinates"/> using vertex instance index (rather than vertex index)
        /// <br/>
        /// <see cref="MeshXYZ.Vertices"/>, <see cref="MeshXYZ.Colors"/>, and <see cref="Faces"/> lists will be modified to contain no shared vertices (vertices shared between polygons)
        /// </remarks>
        public void AlignVerticesWithTexCoordsByIndex()
        {
            if (TextureCoordinates.Count == 0) return;
            if (TextureCoordinatesCount == VerticesCount) return; //Tex-coords already aligned as expected

            var facesUnique = new List<MeshFaceXYZ>(Faces.Count);
            var verticesUnique = new List<XYZ>(TextureCoordinatesCount);
            bool hasColors = Colors.Count > 0;
            var colorsUnique = hasColors ? new List<Color>(TextureCoordinatesCount) : null;

            int nIndex = 0;
            while (nIndex < Faces.Count)
            {
                MeshFaceXYZ n = Faces[nIndex];
                facesUnique.Add(n);
                for (int i = 1; i <= 4; i++)
                {
                    int vertIndex = n[i];
                    int newVertIndex = verticesUnique.Count;

                    var po = GetPoint(i);
                    verticesUnique.Add(po);
                    colorsUnique?.Add(Colors[i]);
                }

                nIndex += 1;
            }
            Vertices = verticesUnique;
            Colors = colorsUnique ?? Colors;
            Faces = facesUnique;
        }


        public bool TransformTo(TransformXYZ transform, out MeshXYZ mesh)
        {
            mesh = new MeshXYZ
            {
                Vertices = transform.ApplyToPoints(Vertices),
                TextureCoordinates = TextureCoordinates,
                Faces = Faces,
                Colors = Colors
            };

            return true;
        }

        public bool TransformTo(TransformXYZ transform, out ITransformable transformed)
        {
            MeshXYZ result = this;
            List<XYZ> points = result.Vertices;
            List<XYZ> pointstrans = transform.ApplyToPoints(points);
            result.Vertices = pointstrans;
            transformed = result;
            return true;
        }
        protected MeshFaceXYZ ToMeshFaceByXY(TriangleFace source)
        {
            int a = -1;
            int b = -1;
            int c = -1;
            XYZ p1xy = new XYZ(source.P1.X, source.P1.Y, 0);
            XYZ p2xy = new XYZ(source.P2.X, source.P2.Y, 0);
            XYZ p3xy = new XYZ(source.P3.X, source.P3.Y, 0);
            List<XYZ> pointxys = new List<XYZ>();
            foreach (XYZ point in Vertices)
            {
                XYZ po = new XYZ(point.X, point.Y, 0);
                po.IntLabel = point.IntLabel;
                pointxys.Add(po);
            }
            foreach (XYZ point in pointxys)
            {
                if (p1xy.IsAlmostEqualTo(point))
                {
                    a = point.IntLabel;
                }
                if (p2xy.IsAlmostEqualTo(point))
                {
                    b = point.IntLabel;
                }
                if (p3xy.IsAlmostEqualTo(point))
                {
                    c = point.IntLabel;
                }
            }
            if (a != -1 && b != -1 && c != -1)
            {
                return new MeshFaceXYZ(a, b, c);
            }
            else
            {
                throw new Exception("三角形转化面失败");
            }
        }
        protected MeshFaceXYZ ToMeshFace(TriangleFace source)
        {
            int a = -1;
            int b = -1;
            int c = -1;
            XYZ p1xy = source.P1;
            XYZ p2xy = source.P2;
            XYZ p3xy =source.P3;            
            foreach (XYZ point in Vertices)
            {
                if (p1xy.IsAlmostEqualTo(point))
                {
                    a = point.IntLabel;
                }
                if (p2xy.IsAlmostEqualTo(point))
                {
                    b = point.IntLabel;
                }
                if (p3xy.IsAlmostEqualTo(point))
                {
                    c = point.IntLabel;
                }
            }
            if (a != -1 && b != -1 && c != -1)
            {
                return new MeshFaceXYZ(a, b, c);
            }
            else
            {
                throw new Exception("三角形转化面失败");
            }
        }
    }


}
