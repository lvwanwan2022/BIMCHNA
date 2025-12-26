using CanalSystem.BaseClass;
using CanalSystem.Constants;
using Lv.BIM;
//using Lv.BIM.DocObjects;
using Lv.BIM.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CanalSystem.BaseTools
{
    [Serializable]
    public class StationLabel
    {
        private double _station;
        private XYZ base_point;
        public double rotation_radians;

        public string Station_format = "000.000";//设置小数点位数，默认三位小数
        public string Prefix = "SL";
        public double Text_height => 3.5 * Scale;
        public double Line_length => 35 * Scale;
        public LineXYZ LabelLine
        {
            get
            {
                XYZ lineVector = new XYZ(Math.Cos(rotation_radians), Math.Sin(rotation_radians), 0);
                return LineXYZ.CreateByStartPointAndVector(base_point, lineVector*Line_length);
            }
        }
        public string LabeText => GetLabelText();
        public string LabelTextAttached = "";
        public double Scale = 1.0;
        public string GetLabelText()
        {
            string result = "";
            if (_station < 1000)
            {
                result = Prefix + "0+" + _station.ToString(Station_format);
            }
            else
            {
                int shang = (int)Math.Floor(_station / 1000);
                double yushu = _station % 1000;
                result = Prefix + shang.ToString() + "+" + yushu.ToString(Station_format);
            }
            return result;
        }
        public StationLabel(double station, XYZ basePoint, double rotationDegree = 90, string prefix = "SL", string textAttached = "")
        {
            _station = station;
            base_point = basePoint;
            double rotationRadians = rotationDegree * Math.PI / 180;
            rotation_radians = rotationRadians;
            //plane_text = new Plane(basePoint, new XYZ(0, 0, 1));
            Prefix = prefix;
        }
        public StationLabel(double stationNumber, PolycurveXYZ poly, double startStation = 0, string prefix = "SL", double scale = 1.0,string textattached="")
        {
            LabelTextAttached = textattached;
            _station = stationNumber;
            Scale = scale;
            double changdu = stationNumber - startStation;
            if (stationNumber < 0 || stationNumber > (poly.GetLength() + startStation+0.1))
            {
                throw new Exception("桩号数值错误");
            }
            else
            {
                //Plane polyPalne;
                //bool planeflag = poly.TryGetPlane(out polyPalne);
                //11111111111
                Prefix = prefix;
                base_point = poly.PointAtLength(changdu);
                //double parameter;
                //bool paraflag = poly.LengthParameter(changdu, out parameter);
               
                    XYZ tangent = poly.TangentAt(changdu);
                    XYZ lineVector = tangent;
                    //22222222222
                    double qiejiao = Math.Atan2(lineVector.Y, lineVector.X);
                    rotation_radians = qiejiao + Math.PI / 2;

              
            }
        }
        //纵断面桩号
        public StationLabel(PolycurveXYZ poly, double stationNumber, string prefix = "SL", double startStation = 0, double scale = 1000, string textattached = "")
        {
            _station = stationNumber;
            double xro = Constantscs.basescale / scale;
            Scale = xro;
            double changdu = (stationNumber - startStation)*xro;
            LabelTextAttached=textattached;
            if (stationNumber < 0 || stationNumber > poly.GetLength() + startStation)
            {
                throw new Exception("桩号数值错误");
            }
            else
            {
                Prefix = prefix;
                base_point = poly.PointAtLength(changdu);
               
                    rotation_radians = Math.PI / 2;
               
            }
        }
        public void ChangeStation(double value)
        {
            _station = value;
        }
        //public string AddToDocument(Lv.BIMDoc doc)
        //{
        //    string result = "";

        //    //当前视图的plane
        //    Plane pl = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();

        //    pl.Origin = LabelLine.PointAtEnd ;
        //    var attr = new ObjectAttributes
        //    {
        //        ColorSource = ObjectColorSource.ColorFromObject,
        //        ObjectColor = Color.FromArgb(0, 0, 255)
        //    };

        //    //var object_id = doc.Objects.AddText(text, attr);
        //    TextEntity text = new TextEntity
        //    {
        //        PlainText = LabeText,
        //        Plane = pl,
        //        Justification = TextJustification.BottomRight,
        //        TextRotationRadians = rotation_radians,
        //        TextHeight = Text_height

        //    };
        //    TextEntity texta = new TextEntity
        //    {
        //        PlainText = LabelTextAttached,
        //        Plane = pl,
        //        Justification = TextJustification.TopRight,
        //        TextRotationRadians = rotation_radians,
        //        TextHeight = Text_height
        //    };
        //    List<Guid> rhobj_ids = new List<Guid>();
        //    rhobj_ids .Add(doc.Objects.AddText(text));
        //    rhobj_ids.Add(doc.Objects.AddText(texta));
        //    rhobj_ids.Add(doc.Objects.Add(LabelLine));
        //    doc.Groups.Add(rhobj_ids);
        //    result = result + "Line";
        //    return result;
        //}
        //public string DeleFromDucment(Lv.BIMDoc doc, ObjRef[] objrefs)
        //{
        //    string result = "";
        //    if (objrefs.Length > 0)
        //    {
        //        foreach (ObjRef oo in objrefs)
        //            doc.Objects.Delete(oo,true);
        //    }
        //    return result;
        //}
        //public string AddToDocumentNoOffset(Lv.BIMDoc doc)
        //{
        //    string result = "";

        //    //当前视图的plane
        //    Plane pl = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
        //    pl.Origin = base_point;
        //    //pl.Origin = base_point + new XYZ(0, 35, 0);
        //    var attr = new ObjectAttributes
        //    {
        //        ColorSource = ObjectColorSource.ColorFromObject,
        //        ObjectColor = Color.FromArgb(0, 0, 255)
        //    };

        //    //var object_id = doc.Objects.AddText(text, attr);
        //    TextEntity text = new TextEntity
        //    {
        //        PlainText = LabeText,
        //        Plane = pl,
        //        Justification = TextJustification.BottomLeft,
        //        TextRotationRadians = rotation_radians,
        //        TextHeight = Text_height

        //    };
        //    TextEntity texta = new TextEntity
        //    {
        //        PlainText = LabelTextAttached,
        //        Plane = pl,
        //        Justification = TextJustification.TopLeft,
        //        TextRotationRadians = rotation_radians,
        //        TextHeight = Text_height
        //    };
        //    doc.Objects.AddText(text);
        //    doc.Objects.AddText(texta);
        //    //doc.Objects.Add(text, attr);
        //    //result = result + "text" + ",";
        //    //doc.Objects.Add(texta, attr);
        //    //result = result + "texta" + ",";
        //    //LineXYZ cu = item as LineXYZ;
        //    doc.Objects.Add(LabelLine);
        //    result = result + "Line";
        //    return result;
        //}
        public override string ToString()
        {
            return LabeText;
        }
    }
    [Serializable]
    public class ArcLabel
    {
        private ArcXYZ _arc;
        private XYZ base_point;
        public double Scale = 1.0;
        public double Text_height => 3.5 * Scale;
        public string Text_format = "0.000";//设置小数点位数，默认三位小数
        private double rotation_radians;
        public string label_text => GetLabelText();
        //public Rectangle label_rec=>


        public ArcLabel(ArcXYZ arc, XYZ basepoint, double scale = 1.0, double rotationDegree = 0)
        {
            _arc = arc;
            rotation_radians = rotationDegree * Math.PI / 180;
            Scale = scale;
            base_point = basepoint + new XYZ(5 * Scale, 10 * Scale, 0);//暂时给定x=5，y=10的偏移量


        }
        
        public string GetLabelText()
        {
            ArcHelper arche = new ArcHelper(_arc);
            XYZ p1 = arche.p_p1;
            XYZ p2 = arche.P2;
            string result = "";
            result += "α=" + (_arc.AngleRadians * 180 / Math.PI).ToString(Text_format) + "°" + "\n";
            result += "R=" + (_arc.Radius).ToString(Text_format) + "m" + "\n";
            result += "S=" + (_arc.Length).ToString(Text_format) + "m" + "\n";
            result += "T=" + (p1.DistanceTo(p2)).ToString(Text_format) + "m" + "\n";
            return result;
        }

        //public void AddToDocument(Lv.BIMDoc doc)
        //{
        //    //string result = "";

        //    //当前视图的plane
        //    Plane pl = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
        //    pl.Origin = base_point;
        //    var attr = new ObjectAttributes
        //    {
        //        ColorSource = ObjectColorSource.ColorFromObject,
        //        ObjectColor = Color.FromArgb(0, 0, 255)
        //    };

        //    //var object_id = doc.Objects.AddText(text, attr);
        //    TextEntity text = new TextEntity
        //    {
        //        PlainText = label_text,
        //        Plane = pl,
        //        Justification = TextJustification.BottomLeft,
        //        TextRotationRadians = rotation_radians,
        //        TextHeight = Text_height

        //    };
        //    doc.Objects.Add(text);

        //    //LineXYZ cu = item as LineXYZ;


        //    //return result;
        //}

    }
    [Serializable]
    public class IPpointLabel
    {
        private int ip_index;
        private XYZ base_point;
        private double rotation_radians;
        public string Prefix = "IP";
        public double Text_height => 3.5 * Scale;
        public double Line_length => 35 * Scale;
        public XYZ Text_Origin {get {
                XYZ temp = base_point;
                temp.TransformTo(TransformXYZ.Translation(Line_length / 2, Line_length, 0),out XYZ result);
                return result;
            } }
        public string IP_format = "000";//设置显示几位数字
        public PolylineXYZ LabelLine
        {
            get
            {
                List<XYZ> result = new List<XYZ>();
                XYZ po = base_point;
                po.TransformTo(TransformXYZ.Translation(Line_length / 2, Line_length, 0),out XYZ po1);
                //XYZ po1 = po;
                po1.TransformTo(TransformXYZ.Translation(Line_length / 2, 0, 0),out XYZ po2);
                result.Add(base_point);
                result.Add(po1);
                result.Add(po2);
                return new PolylineXYZ(result);
            }
        }
        public string LabeText => Prefix + ip_index.ToString(IP_format);
        public string LabelTextAttached = "";
        public double Scale = 1.0;
        //public string GetLabelText()
        //{

        //    return result;
        //}
        public IPpointLabel(int ipIndex, XYZ basePoint, double scale = 1, string prefix = "SL", string textAttached = "", double rotationDegree=0)
        {
            ip_index = ipIndex;
            base_point = basePoint;
            rotation_radians = rotationDegree * Math.PI / 180;
            Prefix = prefix;
            LabelTextAttached = textAttached;
            Scale = scale;
        }

        //public string AddToDocument(Lv.BIMDoc doc)
        //{
        //    string result = "";

        //    //当前视图的plane
        //    Plane pl = doc.Views.ActiveView.ActiveViewport.ConstructionPlane();
        //    pl.Origin = Text_Origin;
        //    var attr = new ObjectAttributes
        //    {
        //        ColorSource = ObjectColorSource.ColorFromObject,
        //        ObjectColor = Color.FromArgb(0, 0, 255)
        //    };

        //    //var object_id = doc.Objects.AddText(text, attr);
        //    TextEntity text = new TextEntity
        //    {
        //        PlainText = LabeText,
        //        Plane = pl,
        //        Justification = TextJustification.BottomLeft,
        //        TextRotationRadians = rotation_radians,
        //        TextHeight = Text_height

        //    };
        //    TextEntity texta = new TextEntity
        //    {
        //        PlainText = LabelTextAttached,
        //        Plane = pl,
        //        Justification = TextJustification.TopLeft,
        //        TextRotationRadians = rotation_radians,
        //        TextHeight = Text_height
        //    };
        //    doc.Objects.Add(text, attr);
        //    result = result + "text" + ",";
        //    doc.Objects.Add(texta, attr);
        //    result = result + "texta" + ",";
        //    //LineXYZ cu = item as LineXYZ;
        //    doc.Objects.Add(LabelLine);
        //    result = result + "Polyline";
        //    return result;
        //}

        public override string ToString()
        {
            return LabeText;
        }
    }
    /// <summary>
    /// 扩展canal的桩号、圆弧、IP标注
    /// </summary>
    /**
    public static class CanalLabelHelper
    {
        /// <summary>
        /// 给canal添加一个桩号标注，调用方法为canal1.AddOneStationLabel
        /// </summary>
        /// <param name="canalIns"></param>
        /// <param name="station"></param>
        /// <param name="doc"></param>
        /// <param name="startStation"></param>
        /// <param name="prefix"></param>
        /// <param name="scale"></param>
        public static void AddOneStationLabel(this Canal canalIns, double station, Lv.BIMDoc doc, double scale = 1.0)
        {
            StationLabel sltemp = new StationLabel(station, canalIns.CenterLine, canalIns.StartStation, canalIns.Prefix, scale);
            sltemp.Prefix = canalIns.Prefix;
            sltemp.Scale = scale;
            sltemp.AddToDocumentNoOffset(doc);
            if (canalIns.StationList == null)
            {
                canalIns.StationList = new List<double>();
            }
            if (!canalIns.StationList.Contains(station))
            {
                canalIns.StationList.Add(station);
            }

        }
        public static void AddOneStationLabelToPolyCurve(this PolycurveXYZ canalIns, double station, Lv.BIMDoc doc, double scale = 1.0,string prefix="")
        {
            StationLabel sltemp = new StationLabel(station, canalIns, 0,prefix, scale);
            sltemp.Prefix = prefix;
            sltemp.Scale = scale;
            sltemp.AddToDocumentNoOffset(doc);
        }
        public static void AddOneStationLabelToPolyCurve_X(this PolycurveXYZ poly, double station_position, Lv.BIMDoc doc, double scale = 1.0, string prefix = "")
        {
            StationLabel sltemp = new StationLabel(station_position, poly, 0, prefix, scale);
            sltemp.rotation_radians = Math.PI / 2;
            XYZ start = poly.PointAtStart;
            XYZ poatlen = poly.PointAtLength(station_position);
            double xvalue = poatlen.X - start.X;
            sltemp.ChangeStation(xvalue);
            sltemp.Prefix = prefix;
            sltemp.Scale = scale;
            sltemp.AddToDocumentNoOffset(doc);
        }
        public static void AddStationLabelList(this Canal canalIns, Lv.BIMDoc doc, double scale = 1.0)
        {
            //View().ActiveViewport.ConstructionPlane();
            //List<string> result = new List<string>();

            //if (canalIns.StationList == null)
            //{
            //    canalIns.StationList = new List<double>();
            //    //以下代码修改canal内容
            //    canalIns.StationList.AddRange(canalIns.CenterLinePointsStationList);
            //    // result.Add(jieguo);
            //}
            List<double> stationlist = canalIns.CenterLinePointsStationList;
            foreach (double sta in stationlist)
            {
                StationLabel sltemp = new StationLabel(sta, canalIns.CenterLine, canalIns.StartStation, canalIns.Prefix, scale);
                sltemp.Scale = scale;
                string jieguo = sltemp.AddToDocumentNoOffset(doc);

            }
            //return result;
        }
        public static void AddLittleStructureStationLabelList(this Canal canalIns, LittleStructure canalstructure, Lv.BIMDoc doc, double scale = 1.0)
        {
            double stationlist = canalstructure.Station;
            string attachedtexts = canalstructure.m_name;
                StationLabel sltemp = new StationLabel(stationlist, canalIns.CenterLine, canalIns.StartStation, canalIns.Prefix, scale, attachedtexts);
                sltemp.Scale = scale;
                //string jieguo = sltemp.AddToDocument(doc);
        }

        public static void AddCanalStructureStationLabelList(this Canal canalIns, CanalStructure canalstructure, Lv.BIMDoc doc, double scale = 1.0)
        {
            //View().ActiveViewport.ConstructionPlane();
            //List<string> result = new List<string>();

            //if (canalIns.StationList == null)
            //{
            //    canalIns.StationList = new List<double>();
            //    //以下代码修改canal内容
            //    canalIns.StationList.AddRange(canalIns.CenterLinePointsStationList);
            //    // result.Add(jieguo);
            //}
            if (canalstructure.stuType.ToDescription().Contains("明渠"))
            {
                return;
            }
            List<double> stationlist = canalstructure.StationList;
            List<string> attachedtexts = canalstructure.GetStationLabelAtteachedTextList();
            int i = 0;
            foreach (double sta in stationlist)
            {
               
                StationLabel sltemp = new StationLabel(sta, canalIns.CenterLine, canalIns.StartStation, canalIns.Prefix, scale,attachedtexts[i]);
                sltemp.Scale = scale;
                //string jieguo = sltemp.AddToDocumentNoOffset(doc);
                i++;
            }
            //return result;
        }
        public static void AddOneArcLabel(this Canal canalIns, Lv.BIMDoc doc, XYZ closestpo, double scale = 1.0)
        {
            PolyCurve poly = canalIns.CenterLine;
            double length = 0;
            bool flag = poly.ClosestPoint(closestpo, out length);
            if (flag)
            {
                int arci = poly.SegmentIndex(length);
                if (poly.SegmentCurve(arci).GetType().Name == "ArcXYZ")
                {
                    ArcXYZ arctemp = poly.SegmentCurve(arci) as ArcXYZ;
                    ArcLabel arclabeltemp = new ArcLabel(arctemp, closestpo, scale);
                    arclabeltemp.AddToDocument(doc);
                }
                else
                {
                    throw new Exception("未获取到最近圆弧");
                }

            }
            else
            {
                throw new Exception("未获取到最近圆弧");
            }

        }
        public static void AddArcLabelList(this Canal canalIns, Lv.BIMDoc doc, double scale = 1.0)
        {
            List<XYZ> constuctpoints = canalIns.ConstructLinePoints;
            List<double> arcradius = canalIns.ArcRadiuList;
            int NumberOfIP = canalIns.NumberOfIP;
            for (int i = 0; i < NumberOfIP - 2; i++)
            {
                ArcHelper ar = new ArcHelper(constuctpoints[i], constuctpoints[i + 1], constuctpoints[i + 2], arcradius[i]);
                Arc inarc = ar.qiehu;
                ArcLabel arclabeltemp = new ArcLabel(inarc, inarc.MidPoint, scale);
                arclabeltemp.AddToDocument(doc);
            }
        }
        public static void AddOneIPLabel(this Canal canalIns, Lv.BIMDoc doc, XYZ closestpo, double scale = 1.0)
        {
            Polyline poly = canalIns.ConstructLine;
            List<XYZ> ips = canalIns.IpPointsList;
            
            XYZ po = poly.ClosestPoint(closestpo);
            int index = canalIns.IndexOfPointInConstructLinePoints(po);                    
            IPpointLabel arclabeltemp = new IPpointLabel(index, ips[index], scale);
            arclabeltemp.AddToDocument(doc);
        }
        public static void AddIPLabelList(this Canal canalIns, Lv.BIMDoc doc, double scale = 1.0)
        {
            List<XYZ> ips = canalIns.IpPointsList;
            
            for(int i=0;i<ips.Count;i++)
            {
                IPpointLabel arclabeltemp = new IPpointLabel(i, ips[i], scale);
                arclabeltemp.AddToDocument(doc);
            }

           
            
        }
    }
    **/
}
