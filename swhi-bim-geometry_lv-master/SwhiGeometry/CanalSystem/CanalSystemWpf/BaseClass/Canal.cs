using CanalSystem.BaseTools;
using CanalSystem.Constants;
using Lv.BIM.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
//渠系类
namespace CanalSystem.BaseClass
{

    [Serializable]
    public class Canal : CanalGeometry
    {

        //以下信息为可空公有字段信息，除通过单个字段的get、set方法获取修改外，可通过AddAboutInfos()和SetAboutInfos()同一添加和设置
        public List<FlowSegment> FlowSegmentList; //定义流量段信息
        public List<double> StationList;//需要标注的桩号列表
        public List<CanalStructure> CanalStructureList;//建筑物列表 
        public List<LittleStructure> LittleStructureList;//小件列表 
        public List<LittleStructure> AllLittleStructureList => GetAllLittleStructureList();//小件排序

        //以下信息通过水面线计算得到
        //几何特征点+建筑物进出口+渐变、连接段进出口
        //以下建筑物列表包含所有默认明渠段
        public List<CanalStructure> AllCanalStructureList => GetAllCanalStructureList();//建筑物列表 
                                                                                        //以下信息通过水面线计算得到
                                                                                        //几何特征点+建筑物进出口+渐变、连接段进出口
                                                                                        // public List<StationCharacteristics> stationElevations;//测试完成使用以下字段
        public List<StationCharacteristics> stationElevations => WaterLineCalculate();

        //2.构造函数
        //私有变量构造函数
     
        public Canal(List<XYZ> constructLinePoints, List<double> centerLineArcRadius, double startStation = 0)
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
            //
            FlowSegmentList = new List<FlowSegment>();
            StationList = new List<double>();
            CanalStructureList = new List<CanalStructure>();
            LittleStructureList = new List<LittleStructure>();
            //stationElevations=new List<StationCharacteristics>();
        }
        public Canal(PolylineXYZ constructline, List<double> radius, double startStation = 0)
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

