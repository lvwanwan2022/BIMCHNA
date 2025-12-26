using Lv.BIM.Base;
using Lv.BIM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry3d
{
    public class DelaunayMesh : MeshXYZ
    {
        private List<TriangleFace> temp_faces = new List<TriangleFace>();
        //private List<XYZ> p_points = new List<XYZ>();
        private double z_max= 1E20;
        private double z_min=-1E20;
        public double Zmax { get { return z_max; }set { z_max = value; } }
        public double Zmin { get { return z_min; } set { z_min = value; } }
        private double len_max = 1E20;
        private double len_min=-1E20;
        public double Lenmax { get { return len_max; } set { len_max = value; } }
        public double Lenmin { get { return len_min; } set { len_min = value; } }
        public DelaunayMesh()
        {

        }
        /// <summary>
        /// 用于生成点列表的德劳内mesh网格
        /// </summary>
        /// <param name="points">点列表</param>
        /// <param name="zmax">设置Z轴最大值</param>
        /// <param name="zmin">设置Z轴最小值</param>
        /// <param name="lengthmax">设置三角形最大边长</param>
        /// <param name="lengthmin">设置三角形最小边长</param>
        public DelaunayMesh(List<XYZ> points,double zmax = 1E20, double zmin=-1E20, double lengthmax = 1E20, double lengthmin = -1E20)
        {
            z_max = zmax;
            z_min = zmin;
            len_max = lengthmax;
            len_min=lengthmin;
            //Interval zdomain = new Interval(z_min, z_max);
            List<XYZ> temp = points.FindAll(a=> a.Z>z_min && a.Z<z_max);
            Vertices = temp;
            Vertices.SetIntLabel();
            initialize(Vertices);
            List<MeshFaceXYZ> tfaces = new List<MeshFaceXYZ>();
            foreach (TriangleFace t in temp_faces)
            {
                TriangleFace tre = t;
                if (!t.IsDirectwithZaxis())
                {
                    tre.Reverse();
                }
                if(tre.MaxEdgeLength<len_max && tre.MaxEdgeLength > len_min)
                {
                    tfaces.Add(ToMeshFaceByXY(tre));
                }
                
            }

            
            Faces = tfaces;
            SetIntLabel();//mesh方法            
            
        }
        
 
        public List<TriangleFace> SplitWithPointsInner(List<TriangleFace> source, List<XYZ> points)
        {
            //List<TriangleFace> result = new List<TriangleFace>();
            List<TriangleFace> temp = new List<TriangleFace>();
            temp.AddRange(source);
            foreach (XYZ point in points)
            {
                List<TriangleFace> poSplitResultTemp = new List<TriangleFace>();
                foreach (TriangleFace triangleFace in temp)
                {
                    if (triangleFace.IsPointInFace(point))
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
        //点List生成网格主要算法
        private void initialize(List<XYZ> points)
        {
            List<double> Xs = new List<double>();
            List<double> Ys = new List<double>();
            List<double> Zs = new List<double>();
            List<string> Ids = new List<string>();
            int ii = 0;
            foreach (XYZ p in points)
            {
                Xs.Add(p.X);
                Ys.Add(p.Y);
                Zs.Add(p.Z);
                Ids.Add(ii.ToString());
                //p.IntLabel = ii;
                //Vertices.Add(p);
                ii++;
            }
            double Xmin = Xs.Min();
            double Xmax = Xs.Max();
            double Ymin = Ys.Min();
            double Ymax = Ys.Max();
            XYZ p1 = new XYZ(Xmin - 1, Ymin - 1, 0);
            p1.StrLabel = "p1";
            XYZ p2 = new XYZ(Xmin - 1, Ymax + 1, 0);
            p2.StrLabel = "p2";
            XYZ p3 = new XYZ(Xmax + 1, Ymax + 1, 0);
            p3.StrLabel = "p3";
            XYZ p4 = new XYZ(Xmax + 1, Ymin - 1, 0);
            p4.StrLabel = "p4";
            temp_faces.Add(new TriangleFace(p2, p1, p3));
            temp_faces.Add(new TriangleFace(p4, p1, p3));
            foreach (XYZ po in Vertices)
            {
                XYZ poxy = new XYZ(po.X, po.Y, 0);
                List<Edge> edges = new List<Edge>();
                foreach (TriangleFace t in temp_faces)
                {
                    XYZ center = t.CenterZ0();
                    double r = t.RadiusZ0();
                    double dis = poxy.DistanceTo(center);
                    if (dis < r)
                    {
                        edges.Add(new Edge(t.P1, t.P2));
                        edges.Add(new Edge(t.P2, t.P3));
                        edges.Add(new Edge(t.P3, t.P1));
                        t.IsValide = false;
                    }
                }
                //删除不满足三角形
                for (int j = temp_faces.Count - 1; j >= 0; j--)
                {
                    if (!temp_faces[j].IsValide)
                    {
                        temp_faces.RemoveAt(j);
                    }
                }
                //删除重复边
                for (int j = 0; j < edges.Count; j++)
                {
                    for (int n = j + 1; n < edges.Count; n++)
                    {
                        if (edges[j].IsOver(edges[n]))
                        {
                            edges[j].IsValide = false;
                            edges[n].IsValide = false;
                        }
                    }
                }
                //组成新三角形
                foreach (Edge e in edges)
                {
                    if (e.IsValide)
                    {
                        temp_faces.Add(new TriangleFace(e.p1, e.p2, po));
                    }
                }

            }
            foreach (TriangleFace t in temp_faces)
            {
                if (t.IsContain(p1) || t.IsContain(p2) || t.IsContain(p3) || t.IsContain(p4))
                {
                    t.IsValide = false;
                }

            }
            for (int i = temp_faces.Count - 1; i >= 0; i--)
            {
                if (!temp_faces[i].IsValide)
                {
                    temp_faces.RemoveAt(i);
                }
            }
        }

        
       
        private class Edge
        {
            public XYZ p1;
            public XYZ p2;
            public bool IsValide { get; set; } = true;
            public Edge(XYZ p1, XYZ p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
            public bool IsOver(Edge source)
            {
                if (p1.IsAlmostEqualTo(source.p1) && p2.IsAlmostEqualTo(source.p2))
                {
                    return true;
                }
                if (p1.IsAlmostEqualTo(source.p2) && p2.IsAlmostEqualTo(source.p1))
                {
                    return true;
                }
                return false;
            }

        }

    }
}
