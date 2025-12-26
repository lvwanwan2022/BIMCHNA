using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
    ///// <summary>
    ///// 暂不使用mesh点存储，先不考虑性能，仅实现功能
    ///// </summary>
    //public  class MeshStorage 
    //{
    //    public string id => this.GetHashCode().ToString();
    //    public  Dictionary<int, XYZ> MeshVertices = new Dictionary<int, XYZ>();

    //}
    public class MeshFaceXYZ:Base
    {
        private int index_1;
        private int index_2;
        private int index_3;
        private int index_4;

        public int I1 { get { return index_1; } set{ index_1 = value; } }
        public int I2 {get{return index_2;}set{index_2=value;}}
        public int I3 {get{return index_3;}set{index_3=value;}}
        public int I4{get{return index_4;}set {index_4=value;}}
        public int A { get { return index_1; } set { index_1 = value; } }
        public int B { get { return index_2; } set { index_2 = value; } }
        public int C { get { return index_3; } set { index_3 = value; } }
        public int D { get { return index_4; } set { index_4 = value; } }
        public bool IsTriangle => index_4 == index_3;
        public bool IsQuadrangular => index_4 != index_3;
        public bool IsValide { get; set; }=true;
        public MeshFaceXYZ() { }
        public MeshFaceXYZ(int i1, int i2, int i3, int i4)
        {
            index_1 = i1;
            index_2 = i2;
            index_3 = i3;            
                index_4 = i4;
            
            
        }
        public MeshFaceXYZ(int i1, int i2, int i3)
        {
            index_1 = i1;
            index_2 = i2;
            index_3 = i3;
            index_4 = i3;
        }
        public int this[int idx] => idx switch
        {
            3 => index_4,
            2 => index_3,
            1 => index_2,
            0 => index_1,
            _ => throw new Exception("索引错误"),
        };

        

        //public LineXYZ L1 => new LineXYZ(point_1, point_2);
        //public LineXYZ L2 => new LineXYZ(point_2, point_3);
        //public LineXYZ L3 => new LineXYZ(point_3, point_1);
        //public List<XYZ> Vertices => new List<XYZ> { point_1, point_2, point_3 };
        //public List<LineXYZ> Edges => new List<LineXYZ> { L1, L2, L3 };
        //public XYZ Normal => L1.Direction.CrossProduct(L2.Direction);
        //public double Area => (L1.Vector.CrossProduct(L2.Vector)).Length / 2.0;
        //public XYZ Center => new XYZ(point_1.X / 3 + point_2.X / 3 + point_3.X / 3, point_1.Y / 3 + point_2.Y / 3 + point_3.Y / 3, point_1.Z / 3 + point_2.Z / 3 + point_3.Z / 3);
        //public bool IsDeclineOnce{get{
        //        bool result = false;
        //        if (point_1.IsAlmostEqualTo(point_2) || point_2.IsAlmostEqualTo(point_3) || point_3.IsAlmostEqualTo(point_1) )
        //        {if (!IsDeclineTwice) 
        //                return true; }
        //        return result;
        //            } }
        //public bool IsDeclineTwice
        //{
        //    get
        //    {
        //        if (point_1.IsAlmostEqualTo(point_2) && point_2.IsAlmostEqualTo(point_3)) { return true; }
        //        else { return false; }
        //    }
        //}
        //public Trimesh(XYZ point1,XYZ point2,XYZ point3)
        //{
        //    point_1 = point1;
        //    point_2 = point2;
        //    point_3 = point3;
        //}
        //public LineXYZ this[int idx] => idx switch
        //{
        //    2 => L3,
        //    1 => L2,
        //    0 => L1,
        //    _ => throw new Exception("索引错误"),
        //};
        //public XYZ PointAt(int index)
        //{
        //    XYZ result = new XYZ();
        //    result=index switch
        //    {
        //        2 => point_3,
        //        1 => point_2,
        //        0 => point_1,
        //        _ => throw new Exception("索引错误"),
        //    };
        //    return result;
        //}
        //public XYZ VertexAt(int index)
        //{
        //    XYZ result = new XYZ();
        //    result = index switch
        //    {
        //        2 => point_3,
        //        1 => point_2,
        //        0 => point_1,
        //        _ => throw new Exception("索引错误"),
        //    };
        //    return result;
        //}
        //public LineXYZ EdgeAt(int index)
        //{
        //    LineXYZ result = new LineXYZ();
        //    result = this[index];
        //    return result;
        //}
        //public bool IsConnectedWithByVertex(Trimesh source)
        //{
        //    bool result = false;
        //    foreach(XYZ vertex1 in Vertices)
        //    {
        //        foreach(XYZ vertex2 in source.Vertices)
        //        {
        //            if (vertex1.IsAlmostEqualTo(vertex2))
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return result;
        //}
        //public bool IsConnectedWithByEdge(Trimesh source)
        //{
        //    bool result = false;
        //    foreach (LineXYZ line1 in Edges)
        //    {
        //        foreach (LineXYZ line2 in source.Edges)
        //        {
        //            if (line1.IsOverLoaded(line2))
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return result;
        //}
        //public void Reverse()
        //{
        //    XYZ temp = P2;
        //    point_2 = point_3;
        //    point_3 = temp;

        //}

    }

}