            FlowSegmentList = new List<FlowSegment>();
            StationList = new List<double>();
            CanalStructureList = new List<CanalStructure>();
            LittleStructureList = new List<LittleStructure>();
            //stationElevations = new List<StationCharacteristics>();

        }//直线段+半径
        /// <summary>
        /// //用带圆弧的多段线构造Canal
        /// </summary>
        /// <param name="centerline"></param>
        public Canal(PolycurveXYZ centerline, double startStation = 0)
        {
            Generateid();
            //int numOfseg = centerline.SegmentCount;
            //List<XYZ> constructpoints_temp=new List<XYZ>();
            // List<double> arcRadius_temp=new List<double>();
            constructLine_points = BaseTools.GeneralTools.GetConstructPolylinePoints(centerline);
            centerLine_arcRadius = BaseTools.GeneralTools.GetCenterLineArcRadius(centerline);
            start_station = startStation;
            SetLength();
            FlowSegmentList = new List<FlowSegment>();
            StationList = new List<double>();
            CanalStructureList = new List<CanalStructure>();
            LittleStructureList = new List<LittleStructure>();
            //stationElevations = new List<StationCharacteristics>();
        }

        public Canal(PolylineXYZ constructline, double radius, double startStation = 0)
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
            FlowSegmentList = new List<FlowSegment>();
            StationList = new List<double>();
            CanalStructureList = new List<CanalStructure>();
            LittleStructureList = new List<LittleStructure>();
            //stationElevations = new List<StationCharacteristics>();

        }

        public Canal()
        {
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


        //获取构造点桩号


        public CanalGeometry GetCanalGeometry()
        {
            CanalGeometry result = new CanalGeometry();
            result = this as CanalGeometry;
            return result;
            //return new CanalGeometry(constructLine_points, centerLine_arcRadius, StartStation);
        }


        //以下方法与CanalStuctureList有关，未来可能将此部分拆分出来
        public void SortCanalStructureList()
        {
            if(CanalStructureList!=null  && CanalStructureList.Count >= 2)
            { CanalStructureList.Sort((a, b) => { return a.StartStation > b.StartStation ? 1 : -1; }); }
        }
        public void SortLittleStructureList()
        {
            if(LittleStructureList.Count>=2&& LittleStructureList!=null)
            { LittleStructureList.Sort((a, b) => { return a.Station > b.Station ? 1 : -1; }); }
        }
        public void SortAllCanalStructureList()
        {
            if (AllCanalStructureList.Count >= 2) {  AllCanalStructureList.Sort((a, b) => { return a.StartStation > b.StartStation ? 1 : -1; }); }
        }
        public void SortAllLittleStructureList()
        {
            if (AllLittleStructureList.Count >= 2) { AllLittleStructureList.Sort((a, b) => { return a.Station > b.Station ? 1 : -1; }); }
        }
        public SectionCo GetPreviousCanalStructureSectionCo(CanalStructure canalstructure)
        {
            SortAllCanalStructureList();
            if (canalstructure.StartStation == start_station)
            {
                return null;
            }
            else if (canalstructure.Key == AllCanalStructureList.First().Key)
            {
                FlowSegment flo = FlowSegmentInfo.GetFlowSegment(canalstructure.FlowSegmentName);
                SectionCo sc = flo.GetSectionInfoByType(flo.defaultstrcuturetype);
                return sc;
            }
            else
            {
                int indexOfCanalstructure = AllCanalStructureList.FindIndex(a => a.Key == canalstructure.Key);
                if (indexOfCanalstructure != -1)
                {
                    CanalStructure cs = AllCanalStructureList[indexOfCanalstructure - 1];
                    FlowSegment flo = FlowSegmentInfo.GetFlowSegment(cs.FlowSegmentName);
                    return flo.GetSectionInfoByType(cs.stuType);
                }
                else
                {
                    throw new Exception("canal中没有该建筑物");
                }
            }
        }
        public SectionCo GetNextCanalStructureSectionCo(CanalStructure canalstructure)
        {
            SortAllCanalStructureList();
            if (canalstructure.EndStation == end_station)
            {
                return null;
            }
            else if (canalstructure.Key == AllCanalStructureList.Last().Key)
            {
                FlowSegment flo = FlowSegmentInfo.GetFlowSegment(canalstructure.FlowSegmentName);
                SectionCo sc = flo.GetSectionInfoByType(flo.defaultstrcuturetype);
                return sc;
            }
            else
            {
                int indexOfCanalstructure = AllCanalStructureList.FindIndex(a => a.Key == canalstructure.Key);
                if (indexOfCanalstructure != -1)
                {
                    CanalStructure cs = AllCanalStructureList[indexOfCanalstructure + 1];
                    FlowSegment flo = FlowSegmentInfo.GetFlowSegment(cs.FlowSegmentName);
                    return flo.GetSectionInfoByType(cs.stuType);
                }
                else
                {
                    throw new Exception("canal中没有该建筑物");
                }
            }
        }

        public void AddAboutInfos(List<double> stationListA = null, List<FlowSegment> flowSegmentListA = null, List<CanalStructure> CanalStructureListA = null,List<LittleStructure> LittleStructureListA=null)
        {
            //StationList初始化，即添加始末桩号
            if (StationList == null)
            {
                StationList = new List<double>();
            }
            if (FlowSegmentList == null)
            {
                FlowSegmentList = new List<FlowSegment>();
            }
            if (CanalStructureList == null)
            {
                CanalStructureList = new List<CanalStructure>();
            }
            if (LittleStructureList == null)
            {
                LittleStructureList = new List<LittleStructure>();
            }


            //添加桩号
            if (stationListA != null && stationListA.Count != 0)
            {
                StationList.AddRange(stationListA);
                //桩号去重
                StationList.Distinct();
            }
            //添加流量段
            if (flowSegmentListA != null && flowSegmentListA.Count != 0)
            {
                FlowSegmentList.AddRange(flowSegmentListA);
                //去除重复元素
                FlowSegmentList.GroupBy(x => x.Key).Select(y => y.First());
            }
            //添加建筑物
            if (CanalStructureListA != null && CanalStructureListA.Count != 0)
            {
                CanalStructureList.AddRange(CanalStructureListA);
                //去除重复元素
                CanalStructureList.GroupBy(x => x.Key).Select(y => y.First());
            }
            //添加小型建筑物
            if (LittleStructureListA != null && LittleStructureListA.Count != 0)
            {
                LittleStructureList.AddRange(LittleStructureListA);
                //去除重复元素
                LittleStructureList.GroupBy(x => x.Key).Select(y => y.First());
            }


        }
        /// <summary>
        /// SetAboutInfos方法会冲掉已经保存的桩号、流量段、建筑无信息，请谨慎使用（推荐使用AddAboutInfos方法）
        /// </summary>
        /// <param name="stationListA"></param>
        /// <param name="flowSegmentListA"></param>
        /// <param name="CanalStructureListA"></param>
        public void SetAboutInfos(List<double> stationListA = null, List<FlowSegment> flowSegmentListA = null, List<CanalStructure> CanalStructureListA = null, List<LittleStructure> LittleStructureListA = null)
        {
            //添加桩号
            if (stationListA != null && stationListA.Count != 0)
            {
                StationList = stationListA;
                //桩号去重
                StationList.Distinct();
            }
            //添加流量段
            if (flowSegmentListA != null && flowSegmentListA.Count != 0)
            {
                FlowSegmentList = flowSegmentListA;
                //去除重复元素
                FlowSegmentList.GroupBy(x => x.Key).Select(y => y.First());
            }
            //添加建筑物
            if (CanalStructureListA != null && CanalStructureListA.Count != 0)
            {
                CanalStructureList = CanalStructureListA;
                //去除重复元素
                CanalStructureList.GroupBy(x => x.Key).Select(y => y.First());
            }
            //添加小型建筑物
            if (LittleStructureListA != null && LittleStructureListA.Count != 0)
            {
                LittleStructureList = LittleStructureListA;
                //去除重复元素
                LittleStructureList.GroupBy(x => x.Key).Select(y => y.First());
            }
        }
        /// <summary>
        /// 此命令会覆盖已有建筑物中的同名建筑物
        /// </summary>
        /// <param name="canalStructure"></param>
        public void AddStructuretoCanal(CanalStructure canalStructure)
        {
            if (IsStructureListContain(canalStructure))
            {
                CanalStructureList.Remove(CanalStructureList.Find(a => a.Key == canalStructure.Key));
            }
            if (CanalStructureList == null)
            {
                CanalStructureList = new List<CanalStructure>();
            }

            CanalStructureList.Add(canalStructure);
            SortCanalStructureList();

        }
        public void AddLittleStructuretoCanal(LittleStructure littleStructure)
        {
            if (IsLittleStructureListContain(littleStructure))
            {
                LittleStructureList.Remove(LittleStructureList.Find(a => a.Key == littleStructure.Key));
            }
            if (LittleStructureList == null)
            {
                LittleStructureList = new List<LittleStructure>();
            }

            LittleStructureList.Add(littleStructure);
            SortLittleStructureList();
        }

        public bool IsStructureListContain(CanalStructure structure)
        {
            bool result = false;
            if (CanalStructureList != null && CanalStructureList.Count > 0)
            {
                List<string> keys = new List<string>();
                foreach (CanalStructure cas in CanalStructureList)
                {
                    keys.Add(cas.Key);
                }
                if (keys.Contains(structure.Key))
                {
                    result = true;
                }
            }
            return result;
        }
        public bool IsLittleStructureListContain(LittleStructure littlestructure)
        {
            bool result = false;
            if (LittleStructureList != null && LittleStructureList.Count > 0)
            {
                List<string> keys = new List<string>();
                foreach (LittleStructure cas in LittleStructureList)
                {
                    keys.Add(cas.Key);
                }
                if (keys.Contains(littlestructure.Key))
                {
                    result = true;
                }
            }
            return result;
        }
        public double GetStructureListMaxStation()
        {
            double result = 0;
            CanalStructureList.ForEach(a => { if (a.EndStation > result) { result = a.EndStation; } });
            return result;
        }
        public void DeleteCanalStructure(string structure_name)
        {
            CanalStructureList.RemoveAt(CanalStructureList.FindIndex(a => a.Name == structure_name));
        }
        //通过name 与基点x、y坐标删除
        public void DeleteLittleCanalStructure(string structure_name,XYZ basep)
        {
            string keys = Constantscs.Littlestructure + "_" + basep.X.ToString("0.000")+ "_" + basep.Y.ToString("0.000"); 
            LittleStructureList.RemoveAt(LittleStructureList.FindIndex(a => a.Key == keys));
        }
        public CanalStructure GetCanalStructure(string structure_name)
        {
            return CanalStructureList.Find(a => a.Name == structure_name);
        }
        public LittleStructure GetLittleStructure(string structure_name, XYZ basep)
        {
            return LittleStructureList.Find(a => (a.m_name == structure_name)&&(a.m_basepoint == basep));
        }
        public DataTable GetCanalStructuresDataTable(string format = "0.000", IFormatProvider provider = null)
        {
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();
            dt.Columns.Add("流量段");
            dt.Columns.Add("建筑物");
            dt.Columns.Add("建筑物类型");
            dt.Columns.Add("长度");
            dt.Columns.Add("起点桩号");
            dt.Columns.Add("起点水位");
            dt.Columns.Add("进口渐变段长");
            dt.Columns.Add("进口连接段长");
            dt.Columns.Add("出口连接段长");
            dt.Columns.Add("出口渐变段长");
            if(CanalStructureList!=null && CanalStructureList.Count > 0) { 
            foreach (CanalStructure item in CanalStructureList)
            {
                DataTable flodt = item.ToDataTable(format, provider);
                foreach (DataRow row in flodt.Rows)
                {
                    dt.ImportRow(row);
                }
            }
            }
            return dt;
        }
        public DataTable GetAllLittleStructuresDataTable(string format="0.000", IFormatProvider provider = null)
        {
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();
            dt.Columns.Add("小建类型");
            dt.Columns.Add("小建名");
            dt.Columns.Add("小建桩号");
            foreach (LittleStructure item in AllLittleStructureList)
            {
                DataTable flodt = item.ToDataTable(format, provider);
                foreach (DataRow row in flodt.Rows)
                {
                    dt.ImportRow(row);
                }
            }
            return dt;
        }
        public DataTable GetAllCanalStructuresDataTable(string format = "0.000", IFormatProvider provider = null)
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
            foreach (CanalStructure item in AllCanalStructureList)
            {
                DataTable flodt = item.ToDataTable(format, provider);
                foreach (DataRow row in flodt.Rows)
                {
                    dt.ImportRow(row);
                }
            }
            return dt;
        }
        public void SetCanalStructuresFromDataTable(DataTable canalStructuresDataTable)
        {
            CanalStructureList = new List<CanalStructure>();
            int num = canalStructuresDataTable.Rows.Count;
            for (int i = 0; i < num - 1; i++)
            {
                DataRow dr = canalStructuresDataTable.Rows[i];
                CanalStructure canalStucturetemp = new CanalStructure();
                canalStucturetemp.FlowSegmentName = dr["流量段"].ToString();  //流量段
                canalStucturetemp.stuType = dr["建筑物类型"].ToString().ToStructureType();
                canalStucturetemp.Name = dr["建筑物"].ToString();
                canalStucturetemp.StartStation = double.Parse(dr["起点桩号"].ToString());
                canalStucturetemp.StartWaterLevel = double.Parse(dr["起点水位"].ToString());
                canalStucturetemp.SetEndStation(double.Parse(dr["起点桩号"].ToString()) + double.Parse(dr["长度"].ToString()));
                canalStucturetemp.Trans_section_Slength = double.Parse(dr["进口渐变段长"].ToString());   //渐变段长度
                canalStucturetemp.Connect_section_Slength = double.Parse(dr["进口连接段长"].ToString());  //连接段长度
                canalStucturetemp.Trans_section_Elength = double.Parse(dr["出口连接段长"].ToString());  //渐变段长度
                canalStucturetemp.Connect_section_Elength = double.Parse(dr["出口渐变段长"].ToString());   //连接段长度
                AddStructuretoCanal(canalStucturetemp);
            }

        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canal2"></param>
        /// <returns></returns>
        public Canal JoinCanal(Canal canal2)
        {
           
                List<XYZ> ca1pos = constructLine_points;
                List<double> ca1rs = centerLine_arcRadius;
                List<XYZ> ca2pos = canal2.ConstructLinePoints;
                List<double> ca2rs = canal2.ArcRadiuList;
                ca1rs.Add(0.0);
                ca1rs.Add(0.0);
                ca1pos.AddRange(ca2pos);
                ca1rs.AddRange(ca2rs);
               
                Canal ca3 = new Canal(ca1pos, ca1rs,StartStation);
                double startChangelen = canal2.StartStation;
                double stationTOadd = ca3.CenterLine.GetLength() + StartStation - canal2.CenterLine.GetLength() - canal2.StartStation;
                canal2.UpdateAboutInfos(startChangelen, stationTOadd);
                List<double> ca2stationList = canal2.StationList;
                List<FlowSegment> ca2flowSegmentList = canal2.FlowSegmentList;
                List<CanalStructure> ca2CanalStructureList = canal2.CanalStructureList;
                List<LittleStructure> ca2LittleStructureList = canal2.LittleStructureList;
                ca3.AddAboutInfos(ca2stationList, ca2flowSegmentList, ca2CanalStructureList,ca2LittleStructureList);
                return ca3;
          

        }

        public Canal JoinCanals(List<Canal> canals)
        {
            Canal first = canals.First();
            List<Canal> remains = canals.GetRange(1, canals.Count - 1);
            foreach (Canal cg in remains)
            {
                first = first.JoinCanal(cg);

            }
            return first;
        }
        public List<Canal> SplitCanal(double station)
        {
            List<Canal> result = new List<Canal>();
            XYZ po = CenterLine.GetPointAtDist(station - start_station);
            int indexOfPo = IndexOfPointInConstructLinePoints(po);
            int count = constructLine_points.Count;
            if (indexOfPo != -1)
            {
                List<XYZ> firstConstructPoints = constructLine_points.GetRange(0, indexOfPo + 1);
                firstConstructPoints.Add(po);
                List<double> firstArcRadius = centerLine_arcRadius.GetRange(0, indexOfPo);
                Canal firstCanal = new Canal(firstConstructPoints, firstArcRadius, StartStation);
                result.Add(firstCanal);
                List<XYZ> secondConstructPoints = constructLine_points.GetRange(indexOfPo + 1, count - indexOfPo - 1);
                secondConstructPoints.Insert(0, po);
                List<double> secondArcRadius = centerLine_arcRadius.GetRange(indexOfPo, count - indexOfPo - 2);
                Canal secondCanal = new Canal(secondConstructPoints, secondArcRadius, station);
                result.Add(secondCanal);
            }
            return result;
        }

        public void AddPointAtEnd(XYZ point, double arcRadius = 0.0)
        {
            if (constructLine_points == null)
            {
                constructLine_points = new List<XYZ>();
            }
            if (centerLine_arcRadius == null)
            {
                centerLine_arcRadius = new List<double>();
            }
            constructLine_points.Add(point);
            centerLine_arcRadius.Add(arcRadius);
            SetLength();
        }
        public void AddPointAtStart(XYZ point, double arcRadius = 0.0)
        {
            constructLine_points.Insert(0, point);
            centerLine_arcRadius.Insert(0, arcRadius);
            ArcHelper ah = new ArcHelper(point, constructLine_points[0], constructLine_points[1], arcRadius);
            double len1 = ah.P0.DistanceTo(ah.P1);
            double len2 = ah.qiehu.Length;
            double len3 = ah.P2.DistanceTo(ah.P3);
            double stationTOadd = len1 + len2 - len3;
            UpdateAboutInfos(len3, stationTOadd);
            SetLength();

        }
        public void InsertPointOnCenterLine(XYZ source, double arcRadius = 0.0)
        {
            int indexOfPosition = IndexOfPointInConstructLinePoints(source);
            if (indexOfPosition != -1)
            {
                constructLine_points.Insert(indexOfPosition, source);
                centerLine_arcRadius.Insert(indexOfPosition - 1, arcRadius);
                SetLength();
            }
            else
            {
                throw new Exception("点不在中心线上！");
            }


        }
        public void InsertPointOffsetCenterLine(XYZ source, double arcRadius = 0.0)
        {
            double closetLen = 0;
            if (CenterLine.ClosestPoint(source, out closetLen))
            {
                XYZ closetPo = CenterLine.PointAtLength(closetLen);
                int indexOfPosition = IndexOfPointInConstructLinePoints(closetPo);
                if (indexOfPosition != -1)
                {
                    constructLine_points.Insert(indexOfPosition+1, source);
                    centerLine_arcRadius.Insert(indexOfPosition , arcRadius);
                    SetLength();
                }
                else
                {
                    throw new Exception("点不在中心线上！");
                }
            }


        }

        /// <summary>
        /// Removes the element at the specified index of the System.Collections.Generic.List`1.
        /// index:The zero-based index of the element to remove.
        /// </summary>
        /// <param name="pointNumber"></param>
        public void DeletePoint(int pointNumber)
        {
            if (pointNumber == 0)
            {
                constructLine_points.RemoveAt(pointNumber);
                centerLine_arcRadius.RemoveAt(pointNumber);
            }
            else if (pointNumber == constructLine_points.Count - 1)
            {
                constructLine_points.RemoveAt(pointNumber);
                centerLine_arcRadius.RemoveAt(pointNumber - 2);
            }
            else
            {
                constructLine_points.RemoveAt(pointNumber);
                centerLine_arcRadius.RemoveAt(pointNumber - 1);
            }
            SetLength();

        }

        public void UpdateAboutInfos(double startChangeStation,double stationToAdd)
        {
       List<FlowSegment> flowtemp=new List<FlowSegment>(); 
            for(int i=0;i< FlowSegmentList.Count;i++)
            {
                FlowSegment item = FlowSegmentList[i];
                if (item.StartStation > startChangeStation)
                {
                    item.ChangeStartStation(item.StartStation+ stationToAdd);
                }
                flowtemp.Add(item);
            }
            FlowSegmentList = flowtemp;      

            List<double> sttemp = new List<double>();
            for (int i = 0; i < StationList.Count; i++)
            {
                double item = StationList[i];
                if (item > startChangeStation)
                {
                    item = item+ stationToAdd;
                }
                sttemp.Add(item);
            }
            StationList = sttemp;
           
            List<CanalStructure> cstemp = new List<CanalStructure>();
            for (int i = 0; i < CanalStructureList.Count; i++)
            {
                CanalStructure item = CanalStructureList[i];
                if (item.StartStation > startChangeStation)
                {
                    item.ChangeStartStation( item.StartStation + stationToAdd);
                }
                cstemp.Add(item);
            }
            CanalStructureList = cstemp;

            List<LittleStructure> lstemp = new List<LittleStructure>();
            for (int i = 0; i < LittleStructureList.Count; i++)
            {
                LittleStructure item = LittleStructureList[i];
                if (item.Station > startChangeStation)
                {
                    item.ChangeStation(item.Station + stationToAdd);
                }
                lstemp.Add(item);
            }
            LittleStructureList = lstemp;
        }
        public void CopyAboutInfosFrom(Canal source)
        {
            FlowSegmentList = source.FlowSegmentList;
            StationList = source.StationList;
            CanalStructureList = source.CanalStructureList;
            LittleStructureList = source.LittleStructureList;
        }

        //水力计算
        public List<StationCharacteristics> WaterLineCalculate()
        {
            List<StationCharacteristics> result = new List<StationCharacteristics>();
            List<CanalStructure> CanalStructureListtemp = GetAllCanalStructureList();
            CanalStructure firstcanal = CanalStructureListtemp[0];
            firstcanal.StartWaterLevel = start_water_level;
            List<StationCharacteristics> first = CanalStructureListtemp.First().GetStationElevationList(this);
            result.AddRange(first);
            double lastWaterLevel = first.Last().WaterElevation;
            int num = CanalStructureListtemp.Count;

            for (int i = 1; i < num; i++)
            {
                CanalStructure tempcanal = CanalStructureListtemp[i];
                tempcanal.StartWaterLevel = lastWaterLevel;
                List<StationCharacteristics> temp = tempcanal.GetStationElevationList(this);
                temp.RemoveAt(0);
                result.AddRange(temp);
                lastWaterLevel = temp.Last().WaterElevation;
            }
            return result;
        }

        public List<LittleStructure> GetAllLittleStructureList()
        {
            SortLittleStructureList();
            List<LittleStructure> CanalStructureListtemp = new List<LittleStructure>();
            CanalStructureListtemp= LittleStructureList;
            return CanalStructureListtemp;
        }
        public List<CanalStructure> GetAllCanalStructureList()
        {
            SortCanalStructureList();
            //20220518暂只考虑起始点为明渠
            //将建筑物补全，没有建筑物的赋值为默认流量段默认建筑物，默认流量段设置为上一建筑物的流量段，第一段如果默认为下一建筑物的流量段
            List<CanalStructure> CanalStructureListtemp = new List<CanalStructure>();
            if(CanalStructureList!=null && CanalStructureList.Count > 0)
            {
                CanalStructure firstCanalStructure = CanalStructureList.First();
                FlowSegment firstFlow = FlowSegmentInfo.GetFlowSegment(firstCanalStructure.FlowSegmentName);
                CanalStructure endCanalStructure = CanalStructureList.Last();
                FlowSegment endFlow = FlowSegmentInfo.GetFlowSegment(endCanalStructure.FlowSegmentName);
                List<double> CanalStuctureStartStationList = new List<double>();
                CanalStructureList.ForEach(a => CanalStuctureStartStationList.Add(a.StartStation));

                if (Math.Abs(firstCanalStructure.StartStation - StartStation) > 1E-2)
                {
                    CanalStructure canalStucturetemp = new CanalStructure();
                    canalStucturetemp.stuType = firstFlow.defaultstrcuturetype;
                    canalStucturetemp.Name = firstFlow.defaultstrcuturetype.ToDescription() + start_station.ToString("0.000");
                    canalStucturetemp.StartStation = start_station;
                    canalStucturetemp.SetEndStation(firstCanalStructure.StartStation);
                    canalStucturetemp.Trans_section_Slength = 0;   //渐变段长度
                    canalStucturetemp.Connect_section_Slength = 0;  //连接段长度
                    canalStucturetemp.Trans_section_Elength = 0;  //渐变段长度
                    canalStucturetemp.Connect_section_Elength = 0;   //连接段长度
                    canalStucturetemp.FlowSegmentName = firstFlow.Name;  //流量段
                    CanalStructureListtemp.Add(canalStucturetemp);
                }

                int num = CanalStructureList.Count;
                for (int i = 0; i < num - 1; i++)
                {
                    CanalStructure csnow = CanalStructureList[i];
                    FlowSegment flow = FlowSegmentInfo.GetFlowSegment(csnow.FlowSegmentName);
                    CanalStructure csnext = CanalStructureList[i + 1];
                    CanalStructureListtemp.Add(csnow);
                    if (Math.Abs(csnow.EndStation - csnext.StartStation) > 1E-2)
                    {
                        CanalStructure canalStucturetemp = new CanalStructure();
                        canalStucturetemp.stuType = flow.defaultstrcuturetype;
                        canalStucturetemp.Name = flow.defaultstrcuturetype.ToDescription() + csnow.EndStation.ToString("0.000");
                        canalStucturetemp.StartStation = csnow.EndStation;
                        canalStucturetemp.SetEndStation(csnext.StartStation);
                        canalStucturetemp.Trans_section_Slength = 0;   //渐变段长度
                        canalStucturetemp.Connect_section_Slength = 0;  //连接段长度
                        canalStucturetemp.Trans_section_Elength = 0;  //渐变段长度
                        canalStucturetemp.Connect_section_Elength = 0;   //连接段长度
                        canalStucturetemp.FlowSegmentName = flow.Name;  //流量段
                        CanalStructureListtemp.Add(canalStucturetemp);
                    }
                }
                CanalStructureListtemp.Add(endCanalStructure);

                if (Math.Abs(endCanalStructure.EndStation - EndStation) > 1E-2)
                {
                    CanalStructure canalStucturetemp = new CanalStructure();
                    canalStucturetemp.stuType = endFlow.defaultstrcuturetype;
                    canalStucturetemp.Name = endFlow.defaultstrcuturetype.ToDescription() + endCanalStructure.EndStation.ToString("0.000");
                    canalStucturetemp.StartStation = endCanalStructure.EndStation;
                    canalStucturetemp.SetEndStation(EndStation);
                    canalStucturetemp.Trans_section_Slength = 0;   //渐变段长度
                    canalStucturetemp.Connect_section_Slength = 0;  //连接段长度
                    canalStucturetemp.Trans_section_Elength = 0;  //渐变段长度
                    canalStucturetemp.Connect_section_Elength = 0;   //连接段长度
                    canalStucturetemp.FlowSegmentName = endFlow.Name;  //流量段
                    CanalStructureListtemp.Add(canalStucturetemp);
                }

            }

            return CanalStructureListtemp;
        }
        public double GetWaterLevelByStation(double station)
        {
            double s1;
            double s2;
            double e1;
            double e2;
            double result;
            int se= stationElevations.FindIndex(a => a.Station > station);
            s1 = stationElevations[se-1].Station;
            s2= stationElevations[se].Station;
            e1 = stationElevations[se-1].WaterElevation;
            e2 = stationElevations[se].WaterElevation;
            result=(station-s1)*(e2 - e1) / (s2 - s1) + e1;
            return result;
        }
        //以下
        public new string ToString(string format, IFormatProvider provider)
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", new object[9]
            {
            "                         ID:"+"\t"+id+"\n",
            "CenterLineLength:"+"\t"+Length.ToString(format, provider)+"\n",
            "          StartStation:"+"\t"+StartStation.ToString(format, provider)+"\n",
            "   StartWaterLevel:"+"\t"+StartWaterLevel.ToString(format, provider)+"\n",
            "             StartPoint:"+"\t"+StartPoint.ToString(format, provider)+"\n",
            "              EndPoint:"+"\t"+EndPoint.ToString(format, provider)+"\n",
            "  IPpointsNumber:"+"\t"+NumberOfIP.ToString()+"\n",
            "         IPpointsList:"+"\t"+IpPointsList.ToString(format,provider)+"\n",
            "      ArcRadiusList:"+"\t"+IpArcRadiuList.ToString(format,provider)+"\n"
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
