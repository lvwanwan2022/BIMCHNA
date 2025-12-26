

using System;
using System.Collections.Generic;
using System.Text;

namespace Lv.BIM.Geometry
{
    [Serializable]
    public class CircleXYZ : Base, ICurve,IHasArea
    {
        //p_plane定义内容过多,为可空内容，主要用于保存其他程序传出信息，自定义方法请尽量使用p_normal
        private PlaneXYZ p_plane;
        //自定义方法请尽量使用p_normal
        private XYZ p_normal;
        private XYZ p_center;
        private double d_radius;
        public XYZ Normal => p_normal;
        public XYZ Center => p_center;
        public double Length => Math.Abs(Math.PI*2 * d_radius);   
        public double Radius => d_radius;
        public PlaneXYZ Plane => p_plane;

        public double Area { get => Math.PI*d_radius*d_radius;  }

        public CircleXYZ()
        {
            p_normal = XYZ.BasisZ;
            p_center = new XYZ();
            d_radius = 1;
            p_plane = new PlaneXYZ();
        }

        public CircleXYZ(PlaneXYZ plane, XYZ center, double radius)
        {
            p_normal = plane.Normal;
            d_radius = radius;
            p_center = center;
            p_plane = plane;
        }
        /// <summary>
        /// 用圆弧上三点创建圆弧
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="pointOnCircle"></param>
        public static CircleXYZ CreateBy3Point(XYZ startPoint, XYZ endPoint, XYZ pointOnCircle)
        {
            XPlane xp = XPlane.CreateBy3Point(startPoint, endPoint, pointOnCircle);
            LineXYZ li13 = new LineXYZ(startPoint, endPoint);
            LineXYZ li12 = new LineXYZ(startPoint, pointOnCircle);
            XLineXYZ vertical13 = li13.GetMiddlePerpendicularXLineOnPlane(xp);
            XLineXYZ vertical12 = li12.GetMiddlePerpendicularXLineOnPlane(xp);
            XYZ centertemp = vertical12.GetIntersectPoint(vertical13);
           XYZ ap_center = centertemp;
            PlaneXYZ ap_plane;
            if (Math.Abs(startPoint.Z - endPoint.Z) < 1E-9 && Math.Abs(endPoint.Z - pointOnCircle.Z) < 1E-9)
            {
                ap_plane = new PlaneXYZ(new XYZ(0, 0, endPoint.Z), new XYZ(1, 0, 0), new XYZ(0, 1, 0));
            }
            else if (Math.Abs(startPoint.Y - endPoint.Y) < 1E-9 && Math.Abs(endPoint.Y - pointOnCircle.Y) < 1E-9)
            {
                ap_plane = new PlaneXYZ(new XYZ(0, endPoint.Y, 0), new XYZ(1, 0, 0), new XYZ(0, 0, 1));
            }
            else if (Math.Abs(startPoint.X - endPoint.X) < 1E-9 && Math.Abs(endPoint.X - pointOnCircle.X) < 1E-9)
            {
                ap_plane = new PlaneXYZ(new XYZ(endPoint.X, 0, 0), new XYZ(0, 1, 0), new XYZ(0, 0, 1));
            }
            else
            {
                ap_plane = new PlaneXYZ(ap_center, endPoint, startPoint);
            }

            return new CircleXYZ(ap_center, startPoint, endPoint);


        }

        public CircleXYZ(XYZ center, XYZ pointOnCircle,XYZ pointOnPlane)
        {
            PlaneXYZ ap_plane;
            if (Math.Abs(center.Z - pointOnCircle.Z) < 1E-9 && Math.Abs(pointOnCircle.Z - pointOnPlane.Z) < 1E-9)
            {
                p_plane = new PlaneXYZ(new XYZ(0, 0, pointOnCircle.Z), new XYZ(1, 0, 0), new XYZ(0, 1, 0));
            }
            else if (Math.Abs(center.Y - pointOnCircle.Y) < 1E-9 && Math.Abs(pointOnCircle.Y - pointOnPlane.Y) < 1E-9)
            {
                p_plane = new PlaneXYZ(new XYZ(0, pointOnCircle.Y, 0), new XYZ(1, 0, 0), new XYZ(0, 0, 1));
            }
            else if (Math.Abs(center.X - pointOnCircle.X) < 1E-9 && Math.Abs(pointOnCircle.X - pointOnPlane.X) < 1E-9)
            {
                p_plane = new PlaneXYZ(new XYZ(pointOnCircle.X, 0, 0), new XYZ(0, 1, 0), new XYZ(0, 0, 1));
            }
            else
            {
                p_plane = new PlaneXYZ(center, pointOnCircle, pointOnPlane);
            }
            XYZ vec1 = pointOnCircle - center;
            XYZ vec2 = pointOnPlane - center;
            if (vec2.IsParallelTo(vec1))
            {
                throw new Exception("三点共线");
            }
            p_normal = (vec1.CrossProduct(vec2)).Normalize();
            p_center = center;
            d_radius = center.DistanceTo(pointOnCircle);
        }
        public CircleXYZ(PlaneXYZ plane, XYZ center, XYZ pointOnCircle)
        {
            if (plane.IsPointOnPlane(center) && plane.IsPointOnPlane(pointOnCircle) )
            {
                p_center = center;
                d_radius = center.DistanceTo(pointOnCircle);
                p_plane = plane;
                p_normal = plane.Normal;
            }
            else
            {
                throw new Exception("点不在平面上");
            }

        }

      
        public bool IsPointOnCurve(XYZ source)
        {
            bool result = false;
            if (Math.Abs(source.DistanceTo(p_center) - d_radius) < 1E-9)
            {               
                        return true;
            }

            return result;
        }

