using CanalSystem.BaseTools;
using CanalSystem.Constants;

using Lv.BIM.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanalSystem.BaseClass
{
    public class Geogroup
    {
        public XYZ m_basepoint;
        public LineXYZ[] m_lines;//平面抽象符号线
        public Curve[] m_profilelines;//纵面抽象符号线
        public Brep[] m_breaps;//几何体对象
        public Geogroup(XYZ po, LineXYZ[] lines, Brep[] breaps)
        {
            m_basepoint = po;
            m_lines = lines;
            m_breaps = breaps;
        }
    }
    //人行桥,放水洞，山溪渡槽,检修梯步

    [Serializable]
    public class LittleStructure
    {
        public string m_name= "";//可暴露字段
        public string m_type="闸";//可暴露字段
        //public Geogroup m_geos;
        //public XYZ m_vector;
        private double m_station;
        public Canal m_canal;
        private StationLabel m_staionLabel;
        public double Scale = 1.0;
        public double Station=>m_station;
        public double Text_height => 3.5 * Scale;
        public XYZ m_basepoint {
        get{
                XYZ po = m_canal.GetCenterLine().PointAtLength(m_station);
                return po;
            } 
        }
        public string Key 
        { get 
            {
                return Constantscs.Littlestructure + "_" + m_basepoint.X.ToString("0.000") + "_" + m_basepoint.Y.ToString("0.000");
            }
        }//用来对list去重
      
        public LittleStructure( double station ,string name, Canal canal,string type= "闸")
        {
            m_station = station;
            m_name = name;
            m_canal = canal;
            m_staionLabel = new StationLabel(m_station, canal.CenterLine, canal.StartStation, canal.Prefix, Scale, m_name);
        }
        public LittleStructure( XYZ po,Geogroup geos,LineXYZ[] lines, Brep[] breaps, double hscale, double vscale, string name,string type, Canal canal, double station=0)
        {
            m_canal = canal;
            m_name = name;
            m_type = type;
            //m_geos = geos;
            m_station = station;
            //m_vector = po - geos.m_basepoint;
            m_staionLabel = new StationLabel(m_station, canal.CenterLine, canal.StartStation, canal.Prefix, Scale, m_name);
        }
        public string ChangeStation(double station)
        {
            string result = "";
            m_station = station;
            result = result+"chenge success";
            return result;
        }
        //public void AddToDucment(Lv.BIMDoc doc)
        //{
        //    m_staionLabel.AddToDocument(doc);
        //}
   
        public DataTable ToDataTable(string format, IFormatProvider provider)
        {
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();
            dt.Columns.Add("小建类型");
            dt.Columns.Add("小建名");
            dt.Columns.Add("小建桩号");
            dr["小建类型"] = m_type;
            dr["小建名"] = m_name;
            dr["小建桩号"] = m_station.ToString(format, provider);
            dt.Rows.Add(dr);
            return dt;
        }

        //public void DeleFromDucment(Lv.BIMDoc doc, ObjRef[] objrefs)
        //{
        //    m_staionLabel.DeleFromDucment(doc, objrefs);
        //}
     
    }
}
