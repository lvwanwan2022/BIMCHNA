using Lv.BIM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.OneTwoThree
{
    public class XYZList3To1
    {

        private int index_class;
        private int[] indexes_class = new int[7] { 0, 1, 2, 3, 4, 5,6 };
        private string[] strings_class = new string[7] { "X", "Y", "Z", "XY", "YZ", "XZ", "XYZ" };
        private double mind_border = 0;
        private double maxd_border = 10;
        //以下用于确定二维分类边界
        private IPoint minP_border;      
        private IPoint maxP_border;
        
        public int ClassIndex { get { if (index_class < 7 && index_class >-1) { return indexes_class[index_class]; }
                else { throw new Exception("索引出错"); }
            } }
        public string ClassString { get { if (index_class< 7 && index_class>-1) { return strings_class[index_class]; }
                else { throw new Exception("索引出错");}
            } }
        public IPoint MinBorder => minP_border;
        public double MinDBorder => mind_border;
        public IPoint MaxPointBorder => maxP_border;
        public double MaxDoubleBorder => maxd_border;
        public XYZList3To1()
        {
            mind_border = 0;
            maxd_border = 1E10;
            index_class = 6;
            minP_border = new XYZ();
        }
        public XYZList3To1(int classIndex, double maxborder)
        {
            mind_border = 0;
            maxd_border = 1E10;
            index_class = classIndex;
            maxd_border = maxborder;
        }
        public XYZList3To1(int classIndex, IPoint maxborder)
        {
            mind_border = 0;
            maxd_border = 1E10;
            index_class = classIndex;
            maxP_border = maxborder;
        }
        public Dictionary<int,List<XYZ>> ClassifyByDis(List<XYZ> pointList,bool IsAddLabel,out List<XYZ> pointList_withlabel)
        {
            //归类字典，int表示归类号，
            Dictionary<int, List<XYZ>> result = new Dictionary<int, List<XYZ>>();
            pointList_withlabel = new List<XYZ>();
            switch (ClassString)
            {
                case "X":
                case "Y":
                case "Z":
                    foreach(XYZ item in pointList)
                    {
                        XYZ potemp = item;
                        //按照X分类函数
                        int key =(int) Math.Floor((item[index_class] - mind_border) / (maxd_border - mind_border));
                        if (!result.ContainsKey(key))
                        {
                            List<XYZ> polisttemp = new List<XYZ>();
                            polisttemp.Add(item);                          
                            
                        }
                        else
                        {
                            List<XYZ> polisttemp =result[key];
                            polisttemp.Add(item);
                            result[key] = polisttemp;

                        }
                        if (IsAddLabel)
                        {
                            potemp.AddLabel(ClassString, key);
                            pointList_withlabel.Add(potemp);
                        }
                    }
                    break;
                case "XY":
                case "YZ":
                case "XZ":
                    foreach (XYZ item in pointList)
                    {
                        XYZ potemp = item;
                        UV itemUV = item.ToUV(ClassString);
                        //按照XYZ分类函数
                        UV minPoint = minP_border as UV;
                        UV maxPoint = maxP_border as UV;
                        int key = (int)Math.Floor((itemUV.DistanceTo(minPoint)) / (maxd_border-mind_border));
                        if (!result.ContainsKey(key))
                        {
                            List<XYZ> polisttemp = new List<XYZ>();
                            polisttemp.Add(item);

                        }
                        else
                        {
                            List<XYZ> polisttemp = result[key];
                            polisttemp.Add(item);
                            result[key] = polisttemp;

                        }
                        if (IsAddLabel)
                        {
                            potemp.AddLabel(ClassString, key);
                            pointList_withlabel.Add(potemp);
                        }
                    }
                    break;
                case "XYZ":
                default:
                    foreach (XYZ item in pointList)
                    {
                        XYZ potemp = item;
                        //按照XYZ分类函数
                        XYZ minPoint = minP_border as XYZ;
                        XYZ maxPoint = maxP_border as XYZ;
                        int key = (int)Math.Floor((item.DistanceTo(minPoint) ) / (maxd_border - mind_border));
                        if (!result.ContainsKey(key))
                        {
                            List<XYZ> polisttemp = new List<XYZ>();
                            polisttemp.Add(item);

                        }
                        else
                        {
                            List<XYZ> polisttemp = result[key];
                            polisttemp.Add(item);
                            result[key] = polisttemp;

                        }
                        if (IsAddLabel)
                        {
                            potemp.AddLabel(ClassString, key);
                            pointList_withlabel.Add(potemp);
                        }
                    }
                    break;
                

            }
            return result;

        }
        public Dictionary<int, List<XYZ>> ClassifyByDisToPolyCurve(List<XYZ> pointList,PolycurveXYZ poly, bool IsAddLabel, out List<XYZ> pointList_withlabel)
        {
            //归类字典，int表示归类号，
            Dictionary<int, List<XYZ>> result = new Dictionary<int, List<XYZ>>();
            pointList_withlabel = new List<XYZ>();
            switch (ClassString)
            {
                case "X":
                case "Y":
                case "Z":
                    throw new NotImplementedException();
                    break;
                case "XY":
                case "YZ":
                case "XZ":
                    throw new NotImplementedException();
                    break;
                case "XYZ":
                default:
                    foreach (XYZ item in pointList)
                    {
                        XYZ potemp = item;
                        //按照XYZ分类函数
                        XYZ minPoint = minP_border as XYZ;
                        XYZ maxPoint = maxP_border as XYZ;
                        double dis = 0;
                        poly.ClosestPoint(item, out dis);
                        int key = (int)Math.Floor(dis / (maxd_border - mind_border));
                        if (!result.ContainsKey(key))
                        {
                            List<XYZ> polisttemp = new List<XYZ>();
                            polisttemp.Add(item);

                        }
                        else
                        {
                            List<XYZ> polisttemp = result[key];
                            polisttemp.Add(item);
                            result[key] = polisttemp;

                        }
                        if (IsAddLabel)
                        {
                            potemp.AddLabel(ClassString, key);
                            pointList_withlabel.Add(potemp);
                        }
                    }
                    break;


            }
            return result;

        }
    }
}
