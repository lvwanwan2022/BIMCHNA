
using Lv.BIM.Geometry;
using System;

namespace CanalSystem.BaseClass
{

    //水头损失计算类，用于计算各种建筑物类型的水头损失
    public abstract class Conduit
    {
        public double Flow;
        public double Velocity;
        public double Area;
        public double Depth;
        public double BottomWidth;
        private string type;
        public string Type { get { return type; } set { type = value; } }
    }

    //用于计算沿程损失
    public abstract class OpenConduit : Conduit
    {
        public double Height;
        public double Roughness;
        public double WetPerimeter;
        public double HydraulicRadius;
        public double LongSlope;
        public OpenConduit()
        {
        }
        //计算沿程水头损失
        public double Frictionloss(ICurveXYZ myobj, double LongSlope)
        {
            double YCLoss;
            if (myobj.GetType().ToString() == "Lv.BIM.Geometry.LineXYZ")
            {
                YCLoss = (myobj as LineXYZ).Length / LongSlope;
            }
            else
            {
                YCLoss = (myobj as ArcXYZ).Length / LongSlope;
            }
            return YCLoss;
        }
        //计算弯道水头损失
        public double Curveloss(ArcXYZ myobj)
        {
            double WDLoss;
            if (myobj.GetType().ToString() == "Lv.BIM.Geometry.ArcXYZ")
            {
                WDLoss = ((myobj.Length * Math.Pow(Roughness, 2.0) / Math.Pow(HydraulicRadius, 1.3333333) * Math.Pow(Velocity, 2.0) * 3.0 / 4.0 * Math.Pow(BottomWidth / (myobj as Lv.BIM.Geometry.ArcXYZ).Radius, 0.5)));
            }
            else
            {
                WDLoss = 0.0;
            }
            return WDLoss;
        }
        //计算预留水头损失
        public double Spareloss(ICurveXYZ myobj)
        {
            double YLLoss;
            if (myobj.GetType().ToString() == "Lv.BIM.Geometry.LineXYZ")
            {
                YLLoss = (myobj as LineXYZ).Length / 50000.0 * 0.0;
            }
            else
            {
                YLLoss = (myobj as ArcXYZ).Length / 50000.0 * 0.0;
            }

            return YLLoss;
        }

        //计算沿程水头损失
        public double Frictionloss(PolycurveXYZ poly, double LongSlope)
        {
            double YCLoss;

                YCLoss = poly.GetLength() / LongSlope;
    
            return YCLoss;
        }
        //计算弯道水头损失
        //public double Curveloss(ArcXYZ ArcXYZ)
        //{
        //    double WDLoss;
        //        WDLoss = (ArcXYZ.Length * Math.Pow(Roughness, 2.0) / Math.Pow(HydraulicRadius, 1.3333333) * Math.Pow(Velocity, 2.0) * 3.0 / 4.0 * Math.Pow(BottomWidth / (ArcXYZ).Radius, 0.5));
        //    return WDLoss;
        //}
        //public double Curveloss(ArcXYZ arc)
        //{
        
        //    double WDLoss;
        //    //
        //    WDLoss = (arc.GetLength() * Math.Pow(Roughness, 2.0) / Math.Pow(HydraulicRadius, 1.3333333) * Math.Pow(Velocity, 2.0) * 3.0 / 4.0 * Math.Pow(BottomWidth / (ArcXYZ).Radius, 0.5));
        //    return WDLoss;
        //}
        //计算预留水头损失
        public double Spareloss(PolycurveXYZ poly)
        {
            double YLLoss;
                YLLoss = poly.GetLength() / 50000.0 * 0.0;
            return YLLoss;
        }
    }
    //梯形渠
    public class OpenChannel : OpenConduit
    {
        public double SlopeRatio;
        public double LiningThickness;
        private void setType()
        {
            Type = " OpenChannel";
        }
        //构造
        public OpenChannel() { setType(); }
        //计算弯道水头损失，隐藏基类方法
        new public double Curveloss(ICurveXYZ myobj)
        {
            double WDLoss;
            if (myobj.GetType().ToString() == "Lv.BIM.Geometry.LineXYZ")
            {
                WDLoss = 0.0;
            }
            else
            {
                WDLoss = (myobj as Lv.BIM.Geometry.ArcXYZ).GetLength() * Math.Pow(Roughness, 2.0) / Math.Pow(HydraulicRadius, 1.3333) * Math.Pow(Velocity, 2.0) * 3.0 / 4.0 * Math.Pow((BottomWidth + Depth * SlopeRatio * 1.3 * 2.0) / (myobj as Lv.BIM.Geometry.ArcXYZ).Radius, 0.5);
            }
            return WDLoss;
        }
        
    }
    //矩形渠道类
    public class RecChannel : OpenConduit
    {
        public double SlopeRatio;
        public double LiningThickness;
        private void setType()
        {
            Type = " RecChannel";
        }
        //构造
        public RecChannel() { setType(); }
        //计算弯道水头损失，隐藏基类方法
        new public double Curveloss(ICurveXYZ myobj)
        {
            double WDLoss;
            if (myobj.GetType().ToString() == "Lv.BIM.Geometry.ArcXYZ")
            {
                WDLoss = (myobj as Lv.BIM.Geometry.ArcXYZ).GetLength() * Math.Pow(Roughness, 2.0) / Math.Pow(HydraulicRadius, 1.3333) * Math.Pow(Velocity, 2.0) * 3.0 / 4.0 * Math.Pow((BottomWidth + Depth * SlopeRatio * 1.3 * 2.0) / (myobj  as Lv.BIM.Geometry.ArcXYZ).Radius, 0.5);
            }
            else
            {
                WDLoss = 0;
            }
            return WDLoss;
        }

    }
    //隧道
    public class Tunnel : OpenConduit
    {
        public double SideWallHeight;
        public double BottomBoardThickness;
        private void setType()
        {
            Type = " Tunnel";
        }
        //构造
        public Tunnel()
        {
            setType();
        }
    }
    //渡槽
    public class Aqueduct : OpenConduit
    {
        public double Radius;
        public double BottomBoardThickness;
        private void setType()
        {
            Type = " Aqueduct ";
        }

        public Aqueduct(int i)
        {
            setType();
        }
    }
    //暗涵
    public class BuriedConduit : OpenConduit
    {
        public double SideWallHeight;
        public double BottomBoardThickness;
        private void setType()
        {
            Type = " BuriedConduit ";
        }
        public BuriedConduit(int i)
        {
            setType();
        }
    }

    //集中水头损失
    public abstract class ConcentratedLossStr : Conduit
    {
        public double SpecifiedLoss;
    }
    //闸
    public class Sluice : ConcentratedLossStr
    {
        public double BottomBoardThickness;//底板厚
        //构造
        public Sluice(double Loss)
        {
        }
    }
    //倒虹管
    public class Siphon : ConcentratedLossStr
    {
        public double IncreasedFlow;//加大流量
        //构造
        public Siphon(double Loss)
        {
        }
    }
    //陡坡
    public class Waterfall : ConcentratedLossStr
    {
        public double BottomBoardThickness;
        //构造
        public Waterfall(double Loss)
        {

        }
    }
}