        public XYZ GetClosestPoint(XYZ source)
        {
            if (!p_plane.IsPointOnPlane(source))
            {
                throw new Exception("点不在平面内");
            }
            XYZ dir = source - p_center;
            XYZ Dpoint = p_center + dir.Normalize() * d_radius;
            return Dpoint;
        
        }
        public XYZ GetPointByAngle(double source)
        {

            XYZ dir =p_plane.ConvertVectorUVToXYZ (new UV(source));
            XYZ Dpoint = p_center + dir.Normalize() * d_radius;
            return Dpoint;

        }
        public double GetAngleByPoint(XYZ source)
        {
            if (p_plane.IsPointOnPlane(source))
            {
                XYZ dir = source - p_center;
                return p_plane.Xaxis.AngleOnPlaneTo(dir,p_plane.Normal);
            }
            else
            {
                throw new Exception("点不在平面内");
            }  

        }
        public XYZ Get1sIntersectPointByBaseAndDirection(XYZ basePoint,XYZ direction)
        {
            LineXYZ li = LineXYZ.CreateByStartPointAndVector(basePoint, direction);
            XYZ projectPo = li.GetProjectPoint(Center);
            double juli = Center.DistanceTo(projectPo);
            if (juli> Radius)
            {
                return null;
            }
            else
            {
                double len = Math.Sqrt(Radius *Radius - juli *juli);
                XYZ po1 = projectPo + (direction.Normalize() * len);
                return po1;
            }
        }
        public XYZ Get2sIntersectPointByBaseAndDirection(XYZ basePoint, XYZ direction)
        {
            LineXYZ li = LineXYZ.CreateByStartPointAndVector(basePoint, direction);
            XYZ projectPo = li.GetProjectPoint(Center);
            double juli = Center.DistanceTo(projectPo);
            if (juli > Radius)
            {
                return null;
            }
            else
            {
                double len = Math.Sqrt(Radius * Radius - juli * juli);
                XYZ po1 = projectPo - direction.Normalize() * len;
                return po1;
            }
        }
        
        public List<double> ToList()
        {
            var list = new List<double>();

            list.AddRange(Center.ToList());
            list.Add(Radius);
            list.Insert(0, CurveTypeEncoding.Circle);
            list.Insert(0, list.Count);
            return list;
        }

