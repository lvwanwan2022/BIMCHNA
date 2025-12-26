using CanalSystem.BaseTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace CanalSystem.BaseClass
{
  
    //流量段类，包含名、流量、桩号、ID、水位、默认横断面类型、横断面
    [Serializable]
    public class FlowSegment : CanalBase
    {
        private string name;
        private double flow;
        public string Name { get { return name; } set { name = value; } }
        public string Key => Name;//用来对list去重
        public double Flow { get { return flow; } set { flow = value; } }//流量
        public StructureType defaultstrcuturetype { get; set; }
        //以下定义为涉及的渠道横断面定义为结构体
        public OpenChannelCo? tixing1_1dot5 { get; set; }//梯形明渠,
        public OpenChannelCo? tixing1_1 { get; set; }
        public OpenChannelCo? tixing1_0dot75 { get; set; }
        public RecChannelCo? juxing { get; set; }
        public TunnelCo? suidongyuanxing { get; set; }//隧洞
        public TunnelCo? suidongarchgatexing { get; set; }//城门洞型
        public AqueductCo? ducaouxing { get; set; }//U型渡槽
        public AqueductCo? ducaojuxing { get; set; }//矩形渡槽
        public BuriedConduitCo? anhan { get; set; }//暗涵
        public SluiceCo? zhashi { get; set; }//闸室
        public SiphonCo? daohongguan { get; set; }//倒虹管
        public WaterFallCo? doupo { get; set; }//陡坡
        //2.构造方法
        public FlowSegment() { }
        public FlowSegment(string FlowSegmentname) {
            name = FlowSegmentname;
            start_station = 0;
            end_station = 0;
            Flow = 0;
        }
        public FlowSegment(string nameA, double startStationA, double endStationA, double FlowA) 
        {
            name = nameA;
            start_station = startStationA;
            end_station = endStationA;
            flow = FlowA;
        }
        //为了不与以上构造函数参数相同，特将以下函数定义为static
        public static FlowSegment CreateByStartStationAndLength(string name,double startStation, double length, double Flow) 
        {
            return new FlowSegment(name, startStation, startStation + length, Flow);
        }
        //3.方法
        //获取默认横断面类型
        public void LoadSectionParameters(StructureType type) { }
        //加载所有横断面参数
        public void LoadAllSectionParameters() { }
        //设置末桩号
        public override void SetEndStation(double value)
        {
            _length = value - start_station;
            end_station = value;
        }
        public void ChangeStartStation(double startStationValue)
        {
            start_station = startStationValue;
            end_station = start_station + _length;
        }
        protected override void Generateid()
        {
            this.id = this.GetType().Name.ToString() + this.GetHashCode().ToString();
        }
        //"1;1.5梯形明渠":

        // "1;1梯形明渠":

        // "1;0.75梯形明渠":

        // "矩形明渠":

        // "圆形隧洞":

        // "城门洞型隧洞":

        // "矩形渡槽":

        // "U形渡槽":

        // "暗涵":

        // "闸":

        // "倒虹管":

        // "陡坡":
        public List<string> GetCanalStructureTypeList()
        {
            List<string> dt = new List<string>();
            
            GeneralSectionCo temp = new GeneralSectionCo();
            //DataRow dr = dt.NewRow();
            List<string> qudaoleixings = StructureTypeExtension.GetStructureTypeAllDescriptions();
            foreach (string qudao in qudaoleixings)
            {
                StructureType qudaotype = qudao.ToStructureType();
                switch (qudao)
                {
                    case "1:1.5梯形明渠":
                        if (tixing1_1dot5 != null)
                        {
                            dt.Add(qudao);
                        }
                        break;
                    case "1:1梯形明渠":
                        if (tixing1_1 != null)
                        {
                            dt.Add(qudao);
                        }
                        break;
                    case "1:0.75梯形明渠":
                        if (tixing1_0dot75 != null)
                        {
                            dt.Add(qudao);
                        }
                        break;
                    case "矩形明渠":
                        if (juxing != null)
                        {
                            dt.Add(qudao);
                        }
                        break;
                    case "圆形隧洞":
                        if (suidongyuanxing != null)
                        {
                            dt.Add(qudao);
                        }
                        break;
                    case "城门洞型隧洞":
                        if (suidongarchgatexing != null)
                        {
                            dt.Add(qudao);
                        }
                        break;
                    case "矩形渡槽":
                        if (ducaojuxing != null)
                        {
                            dt.Add(qudao);
                        }
                        break;
                    case "U形渡槽":
                        if (ducaouxing != null)
                        {
                            dt.Add(qudao);
                        }
                        break;
                    case "暗涵":
                        if (anhan != null)
                        {
                            dt.Add(qudao);
                        }
                        break;
                    case "闸":
                        if (zhashi != null)
                        {
                            dt.Add(qudao);
                        }
                        break;
                    case "倒虹管":
                        if (daohongguan != null)
                        {
                            dt.Add(qudao);
                        }
                        break;
                    case "陡坡":
                        if (doupo != null)
                        {
                            dt.Add(qudao);
                        }
                        break;
                    default:
                        if (tixing1_1dot5 != null)
                        {
                            dt.Add(qudao);
                        }
                        break;
                }



            }

            return dt;
        }
        public  DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("流量段");
            dt.Columns.Add("渠道类型");
            dt.Columns.Add("流量");
            dt.Columns.Add("流速");
            dt.Columns.Add("面积");
            dt.Columns.Add("糙率");
            dt.Columns.Add("湿周");
            dt.Columns.Add("水力半径");
            dt.Columns.Add("底坡");
            dt.Columns.Add("渠道高度");
            dt.Columns.Add("水深");
            dt.Columns.Add("底宽");
            dt.Columns.Add("边坡");
            dt.Columns.Add("衬砌厚度");
            dt.Columns.Add("边墙高度");
            dt.Columns.Add("底板厚度");
            dt.Columns.Add("半径");
            dt.Columns.Add("加大流量");
                GeneralSectionCo temp = new GeneralSectionCo();
                //DataRow dr = dt.NewRow();
                List<string> qudaoleixings = StructureTypeExtension.GetStructureTypeAllDescriptions();
                foreach (string qudao in qudaoleixings)
                {
                    StructureType qudaotype = qudao.ToStructureType();
                    switch (qudao)
                    {
                        case "1:1.5梯形明渠":
                            if (tixing1_1dot5 != null)
                            {

                            BaseTools.GeneralTools.Mapper<OpenChannelCo, GeneralSectionCo>(tixing1_1dot5, ref temp);
                                 temp.flowsegname = name;temp.leixing = qudao;
                                DataRow dr1 = dt.NewRow();
                                dr1.ItemArray = temp.ToArray();
                                dt.Rows.Add(dr1);
                                temp.Clear();
                            }
                            break;
                        case "1:1梯形明渠":
                            if (tixing1_1 != null)
                            {
                                BaseTools.GeneralTools.Mapper<OpenChannelCo, GeneralSectionCo>(tixing1_1, ref temp);
                                 temp.flowsegname = name;temp.leixing = qudao;
                                DataRow dr1 = dt.NewRow();
                                dr1.ItemArray = temp.ToArray();
                                dt.Rows.Add(dr1);
                                temp.Clear();
                            }
                            break;
                        case "1:0.75梯形明渠":
                            if (tixing1_0dot75 != null)
                            {
                                BaseTools.GeneralTools.Mapper<OpenChannelCo, GeneralSectionCo>(tixing1_0dot75, ref temp);
                                 temp.flowsegname = name;temp.leixing = qudao;
                                DataRow dr3 = dt.NewRow();
                                dr3.ItemArray = temp.ToArray();
                                dt.Rows.Add(dr3);
                                temp.Clear();
                            }
                            break;
                        case "矩形明渠":
                            if (juxing != null)
                            {
                                BaseTools.GeneralTools.Mapper<RecChannelCo, GeneralSectionCo>(juxing, ref temp);
                                 temp.flowsegname = name;temp.leixing = qudao;
                                DataRow dr4 = dt.NewRow();
                                dr4.ItemArray = temp.ToArray();
                                dt.Rows.Add(dr4);
                                temp.Clear();
                            }
                            break;
                        case "圆形隧洞":
                            if (suidongyuanxing != null)
                            {
                                BaseTools.GeneralTools.Mapper<TunnelCo, GeneralSectionCo>(suidongyuanxing, ref temp);
                                 temp.flowsegname = name;temp.leixing = qudao;
                                DataRow dr5 = dt.NewRow();
                                dr5.ItemArray = temp.ToArray();
                                dt.Rows.Add(dr5);
                                temp.Clear();
                            }
                            break;
                        case "城门洞型隧洞":
                            if (suidongarchgatexing != null)
                            {
                                BaseTools.GeneralTools.Mapper<TunnelCo, GeneralSectionCo>(suidongarchgatexing, ref temp);
                                 temp.flowsegname = name;temp.leixing = qudao;
                                DataRow dr6 = dt.NewRow();
                                dr6.ItemArray = temp.ToArray();
                                dt.Rows.Add(dr6);
                                temp.Clear();
                            }
                            break;
                        case "矩形渡槽":
                            if (ducaojuxing != null)
                            {
                                BaseTools.GeneralTools.Mapper<AqueductCo, GeneralSectionCo>(ducaojuxing, ref temp);
                                 temp.flowsegname = name;temp.leixing = qudao;
                                DataRow dr7 = dt.NewRow();
                                dr7.ItemArray = temp.ToArray();
                                dt.Rows.Add(dr7);
                                temp.Clear();
                            }
                            break;
                        case "U形渡槽":
                            if (ducaouxing != null)
                            {
                                BaseTools.GeneralTools.Mapper<AqueductCo, GeneralSectionCo>(ducaouxing, ref temp);
                                 temp.flowsegname = name;temp.leixing = qudao;
                                DataRow dr8 = dt.NewRow();
                                dr8.ItemArray = temp.ToArray();
                                dt.Rows.Add(dr8);
                                temp.Clear();
                            }
                            break;
                        case "暗涵":
                            if (anhan != null)
                            {
                                BaseTools.GeneralTools.Mapper<BuriedConduitCo, GeneralSectionCo>(anhan, ref temp);
                                 temp.flowsegname = name;temp.leixing = qudao;
                                DataRow dr9 = dt.NewRow();
                                dr9.ItemArray = temp.ToArray();
                                dt.Rows.Add(dr9);
                                temp.Clear();
                            }
                            break;
                        case "闸":
                            if (zhashi != null)
                            {
                                BaseTools.GeneralTools.Mapper<SluiceCo, GeneralSectionCo>(zhashi, ref temp);
                                 temp.flowsegname = name;temp.leixing = qudao;
                                DataRow dr10 = dt.NewRow();
                                dr10.ItemArray = temp.ToArray();
                                dt.Rows.Add(dr10);
                                temp.Clear();
                            }
                            break;
                        case "倒虹管":
                            if (daohongguan != null)
                            {
                                BaseTools.GeneralTools.Mapper<SiphonCo, GeneralSectionCo>(daohongguan, ref temp);
                                 temp.flowsegname = name;temp.leixing = qudao;
                                DataRow dr11 = dt.NewRow();
                                dr11.ItemArray = temp.ToArray();
                                dt.Rows.Add(dr11);
                                temp.Clear();
                            }
                            break;
                        case "陡坡":
                            if (doupo != null)
                            {
                                BaseTools.GeneralTools.Mapper<WaterFallCo, GeneralSectionCo>(doupo, ref temp);
                                 temp.flowsegname = name;temp.leixing = qudao;
                                DataRow dr12 = dt.NewRow();
                                dr12.ItemArray = temp.ToArray();
                                dt.Rows.Add(dr12);
                                temp.Clear();
                            }
                            break;
                        default:
                            if (tixing1_1dot5 != null)
                            {
                                BaseTools.GeneralTools.Mapper<OpenChannelCo, GeneralSectionCo>(tixing1_1dot5, ref temp);
                                 temp.flowsegname = name;temp.leixing = qudao;
                                DataRow dr13 = dt.NewRow();
                                dr13.ItemArray = temp.ToArray();
                                dt.Rows.Add(dr13);
                                temp.Clear();
                            }
                            break;
                    }



                }

            return dt;
        }
        public void UpdateFromDataTable(DataTable source) 
        {
            //source结构如下
            //DataTable dt = new DataTable();
            //dt.Columns.Add("流量段");
            //dt.Columns.Add("渠道类型");
            //dt.Columns.Add("流量");
            //dt.Columns.Add("流速");
            //dt.Columns.Add("面积");
            //dt.Columns.Add("糙率");
            //dt.Columns.Add("湿周");
            //dt.Columns.Add("水力半径");
            //dt.Columns.Add("底坡");
            //dt.Columns.Add("渠道高度");
            //dt.Columns.Add("水深");
            //dt.Columns.Add("底宽");
            //dt.Columns.Add("边坡");
            //dt.Columns.Add("衬砌厚度");
            //dt.Columns.Add("边墙高度");
            //dt.Columns.Add("底板厚度");
            //dt.Columns.Add("半径");
            //dt.Columns.Add("加大流量");           
            int num = source.Rows.Count;
            for (int i = 0; i < num - 1; i++)
            {
                DataRow dr = source.Rows[i];
                string dtsegname= dr["流量段"].ToString();  //流量段
                string qudaotype = dr["渠道类型"].ToString();
                GeneralSectionCo secCo = new GeneralSectionCo();//标准断面用于存储建筑物断面参数信息
                secCo.Flow= double.Parse(dr["流量"].ToString());
                secCo.Velocity = double.Parse(dr["流速"].ToString());
                secCo.Area = double.Parse(dr["面积"].ToString());
                secCo.Roughness = double.Parse(dr["糙率"].ToString());
                secCo.WetPerimeter = double.Parse(dr["湿周"].ToString());
                secCo.HydraulicRadius = double.Parse(dr["水力半径"].ToString());
                secCo.LongSlope = double.Parse(dr["底坡"].ToString());
                secCo.Height = double.Parse(dr["渠道高度"].ToString());
                secCo.Depth = double.Parse(dr["水深"].ToString());
                secCo.BottomWidth = double.Parse(dr["底宽"].ToString());
                secCo.SlopeRatio = double.Parse(dr["边坡"].ToString());
                secCo.LiningThickness = double.Parse(dr["衬砌厚度"].ToString());
                secCo.SideWallHeight = double.Parse(dr["边墙高度"].ToString());
                secCo.BottomBoardThickness = double.Parse(dr["底板厚度"].ToString());
                secCo.Radius = double.Parse(dr["半径"].ToString());
                secCo.IncreasedFlow = double.Parse(dr["加大流量"].ToString());
                if (name == dtsegname)
                {
                    switch (qudaotype)
                    {
                        case "1:1.5梯形明渠":
                            OpenChannelCo temp = new OpenChannelCo();
                            GeneralTools.Mapper<GeneralSectionCo, OpenChannelCo>(secCo, ref temp);
                            tixing1_1dot5 = temp;
                            break;
                        case "1:1梯形明渠":
                            OpenChannelCo temp1 = new OpenChannelCo();
                            GeneralTools.Mapper<GeneralSectionCo, OpenChannelCo>(secCo, ref temp1);
                            tixing1_1 = temp1;
                            break;
                        case "1:0.75梯形明渠":
                            OpenChannelCo temp2 = new OpenChannelCo();
                            GeneralTools.Mapper<GeneralSectionCo, OpenChannelCo>(secCo, ref temp2);
                            tixing1_0dot75 = temp2;
                            break;
                        case "矩形明渠":
                            RecChannelCo rectemp = new RecChannelCo();
                            GeneralTools.Mapper<GeneralSectionCo, RecChannelCo>(secCo, ref rectemp);
                            juxing = rectemp;
                            break;
                        case "圆形隧洞":
                            TunnelCo tutemp = new TunnelCo();
                            GeneralTools.Mapper<GeneralSectionCo, TunnelCo>(secCo, ref tutemp);
                            suidongyuanxing = tutemp;
                            break;
                        case "城门洞型隧洞":
                            TunnelCo tutemp1 = new TunnelCo();
                            GeneralTools.Mapper<GeneralSectionCo, TunnelCo>(secCo, ref tutemp1);
                            suidongarchgatexing = tutemp1;
                            break;
                        case "矩形渡槽":
                            AqueductCo aqtemp = new AqueductCo();
                            GeneralTools.Mapper<GeneralSectionCo, AqueductCo>(secCo, ref aqtemp);
                            ducaojuxing = aqtemp;
                            break;
                        case "U形渡槽":
                            AqueductCo aqtemp1 = new AqueductCo();
                            GeneralTools.Mapper<GeneralSectionCo, AqueductCo>(secCo, ref aqtemp1);
                            ducaouxing = aqtemp1;
                            break;
                        case "暗涵":
                            BuriedConduitCo butemp = new BuriedConduitCo();
                            GeneralTools.Mapper<GeneralSectionCo, BuriedConduitCo>(secCo, ref butemp);
                            anhan = butemp;
                            break;
                        case "闸":
                            SluiceCo sltemp = new SluiceCo();
                            GeneralTools.Mapper<GeneralSectionCo, SluiceCo>(secCo, ref sltemp);
                            zhashi = sltemp;
                            break;
                        case "倒虹管":
                            SiphonCo sitemp = new SiphonCo();
                            GeneralTools.Mapper<GeneralSectionCo, SiphonCo>(secCo, ref sitemp);
                            daohongguan = sitemp;
                            break;
                        case "陡坡":
                            WaterFallCo watemp = new WaterFallCo();
                            GeneralTools.Mapper<GeneralSectionCo, WaterFallCo>(secCo, ref watemp);
                            doupo = watemp;
                            break;
                        default:
                            OpenChannelCo tempd = new OpenChannelCo();
                            GeneralTools.Mapper<GeneralSectionCo, OpenChannelCo>(secCo, ref tempd);
                            tixing1_1dot5 = tempd;
                            break;
                    }
                }
               
            }

        }
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", new object[13]
            {
            "                    Name:"+"\t"+Name+"\r\n",
            "     1;1.5梯形明渠:"+"\t"+tixing1_1dot5.ToString(format, provider)+"\r\n",
            "        1;1梯形明渠:"+"\t"+tixing1_1.ToString(format, provider)+"\r\n",
            "   1;0.75梯形明渠:"+"\t"+tixing1_0dot75.ToString(format, provider)+"\r\n",
            "             矩形明渠:"+"\t"+juxing.ToString()+"\r\n",
            "             圆形隧洞:"+"\t"+suidongyuanxing.ToString(format,provider)+"\r\n",
            "     城门洞型隧洞:"+"\t"+suidongarchgatexing.ToString(format,provider)+"\r\n"," " +
            "             矩形渡槽:"+"\t"+ducaojuxing.ToString(format, provider)+"\r\n",
            "              U形渡槽:"+"\t"+ducaouxing.ToString(format, provider)+"\r\n",
            "                     暗涵:"+"\t"+anhan.ToString(format, provider)+"\r\n",
            "                         闸:"+"\t"+zhashi.ToString()+"\r\n",
            "                 倒虹管:"+"\t"+daohongguan.ToString(format,provider)+"\r\n",
            "                     陡坡:"+"\t"+doupo.ToString(format,provider)+"\r\n"
            });
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }
        public SectionCo GetSectionInfoByType(StructureType type)
        {
            string qudao = type.ToDescription();
            switch (qudao)
            {
                case "1:1.5梯形明渠":
                    if (tixing1_1dot5 != null)
                    {
                        return tixing1_1dot5 as SectionCo;
                    }
                    else { return null;} 
                    
                case "1:1梯形明渠":
                    if (tixing1_1 != null)
                    {
                        return tixing1_1 as SectionCo;
                    }
                    else { return null;} 
                    
                case "1:0.75梯形明渠":
                    if (tixing1_0dot75 != null)
                    {
                        return tixing1_1dot5 as SectionCo;
                    }
                    else { return null;} 
                    
                case "矩形明渠":
                    if (juxing != null)
                    {
                        return juxing as SectionCo;
                    }
                    else { return null;} 
                    
                case "圆形隧洞":
                    if (suidongyuanxing != null)
                    {
                        return suidongyuanxing as SectionCo;
                    }
                    else { return null;} 
                    
                case "城门洞型隧洞":
                    if (suidongarchgatexing != null)
                    {
                        return suidongarchgatexing as SectionCo;
                    }
                    else { return null;} 
                    
                case "矩形渡槽":
                    if (ducaojuxing != null)
                    {
                        return ducaojuxing as SectionCo;
                    }
                    else { return null;} 
                    
                case "U形渡槽":
                    if (ducaouxing != null)
                    {
                        return ducaouxing as SectionCo;
                    }
                    else { return null;} 
                    
                case "暗涵":
                    if (anhan != null)
                    {
                        return anhan as SectionCo;
                    }
                    else { return null;} 
                    
                case "闸":
                    if (zhashi != null)
                    {
                        return zhashi as SectionCo;
                    }
                    else { return null;} 
                    
                case "倒虹管":
                    if (daohongguan != null)
                    {
                        return daohongguan as SectionCo;
                    }
                    else { return null;} 
                    
                case "陡坡":
                    if (doupo != null)
                    {
                        return doupo as SectionCo;
                    }
                    else { return null;} 
                    
                default:
                    if (tixing1_1dot5 != null)
                    {
                        return tixing1_1dot5 as SectionCo;
                    }
                    else { return null;} 
            }

        }

        public override string ToString()
        {
            return ToString(null, null);
        }
    }

    /// <summary>
    /// 用于保存所有流量段信息
    /// </summary>
    [Serializable]
    public static class FlowSegmentInfo
    {
        public static Dictionary<string, FlowSegment> InfoDictionary = new Dictionary<string, FlowSegment>();
        public static FlowSegment GetFlowSegment(string key)
        {
            if (InfoDictionary.ContainsKey(key))
            {
                FlowSegment result = InfoDictionary[key];
                return result;
            }
            else
            {
                return null;
            }
        }
        public static void SetFlowSegment(FlowSegment flowsegment)
        {
            string key = flowsegment.Name;
            if (InfoDictionary.ContainsKey(key))
            {
                InfoDictionary[key] = flowsegment;
            }
            else
            {
                InfoDictionary.Add(key, flowsegment);
            }

        }
        public static void RemoveFlowSegment(FlowSegment flowsegment)
        {
            string key = flowsegment.Name;
            //InfoDictionary[key] = flowsegment;
            if (InfoDictionary.ContainsKey(key))
            {
                InfoDictionary.Remove(key);
            }
        }
        public static void RemoveFlowSegmentByname(string name)
        {
            string key = name;
            //InfoDictionary[key] = flowsegment;
            if (InfoDictionary.ContainsKey(key))
            {
                InfoDictionary.Remove(key);
            }
        }
        public static List<string> GetAllSegmentNames()
        {
            int num = InfoDictionary.Count;
            List<string> result = new List<string>();
            foreach (var item in InfoDictionary)
            {
                result.Add(item.Key);
            }
            return result;
        }
        public static DataTable ToDataTable()
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("流量段");
            dt.Columns.Add("渠道类型");
            dt.Columns.Add("流量");
            dt.Columns.Add("流速");
            dt.Columns.Add("面积");
            dt.Columns.Add("糙率");
            dt.Columns.Add("湿周");
            dt.Columns.Add("水力半径");
            dt.Columns.Add("底坡");
            dt.Columns.Add("渠道高度");
            dt.Columns.Add("水深");
            dt.Columns.Add("底宽");
            dt.Columns.Add("边坡");
            dt.Columns.Add("衬砌厚度");
            dt.Columns.Add("边墙高度");
            dt.Columns.Add("底板厚度");
            dt.Columns.Add("半径");
            dt.Columns.Add("加大流量");
            foreach (var item in InfoDictionary)
            {
                string name = item.Key;
                FlowSegment flo = item.Value;
                DataTable flodt = flo.ToDataTable();
                foreach (DataRow row in flodt.Rows)
                {
                    dt.ImportRow(row);
                }
            }

            return dt;
        }
        public static void UpdateFromDataTable(DataTable source)
        {
            Dictionary<string, FlowSegment> InfoDictionarytemp = new Dictionary<string, FlowSegment>();
            foreach (var item in InfoDictionary)
            {
                string name = item.Key;
                FlowSegment flo = item.Value;
                FlowSegment flotemp = flo;
                flotemp.UpdateFromDataTable(source);
                InfoDictionarytemp.Add(name, flotemp);
            }
            FlowSegmentInfo.InfoDictionary = InfoDictionarytemp;
            }
        //public static void GetDataFromRhinoDoc()
        //{
        //    object data = CanalSystemPlugIn.Instance.GetCanalData("FlowSegmentInfoDictionaryBIM");
        //    if (data != null)
        //    {
        //        InfoDictionary = data as Dictionary<string, FlowSegment>;
        //    }
        //}
    }
}
