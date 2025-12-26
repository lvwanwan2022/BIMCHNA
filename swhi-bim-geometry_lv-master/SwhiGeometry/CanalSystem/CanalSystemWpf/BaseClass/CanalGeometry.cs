using CanalSystem.BaseTools;

using Lv.BIM.Geometry;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanalSystem.BaseClass
{
    [Serializable]
    public class CanalGeometry : CanalBase
    {

        protected List<XYZ> constructLine_points;//序号和点列表
        protected List<double> centerLine_arcRadius;//两个变量定义唯一的中心线和构造线,没有圆弧时设置半径为0,


        //以下信息通过水面线计算得到
        //几何特征点+建筑物进出口+渐变、连接段进出口

        //public string Lv.BIMObjectId;
        public string Key => ID;//用来确定对象唯一性
        public int NumberOfIP => IpPointsList.Count;
        public PolycurveXYZ CenterLine => GetCenterLine();//get { 通过以上两个私有变量计算得到；} 无set方法
        public PolylineXYZ ConstructLine { get { return new PolylineXYZ(constructLine_points); } }//get { 通过以上两个私有变量计算得到；} 无set方法
        public List<XYZ> ConstructLinePoints => constructLine_points;
        public List<XYZ> CenterLinePoints => GetCenterLinePoints();
        public List<double> ArcRadiuList => centerLine_arcRadius;
        public XYZ StartPoint => constructLine_points.First();
        public XYZ EndPoint => constructLine_points.Last();
        public List<double> CenterLinePointsStationList => GetCenterLinePointsStationList();//需要标注的桩号列表
        public List<XYZ> IpPointsList => GetIpPoints();
        public List<double> IpArcRadiuList => GetIpArcRadiuList();
        ///以下信息需要保存在Lv.BIM或CAD中，方便CAD关闭再打开后调用//Lv.BIM添加附加数据参考https://frasergreenroyd.com/how-to-add-custom-user-data-to-Lv.BIM-objects/；
        //public List<XYZ> DragLinePoints => constructLine_points;//直线端点拖拽点list

        //2.构造函数
        //私有变量构造函数

        public CanalGeometry() { }
        public CanalGeometry(List<XYZ> constructLinePoints, List<double> centerLineArcRadius, double startStation = 0)
        {
            if (constructLinePoints.Count - centerLineArcRadius.Count == 2)
            {
                Generateid();
                constructLine_points = constructLinePoints;
                centerLine_arcRadius = centerLineArcRadius;
                start_station = startStation;
                SetLength();
            }
            else
            {
                throw new Exception("半径个数与构造点-2个数不一致");
            }

        }
        public CanalGeometry(PolylineXYZ constructline, List<double> radius, double startStation = 0)
        {

            int numOfpo = constructline.PointCount;
            int numOfra = radius.Count;
            List<XYZ> pos = new List<XYZ>();
            if (numOfpo - 2 - numOfra == 0)
            {
                throw new Exception("错误：Ip点数量-半径数量不等于2");
            }
            else
            {
                Generateid();
                for (int i = 0; i < numOfpo; i++)
                {
                    pos.Add(constructline.Point(i));
                }
                constructLine_points = pos;
                centerLine_arcRadius = radius;
                start_station = startStation;
                SetLength();


            }
        }//直线段+半径
        /// <summary>
        /// //用带圆弧的多段线构造canal
        /// </summary>
        /// <param name="centerline"></param>
        public CanalGeometry(PolycurveXYZ centerline, double startStation = 0)
        {
            Generateid();
            //int numOfseg = centerline.SegmentCount;
            //List<XYZ> constructpoints_temp=new List<XYZ>();
            // List<double> arcRadius_temp=new List<double>();
            constructLine_points = BaseTools.GeneralTools.GetConstructPolylinePoints(centerline);
            centerLine_arcRadius = BaseTools.GeneralTools.GetCenterLineArcRadius(centerline);
            start_station = startStation;
            SetLength();
        }

        public CanalGeometry(PolylineXYZ constructline, double radius, double startStation = 0)
        {
            Generateid();
            int numOfpo = constructline.PointCount;
            List<XYZ> pos = new List<XYZ>();
            List<double> rs = new List<double>();
            for (int i = 0; i < numOfpo - 1; i++)
            {
                pos.Add(constructline.Point(i));
                rs.Add(radius);
            }
            pos.Add(constructline.Point(numOfpo - 1));
            constructLine_points = pos;
            centerLine_arcRadius = rs;
            centerLine_arcRadius.RemoveAt(0);
            SetLength();
            //初始化stationElevations

        }
        //3.方法：


        protected override void Generateid()
        {
            this.id = this.GetType().Name.ToString() + this.GetHashCode().ToString();
        }
        public override void SetLength()
        {
            _length = CenterLine.GetLength();
            end_station = start_station + _length;
        }
        public PolycurveXYZ GetCenterLine()
        {
            PolycurveXYZ poresult = new PolycurveXYZ();
            XYZ end = constructLine_points[0];
            //每次均添加一个直线，一个圆弧
            //1、添加直线
            //a添加起点，起点使用上次添加的末端，

            //b添加末端需计算是否圆弧半径为0
            //2、添加圆弧，如果没有则不添加
            for (int i = 0; i < NumberOfIP - 2; i++)
            {
                XYZ stpo = end;
                XYZ edpo;
                if (Math.Abs(centerLine_arcRadius[i]) < 1E-6)//直线，i=1
                                                             //if (centerLine_arcRadius[i] < 1E-6)//直线，i=1
                {
                    edpo = constructLine_points[i + 1];
                    LineXYZ li = new LineXYZ(stpo, edpo);
                    poresult.Append(li);
                    end = edpo;
                }

                else//首端圆弧i=1，r0!=0
                {

                    ArcHelper ar = new ArcHelper(constructLine_points[i], constructLine_points[i + 1], constructLine_points[i + 2], centerLine_arcRadius[i]);
                    ArcXYZ inarc = ar.qiehu;
                    edpo = ar.p_p1;
                    LineXYZ li = new LineXYZ(stpo, edpo);
                    poresult.Append(li);
                    poresult.Append(inarc);
                    end = ar.p_p3;
                }
            }
            LineXYZ liend = new LineXYZ(end, constructLine_points.Last());
            poresult.Append(liend);
            return poresult;

        }

        public List<XYZ> GetCenterLinePoints()
        {
            List<XYZ> result = new List<XYZ>();
            result.Add(ConstructLinePoints[0]);
            int num = ConstructLinePoints.Count;
            for (int i = 1; i < num - 1; i++)
            {
                double r = ArcRadiuList[i - 1];
                if (r < 1E-7)
                {
                    result.Add(constructLine_points[i]);
                }
                else
                {
                    ArcHelper arc = new ArcHelper(constructLine_points[i - 1], constructLine_points[i], constructLine_points[i + 1], r);
                    XYZ p1 = arc.p_p1;
                    XYZ p3 = arc.p_p3;
                    result.Add(p1);
                    result.Add(p3);
                }
            }
            result.Add(constructLine_points.Last());
            return result;
        }

        //获取构造点桩号
        public List<double> GetConstructLinePointsStationList()
        {
            List<double> result = new List<double>();
            foreach (XYZ item in constructLine_points)
            {
                double len = 0;
                if (CenterLine.ClosestPoint(item, out len))
                {
                    result.Add(len * CenterLine.GetLength() + StartStation);
                }

            }
            return result;
        }

        public bool IsPointOnCenterLine(XYZ source)
        {
            bool result = false;
            result = CenterLine.IsPointOnCurve(source) ? true : false;
            return result;
        }
        public CanalGeometry GetPartOfCanalGeometry(double startStation,double partLength)
        {
            List<XYZ> conspos = new List<XYZ>();
            List<double> arcras = new List<double>();
            double startlen = startStation - start_station < 0 ? 0 : startStation - start_station;
            XYZ start = CenterLine.GetPointAtDist(startlen);
            XYZ end = CenterLine.GetPointAtDist(startStation - start_station + partLength);
            int startindex = IndexOfPointInConstructLinePoints(start);
            int endindex = IndexOfPointInConstructLinePoints(end);
            if(startindex!=-1 && endindex != -1)
            {
                conspos.Add(start);
                if (!(endindex>startindex))
                {
                    conspos.Add(end);
                    return new CanalGeometry(conspos, arcras, startStation);
                }
                conspos.AddRange(constructLine_points.GetRange(startindex + 1, endindex - startindex));
                arcras.AddRange(ArcRadiuList.GetRange(startindex , endindex - startindex));
                conspos.Add(end);
                return new CanalGeometry(conspos, arcras, startStation);
            }
            else
            {
                throw new Exception("部分不在此CanalGeometry上");
            }
        }
        /// <summary>
        /// 判断点在哪两个构造线顶点点之间，返回结果如果为-1，则表示该点没有位于任何两点之间
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexOfPointInConstructLinePoints(XYZ point)
        {
            int num = constructLine_points.Count;
            for (int i=0;i<num-1;i++)
            {
                XYZ start = constructLine_points[i];
                XYZ end = constructLine_points[i + 1];
                XYZ l1 = new LineXYZ(start, point).Direction;
                XYZ l2 = new LineXYZ(point, end).Direction;
                if (l1.IsParallelTo(l2) || l1.Length==0 || l2.Length == 0)
                {
                    return i;
                }
                //else if (l2.Length == 0)
                //{
                //    return i+1;
                //}
            }
            return -1;
        }
        public List<double> GetCenterLinePointsStationList()
        {
            List<double> result = new List<double>();
            foreach (XYZ item in CenterLinePoints)
            {
                double len = 0;
                if (CenterLine.ClosestPoint(item, out len))
                {
                    result.Add(len + StartStation);
                }

            }
            result[result.Count - 1] = result[result.Count - 1] - 0.001;//由于精度问题最后一位桩号容易出错，特做此修改
            return result;
        }
        public XYZ GetPointByStation(double station)
        {
            XYZ point = new XYZ();
            point = CenterLine.GetPointAtDist(station - start_station);
            return point;
        }
        public List<double> GetTerrainElevationByStation(List<double> stations,MeshXYZ mesh)
        {

            //XYZ point = GetPointByStation(station);
            List<MeshXYZ> meshes= new List<MeshXYZ>();
            meshes.Add(mesh);
            List<XYZ> points= new List<XYZ>();
            List<double> result = new List<double>();
            stations.ForEach(a => points.Add(GetPointByStation(a)));
            points.ForEach(a => result.Add(mesh.ProjectToMesh(a).Z));            
            
            return result;
        }

        // public  virtual void AddOneStationLabel(this Canal canalIns, double station, Lv.BIMDoc doc, double startStation = 0, string prefix = "SL", double scale = 1.0) { }
        public List<XYZ> GetIpPoints()
        {
            List<XYZ> result = new List<XYZ>();
            result.Add(constructLine_points[0]);
            int num = constructLine_points.Count;
            for (int i=1;i<num-1;i++)
            {
                XYZ before = constructLine_points[i - 1];
                XYZ now = constructLine_points[i ];
                XYZ after = constructLine_points[i +1];
                LineXYZ first = new LineXYZ(before, now);
                LineXYZ second = new LineXYZ(now, after);
                XYZ vfirst = first.Direction;
                XYZ vsecond = second.Direction;

                if (!vfirst.IsParallelTo(vsecond))
                {
                    result.Add(now);
                }
            }
            result.Add(constructLine_points.Last());
            return result;
        }
        public List<double> GetIpArcRadiuList()
        {
            List<double> result = new List<double>();
            int num = constructLine_points.Count;
            for (int i = 1; i < num - 1; i++)
            {
                XYZ before = constructLine_points[i - 1];
                XYZ now = constructLine_points[i];
                XYZ after = constructLine_points[i + 1];
                LineXYZ first = new LineXYZ(before, now);
                LineXYZ second = new LineXYZ(now, after);
                XYZ vfirst = first.Direction;
                XYZ vsecond = second.Direction;

                if (!vfirst.IsParallelTo(vsecond))
                {
                    result.Add(ArcRadiuList[i-1]);
                }
            }
   
            return result;
        }
        //以下
        public string ToString(string format="0.000", IFormatProvider provider=null)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", new object[9]
            {
            "                         ID:"+"\t"+id+"\n",
            "CenterLineLength:"+"\t"+Length.ToString(format, provider)+"\n",
            "          StartStation:"+"\t"+StartStation.ToString(format, provider)+"\n",
            "   StartWaterLevel:"+"\t"+StartWaterLevel.ToString(format, provider)+"\n",
            "             StartPoint:"+"\t"+StartPoint.ToString(format, provider)+"\n",
            "              EndPoint:"+"\t"+EndPoint.ToString(format, provider)+"\n",
            "      pointsNumber:"+"\t"+constructLine_points.Count.ToString()+"\n",
            "             pointsList:"+"\t"+constructLine_points.ToString(format,provider)+"\n",
            "      ArcRadiusList:"+"\t"+ArcRadiuList.ToString(format,provider)+"\n"
            });
        }
       

    }
}
