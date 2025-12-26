namespace CanalSystem.Constants
{
    static class Constantscs
    {
        static public double tolerance = 1.0E-4;
        static public double RADIOZARO = 0;
        //纵断面表格向下偏移的距离
        static public double canalprofileydis = 10;
        //纵断面表格高
        //表头宽
        static public double canalprofiletablewidth = 50;
        static public double canalprofiletablehigh = 12;
        static public double canalprofilestationhigh = 30;
        static public double canalprofilelevelhigh = 15;
        static public double canalprofilenotehigh = 40;
        //纵断面表格文字大小
        static public double canalprofiletextsize1 = 5;
        static public double canalprofiletextsize2 = 3.5;
        //断面表格行数
        static public int canalprofiletablerownumber = 7;
        //表头文字
        static public string Structurname="建筑物名称";
        static public string LongSlope = "比降";
        static public string Station= "桩号";
        static public string Desighwaterlevel = "设计水位";
        static public string Topelevation = "渠顶高程";
        static public string Buttomelevation = "渠底高程";
        static public string Note = "备注";
        //旋转角度
        static public double rotatezero = 0;
        static public double rotate90 = System.Math.PI/2;
        //桩号高程
        static public string Station_format = "000.000";
        //基本缩放比例尺
        static public double basescale=1000;
        static public string Littlestructure = "Littlestructure";
        //放坡半径
        static public double slope_radios = 1.5;
        //小件底板高程
        static public double fuzhi = -999999.0;

    }
}
