using CanalSystem.Constants;
using Lv.BIM;
using Lv.BIM.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace CanalSystem.BaseClass
{
    [Serializable]
    public class CanalStructure : CanalBase
    {
        public StructureType stuType;
        public string Name;
        public double Trans_section_Slength;   //渐变段长度
        public double Connect_section_Slength;  //连接段长度
        public double Trans_section_Elength;  //渐变段长度
        public double Connect_section_Elength;   //连接段长度
        public string FlowSegmentName;  //流量段
        public double Sloperio => GetSloperio();//坡降//此字段最后需改用下面LongSlope方法获得，下面字段为临时字段
        public List<double> StationList => GetEnterExitStationList();
        //public double LongSlope => GetSloperio();
        //起始点坐标用来识别线路是否更改，（情况1：线路更改，起点坐标没改）（情况2：起点坐标更改）
        //public XYZ StartPoint { get; set; }
        //固定水损
        public double SpecifiedLoss = 0;
        //public List<StationCharacteristics> StationElevationList => GetStationElevationList();
        public string Key { get { if (FlowSegmentName == null) { return Name; } else { return FlowSegmentName + "_" + Name; } } }//用来对list去重
        //②构造函数:
        public CanalStructure() { }
        
        public CanalStructure(string name, double startStation, double length, StructureType type = StructureType.OpenChannel1_1dot5, double StartTranslength = 0, double StartConnectlength = 10, double EndTranslength = 0, double EndConnectlength = 10)
        {
            Generateid();
            //Sloperio = sloperhio;
            Name = name;
            start_station = startStation;
            end_station= startStation+ length;
            _length = length;
            stuType = type;
            Trans_section_Slength = StartTranslength;
            Connect_section_Slength = StartConnectlength;
            Trans_section_Elength = EndTranslength;
            Connect_section_Elength = EndConnectlength;
        }
        //③方法：
        public void UpdateOpenConduit(int type)//更新横断面
        {
            throw new NotImplementedException();
        }
        public List<double> CalculateWaterLevel(Canal canal)
        {
            List<double> result = new List<double>();
            CanalGeometry cg = GetCanalGeometry(canal);
            FlowSegment flo = FlowSegmentInfo.GetFlowSegment(FlowSegmentName);
            SectionCo sc = flo.GetSectionInfoByType(stuType);//横断面信息
            SectionCo preSc = canal.GetPreviousCanalStructureSectionCo(this);//上游断面
            SectionCo nextSc = canal.GetNextCanalStructureSectionCo(this);//下游断面
            switch (stuType) 
            {
                case StructureType.Siphon:
                case StructureType.Sluice:
                case StructureType.WaterFall:
                    result.Add(0.0);
                    result.Add(-SpecifiedLoss);
                    break;
                case StructureType.OpenChannel1_1dot5:
                case StructureType.OpenChannel1_0dot75:
                case StructureType.OpenChannel1_1:
                case StructureType.RecChannel:
                    result = sc.ChannelWaterlevelWithSpareloss(cg);
                    break;
                case StructureType.AqueductRec:
                case StructureType.AqueductU:
                case StructureType.TunnelArchGateCo:
                case StructureType.TunnelCircleCo:
                    List<double> channelwaterlevel = sc.ChannelWaterlevelWithoutSpareloss(cg);
                    double enterLoss = sc.EnterTranSectionLoss(preSc.Velocity);
                    double exitLoss = sc.ExitTranSectionLoss(nextSc.Velocity);
                    int num = channelwaterlevel.Count;
                    //插入进口渐变段处水位，插入出口渐变段处水位
                    
                    channelwaterlevel.ForEach(a => result.Add(a - enterLoss));
                    result.Insert(0,0.0);
                    end_water_level = result.Last()-exitLoss;
                    result.Add(end_water_level);
                    //channelwaterlevel.Insert(1,)
                    break;
                
            }

            return result;
        }
        /// <summary>
        /// 计算结果默认StartWaterLevel为0
        /// </summary>
        /// <param name="canal"></param>
        /// <returns></returns>
        public List<StationCharacteristics> CalculateStationCharacteristics(Canal canal)
        {
            List<StationCharacteristics> result = new List<StationCharacteristics>();
            CanalGeometry cg = GetCanalGeometry(canal);
            List<double> stations = cg.GetCenterLinePointsStationList();
            List<double> waterlevels = CalculateWaterLevel(canal);
          
            FlowSegment flo = FlowSegmentInfo.GetFlowSegment(FlowSegmentName);
            SectionCo sc = flo.GetSectionInfoByType(stuType);//横断面信息
            SectionCo preSc = canal.GetPreviousCanalStructureSectionCo(this);//上游断面
            SectionCo nextSc = canal.GetNextCanalStructureSectionCo(this);//下游断面
            switch (stuType)
            {
                case StructureType.Siphon:
                case StructureType.Sluice:
                case StructureType.WaterFall:
                    StationCharacteristics entertemp= new StationCharacteristics(start_station, 0, -sc.Depth, -sc.Depth + sc.Height, GetNameKeyString()+"进");
                    StationCharacteristics exittemp = new StationCharacteristics(end_station, waterlevels[1], -sc.Depth+waterlevels[1], -sc.Depth + sc.Height +waterlevels[1], GetNameKeyString()+"出");
                    result.Add(entertemp);
                    result.Add(exittemp);
                    break;
                case StructureType.OpenChannel1_1dot5:
                case StructureType.OpenChannel1_0dot75:
                case StructureType.OpenChannel1_1:
                case StructureType.RecChannel:
                    StationCharacteristics entertempstart = new StationCharacteristics(start_station, 0, -sc.Depth, -sc.Depth + sc.Height, GetNameKeyString()+"进");
                    result.Add(entertempstart);
                    for(int i = 1;i<waterlevels.Count-1;i++ )
                    {
                        string wanqianhou = "";
                        if (i % 2 == 0)
                        {
                            wanqianhou = "EC";
                        }
                        else
                        {
                            wanqianhou = "BC";
                        }
                        StationCharacteristics temp0 = new StationCharacteristics(stations[i], waterlevels[i], waterlevels[i] - sc.Depth, waterlevels[i] - sc.Depth + sc.Height, GetNameKeyString()+wanqianhou);
                        result.Add(temp0);
                    }
                    StationCharacteristics tempend= new StationCharacteristics(end_station, waterlevels.Last(), waterlevels.Last() - sc.Depth, waterlevels.Last() - sc.Depth + sc.Height, GetNameKeyString() + "出");
                    result.Add(tempend);
                    break;
                case StructureType.AqueductRec:
                case StructureType.AqueductU:
                case StructureType.TunnelArchGateCo:
                case StructureType.TunnelCircleCo:
                    stations.Insert(1, start_station + Trans_section_Slength + Connect_section_Slength);
                    stations.Insert(stations.Count - 1, end_station - Trans_section_Elength - Connect_section_Elength);
                    StationCharacteristics entertempstart1 = new StationCharacteristics(start_station, 0, -sc.Depth, -sc.Depth + sc.Height, GetNameKeyString() + "渐进");
                    result.Add(entertempstart1);
                    StationCharacteristics temp = new StationCharacteristics(stations[1], waterlevels[1], waterlevels[1] - sc.Depth, waterlevels[1] - sc.Depth + sc.Height, GetNameKeyString() + "进");
                    result.Add(temp);
                    for (int i = 2; i < waterlevels.Count - 2; i++)
                    {
                        string wanqianhou = "";
                        if (i % 2 == 0)
                        {
                            wanqianhou = "BC";
                        }
                        else
                        {
                            wanqianhou = "EC";
                        }
                        temp = new StationCharacteristics(stations[i], waterlevels[i], waterlevels[i] - sc.Depth, waterlevels[i] - sc.Depth + sc.Height,  wanqianhou);
                        result.Add(temp);
                    }
                    temp = new StationCharacteristics(stations[waterlevels.Count - 2], waterlevels[waterlevels.Count - 2], waterlevels[waterlevels.Count - 2] - sc.Depth, waterlevels[waterlevels.Count - 2] - sc.Depth + sc.Height, GetNameKeyString() + "出");
                    result.Add(temp);
                    StationCharacteristics tempend1 = new StationCharacteristics(end_station, waterlevels.Last(), waterlevels.Last() - sc.Depth, waterlevels.Last() - sc.Depth + sc.Height, GetNameKeyString() + "渐出");
                    result.Add(tempend1);
                    //channelwaterlevel.Insert(1,)
                    break;

            }

            return result;

        }

        /// <summary>
        /// 计算结果考虑了StartWaterLevel
        /// </summary>
        /// <returns></returns>
        public List<StationCharacteristics> GetStationElevationList(Canal canal)
        {
            List<StationCharacteristics> calcuwaters = CalculateStationCharacteristics(canal);
            List<StationCharacteristics> result = new List<StationCharacteristics>();
            calcuwaters.ForEach(a => result.Add(a.AddElevation(start_water_level)));
            return result;
            
        }
        public override void SetEndStation(double value)
        {
            _length = value - start_station;
            end_station = value;
        }
        public void ChangeStartStation(double value)
        {
            start_station = value;
            end_station = start_station + _length;
        }

        protected override void Generateid()
        {
            this.id = this.GetType().Name.ToString() + this.GetHashCode().ToString();
        }
        public CanalGeometry GetCanalGeometry(Canal sourceCanal)
        {
            return sourceCanal.GetPartOfCanalGeometry(StartStation,Length);
        }
        public override void SetLength()
        {
            throw new NotImplementedException();
        }
        public double GetSloperio()
        {
            //FlowSegmentInfo.GetDataFromRhinoDoc();
            FlowSegment seg = FlowSegmentInfo.GetFlowSegment(FlowSegmentName);
            return seg.GetSectionInfoByType(stuType).LongSlope;
        }
        public FlowSegment GetFlowSegment()
        {
            return  FlowSegmentInfo.GetFlowSegment(FlowSegmentName);
        }
        public List<double> GetEnterExitStationList()
        {
            List<double> result = new List<double>();
            result.Add(StartStation);
            
            if (Trans_section_Slength != 0)
            {
                result.Add(start_station + Trans_section_Slength);
            }
            if (Connect_section_Slength != 0)
            {
                result.Add(start_station + Trans_section_Slength+Connect_section_Slength);
            }
           
            if (Connect_section_Elength != 0)
            {
                result.Add(end_station - Trans_section_Elength -Connect_section_Elength);

            }
            if (Trans_section_Elength != 0)
            {
                result.Add(end_station - Trans_section_Elength);

            }
            result.Add(end_station);
            return result;

        }
        public string GetNameKeyString()
        {
            string type = stuType.ToDescription();
            string keystring = "";
            string nametoadd = Name;
            if (type.Contains("渡槽"))
            {
                keystring = "渡";
            }
            else if (type.Contains("隧洞"))
            {
                keystring = "隧";
            }
            else if (type.Contains("倒虹"))
            {
                keystring = "倒";
            }
            else if (type.Contains("暗"))
            {
                keystring = "暗";
            }
            else if (type.Contains("闸"))
            {
                keystring = "闸";
            }
            else if (type.Contains("陡坡"))
            {
                keystring = "陡";
            }
            nametoadd = nametoadd.Replace("渡槽", "");
            nametoadd = nametoadd.Replace("隧洞", "");
            nametoadd = nametoadd.Replace("倒虹管", "");
            nametoadd = nametoadd.Replace("倒虹吸", "");
            nametoadd = nametoadd.Replace("暗涵", "");
            nametoadd = nametoadd.Replace("暗渠", "");
            nametoadd = nametoadd.Replace("闸室", "");
            nametoadd = nametoadd.Replace("陡坡", "");
            return nametoadd + keystring;
        }
        public List<string> GetStationLabelAtteachedTextList()
        {
            string type = stuType.ToDescription();
            string keystring = "";
            string nametoadd = Name;
            if (type.Contains("渡槽"))
            {
                keystring = "渡";
            }
            else if (type.Contains("隧洞"))
            {
                keystring = "隧";
            }
            else if (type.Contains("倒虹"))
            {
                keystring = "倒";
            }
            else if (type.Contains("暗"))
            {
                keystring = "暗";
            }
            else if (type.Contains("闸"))
            {
                keystring = "闸";
            }
            else if (type.Contains("陡坡"))
            {
                keystring = "陡";
            }
            nametoadd=nametoadd.Replace("渡槽", "");
            nametoadd = nametoadd.Replace("隧洞", "");
            nametoadd = nametoadd.Replace("倒虹管", "");
            nametoadd = nametoadd.Replace("倒虹吸", "");
            nametoadd = nametoadd.Replace("暗涵", "");
            nametoadd = nametoadd.Replace("暗渠", "");
            nametoadd = nametoadd.Replace("闸室", "");
            nametoadd = nametoadd.Replace("陡坡", "");
            //string attached = Name + keystring;
            List<string> result = new List<string>();
            
            if ((Connect_section_Slength+ Trans_section_Slength )!= 0)
            {
                result.Add(nametoadd + "渐" + "进");
                if (Connect_section_Slength != 0 && Trans_section_Slength != 0)
                {
                    result.Add(nametoadd + "连" + "进");
                }
                result.Add(nametoadd + keystring + "进");
            }
            else
            {
                result.Add(nametoadd + keystring + "进");
            }
           
            if ((Connect_section_Elength +Trans_section_Elength)!= 0)
            {
                result.Add(nametoadd + keystring + "出");
                if (Connect_section_Elength != 0 && Trans_section_Elength != 0)
                {
                    result.Add(nametoadd + "连" + "出");
                }
                result.Add(nametoadd + "渐" + "出");
            }
            else
            {
                result.Add(nametoadd + keystring + "出");
            }
            return result;
        }
    //    public void AddToDocument(Lv.BIMDoc doc, Canal sourceCanal)
    //    {
    //        CanalGeometry csge = GetCanalGeometry(sourceCanal);
    //        PolyCurve csgecenterline = csge.CenterLine;
    //        ObjectAttributes oa = new ObjectAttributes();
    //        oa.ColorSource = ObjectColorSource.ColorFromObject;
    //        StructureType st = stuType;
    //        string stname = st.ToDescription();
    //        if (stname.Contains("渡槽"))
    //        {
    //            oa.ObjectColor = Color.Red;
    //            oa.PlotColor = Color.Red;
    //        }
    //        else if (stname.Contains("隧洞"))
    //        {
    //            oa.ObjectColor = Color.Yellow;
    //            oa.PlotColor = Color.Yellow;
    //        }
    //        else if (stname.Contains("矩形明渠"))
    //        {
    //            oa.ObjectColor = Color.Green;
    //            oa.PlotColor = Color.Green;
    //        }

    //        else if (stname.Contains("倒虹管"))
    //        {
    //            oa.ObjectColor = Color.Blue;
    //            oa.PlotColor = Color.Blue;
    //        }
    //        else if (stname.Contains("梯形明渠"))
    //        {
    //            oa.ObjectColor = Color.DarkGray;
    //            oa.PlotColor = Color.DarkGray;
    //        }
    //        else
    //        {
    //            oa.ObjectColor = Color.Gray;
    //            oa.PlotColor = Color.Gray;
    //        }

    //        doc.Objects.AddCurve(csgecenterline, oa);
    //}
        public DataTable ToDataTable(string format, IFormatProvider provider)
        {
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();
            dt.Columns.Add("流量段");            
            dt.Columns.Add("建筑物");
            dt.Columns.Add("建筑物类型");
            dt.Columns.Add("长度");
            dt.Columns.Add("起点桩号");
            dt.Columns.Add("终点桩号");
            dt.Columns.Add("起点水位");
            dt.Columns.Add("进口渐变段长");
            dt.Columns.Add("进口连接段长");
            dt.Columns.Add("出口连接段长");
            dt.Columns.Add("出口渐变段长");
            dr["流量段"]= FlowSegmentName;
            dr["建筑物"]= Name;
            dr["建筑物类型"]= stuType.ToDescription();
            dr["长度"]= Length.ToString(format, provider);
            dr["起点桩号"]= StartStation.ToString(format, provider);
            dr["终点桩号"] = EndStation.ToString(format, provider);
            dr["起点水位"]= StartWaterLevel.ToString(format, provider);
            dr["进口渐变段长"]= Trans_section_Slength.ToString(format, provider);
            dr["进口连接段长"]= Connect_section_Slength.ToString(format, provider);
            dr["出口连接段长"]= Connect_section_Elength.ToString();
            dr["出口渐变段长"]= Trans_section_Elength.ToString(format, provider);
            dt.Rows.Add(dr);
            return dt;
        }
        public DataTable ToDataTable(string format)
        {
            
            return ToDataTable(format,null);
        }
        public DataTable ToDataTable()
        {

            return ToDataTable(null, null);
        }
        public  string ToString(string format, IFormatProvider provider)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", new object[10]
            {
            "            流量段:"+"\t"+FlowSegmentName+"\n",
            "            建筑物:"+"\t"+Name+"\n",
            "    建筑物类型:"+"\t"+stuType+"\n",
            "                长度:"+"\t"+Length.ToString(format, provider)+"\n",
            "        起点桩号:"+"\t"+StartStation.ToString(format, provider)+"\n",
            "        起点水位:"+"\t"+StartWaterLevel.ToString(format, provider)+"\n",
            "进口渐变段长:"+"\t"+Trans_section_Slength.ToString(format, provider)+"\n",
            "进口连接段长:"+"\t"+Connect_section_Slength.ToString(format, provider)+"\n",
            "出口连接段长:"+"\t"+Connect_section_Elength.ToString()+"\n",
            "出口渐变段长:"+"\t"+Trans_section_Elength.ToString(format,provider)+"\n"
            });
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }


        public sealed override string ToString()
        {
            return ToString(null, null);
        }
    }
}
