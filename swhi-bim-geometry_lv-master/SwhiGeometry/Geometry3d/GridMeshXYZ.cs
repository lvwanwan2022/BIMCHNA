using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
    public class GridMeshXYZ:MeshXYZ
    {
        private List<XYZ> point_list = new List<XYZ>();
        private List<PolylineXYZ> polyline_list = new List<PolylineXYZ>();
        private int x_divide_num;
        private int y_divide_num;
        private List<GridMeshXYZ> inside_meshes=new List<GridMeshXYZ>();
        public List<XYZ> PointList => point_list;
        public  List<PolylineXYZ> PolylineList=>polyline_list;
        public Interval XDomain=>GetXDomain();
        public Interval YDomain=>GetYDomain();
        public Interval ZDomain=>GetZDomain();
        public List<double> XGridLine => GetXGridLine();
        public List<double> YGridLine => GetYGridLine();
        public PolylineXYZ BoundaryOutside => GetBoundaryOutside();
        


        public Interval GetXDomain()
        {            
            double min = 0;
            double max = 10;
            return new Interval(min,max);
        }
        public Interval GetYDomain()
        {
            double min = 0;
            double max = 10;
            return new Interval(min, max);
        }
        public Interval GetZDomain()
        {
            double min = 0;
            double max = 10;
            return new Interval(min, max);
        }
        public List<double> GetXGridLine()
        {
            List<double> result = new List<double>();
            
            for(int i = 0; i < x_divide_num; i++)
            {
                result.Add(XDomain.Start + XDomain.Length / x_divide_num * i);
            }
            result.Add(XDomain.End);
            return result;        
        }
        public List<double> GetYGridLine()
        {
            List<double> result = new List<double>();

            for (int i = 0; i < y_divide_num; i++)
            {
                result.Add(YDomain.Start + YDomain.Length / y_divide_num * i);
            }
            result.Add(YDomain.End);
            return result;
        }
        public PolylineXYZ GetBoundaryOutside()
        {
            PolylineXYZ result = new PolylineXYZ();

            return result;

        }
    }
}
