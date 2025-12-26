using CanalSystem.BaseTools;
using Lv.BIM.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CanalSystem.BaseClass
{

    [Serializable]
    public struct StationElevation
    {
        public double Station; //桩号
        public double Elevation;//高程
        public StationElevation(double Station1, double Elevation1)
        {
            Station = Station1;
            Elevation = Elevation1;
        }
    }

    [Serializable]
    public struct StationCharacteristics
    {
        public double Station; //桩号
        public double WaterElevation;//高程
        public double ButtomElevation;//建筑底高程
        public double TopElevation;//建筑顶高程
        public string Stationname;//特征点名
        
        public StationCharacteristics(double Station1, double waterElevation,double buttomElevation,double topElevation,string stationname)
        {
            Station = Station1;
            WaterElevation = waterElevation;
            ButtomElevation = buttomElevation;
            TopElevation = topElevation;
            Stationname = stationname;
        }
        public string ToString(string format = "0.000", IFormatProvider provider = null)
        {
            return string.Format("{0}{1}{2}{3}{4}", new object[5]
            {
            "                   Station:"+"\t"+Station+"\n",
            "     WaterElevation:"+"\t"+WaterElevation.ToString(format, provider)+"\n",
            "  ButtomElevation:"+"\t"+ButtomElevation.ToString(format, provider)+"\n",
            "        TopElevation:"+"\t"+TopElevation.ToString(format, provider)+"\n",
            "         Stationname:"+"\t"+Stationname+"\n"
            });
        }
        public DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();
            dt.Columns.Add("桩号");
            dt.Columns.Add("名称");
            dt.Columns.Add("水位");
            dt.Columns.Add("底板高程");
            dt.Columns.Add("顶板高程");
            dr["桩号"] = Station;
            dr["名称"] = Stationname;
            dr["水位"] = WaterElevation;
            dr["底板高程"] = ButtomElevation;
            dr["顶板高程"] = TopElevation;
   
            dt.Rows.Add(dr);
            return dt;
        }
        public StationCharacteristics AddElevation(double valueToAdd)
        {
            StationCharacteristics result = new StationCharacteristics();
            result.Station = Station;
            result.WaterElevation = WaterElevation+ valueToAdd;
            result.ButtomElevation = ButtomElevation + valueToAdd;
            result.TopElevation = TopElevation + valueToAdd;
            result.Stationname = Stationname;
            return result;
        }
        public StationCharacteristics AddStation(double valueToAdd)
        {
            StationCharacteristics result = new StationCharacteristics();
            result.Station = Station+valueToAdd;
            result.WaterElevation = WaterElevation ;
            result.ButtomElevation = ButtomElevation ;
            result.TopElevation = TopElevation ;
            result.Stationname = Stationname;
            return result;
        }
    }
    [Serializable]
    public enum StructureType
    {
        OpenChannel1_1dot5 = 0,//梯形明渠
        OpenChannel1_1 = 1,//梯形明渠
        OpenChannel1_0dot75 = 2,//梯形明渠
        RecChannel = 3,//矩形渠
        TunnelCircleCo = 4,//圆形隧洞
        TunnelArchGateCo = 5,//城门洞型隧洞
        AqueductRec = 6,//矩形渡槽
        AqueductU = 7,//U形渡槽
        BuriedConduit = 8,//暗涵
        Sluice = 9, //闸
        Siphon = 10,//倒虹管
        WaterFall = 11//陡坡
    }
    [Serializable]
    public static class StructureTypeExtension
    {
        public static string ToDescription(this StructureType type)
        {
            switch (type)
            {
                case StructureType.OpenChannel1_1dot5:
                    return "1:1.5梯形明渠";
                case StructureType.OpenChannel1_1:
                    return "1:1梯形明渠";
                case StructureType.OpenChannel1_0dot75:
                    return "1:0.75梯形明渠";
                case StructureType.RecChannel:
                    return "矩形明渠";
                case StructureType.TunnelCircleCo:
                    return "圆形隧洞";
                case StructureType.TunnelArchGateCo:
                    return "城门洞型隧洞";
                case StructureType.AqueductRec:
                    return "矩形渡槽";
                case StructureType.AqueductU:
                    return "U形渡槽";
                case StructureType.BuriedConduit:
                    return "暗涵";
                case StructureType.Sluice:
                    return "闸";
                case StructureType.Siphon:
                    return "倒虹管";
                case StructureType.WaterFall:
                    return "陡坡";
                default:
                    return "1:1.5梯形明渠";
            }
        }
        public static StructureType ToStructureType(this string type)
        {
            switch (type)
            {
                case "1;1.5梯形明渠":
                    return StructureType.OpenChannel1_1dot5;
                case "1;1梯形明渠":
                    return StructureType.OpenChannel1_1;
                case "1;0.75梯形明渠":
                    return StructureType.OpenChannel1_0dot75;
                case "矩形明渠":
                    return StructureType.RecChannel;
                case "圆形隧洞":
                    return StructureType.TunnelCircleCo;
                case "城门洞型隧洞":
                    return StructureType.TunnelArchGateCo;
                case "矩形渡槽":
                    return StructureType.AqueductRec;
                case "U形渡槽":
                    return StructureType.AqueductU;
                case "暗涵":
                    return StructureType.BuriedConduit;
                case "闸":
                    return StructureType.Sluice;
                case "倒虹管":
                    return StructureType.Siphon;
                case "陡坡":
                    return StructureType.WaterFall;
                default:
                    return StructureType.OpenChannel1_1dot5;

            }
        }
        /// <summary>
        /// Gets all items for an enum value.（通过枚举对象获取所有枚举）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static List<string> GetStructureTypeAllDescriptions()
        {
            List<string> result = new List<string>();
            foreach (StructureType item in Enum.GetValues(typeof(StructureType)))
            {
                string aa = item.ToDescription();
                result.Add(aa);
            }
            return result;
        }
    }

    /// <summary>
    /// 包含所有参数的断面
    /// </summary>
    [Serializable]
    public class GeneralSectionCo
    {
        public string flowsegname;
        public string leixing;
          public  double Flow;
          public  double Velocity;
          public  double Area;
          public  double Roughness;
          public  double WetPerimeter;
          public  double HydraulicRadius;
          public  double LongSlope;
          public  double Height;
          public  double Depth;
          public  double BottomWidth;
          public  double SlopeRatio;
          public  double LiningThickness;

          public  double SideWallHeight;//隧洞参数
          public  double BottomBoardThickness;//隧洞参数

          public  double Radius;//U型或圆形

          public  double IncreasedFlow;//加大流量
        public GeneralSectionCo() { }
        public object[] ToArray()
        {
            object[] result;
            result = new object[] {flowsegname,leixing, Flow, Velocity, Area, Roughness, WetPerimeter, HydraulicRadius, LongSlope, Height, Depth, BottomWidth, SlopeRatio, LiningThickness,  SideWallHeight,   BottomBoardThickness, Radius, IncreasedFlow};
            return result;
        }
        public void Clear()
        {
            flowsegname = "";
            leixing = "";
            Flow=0;
            Velocity=0;
            Area=0;
            Roughness=0;
            WetPerimeter=0;
            HydraulicRadius=0;
            LongSlope=0;
            Height=0;
            Depth=0;
            BottomWidth=0;
            SlopeRatio=0;
            LiningThickness=0;

            SideWallHeight=0;//隧洞参数
            BottomBoardThickness=0;//隧洞参数

            Radius=0;//U型或圆形

            IncreasedFlow=0;//加大流量

        }
    }
    [Serializable]
    public class SectionCo
    {
        public double Flow ;
        public double Velocity ;
        public double Area ;
        public double Roughness ;
        public double WetPerimeter ;
        public double HydraulicRadius ;
        public double LongSlope ;
        public double Depth ;
        public double BottomWidth ;
        public double SlopeRatio ;
        public double Height;
        public SectionCo() { }
        //计算沿程水头损失
        public double Frictionloss(double length)
        {
            double YCLoss;

            YCLoss = length / LongSlope;

            return YCLoss;
        }
        //计算弯道水头损失
        public double Curveloss(ArcXYZ arc)
        {
            double WDLoss;
            WDLoss = arc.GetLength() * Math.Pow(Roughness, 2.0) / Math.Pow(HydraulicRadius, 1.3333) * Math.Pow(Velocity, 2.0) * 3.0 / 4.0 * Math.Pow((BottomWidth + Depth * SlopeRatio * 1.3 * 2.0) / arc.Radius, 0.5);
            return WDLoss;
        }
   
        public List<double> Curveloss(CanalGeometry can)
        {
            List<double> result = new List<double>();
            double WDLoss = 0;
            List<double> arrras = can.ArcRadiuList;
            List<XYZ> pos = can.ConstructLinePoints;
            if (arrras != null && arrras.Count > 0)
            {
                for (int i = 0; i < arrras.Count; i++)
                {
                    ArcHelper ar = new ArcHelper(pos[i], pos[i + 1], pos[i + 2], arrras[i]);
                    ArcXYZ inarc = ar.qiehu;
                    double temploss = Curveloss(inarc);
                    result.Add(temploss);
                    WDLoss += temploss;
                }
            }
            return result;
        }
        //计算预留水头损失
        public double Spareloss(PolycurveXYZ poly)
        {
            double YLLoss;
            YLLoss = poly.GetLength() / 50000.0 * 0.0;
            return YLLoss;
        }
        public double Spareloss(double length)
        {
            double YLLoss;
            YLLoss = length / 50000.0 * 0.0;
            return YLLoss;
        }
        //进口渐变段水头损失
        public double EnterTranSectionLoss(double inLetVelocity)
        {
            double rawWaterLoss = (Math.Pow(inLetVelocity, 2) - Math.Pow(Velocity, 2)) / 2 / 9.81;
            double waterLoss = rawWaterLoss >= 0 ? rawWaterLoss * 1.15 : rawWaterLoss * 0.7;
            return waterLoss;
        }
        //出口渐变段水头损失
        public double ExitTranSectionLoss(double outLetVelocity)
        {
            double rawWaterLoss = (Math.Pow(Velocity, 2) - Math.Pow(outLetVelocity, 2)) / 2 / 9.81;
            double waterLoss = rawWaterLoss >= 0 ? rawWaterLoss * 1.15 : rawWaterLoss * 0.7;
            return waterLoss;
        }
        
        public List<double> ChannelWaterlevelWithSpareloss(CanalGeometry can)
        {
            List<double> result = new List<double>();
            result.Add(0.0);
            List<XYZ> constructLine_points = can.ConstructLinePoints;
            List<double> centerLine_arcRadius = can.ArcRadiuList;
            //PolyCurve poresult = new PolyCurve();
            XYZ end = constructLine_points[0];
            //每次均添加一个直线，一个圆弧
            //1、添加直线
            //a添加起点，起点使用上次添加的末端，

            //b添加末端需计算是否圆弧半径为0
            //2、添加圆弧，如果没有则不添加
            for (int i = 0; i < can.NumberOfIP - 2; i++)
            {
                XYZ stpo = end;
                XYZ edpo;
                if (Math.Abs(centerLine_arcRadius[i]) < 1E-6)//直线，i=1
                                                             //if (centerLine_arcRadius[i] < 1E-6)//直线，i=1
                {
                    edpo = constructLine_points[i + 1];
                    LineXYZ li = new LineXYZ(stpo, edpo);
                    double frictionlosstemp = Frictionloss(li.GetLength());
                    double sparelosstemp = Spareloss(li.GetLength());
                    result.Add(result.Last()- frictionlosstemp-sparelosstemp);
                    //poresult.Append(li);
                    end = edpo;
                }

                else//首端圆弧i=1，r0!=0
                {

                    ArcHelper ar = new ArcHelper(constructLine_points[i], constructLine_points[i + 1], constructLine_points[i + 2], centerLine_arcRadius[i]);
                    ArcXYZ inarc = ar.qiehu;
                    double frictionlosstemp = Curveloss(inarc)+Frictionloss(inarc.Length);
                    double Sparelosstemp = Spareloss(inarc.Length);
                    result.Add(result.Last() - Sparelosstemp - frictionlosstemp);
                    edpo = ar.p_p1;
                    LineXYZ li = new LineXYZ(stpo, edpo);
                   frictionlosstemp = Frictionloss(li.GetLength());
                    double sparelosstemp = Spareloss(li.GetLength());
                    result.Add(result.Last() - frictionlosstemp - sparelosstemp);
                    //result.Add(result.Last() - losstemp);
                    //poresult.Append(li);
                    //poresult.Append(inarc);
                    end = ar.p_p3;
                }
            }
            LineXYZ liend = new LineXYZ(end, constructLine_points.Last());
            double frictionlosstemp1 = Frictionloss(liend.GetLength());
            double Sparelosstemp1 = Spareloss(liend.GetLength());
            result.Add(result.Last() - frictionlosstemp1- Sparelosstemp1);
            return result;
        }

        public List<double> ChannelWaterlevelWithoutSpareloss(CanalGeometry can)
        {
            List<double> result = new List<double>();
            result.Add(0.0);
            List<XYZ> constructLine_points = can.ConstructLinePoints;
            List<double> centerLine_arcRadius = can.ArcRadiuList;
            //PolyCurve poresult = new PolyCurve();
            XYZ end = constructLine_points[0];
            //每次均添加一个直线，一个圆弧
            //1、添加直线
            //a添加起点，起点使用上次添加的末端，

            //b添加末端需计算是否圆弧半径为0
            //2、添加圆弧，如果没有则不添加
            for (int i = 0; i < can.NumberOfIP - 2; i++)
            {
                XYZ stpo = end;
                XYZ edpo;
                if (Math.Abs(centerLine_arcRadius[i]) < 1E-6)//直线，i=1
                                                             //if (centerLine_arcRadius[i] < 1E-6)//直线，i=1
                {
                    edpo = constructLine_points[i + 1];
                    LineXYZ li = new LineXYZ(stpo, edpo);
                    double frictionlosstemp = Frictionloss(li.GetLength());
                    double sparelosstemp = 0;
                    result.Add(result.Last() - frictionlosstemp - sparelosstemp);
                    //poresult.Append(li);
                    end = edpo;
                }

                else//首端圆弧i=1，r0!=0
                {

                    ArcHelper ar = new ArcHelper(constructLine_points[i], constructLine_points[i + 1], constructLine_points[i + 2], centerLine_arcRadius[i]);
                    ArcXYZ inarc = ar.qiehu;
                    double frictionlosstemp = Curveloss(inarc) + Frictionloss(inarc.Length);
                    double Sparelosstemp = 0;
                    result.Add(result.Last() - Sparelosstemp - frictionlosstemp);
                    edpo = ar.p_p1;
                    LineXYZ li = new LineXYZ(stpo, edpo);
                    frictionlosstemp = Frictionloss(li.GetLength());
                    double sparelosstemp = 0;
                    result.Add(result.Last() - frictionlosstemp - sparelosstemp);
                    //result.Add(result.Last() - losstemp);
                    //poresult.Append(li);
                    //poresult.Append(inarc);
                    end = ar.p_p3;
                }
            }
            LineXYZ liend = new LineXYZ(end, constructLine_points.Last());
            double frictionlosstemp1 = Frictionloss(liend.GetLength());
            double Sparelosstemp1 = 0;
            result.Add(result.Last() - frictionlosstemp1 - Sparelosstemp1);
            return result;
        }
    }
    //梯形渠参数1:1.5
    [Serializable]
    public class OpenChannelCo:SectionCo
    {
        

        public double LiningThickness;
        public OpenChannelCo()
        {

        }
        //public OpenChannelCo(int i, ISheet sheet)
        //{
        //    int y = 74;//一个流量段输入数据在excel表中的行数
        //    Flow = sheet.GetRow((i - 1) * y + 7).GetCell(11).NumericCellValue;
        //    Velocity = sheet.GetRow((i - 1) * y + 7).GetCell(10).NumericCellValue;
        //    Area = sheet.GetRow((i - 1) * y + 7).GetCell(4).NumericCellValue;
        //    Roughness = sheet.GetRow((i - 1) * y + 7).GetCell(7).NumericCellValue;
        //    WetPerimeter = sheet.GetRow((i - 1) * y + 7).GetCell(5).NumericCellValue;
        //    HydraulicRadius = sheet.GetRow((i - 1) * y + 7).GetCell(6).NumericCellValue;
        //    Height = sheet.GetRow((i - 1) * y + 15).GetCell(12).NumericCellValue;
        //    LongSlope = sheet.GetRow((i - 1) * y + 7).GetCell(8).NumericCellValue;
        //    Depth = sheet.GetRow((i - 1) * y + 7).GetCell(1).NumericCellValue;
        //    BottomWidth = sheet.GetRow((i - 1) * y + 7).GetCell(2).NumericCellValue;
        //    SlopeRatio = sheet.GetRow((i - 1) * y + 7).GetCell(3).NumericCellValue;
        //    LiningThickness = sheet.GetRow((i - 1) * y + 15).GetCell(13).NumericCellValue;

        //}
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}", new object[12]
            {
            Flow.ToString(format, provider)+"\t",
            Velocity.ToString(format, provider)+"\t",
            Area.ToString(format, provider)+"\t",
            Roughness.ToString(format, provider)+"\t",
            WetPerimeter.ToString(format, provider)+"\t",
            HydraulicRadius.ToString(format, provider)+"\t",
            Height.ToString(format, provider)+"\t",
            LongSlope.ToString(format, provider)+"\t",
            Depth.ToString(format, provider)+"\t",
            BottomWidth.ToString(format, provider)+"\t",
            SlopeRatio.ToString(format, provider)+"\t",
            LiningThickness.ToString(format, provider)+"\t"
            });
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }


        public  override string ToString()
        {
            return ToString(null, null);
        }
    }

    //矩形渠
    [Serializable]
    public class RecChannelCo:SectionCo
    {

        public double LiningThickness;

        public RecChannelCo(){}
        //public RecChannelCo(int i, ISheet sheet)
        //{
        //    int y = 74;//一个流量段输入数据在excel表中的行数
        //    Flow = sheet.GetRow((i - 1) * y + 7).GetCell(11).NumericCellValue;
        //    Velocity = sheet.GetRow((i - 1) * y + 8).GetCell(10).NumericCellValue;
        //    Area = sheet.GetRow((i - 1) * y + 8).GetCell(4).NumericCellValue;
        //    Roughness = sheet.GetRow((i - 1) * y + 8).GetCell(7).NumericCellValue;
        //    WetPerimeter = sheet.GetRow((i - 1) * y + 8).GetCell(5).NumericCellValue;
        //    HydraulicRadius = sheet.GetRow((i - 1) * y + 8).GetCell(6).NumericCellValue;
        //    Height = sheet.GetRow((i - 1) * y + 16).GetCell(12).NumericCellValue;
        //    LongSlope = sheet.GetRow((i - 1) * y + 8).GetCell(8).NumericCellValue;
        //    Depth = sheet.GetRow((i - 1) * y + 8).GetCell(1).NumericCellValue;
        //    BottomWidth = sheet.GetRow((i - 1) * y + 8).GetCell(2).NumericCellValue;
        //    SlopeRatio = sheet.GetRow((i - 1) * y + 8).GetCell(3).NumericCellValue;
        //    LiningThickness = sheet.GetRow((i - 1) * y + 16).GetCell(13).NumericCellValue;

        //}
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}", new object[12]
            {
            Flow.ToString(format, provider)+"\t",
            Velocity.ToString(format, provider)+"\t",
            Area.ToString(format, provider)+"\t",
            Roughness.ToString(format, provider)+"\t",
            WetPerimeter.ToString(format, provider)+"\t",
            HydraulicRadius.ToString(format, provider)+"\t",
            Height.ToString(format, provider)+"\t",
            LongSlope.ToString(format, provider)+"\t",
            Depth.ToString(format, provider)+"\t",
            BottomWidth.ToString(format, provider)+"\t",
            SlopeRatio.ToString(format, provider)+"\t",
            LiningThickness.ToString(format, provider)+"\t"
            });
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }


        public override string ToString()
        {
            return ToString(null, null);
        }
    }

    //隧洞参数
    [Serializable]
    public class TunnelCo:SectionCo
    {

        public double SideWallHeight;
        public double BottomBoardThickness;
        public TunnelCo() { }
        //public TunnelCo(int i, ISheet sheet)
        //{
        //    int y = 74;//一个流量段输入数据在excel表中的行数
        //    Flow = sheet.GetRow((i - 1) * y + 45).GetCell(10).NumericCellValue;
        //    Velocity = sheet.GetRow((i - 1) * y + 45).GetCell(11).NumericCellValue;
        //    Area = sheet.GetRow((i - 1) * y + 45).GetCell(5).NumericCellValue;
        //    Roughness = sheet.GetRow((i - 1) * y + 45).GetCell(8).NumericCellValue;
        //    WetPerimeter = sheet.GetRow((i - 1) * y + 45).GetCell(6).NumericCellValue;
        //    HydraulicRadius = sheet.GetRow((i - 1) * y + 45).GetCell(7).NumericCellValue;
        //    Height = sheet.GetRow((i - 1) * y + 45).GetCell(14).NumericCellValue;
        //    LongSlope = sheet.GetRow((i - 1) * y + 45).GetCell(9).NumericCellValue;
        //    Depth = sheet.GetRow((i - 1) * y + 45).GetCell(1).NumericCellValue;
        //    BottomWidth = sheet.GetRow((i - 1) * y + 45).GetCell(2).NumericCellValue;
        //    SideWallHeight = sheet.GetRow((i - 1) * y + 45).GetCell(3).NumericCellValue;
        //    BottomBoardThickness = sheet.GetRow((i - 1) * y + 45).GetCell(17).NumericCellValue;

        //}
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}", new object[12]
            {
            Flow.ToString(format, provider)+"\t",
            Velocity.ToString(format, provider)+"\t",
            Area.ToString(format, provider)+"\t",
            Roughness.ToString(format, provider)+"\t",
            WetPerimeter.ToString(format, provider)+"\t",
            HydraulicRadius.ToString(format, provider)+"\t",
            Height.ToString(format, provider)+"\t",
            LongSlope.ToString(format, provider)+"\t",
            Depth.ToString(format, provider)+"\t",
            BottomWidth.ToString(format, provider)+"\t",
            SideWallHeight.ToString(format, provider)+"\t",
            BottomBoardThickness.ToString(format, provider)+"\t"
            });
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }


        public override string ToString()
        {
            return ToString(null, null);
        }
    }

    //渡槽参数
    [Serializable]
    public class AqueductCo:SectionCo
    {

        public double Radius;//U型
        public double BottomBoardThickness;
        public AqueductCo() { }
        //public AqueductCo(int i, string shape, ISheet sheet)//U型构造
        //{
        //    int y = 74;//一个流量段输入数据在excel表中的行数
        //    Flow = sheet.GetRow((i - 1) * y + 25).GetCell(8).NumericCellValue;
        //    Velocity = sheet.GetRow((i - 1) * y + 25).GetCell(9).NumericCellValue;
        //    Area = sheet.GetRow((i - 1) * y + 25).GetCell(3).NumericCellValue;
        //    Roughness = sheet.GetRow((i - 1) * y + 25).GetCell(6).NumericCellValue;
        //    WetPerimeter = sheet.GetRow((i - 1) * y + 25).GetCell(4).NumericCellValue;
        //    HydraulicRadius = sheet.GetRow((i - 1) * y + 25).GetCell(5).NumericCellValue;
        //    Height = sheet.GetRow((i - 1) * y + 25).GetCell(10).NumericCellValue;
        //    LongSlope = sheet.GetRow((i - 1) * y + 25).GetCell(7).NumericCellValue;
        //    Depth = sheet.GetRow((i - 1) * y + 25).GetCell(1).NumericCellValue;
        //    Radius = sheet.GetRow((i - 1) * y + 25).GetCell(2).NumericCellValue;
        //    BottomWidth = 0.0;
        //    BottomBoardThickness = sheet.GetRow((i - 1) * y + 25).GetCell(14).NumericCellValue;
        //}

        //public AqueductCo(string shape, int i, ISheet sheet)//矩形构造
        //{
        //    int y = 74;//一个流量段输入数据在excel表中的行数
        //    Flow = sheet.GetRow((i - 1) * y + 34).GetCell(9).NumericCellValue;
        //    Velocity = sheet.GetRow((i - 1) * y + 34).GetCell(10).NumericCellValue;
        //    Area = sheet.GetRow((i - 1) * y + 34).GetCell(4).NumericCellValue;
        //    Roughness = sheet.GetRow((i - 1) * y + 34).GetCell(7).NumericCellValue;
        //    WetPerimeter = sheet.GetRow((i - 1) * y + 34).GetCell(5).NumericCellValue;
        //    HydraulicRadius = sheet.GetRow((i - 1) * y + 34).GetCell(6).NumericCellValue;
        //    Height = sheet.GetRow((i - 1) * y + 34).GetCell(11).NumericCellValue;
        //    LongSlope = sheet.GetRow((i - 1) * y + 34).GetCell(8).NumericCellValue;
        //    Depth = sheet.GetRow((i - 1) * y + 34).GetCell(1).NumericCellValue;
        //    Radius = 0.0;
        //    BottomWidth = sheet.GetRow((i - 1) * y + 34).GetCell(2).NumericCellValue;
        //    BottomBoardThickness = sheet.GetRow((i - 1) * y + 34).GetCell(13).NumericCellValue;
        //}
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}", new object[12]
            {
            Flow.ToString(format, provider)+"\t",
            Velocity.ToString(format, provider)+"\t",
            Area.ToString(format, provider)+"\t",
            Roughness.ToString(format, provider)+"\t",
            WetPerimeter.ToString(format, provider)+"\t",
            HydraulicRadius.ToString(format, provider)+"\t",
            Height.ToString(format, provider)+"\t",
            LongSlope.ToString(format, provider)+"\t",
            Depth.ToString(format, provider)+"\t",
            BottomWidth.ToString(format, provider)+"\t",
            Radius.ToString(format, provider)+"\t",
            BottomBoardThickness.ToString(format, provider)+"\t"
            });
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }


        public override string ToString()
        {
            return ToString(null, null);
        }
    }


    //暗涵参数
    [Serializable]
    public class BuriedConduitCo:SectionCo
    {
        public double SideWallHeight;
        public double BottomBoardThickness;

        public BuriedConduitCo() { }
        //public BuriedConduitCo(int i, ISheet sheet)
        //{
        //    int y = 74;//一个流量段输入数据在excel表中的行数
        //    Flow = sheet.GetRow((i - 1) * y + 56).GetCell(10).NumericCellValue;
        //    Velocity = sheet.GetRow((i - 1) * y + 56).GetCell(11).NumericCellValue;
        //    Area = sheet.GetRow((i - 1) * y + 56).GetCell(5).NumericCellValue;
        //    Roughness = sheet.GetRow((i - 1) * y + 56).GetCell(8).NumericCellValue;
        //    WetPerimeter = sheet.GetRow((i - 1) * y + 56).GetCell(6).NumericCellValue;
        //    HydraulicRadius = sheet.GetRow((i - 1) * y + 56).GetCell(7).NumericCellValue;
        //    Height = sheet.GetRow((i - 1) * y + 56).GetCell(14).NumericCellValue;
        //    LongSlope = sheet.GetRow((i - 1) * y + 56).GetCell(9).NumericCellValue;
        //    Depth = sheet.GetRow((i - 1) * y + 56).GetCell(1).NumericCellValue;
        //    BottomWidth = sheet.GetRow((i - 1) * y + 56).GetCell(2).NumericCellValue;
        //    SideWallHeight = sheet.GetRow((i - 1) * y + 56).GetCell(3).NumericCellValue;
        //    BottomBoardThickness = sheet.GetRow((i - 1) * y + 56).GetCell(17).NumericCellValue;
        //}
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}", new object[12]
            {
            Flow.ToString(format, provider)+"\t",
            Velocity.ToString(format, provider)+"\t",
            Area.ToString(format, provider)+"\t",
            Roughness.ToString(format, provider)+"\t",
            WetPerimeter.ToString(format, provider)+"\t",
            HydraulicRadius.ToString(format, provider)+"\t",
            Height.ToString(format, provider)+"\t",
            LongSlope.ToString(format, provider)+"\t",
            Depth.ToString(format, provider)+"\t",
            BottomWidth.ToString(format, provider)+"\t",
            SideWallHeight.ToString(format, provider)+"\t",
            BottomBoardThickness.ToString(format, provider)+"\t"
            });
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }


        public override string ToString()
        {
            return ToString(null, null);
        }
    }

    //闸参数
    [Serializable]
    public class SluiceCo : SectionCo
    {


        public double BottomBoardThickness;

        public SluiceCo() { }
        //public SluiceCo(int i, ISheet sheet)
        //{
        //    int y = 74;//一个流量段输入数据在excel表中的行数
        //    Flow = sheet.GetRow((i - 1) * y + 65).GetCell(0).NumericCellValue;
        //    Velocity = sheet.GetRow((i - 1) * y + 65).GetCell(11).NumericCellValue;
        //    Area = sheet.GetRow((i - 1) * y + 65).GetCell(15).NumericCellValue;
        //    Height = sheet.GetRow((i - 1) * y + 68).GetCell(14).NumericCellValue;
        //    Depth = sheet.GetRow((i - 1) * y + 65).GetCell(13).NumericCellValue;
        //    BottomWidth = sheet.GetRow((i - 1) * y + 65).GetCell(1).NumericCellValue;
        //    BottomBoardThickness = sheet.GetRow((i - 1) * y + 68).GetCell(15).NumericCellValue;
        //}
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}", new object[7]
            {
            Flow.ToString(format, provider)+"\t",
            Velocity.ToString(format, provider)+"\t",
            Area.ToString(format, provider)+"\t",    
            Height.ToString(format, provider)+"\t",
            Depth.ToString(format, provider)+"\t",
            BottomWidth.ToString(format, provider)+"\t",
            BottomBoardThickness.ToString(format, provider)+"\t"
            });
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }


        public override string ToString()
        {
            return ToString(null, null);
        }
    }
    //倒虹管参数
    [Serializable]
    public class SiphonCo : SectionCo
    {

        public double IncreasedFlow;



        public SiphonCo() { }
        //public SiphonCo(int i, ISheet sheet)
        //{
        //    int y = 74;//一个流量段输入数据在excel表中的行数
        //    Flow = sheet.GetRow((i - 1) * y + 71).GetCell(2).NumericCellValue;
        //    Velocity = sheet.GetRow((i - 1) * y + 71).GetCell(3).NumericCellValue;
        //    Area = sheet.GetRow((i - 1) * y + 71).GetCell(6).NumericCellValue;
        //    Depth = sheet.GetRow((i - 1) * y + 71).GetCell(1).NumericCellValue;
        //    BottomWidth = sheet.GetRow((i - 1) * y + 71).GetCell(0).NumericCellValue;
        //    IncreasedFlow = sheet.GetRow((i - 1) * y + 71).GetCell(5).NumericCellValue;
        //}
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}", new object[6]
            {
            Flow.ToString(format, provider)+"\t",
            Velocity.ToString(format, provider)+"\t",
            Area.ToString(format, provider)+"\t",
            IncreasedFlow.ToString(format, provider)+"\t",
            Depth.ToString(format, provider)+"\t",
            BottomWidth.ToString(format, provider)+"\t"
           
            });
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }


        public override string ToString()
        {
            return ToString(null, null);
        }
    }

    //陡坡参数
    [Serializable]
    public class WaterFallCo:SectionCo
    {

        public double BottomBoardThickness;
        //以下字段无意义

        public WaterFallCo() { }
        //public WaterFallCo(int i, ISheet sheet)
        //{
        //    int y = 74;//一个流量段输入数据在excel表中的行数
        //    Flow = sheet.GetRow((i - 1) * y + 74).GetCell(3).NumericCellValue;
        //    Velocity = sheet.GetRow((i - 1) * y + 74).GetCell(5).NumericCellValue;
        //    Area = sheet.GetRow((i - 1) * y + 74).GetCell(10).NumericCellValue;
        //    Height = sheet.GetRow((i - 1) * y + 74).GetCell(8).NumericCellValue;
        //    Depth = sheet.GetRow((i - 1) * y + 74).GetCell(7).NumericCellValue;
        //    BottomWidth = sheet.GetRow((i - 1) * y + 74).GetCell(0).NumericCellValue;
        //    BottomBoardThickness = sheet.GetRow((i - 1) * y + 74).GetCell(9).NumericCellValue;
        //}
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}", new object[7]
            {
            Flow.ToString(format, provider)+"\t",
            Velocity.ToString(format, provider)+"\t",
            Area.ToString(format, provider)+"\t",
            Height.ToString(format, provider)+"\t",
            Depth.ToString(format, provider)+"\t",
            BottomWidth.ToString(format, provider)+"\t",
            BottomBoardThickness.ToString(format, provider)+"\t"
            });
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }


        public override string ToString()
        {
            return ToString(null, null);
        }

    }
}
