using CanalSystem.BaseClass;
using CanalSystem.Constants;
using Lv.BIM;
using Lv.BIM.Geometry;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CanalSystem.BaseTools
{
    //转换类
    public static class ConvertTools
    {
        //长度转换为桩号，格式化
        //渠道名格式化
        public static string DoubleToStr(this double chainage, string canalName)
        {
            string mystr = chainage.ToString("0.000").PadLeft(10);
            int KM = (int)chainage / 1000;
            string zfc1 = canalName.Substring(0, 1);
            string mystring = zfc1 + KM.ToString() + "+" + mystr.Substring((mystr.Length - 7), 7);
            return mystring;
        }
        //桩号格式化
        public static string GetLabelText(double _station, string Prefix,string Station_format="000.000")
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
       
        public static  string GetArcnotetext(ArcXYZ _arc)
        {
            string Text_format = "0.000";//设置小数点位数，默认三位小数
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

        //获取圆弧段中心点桩号
        public static double GetConstructLinePointsStationList(PolycurveXYZ CenterLine,XYZ po,double startstation)
        {
            double result = 0;
            double len = 0;
                if (CenterLine.ClosestPoint(po, out len))
                {
                   result=startstation + len;
                }

            return result;
        }
    }


    //几何计算类
    public static class GeneralTools
    {
        //求直线与曲线的远交点,通过包容盒重计算Line，使之与曲线有交点
    //public static CurveIntersections IntersectCurveLine(
    // Lv.BIM.Geometry.Curve curve,
    // Lv.BIM.Geometry.Line line,
    // double tolerance,
    // double overlapTolerance
    // )
    //    {
    //        if (!curve.IsValid || !line.IsValid || line.Length < //BIMMath.SqrtEpsilon)
    //            return null;

    //        // Extend the line through the curve's bounding box
    //        var bbox = curve.GetBoundingBox(false);
    //        if (!bbox.IsValid)
    //            return null;

    //        var dir = line.Direction;
    //        dir.Unitize();

    //        var points = bbox.GetCorners();
    //        var plane = new Lv.BIM.Geometry.Plane(line.From, dir);

    //        double max_dist;
    //        var min_dist = max_dist = plane.DistanceTo(points[0]);
    //        for (var i = 1; i < points.Length; i++)
    //        {
    //            var dist = plane.DistanceTo(points[i]);
    //            if (dist < min_dist)
    //                min_dist = dist;
    //            if (dist > max_dist)
    //                max_dist = dist;
    //        }

    //        // +- 1.0 makes the line a little bigger than the bounding box
    //        line.From = line.From + dir * (min_dist - 1.0);
    //        line.To = line.From + dir * (max_dist + 1.0);

    //        // Calculate curve-curve intersection
    //        var line_curve = new LineXYZ(line);
    //        return Intersection.CurveCurve(curve, line_curve, tolerance, overlapTolerance);
    //    }
        public static List<double> GetRadius(PolycurveXYZ PolycurveXYZ)
        {
            List<double> result = new List<double>();
            int num = PolycurveXYZ.SegmentCount;
            
            for (int i = 0; i < num; i++)
            {
                ICurveXYZ item = PolycurveXYZ.SegmentCurve(i);
                
                if (item.IsArc())
                {
                    ArcXYZ arc = item as  ArcXYZ;
                    result.Add(arc.Radius);
                }
            }
            return result;
        }
        public static List<double> GetRadius1(PolycurveXYZ PolycurveXYZ)
        {
            List<double> result = new List<double>();
            int num = PolycurveXYZ.SegmentCount;
           
            for (int i = 0; i < num; i++)
            {
                ICurveXYZ item = PolycurveXYZ.SegmentCurve(i);

                if (item.IsArc())
                {
                    ArcXYZ arc = item as ArcXYZ;
                    result.Add(arc.Radius);
                }
                else
                {
                    result.Add(0);
                }
            }
            return result;
        }
        ////构造线圆弧半径赋值
        public static List<double> GetConsLineArcRadiu(List<XYZ> polyline, List<double> radius)
        {
            List<double> result = new List<double>();
            int j = 0;
            for (int i = 1; i < polyline.Count - 1; i++)
            {
                XYZ v1 = polyline[i] - polyline[i - 1];
                XYZ v2 = polyline[i + 1] - polyline[i];
                if (!v1.IsParallelTo(v2))
                {
                    result.Add(radius[j]);
                    j++;
                }
                else
                {
                    result.Add(0);
                }
            }
            return result;
        }
        public static XYZ getConspoint(ArcXYZ arc)
        {
            double sa = (arc.EndAngle - arc.StartAngle) / 2;
            double len = arc.Radius * Math.Tan(sa);
            XYZ v1 = arc.StartPoint - arc.Center;
            XYZ v2 = arc.EndPoint - arc.Center;
            XYZ ax = v1.CrossProduct( v2);
            v1.RotateByAxisAndDegree(ax,Math.PI / 2);
            v1.Unitize();
            XYZ point = arc.StartPoint + v1 * len;
            return point;
        }

        //public static List<XYZ> Getpolylinepoints(PolycurveXYZ PolycurveXYZ)
        //{
        //    int num = PolycurveXYZ.SegmentCount;
        //    List<double> r = new List<double>();
        //    r = GetRadius1(PolycurveXYZ);
        //    Arc arc = new Arc();
        //    //构造线List
        //    List<XYZ> points = new List<XYZ>();
        //    //加起始点
        //    points.Add(PolycurveXYZ.SegmentCurve(0).PointAtStart);

        //    for (int j = 1; j < num; j++)
        //    {
        //        //判断前一个对象是否为圆弧
        //        if (r[j - 1] != Constantscs.RADIOZARO)
        //        {
        //            if (PolycurveXYZ.SegmentCurve(j).TryGetArc(out arc))
        //            {
        //                points.Add(getConspoint(arc));
        //            }
        //        }
        //        else
        //        {
        //            if (PolycurveXYZ.SegmentCurve(j).TryGetArc(out arc))
        //            {
        //                points.Add(getConspoint(arc));
        //            }
        //            else
        //            {
        //                points.Add(PolycurveXYZ.SegmentCurve(j).PointAtStart);
        //            }
        //        }
        //    }
        //    points.Add(PolycurveXYZ.SegmentCurve(num - 1).PointAtEnd);
        //    return points;
        //}

        
        //求圆弧中心点
        public static List<XYZ> Getcenterpoints(PolycurveXYZ PolycurveXYZ)
        {
            List<XYZ> result = new List<XYZ>();
            int num = PolycurveXYZ.SegmentCount;
            //ArcXYZ arc = new ArcXYZ();

            for (int i = 0; i < num; i++)
            {
                if (PolycurveXYZ.SegmentCurve(i).Length != 0 && PolycurveXYZ.SegmentCurve(i).IsArc())
                {
                    if (PolycurveXYZ.SegmentCurve(i).IsArc())
                    {
                        ArcXYZ arc= PolycurveXYZ.SegmentCurve(i) as ArcXYZ;
                        result.Add(arc.Center);
                    }
                }
                else if (PolycurveXYZ.SegmentCurve(i).IsArc() && PolycurveXYZ.SegmentCurve(i).Length == 0)
                {
                    XYZ p = PolycurveXYZ.SegmentCurve(i).GetPointAtDist(0.5);
                    result.Add(p);
                }
            }
            return result;
        }
        //构造线圆弧中心点标准化
        public static List<XYZ> GetConsLineArcCenter(List<XYZ> polyline, List<double> radius, List<XYZ> centerpoints)
        {
            List<XYZ> result = new List<XYZ>();
            //XYZ po = new XYZ(0, 0, 0);
            int j = 0;
            if (centerpoints.Count > 0)
            {
                for (int i = 1; i < polyline.Count - 1; i++)
                {

                    if (radius[i - 1] != 0)
                    {
                        result.Add(centerpoints[j]);
                        j++;
                    }
                    else
                    {
                        result.Add(polyline[i]);
                    }
                }
            }
            else
            {
                for (int i = 1; i < polyline.Count - 1; i++)
                {
                    result.Add(polyline[i]);
                }
            }
            return result;
        }

        public static List<XYZ> GetConstructPolylinePoints(PolycurveXYZ PolycurveXYZ)
        {
            
            return PolycurveXYZ.Points;
        }

        public static List<double> GetCenterLineArcRadius(PolycurveXYZ PolycurveXYZ)
        {
            int numOfseg = PolycurveXYZ.SegmentCount;
            List<XYZ> constructpoints_temp = new List<XYZ>();
            List<double> arcRadius_temp = new List<double>();
            bool skipflag = false;
            for (int i = 0; i < numOfseg; i++)
            {
                ICurveXYZ item = PolycurveXYZ.SegmentAt(i);
                string nameaa = item.GetType().Name;
              
                if (item.IsArc())//line就添加一个切线交点
                {
                    ArcHelper arc = new ArcHelper(item as ArcXYZ);
                    XYZ po = arc.P2;
                    constructpoints_temp.Add(po);
                    arcRadius_temp.Add(arc.Radius);
                    skipflag = true;//如果碰到圆弧则需要跳过一个点，定义一个skipflag
                }
                if (item.IsLine() )//line就添加一个起始点
                {
                    Curve aa = item as Curve;
                    if (!skipflag)
                    {
                        XYZ po = item.StartPoint;
                        constructpoints_temp.Add(po);
                        arcRadius_temp.Add(0.0);
                    }
                    skipflag = false;//每次结束将跳过flag设为初始值
                }
            }
            constructpoints_temp.Add(PolycurveXYZ.PointAtEnd);
            //arcRadius_temp.RemoveAt(numOfseg - 1);
            arcRadius_temp.RemoveAt(0);
            return arcRadius_temp;
        }

      
        /// <summary>
        /// 传入类型A的对象a,类型B的对象b，将b和a相同名称的值进行赋值给a
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Mapper<B, A>(B b, ref A a)
        {
            try
            {
                Type Typeb = b.GetType();//获得类型  
                Type Typea = typeof(A);
                Type Typea_parent = Typea.BaseType;
                Type Typeb_parent = Typeb.BaseType;
                FieldInfo[] sfs = Typeb.GetFields();
               //FieldInfo[] sfs_parent = Typeb_parent.GetFields();
               //FieldInfo[] b_diejia = sfs.Concat(sfs_parent).ToArray();
                FieldInfo[] afs = Typea.GetFields();
                //FieldInfo[] afs_parent = Typea_parent.GetFields();//获取父类字段
                //FieldInfo[] a_diejia = afs.Concat(afs_parent).ToArray();
                //PropertyInfo[] sps = Typeb.GetProperties(BindingFlags.Public);
                foreach (var sp in sfs)//获得类型的属性字段  
                {
                   
                   // PropertyInfo[] aps = Typea.GetProperties(BindingFlags.Public);
                    foreach (var ap in afs)
                    {
                        if (ap.Name == sp.Name)//判断属性名是否相同  
                        {
                            ap.SetValue(a, sp.GetValue(b));//获得b对象属性的值复制给a对象的属性  
                        }
                    }
                }
                // return a;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public static class OutPutTools
    {
        public static string ToString(this List<XYZ> list, string fromat, IFormatProvider provider)
        {
            string result = "";
            foreach (XYZ po in list)
            {
                result += po.ToString(fromat, null) + "\t";
            }
            return result;
        }
        public static string ToString(this List<double> list, string fromat, IFormatProvider provider)
        {
            string result = "";
            foreach (double po in list)
            {
                result += po.ToString(fromat, null) + "\t";
            }
            return result;
        }
        public static string ToBIMString(this List<StationCharacteristics> list, string fromat = "0.000", IFormatProvider provider = null)
        {
            string result = "";
            foreach (StationCharacteristics po in list)
            {
                result += po.ToString(fromat, null) + "\t";
            }
            return result;
        }

        public static DataTable ToDataTable(this List<StationCharacteristics> list)
        {
            DataTable dt = new DataTable();
            DataRow dr = dt.NewRow();
            dt.Columns.Add("桩号");
            dt.Columns.Add("名称");
            dt.Columns.Add("水位");
            dt.Columns.Add("底板高程");
            dt.Columns.Add("顶板高程");
            foreach (StationCharacteristics item in list)
            {
                DataTable flodt = item.ToDataTable();
                foreach (DataRow row in flodt.Rows)
                {
                    dt.ImportRow(row);
                }
            }
            return dt;
        }
    }
    //地形几何计算类
    //public static class CalculateIntersectionoftopo
    //{
    //    //插值计算高程点与中心线投影交点
    //    public static List<XYZ> CaLculateIntersectionPoints(List<XYZ> elevationpoints, Curve centerline)
    //    {
    //        List<XYZ> points = new List<XYZ>();
    //        List<XYZ> boxpoints = new List<XYZ>();
    //        List<XYZ> Litterboxpoints = new List<XYZ>();
    //        BoundingBox box=centerline.GetBoundingBox(false);
    //        //计算包容盒以内的点
    //        foreach (XYZ pp in box.GetCorners())
    //        {
    //            XYZ ppx = pp;
    //            ppx.Z = 0;
    //            boxpoints.Add(ppx);
    //        }
    //        BoundingBox nbox = new BoundingBox(boxpoints);
    //        foreach (XYZ pt in elevationpoints)
    //        {
    //            XYZ npt = pt;
    //            npt.Z = 0;
    //            if (nbox.Contains(pt))
    //            {
    //                Litterboxpoints.Add(pt);
    //            }
    //        }
    //        //连接高程相同的点

    //        return points;
    //    }
    //    //排序
    //}
}

