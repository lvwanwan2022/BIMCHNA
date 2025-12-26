using System;
//渠系基类
namespace CanalSystem.BaseClass
{
    [Serializable]
    public abstract class CanalBase
    {
        protected string prefix = "";//桩号前缀
        protected double start_station = 0;//起始桩号
        protected double end_station = 0;//终止桩号
        protected double _length = 0;//长
        protected double start_water_level;//起始水位
        protected double end_water_level;//终止水位
        protected string id;
        //public static Dictionary<string, object> myStaticFieldDict = new Dictionary<string, object>();考虑在主程序中定义存储所有类实例的字典
        public double StartStation
        {
            get { return start_station; }
            set { start_station = value; }
        }
        public double EndStation => end_station;
        public double StartWaterLevel
        {
            get { return start_water_level; }
            set { start_water_level = value; }
        }
        public double EndWaterLevel
        {
            get { return end_water_level; }
            // set{end_water_level=value;}
        }
        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }
        public double Length => _length;
        public string ID => id;

        ////2.构造方法
        //public CanalBase() { }
        //public CanalBase(string prefix,double startStation, double endStation, double Startwaterlevel) { }
        //public CanalBase(double startStation,double length,double Startwaterlevel, string prefix) { }
        //3.普通方法
        //***小知识***abstract类在基类中不能有具体实现方法，在子类中必须有方法重写
        protected abstract void Generateid();
        //***小知识***virtual类在基类中要有具体实现方法，在子类中可以不进行方法重写，不重写则调用基类方法，重写则调用子类方法
        //SetLength供Canal使用，同时设置_length和end_station
        public virtual void SetLength() { }
        //SetEndStation供流量段使用，同时设置_length
        public virtual void SetEndStation(double value)
        {
            end_station = value;
        }
    }
}