        public static CircleXYZ FromList(List<double> list)
        {
            XYZ startpo = XYZ.FromList(list.GetRange(0, 2));
            XYZ midpo = XYZ.FromList(list.GetRange(3, 5));
            XYZ endpo = XYZ.FromList(list.GetRange(6, 8));
            var Circle = new CircleXYZ(startpo, endpo, midpo);
            return Circle;
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0},{1},{2},{3})", new object[4]
            {
            "Center:"+this.Center.ToString(format, provider)+"\n",
            "Radius:"+this.Radius.ToString(format, provider) +"\n",
            "Length:"+this.Length.ToString(format, provider)+"\n",
            "Area:"+this.Area.ToString(format, provider)
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

    [Serializable]
    public class CircleUV : Base, ICurve,IHasArea
    {

        private UV p_center;
        private double d_radius;
        public UV Center => p_center;
        public double Radius => d_radius;
        public double Length => Math.Abs(Math.PI*2 * d_radius);
        public double Area { get => Math.PI * d_radius * d_radius ; }

        public CircleUV()
        {
            p_center = new UV();
            d_radius = 1;
        }

        public CircleUV(UV center, double radius)
        {
            d_radius = radius;
            p_center = center;
        }
        /// <summary>
        /// 用圆弧上三点创建圆弧
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="pointOnCircle"></param>
        public CircleUV(UV startPoint, UV endPoint, UV pointOnCircle)
        {
            LineUV li13 = new LineUV(startPoint, endPoint);
            LineUV li12 = new LineUV(startPoint, pointOnCircle);
            XLineUV vertical13 = li13.GetMiddlePerpendicularXLine();
            XLineUV vertical12 = li12.GetMiddlePerpendicularXLine();
            UV centertemp = vertical12.GetIntersectPoint(vertical13);
            p_center = centertemp;
            d_radius = p_center.DistanceTo(startPoint);

        }

        public CircleUV(UV center, UV pointOnCircle)
        {
            p_center = center;
            d_radius = center.DistanceTo(pointOnCircle);
        }


        /// <summary>
        /// 根据两条切线上的三个点和半径求圆
        /// </summary>
        /// <param name="firstPoint">起点切线上任一点</param>
        /// <param name="intersectPoint">切线交点</param>
        /// <param name="thirdPoint">终点切线上任一点</param>
        /// <param name="radius"></param>
        public CircleUV(UV firstPoint, UV intersectPoint, UV thirdPoint, double radius)
        {
            UV li21 = firstPoint - intersectPoint;
            UV li23 = thirdPoint - intersectPoint;
            double flag = Math.Abs(li23.Angle - (li21.Angle));
            double angli2o;
            if (Math.Abs(flag - Math.PI) < 1E-9 || Math.Abs(flag) < 1E-9)
            {
                throw new Exception("三点共线");
            }
            else
            {
               
                double sitahalf = 0;
                if (flag < Math.PI)
                {
                    angli2o = (li21.Angle + (li23.Angle)) / 2;
                    sitahalf = flag / 2;
                }
                else
                {
                    angli2o = Math.PI + (li21.Angle + (li23.Angle)) / 2;
                    sitahalf = Math.PI - flag / 2;
                }
                double len2o = Math.Abs(radius / Math.Sin(sitahalf));
                UV li2o = new UV(angli2o);
                UV oPoint = intersectPoint + (li2o * len2o);
                p_center = oPoint;
                d_radius = radius;
            }


        }

    
        public bool IsPointOnCurve(UV source)
        {
            bool result = false;

            if (Math.Abs(source.DistanceTo(p_center) - d_radius) < 1E-9)
            {
                result = true;
                return result;
            }


            return result;
        }

        //尚未测试20220419
        public UV GetClosestPoint(UV source)
        {

            UV dir = source - p_center;
            UV Dpoint = p_center + dir.Normalize() * d_radius;
            return Dpoint;
               
            }
        public UV GetPointByAngle(double source)
        {

            UV dir = new UV(source);
            UV Dpoint = p_center + dir.Normalize() * d_radius;
            return Dpoint;

        }
        public double GetAngleByPoint(UV source)
        {  
            UV dir = source-p_center;
            return dir.Angle;

        }

        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0},{1},{2},{3})", new object[4]
            {
            "Center:"+this.Center.ToString(format, provider)+"\n",
            "Radius:"+this.Radius.ToString(format, provider) +"\n",
            "Length:"+this.Length.ToString(format, provider)+"\n",
            "Area:"+this.Area.ToString(format, provider)
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

        public UV Get1sIntersectPointByBaseAndDirection(UV basePoint, UV direction)
        {
            LineUV li = LineUV.CreateByStartPointAndVector(basePoint, direction);
            UV projectPo = li.GetProjectPoint(Center);
            double juli = Center.DistanceTo(projectPo);
            if (juli > Radius)
            {
                return null;
            }
            else
            {
                double len = Math.Sqrt(Radius * Radius - juli * juli);
                UV po1 = projectPo + (direction.Normalize() * len);
                return po1;
            }
        }
        public UV Get2sIntersectPointByBaseAndDirection(UV basePoint, UV direction)
        {
            LineUV li = LineUV.CreateByStartPointAndVector(basePoint, direction);
            UV projectPo = li.GetProjectPoint(Center);
            double juli = Center.DistanceTo(projectPo);
            if (juli > Radius)
            {
                return null;
            }
            else
            {
                double len = Math.Sqrt(Radius * Radius - juli * juli);
                UV po1 = projectPo - direction.Normalize() * len;
                return po1;
            }
        }

        public List<double> ToList()
        {
            var list = new List<double>();

            list.AddRange(Center.ToList());
            list.Add(Radius);
            list.Insert(0, CurveTypeEncoding.Circle);
            list.Insert(0, list.Count);
            return list;
        }

        public static CircleUV FromList(List<double> list)
        {
            UV startpo = UV.FromList(list.GetRange(0, 2));
            double radius = list[3];
            var Circle = new CircleUV(startpo, radius);
            return Circle;
        }
    }

}
