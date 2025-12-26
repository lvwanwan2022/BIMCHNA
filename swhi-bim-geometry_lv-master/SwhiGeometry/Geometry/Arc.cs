
using Lv.BIM.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lv.BIM.Geometry
{
    [Serializable]
    public class ArcXYZ : Base, ICurveXYZ,ITransformable<ArcXYZ>,ITransformable
    {
        private PlaneXYZ p_plane;
        private XYZ p_center;
        private double d_radius;
        /// <summary>
        /// 相对于plane的X轴逆时针转过的角度的弧度值
        /// </summary>
        private double start_angle;
        private double end_angle;
        /// <summary>
        /// 是否顺时针
        /// </summary>
        private bool clock_wise;
        public XYZ Normal => clock_wise ? -p_plane.Normal : p_plane.Normal;
        public XYZ StartRadiusVector => (p_plane.ConvertVectorUVToXYZ(new UV(start_angle))).Normalize();
        public XYZ StartTangentVector => StartRadiusVector.RotateByAxisAndDegree(Normal, Math.PI / 2);
        public XYZ TangentAtStart => StartTangentVector;
        public XYZ EndRadiusVector =>( p_plane.ConvertVectorUVToXYZ(new UV(end_angle))).Normalize();
        public XYZ EndTangentVector => EndRadiusVector.RotateByAxisAndDegree(Normal, Math.PI / 2);
        public XYZ TangentAtEnd => EndTangentVector;
        public XYZ StartPoint => p_center+ (StartRadiusVector*d_radius);
        public XYZ EndPoint => p_center+ (EndRadiusVector * d_radius);
        public Interval AngelInterval => new Interval(start_angle, end_angle);
        /// <summary>
        /// 起点到终点转过的角度，×××①均为正值。②逆时针为正值，顺时针为负值×××。
        /// </summary>
        public double AngleRadians => clock_wise ? (start_angle - end_angle).RadiansTo0_2PI() : (end_angle - start_angle).RadiansTo0_2PI();
        /// <summary>
        /// 圆弧中点.
        /// </summary>
        public XYZ MidPoint => p_center + (StartRadiusVector.RotateByAxisAndDegree(Normal, AngleRadians / 2)*d_radius);

        public XYZ Center => p_center;
        public PlaneXYZ Plane => p_plane;

        public double Length => Math.Abs(AngleRadians * d_radius);
        public double StartAngle => start_angle.RadiansTo0_2PI();
        public double StartAngleDegree => start_angle*180/Math.PI;
        public double EndAngle => end_angle.RadiansTo0_2PI();
        public double EndAngleDegree => end_angle * 180 / Math.PI;
        public bool ClockWise => clock_wise;
        public double Radius=>d_radius;

        public ArcXYZ()
        {
            p_plane = new PlaneXYZ();
            p_center = new XYZ();
            d_radius = 1;
            start_angle = Math.PI;
            end_angle = 0;
            clock_wise = true;
            GenerateId();
        }
        /// <summary>
        /// 需定义圆弧平面、圆心、半径、起点角度、终点角度和圆弧方向
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="clockWise"></param>
        public ArcXYZ(PlaneXYZ plane, XYZ center, double radius, double startAngle, double endAngle, bool clockWise)
        {
            p_plane = plane;
            d_radius = radius;
            start_angle = startAngle;
            end_angle = endAngle ;
            p_center =center;
            clock_wise = clockWise;
            GenerateId();
        }
        /// <summary>
        /// 用圆弧上三点创建圆弧
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="pointOnArc"></param>
        public ArcXYZ(XYZ startPoint, XYZ endPoint, XYZ pointOnArc)
        {
            XPlane xp = XPlane.CreateBy3Point(startPoint, endPoint, pointOnArc);
            LineXYZ li13 = new LineXYZ(startPoint, endPoint);
            LineXYZ li12 = new LineXYZ(startPoint, pointOnArc);
            XLineXYZ vertical13 = li13.GetMiddlePerpendicularXLineOnPlane(xp);
            XLineXYZ vertical12 = li12.GetMiddlePerpendicularXLineOnPlane(xp);
            XYZ centertemp = vertical12.GetIntersectPoint(vertical13);
            p_center = centertemp;
            if (Math.Abs(startPoint.Z-endPoint.Z)<1E-9 && Math.Abs(endPoint.Z- pointOnArc.Z) < 1E-9)
            {
                p_plane = new PlaneXYZ(new XYZ(0, 0, endPoint.Z), new XYZ(1, 0, 0), new XYZ(0, 1, 0));
            }
            else if(Math.Abs(startPoint.Y - endPoint.Y) < 1E-9 && Math.Abs(endPoint.Y - pointOnArc.Y) < 1E-9)
                {
                    p_plane = new PlaneXYZ(new XYZ(0, endPoint.Y, 0), new XYZ(1, 0, 0), new XYZ(0, 0, 1));
                }
            else if (Math.Abs(startPoint.X - endPoint.X) < 1E-9 && Math.Abs(endPoint.X - pointOnArc.X) < 1E-9)
            {
                p_plane = new PlaneXYZ(new XYZ( endPoint.X, 0,0), new XYZ(0, 1, 0), new XYZ(0, 0, 1));
            }
            else
            {
                p_plane = PlaneXYZ.CreateBy3Point(p_center, endPoint, startPoint);
            }

            d_radius = p_center.DistanceTo(startPoint);
            start_angle = (p_plane.Xaxis.AngleOnPlaneTo((startPoint - centertemp), p_plane.Normal)).RadiansTo0_2PI();
            end_angle = (p_plane.Xaxis.AngleOnPlaneTo((endPoint - centertemp), p_plane.Normal)).RadiansTo0_2PI();
            double poangle = (p_plane.Xaxis.AngleOnPlaneTo((pointOnArc - centertemp), p_plane.Normal)).RadiansTo0_2PI();
            Interval anin = new Interval(start_angle, end_angle);
            if(anin.IsValueInInterval(poangle) )
            {
                if (anin.IsForward())
                {
                    clock_wise = false;
                }
                else
                {
                    clock_wise = true;
                }

            }
            else
            {
                if (!anin.IsForward())
                {
                    clock_wise = false;
                }
                else
                {
                    clock_wise = true;
                }
            }
            GenerateId();
        }
        /// <summary>
        /// ☆☆由于圆弧平面为根据三点生成，可能生成的圆弧直觉不符
        /// </summary>
        /// <param name="center"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="clockwise"></param>
        public ArcXYZ(XYZ center, XYZ startPoint, XYZ endPoint,bool clockwise)
        {
            //try 
            //{
            //    p_plane = PlaneXYZ.CreateBy3Point(center, startPoint, endPoint);
            //}
            //catch(Exception e)
            //{
            //    XYZ ptemp =(startPoint - center).PerpendicularVector()+center;
            //    p_plane = PlaneXYZ.CreateBy3Point(center, startPoint, ptemp); 
            //}
            if (Math.Abs(startPoint.Z - endPoint.Z) < 1E-9 && Math.Abs(endPoint.Z - center.Z) < 1E-9)
            {
                p_plane = new PlaneXYZ(new XYZ(0, 0, endPoint.Z), new XYZ(1, 0, 0), new XYZ(0, 1, 0));
            }
            else if (Math.Abs(startPoint.Y - endPoint.Y) < 1E-9 && Math.Abs(endPoint.Y - center.Y) < 1E-9)
            {
                p_plane = new PlaneXYZ(new XYZ(0, endPoint.Y, 0), new XYZ(1, 0, 0), new XYZ(0, 0, 1));
            }
            else if (Math.Abs(startPoint.X - endPoint.X) < 1E-9 && Math.Abs(endPoint.X - center.X) < 1E-9)
            {
                p_plane = new PlaneXYZ(new XYZ(endPoint.X, 0, 0), new XYZ(0, 1, 0), new XYZ(0, 0, 1));
            }
            else
            {
                p_plane = PlaneXYZ.CreateBy3Point(center, endPoint, startPoint);
            }


            p_center = center;
            d_radius = center.DistanceTo(startPoint);
            start_angle = p_plane.Xaxis.AngleOnPlaneTo((startPoint - center),p_plane.Normal);
            end_angle = p_plane.Xaxis.AngleOnPlaneTo((endPoint - center), p_plane.Normal);
            clock_wise = clockwise;
            GenerateId();
        }
        /// <summary>
        /// 需定义圆弧平面、圆心、半径、起点角度、终点角度和圆弧方向
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="center"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="clockwise"></param>
        /// <exception cref="Exception"></exception>
        public ArcXYZ(PlaneXYZ plane, XYZ center, XYZ startPoint, XYZ endPoint, bool clockwise = false)
        {
            if(plane.IsPointOnPlane(center) && plane.IsPointOnPlane(startPoint) && plane.IsPointOnPlane(endPoint))
            {
                p_plane = plane;
                p_center = center;
                d_radius = center.DistanceTo(startPoint);
                start_angle = (plane.Xaxis).AngleOnPlaneTo((startPoint - center), plane.Normal);
                end_angle = (plane.Xaxis).AngleOnPlaneTo((endPoint - center), plane.Normal);                
                clock_wise = clockwise;
                GenerateId();
            }
            else
            {
                throw new Exception("点不在平面上");
            }
            
        }
        protected override void GenerateId()
        {
            id = this.GetType().Name + ":" + this.GetHashCode().ToString();
        }
        public double GetLength()
        {
            return Math.Abs(AngleRadians * d_radius);
        }
        public XYZ GetPointAtDist(double dist)
        {
            XYZ result = p_center + StartRadiusVector.RotateByAxisAndDegree(Normal, dist / d_radius)*d_radius;
            return result;
        }
        public bool IsPointOnCurve(XYZ source)
        {
            bool result = false;
            if (Math.Abs(source.DistanceTo(p_center) - d_radius) < 1E-9)
            {
                if (clock_wise)
                {
                    double angle1 = (source - p_center).AngleTo(EndRadiusVector);
                    double angle2 = (StartRadiusVector).AngleTo(EndRadiusVector);
                    if (angle1 <= angle2)
                    {
                        return true;
                    }

                }
                else
                {
                    double angle1 = (source - p_center).AngleTo(StartRadiusVector);
                    double angle2 = (EndRadiusVector).AngleTo(StartRadiusVector);
                    if (angle1 <= angle2)
                    {
                        return true;
                    }
                }

            }

            return result;
        }
        public double GetDistAtPoint(XYZ source)
        {
            double result = 0;
            bool isOnCurve = this.IsPointOnCurve(source);
            if (!isOnCurve)
            {
                throw new Exception("点不在线上");
            }
            else
            {
                XYZ op = source - p_center;
                double angle = clock_wise ? StartRadiusVector.AngleTo(op) : op.AngleTo(StartRadiusVector);
                result = Math.Abs(angle / AngleRadians);
            }

            return result;
        }
        public XYZ  GetClosestPoint(XYZ source)
        {
            if (!p_plane.IsPointOnPlane(source))
            {
                throw new Exception("点不在平面内");
            }
            XYZ dir = source - p_center;
            XYZ Dpoint = p_center+ dir.Normalize()*d_radius;
            if (clock_wise)
            {
                double angle1 = (source - p_center).AngleTo(EndRadiusVector);
                double angle2 = (StartRadiusVector).AngleTo(EndRadiusVector);
                if (angle1 <= angle2)
                {
                    return Dpoint;
                }
                else if(angle1> angle2 && angle1 <= Math.PI + AngleRadians / 2)
                {
                    return StartPoint;
                }
                else
                {
                    return EndPoint;
                }

            }
            else
            {
                double angle1 = (source - p_center).AngleTo(StartRadiusVector);
                double angle2 = (EndRadiusVector).AngleTo(StartRadiusVector);
                if (angle1 <= angle2)
                {
                    return Dpoint;
                }
                else if (angle1 > angle2 && angle1 <= Math.PI + AngleRadians / 2)
                {
                    return EndPoint;
                }
                else
                {
                    return StartPoint;
                }
            }
        }
        /// <summary>
        /// 点到直线的最近点返回结果仅限于直线段内部，输出点到直线内部点的最小distance
        /// </summary>
        /// <param name="source"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public XYZ GetClosestPointWithinLine(XYZ source, out double distance) 
        {
            double t;
            ClosestPoint(source, out t);
            XYZ po = GetPointAtDist(t);
            distance = po.DistanceTo(source);
            return po;

        }

        /// <summary>
        /// testPoint在线上投影点在线内部，则返回true，并返回距离起始点的长度；否则返回false
        /// </summary>
        /// <param name="testPoint"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool ClosestPoint(XYZ testPoint, out double t) 
        {
            if (!p_plane.IsPointOnPlane(testPoint))
            {
                throw new Exception("点不在平面内");
            }
            XYZ dir = testPoint - p_center;
            XYZ Dpoint = p_center + dir.Normalize() * d_radius;
            if (clock_wise)
            {
                double angle1 = (testPoint - p_center).AngleOnPlaneTo(EndRadiusVector,Normal);
                double angle2 = (StartRadiusVector).AngleOnPlaneTo(EndRadiusVector,Normal);
                if (angle1 <= angle2)
                {
                    t = (1-angle1/angle2)*Length;
                    return true;
                }
                else if (angle1 > angle2 && angle1 <= Math.PI + AngleRadians / 2)
                {
                    t = 0;
                    return false;
                }
                else
                {
                    t = Length;
                    return false;
                }

            }
            else
            {
                double angle1 = (testPoint - p_center).AngleOnPlaneTo(StartRadiusVector,Normal);
                double angle2 = (EndRadiusVector).AngleOnPlaneTo(StartRadiusVector,Normal);
                if (angle1 <= angle2)
                {
                    t = (angle1 / angle2) * Length;
                    return true;
                }
                else if (angle1 > angle2 && angle1 <= Math.PI + AngleRadians / 2)
                {
                    t = Length;
                    return false;
                }
                else
                {
                    t = 0;
                    return false;
                }
            }
        }
        public void Reverse()
        {
            double temp = end_angle;
            end_angle = start_angle;
            start_angle = temp;
            clock_wise = !clock_wise;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDirection"></param>
        /// <param name="endDirection"></param>
        /// <param name="offsetDis">向圆心为负值，向圆外为正值</param>
        /// <returns></returns>
        public ArcXYZ OffsetBy2Direction(XYZ startDirection, XYZ endDirection, double offsetDis)
        {
            
            XYZ midDirection = (Center - MidPoint).Normalize();
            XYZ normal1 = startDirection.CrossProduct(TangentAtStart);
            XYZ normal2 = endDirection.CrossProduct(TangentAtEnd);
            XYZ po1 = StartPoint + startDirection;
            XYZ po2 = EndPoint + endDirection;
            double d1flag = po1.DistanceTo(Center);
            //以下注释代码限制平行偏移
            //if (!normal1.IsParallelTo(normal2))
            //{
            //    throw new Exception("给定偏移方向与圆弧线不在同一平面");
            //}
            //sinθ1
            XYZ startDirectionproject=startDirection;
            XYZ endDirectionproject=endDirection;
            if (!normal1.IsParallelTo(normal2))
            {
                TransformXYZ tranPro = TransformXYZ.PlanarProjection(Plane);                
                startDirection.TransformTo(tranPro, out startDirectionproject);
                endDirection.TransformTo(tranPro, out endDirectionproject);
                startDirectionproject = startDirectionproject - Plane.Origin;
                endDirectionproject = endDirectionproject - Plane.Origin;
            }            
            CircleXYZ circleOff = new CircleXYZ(Plane, Center, Radius+offsetDis);           
            XYZ startinterPo;
            XYZ endinterPo;
            XYZ midinterPo;
            XYZ startOffPo;
            XYZ endOffPo;
            XYZ midOffPo;
            if ((offsetDis > 0 && d1flag>Radius) || (offsetDis < 0 && d1flag > Radius))
            {
                startinterPo = circleOff.Get1sIntersectPointByBaseAndDirection(StartPoint, startDirectionproject);
                endinterPo = circleOff.Get1sIntersectPointByBaseAndDirection(EndPoint, endDirectionproject);
                midinterPo = circleOff.Get1sIntersectPointByBaseAndDirection(MidPoint, midDirection);
            }
            //以下Else代码是有关改变偏移距离正负号的
            else
            {
                startinterPo = circleOff.Get2sIntersectPointByBaseAndDirection(StartPoint, startDirectionproject);
                endinterPo = circleOff.Get2sIntersectPointByBaseAndDirection(EndPoint, endDirectionproject);
                midinterPo = circleOff.Get1sIntersectPointByBaseAndDirection(MidPoint, midDirection);
            }
            double len1 = (startinterPo - StartPoint).Length;
            double len2 = (endinterPo - EndPoint).Length;
            
            startOffPo = StartPoint + ((startDirection.Normalize()) * len1);
            endOffPo= EndPoint + ((endDirection.Normalize()) * len2);
            if (offsetDis < 0)
            {
                startOffPo = StartPoint - ((startDirection.Normalize()) * len1);
                endOffPo = EndPoint - ((endDirection.Normalize()) * len2);
            }
            midOffPo = midinterPo;
            if (startOffPo.IsAlmostEqualTo(endOffPo))
            {
                return null;
            }
            ArcXYZ arctemp = new ArcXYZ(Center, startOffPo, endOffPo,ClockWise);
            //ArcXYZ arctemp = new ArcXYZ(startOffPo, endOffPo,midOffPo);
            //if (clock_wise != arctemp.ClockWise)
            //{
            //    return null;
            //}
            return arctemp;
        }
        public ArcXYZ OffsetByDistance(double offsetDis)
        {
            if (offsetDis < -Radius ||offsetDis==-Radius)
            {
                return null;
            }
            XYZ startOffPo = StartPoint + StartRadiusVector.Normalize() * offsetDis;
            XYZ endOffPo = EndPoint + EndRadiusVector.Normalize() * offsetDis;
            return new ArcXYZ(Center, startOffPo, endOffPo,clock_wise);
        }
        public List<double> ToList()
        {
            var list = new List<double>();

            list.AddRange(StartPoint.ToList());
            list.AddRange(MidPoint.ToList());
            list.AddRange(EndPoint.ToList());
            list.Insert(0, CurveTypeEncoding.Arc);
            list.Insert(0, list.Count);
            return list;
        }

        public static ArcXYZ FromList(List<double> list)
        {
            XYZ startpo = XYZ.FromList(list.GetRange(0, 2));
            XYZ midpo = XYZ.FromList(list.GetRange(3, 5));
            XYZ endpo = XYZ.FromList(list.GetRange(6, 8));
            var arc = new ArcXYZ(startpo, endpo, midpo);
            return arc;
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0},{1},{2},{3},{4},{5},{6},{7})", new object[8]
            {
            "Id:"+this.ID+"\n",
            "Center:"+this.Center.ToString(format, provider)+"\n",
            "Radius:"+this.Radius.ToString(format, provider)+"\n",
            "StartPoint:"+this.StartPoint.ToString(format, provider)+"\n",
            "EndPoint:"+this.EndPoint.ToString(format, provider)+"\n",
            "StartAngle:"+this.StartAngleDegree.ToString(format, provider)+"\n",
            "EndAngle:"+this.EndAngleDegree.ToString(format, provider)+"\n",
           "ClockWise?:"+ this.ClockWise.ToString()
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

        public bool IsLine()
        {
            return false;
        }

        public bool IsArc()
        {
            return true;
        }

        public bool IsPolyline()
        {
            return false;
        }
        public bool IsPolyCurve()
        {
            return false;
        }
        public bool TransformTo(TransformXYZ transform, out ArcXYZ transformed)
        {
            List<XYZ> pos = new List<XYZ>();
            pos.Add(StartPoint);
            pos.Add(EndPoint);
            pos.Add(MidPoint);            
            List<XYZ> postrans = transform.ApplyToPoints(pos);
            transformed = new ArcXYZ(postrans[0], postrans[1], postrans[2]);
            return true;

        }
        public bool TransformTo(TransformXYZ transform, out ITransformable transformed)
        {
            List<XYZ> pos = new List<XYZ>();
            pos.Add(StartPoint);
            pos.Add(EndPoint);
            pos.Add(MidPoint);
            List<XYZ> postrans = transform.ApplyToPoints(pos);
            transformed = new ArcXYZ(postrans[0], postrans[1], postrans[2]);
            return true;

        }

        public XYZ TangentAt(double dist)
        {
            XYZ po = GetPointAtDist(dist);
            XYZ vec = po - Center;
            
            return vec.RotateByAxisAndDegree(Normal, Math.PI / 2); 
        }
    }

    [Serializable]
    public class ArcUV : Base, ICurve,ICurveUV, ITransformable<ArcUV>,ITransformable
    {
        
        private UV p_center;
        private double d_radius;
        /// <summary>
        /// 相对于plane的X轴逆时针转过的角度的弧度值,范围[0,2π]
        /// </summary>
        private double start_angle;
        private double end_angle;
        /// <summary>
        /// 是否顺时针
        /// </summary>
        private bool clock_wise;
        public UV StartRadiusVector => new UV(start_angle);
        public UV StartTangentVector => clock_wise? new UV(start_angle+Math.PI / 2*3):new UV(start_angle+Math.PI / 2);
        public UV EndRadiusVector => new UV(end_angle);
        public UV EndTangentVector => clock_wise ? new UV(end_angle + Math.PI / 2 * 3) : new UV(end_angle + Math.PI / 2);
        public UV StartPoint => p_center + (StartRadiusVector * d_radius);
        public UV EndPoint => p_center + (EndRadiusVector * d_radius);
        public Interval AngelInterval => new Interval(start_angle, end_angle);
        /// <summary>
        /// 起点到终点转过的角度，×××①均为正值。②逆时针为正值，顺时针为负值×××。
        /// </summary>
        public double AngleRadians => clock_wise ? -((start_angle - end_angle).RadiansTo0_2PI()) : (end_angle - start_angle).RadiansTo0_2PI();
        /// <summary>
        /// 圆弧中点.
        /// </summary>
        public UV MidPoint => p_center + (StartRadiusVector.RotateByDegree(AngleRadians / 2) * d_radius);

        public UV Center => p_center;
        public double Radius => d_radius;

        public double Length => Math.Abs(AngleRadians * d_radius);
        public double StartAngle => start_angle.RadiansTo0_2PI();
        public double EndAngle => end_angle.RadiansTo0_2PI();
        public double StartAngleDegree => start_angle * 180 / Math.PI;
        public double EndAngleDegree => end_angle * 180 / Math.PI;
        public bool ClockWise => clock_wise;

        public UV TangentAtStart => throw new NotImplementedException();

        public UV TangentAtEnd => throw new NotImplementedException();

        public ArcUV()
        {
            p_center = new UV();
            d_radius = 1;
            start_angle = Math.PI;
            end_angle = 0;
            clock_wise = true;
            GenerateId();
        }

        public ArcUV(UV center, double radius, double startAngle, double endAngle, bool clockWise)
        {
            d_radius = radius;
            start_angle = startAngle;
            end_angle = endAngle;
            p_center = center;
            clock_wise = clockWise;
            GenerateId();
        }
        /// <summary>
        /// 用圆弧上三点创建圆弧
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="pointOnArc"></param>
        public ArcUV(UV startPoint, UV endPoint, UV pointOnArc)
        {
            LineUV li13 = new LineUV(startPoint, endPoint);
            LineUV li12 = new LineUV(startPoint, pointOnArc);
            XLineUV vertical13 = li13.GetMiddlePerpendicularXLine();
            XLineUV vertical12 = li12.GetMiddlePerpendicularXLine();
            UV centertemp = vertical12.GetIntersectPoint(vertical13);
            p_center = centertemp;
            d_radius = p_center.DistanceTo(startPoint);
            start_angle = (startPoint - centertemp).Angle;
            end_angle = (endPoint - centertemp).Angle;
            double poang = (pointOnArc - centertemp).Angle; 
            Interval angIn = new Interval(start_angle, end_angle);
            if (angIn.IsForward())//起始角度小于终止角度
            {
                if (angIn.IsValueInInterval(poang))//圆上点角度在角度范围内
                {
                    clock_wise = false;
                }
                else
                {
                    clock_wise = true;
                }
               
            }
            else
            {
                if (!angIn.IsValueInInterval(poang))
                {
                    clock_wise = false;
                }
                else
                {
                    clock_wise = true;
                }
            }
            GenerateId();
        }

        public ArcUV(UV center, UV startPoint, UV endPoint, bool clockwise = false)
        {    
            p_center = center;
            d_radius = center.DistanceTo(startPoint);
            start_angle = (startPoint - center).Angle;
            end_angle = (endPoint - center).Angle;
            clock_wise = clockwise;
            GenerateId();
        }

        
        /// <summary>
        /// 根据两条切线上的三个点和半径求圆弧
        /// </summary>
        /// <param name="firstPoint">起点切线上任一点</param>
        /// <param name="intersectPoint">切线交点</param>
        /// <param name="thirdPoint">终点切线上任一点</param>
        /// <param name="radius"></param>
        public ArcUV(UV firstPoint, UV intersectPoint,UV thirdPoint,double radius)
        {
            UV li21 = firstPoint-intersectPoint;
            UV li23 = thirdPoint-intersectPoint;
            double flag = Math.Abs( li23.Angle - (li21.Angle));
            double angli2o;
            if (Math.Abs( flag-Math.PI)<1E-9 || Math.Abs(flag) < 1E-9)
            {
                throw new Exception("三点共线");
            }
            else
            {
                clock_wise = (li23.Angle - (intersectPoint - firstPoint).Angle).RadiansTo0_2PI()>Math.PI;
                if (clock_wise)
                {
                    start_angle = ((intersectPoint - firstPoint).Angle + Math.PI / 2).RadiansTo0_2PI();
                    end_angle = ((thirdPoint - intersectPoint).Angle + Math.PI / 2).RadiansTo0_2PI();
                }
                else
                {
                    start_angle = ((intersectPoint - firstPoint).Angle + Math.PI / 2 * 3).RadiansTo0_2PI();
                    end_angle = ((thirdPoint - intersectPoint).Angle + Math.PI / 2 * 3).RadiansTo0_2PI();
                }
                double sitahalf = 0;
                if (flag < Math.PI)
                {
                    angli2o = (li21.Angle + (li23.Angle)) / 2;
                    sitahalf = flag/2;                   
                }
                else
                {
                    angli2o = Math.PI+(li21.Angle + (li23.Angle)) / 2;
                    sitahalf = Math.PI-flag / 2;                   
                }
                double len2o = Math.Abs( radius / Math.Sin(sitahalf));
                UV li2o = new UV(angli2o);
                UV oPoint = intersectPoint + (li2o * len2o);
                p_center = oPoint;
                d_radius = radius;
                GenerateId();
            }
            

        }

        public UV GetPointAtDist(double dist)
        {
            double roangle = clock_wise ? -dist / d_radius : dist / d_radius;
            UV result = p_center + (new UV(start_angle+ roangle))*d_radius;
            return result;
        }
        public bool IsPointOnCurve(UV source)
        {
            bool result = false;
            double poAng = (source - p_center).Angle;
            double poAngHelper = clock_wise ? -((poAng - end_angle).RadiansTo0_2PI()) : (poAng - start_angle).RadiansTo0_2PI();
            Interval angIn = new Interval(0, AngleRadians);

            if (angIn.IsValueInInterval(poAngHelper))//圆上点角度在角度范围内
            {
                result = true;
            }
            //if (Math.Abs(source.DistanceTo(p_center) - d_radius) < 1E-9)
            //{
            //    if (clock_wise)//顺时针
            //    {
            //        if (angIn.IsBackward() && angIn.IsValueInInterval(poAng))//圆上点角度在角度范围内
            //        {
            //            result = true;
            //            return result;
            //        }
            //        else if(angIn.IsForward() && !angIn.IsValueInInterval(poAng))
            //        {
            //            result = true;
            //            return result;
            //        }
            //    }
            //    else//逆时针
            //    {
            //        if (angIn.IsForward() && angIn.IsValueInInterval(poAng))//圆上点角度在角度范围内
            //        {
            //            result = true;
            //            return result;
            //        }
            //        else if (angIn.IsBackward() && !angIn.IsValueInInterval(poAng))
            //        {
            //            result = true;
            //            return result;
            //        }

            //    }

            return result;
        }

        public double GetDistAtPoint(UV source)
        {
            double result = 0;
            bool isOnCurve = this.IsPointOnCurve(source);
            if (!isOnCurve)
            {
                throw new Exception("点不在线上");
            }
            else
            {
                double angop = (source - p_center).Angle;
                double angle = clock_wise ? (start_angle - angop).RadiansTo0_2PI() : (angop - start_angle).RadiansTo0_2PI();
                result = Math.Abs(angle * d_radius);
            }

            return result;
        }
        //尚未测试20220419
        public UV GetClosestPoint(UV source)
        {
          
            UV dir = source - p_center;
            UV Dpoint = p_center + dir.Normalize() * d_radius;
            if (clock_wise)
            {
                double angle1 = (source - p_center).AngleTo(EndRadiusVector);
                double angle2 = (StartRadiusVector).AngleTo(EndRadiusVector);
                if (angle1 <= angle2)
                {
                    return Dpoint;
                }
                else if (angle1 > angle2 && angle1 <= Math.PI + AngleRadians / 2)
                {
                    return StartPoint;
                }
                else
                {
                    return EndPoint;
                }

            }
            else
            {
                double angle1 = (source - p_center).AngleTo(StartRadiusVector);
                double angle2 = (EndRadiusVector).AngleTo(StartRadiusVector);
                if (angle1 <= angle2)
                {
                    return Dpoint;
                }
                else if (angle1 > angle2 && angle1 <= Math.PI + AngleRadians / 2)
                {
                    return EndPoint;
                }
                else
                {
                    return StartPoint;
                }
            }
        }
        public void Reverse()
        {
            double temp = end_angle;
            end_angle = start_angle;
            start_angle = temp;
            clock_wise = !clock_wise;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDirection"></param>
        /// <param name="endDirection"></param>
        /// <param name="offsetDis">向圆心为负值，向圆外为正值</param>
        /// <returns></returns>
        public ArcUV OffsetBy2Direction(UV startDirection, UV endDirection, double offsetDis)
        {

            UV midDirection = (Center - MidPoint).Normalize();
            double normal1 = startDirection.CrossProduct(TangentAtStart);
            double normal2 = endDirection.CrossProduct(TangentAtEnd);
            UV po1 = StartPoint + startDirection;
            UV po2 = EndPoint + endDirection;
            double d1flag = po1.DistanceTo(Center);
            //以下注释代码限制平行偏移
            //if (!normal1.IsParallelTo(normal2))
            //{
            //    throw new Exception("给定偏移方向与圆弧线不在同一平面");
            //}
            //sinθ1
            UV startDirectionproject = startDirection;
            UV endDirectionproject = endDirection;
            
            CircleUV circleOff = new CircleUV(Center, Radius + offsetDis);
            UV startinterPo;
            UV endinterPo;
            UV midinterPo;
            UV startOffPo;
            UV endOffPo;
            UV midOffPo;
            if ((offsetDis > 0 && d1flag > Radius) || (offsetDis < 0 && d1flag > Radius))
            {
                startinterPo = circleOff.Get1sIntersectPointByBaseAndDirection(StartPoint, startDirectionproject);
                endinterPo = circleOff.Get1sIntersectPointByBaseAndDirection(EndPoint, endDirectionproject);
                midinterPo = circleOff.Get1sIntersectPointByBaseAndDirection(MidPoint, midDirection);
            }
            //以下Else代码是有关改变偏移距离正负号的
            else
            {
                startinterPo = circleOff.Get2sIntersectPointByBaseAndDirection(StartPoint, startDirectionproject);
                endinterPo = circleOff.Get2sIntersectPointByBaseAndDirection(EndPoint, endDirectionproject);
                midinterPo = circleOff.Get1sIntersectPointByBaseAndDirection(MidPoint, midDirection);
            }
            double len1 = (startinterPo - StartPoint).Length;
            double len2 = (endinterPo - EndPoint).Length;

            startOffPo = StartPoint + ((startDirection.Normalize()) * len1);
            endOffPo = EndPoint + ((endDirection.Normalize()) * len2);
            if (offsetDis < 0)
            {
                startOffPo = StartPoint - ((startDirection.Normalize()) * len1);
                endOffPo = EndPoint - ((endDirection.Normalize()) * len2);
            }
            midOffPo = midinterPo;
            if (startOffPo.IsAlmostEqualTo(endOffPo))
            {
                return null;
            }
            ArcUV arctemp = new ArcUV(Center, startOffPo, endOffPo, ClockWise);
            //ArcUV arctemp = new ArcUV(startOffPo, endOffPo,midOffPo);
            //if (clock_wise != arctemp.ClockWise)
            //{
            //    return null;
            //}
            return arctemp;
        }
        public ArcUV OffsetByDistance(double offsetDis)
        {
            if (offsetDis < -Radius || offsetDis == -Radius)
            {
                return null;
            }
            UV startOffPo = StartPoint + StartRadiusVector.Normalize() * offsetDis;
            UV endOffPo = EndPoint + EndRadiusVector.Normalize() * offsetDis;
            return new ArcUV(Center, startOffPo, endOffPo, clock_wise);
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0},{1},{2},{3},{4},{5},{6},{7})", new object[8]
            {
            "Id:"+this.ID+"\n",
            "Center:"+this.Center.ToString(format, provider)+"\n",
            "Radius:"+this.Radius.ToString(format, provider)+"\n",
            "StartPoint:"+this.StartPoint.ToString(format, provider)+"\n",
            "EndPoint:"+this.EndPoint.ToString(format, provider)+"\n",
            "StartAngle:"+this.StartAngleDegree.ToString(format, provider)+"\n",
            "EndAngle:"+this.EndAngleDegree.ToString(format, provider)+"\n",
           "ClockWise?:"+ this.ClockWise.ToString()
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
        public List<double> ToList()
        {
            var list = new List<double>();

            list.AddRange(StartPoint.ToList());
            list.AddRange(MidPoint.ToList());
            list.AddRange(EndPoint.ToList());
            list.Insert(0, CurveTypeEncoding.Arc);
            list.Insert(0, list.Count);
            return list;
        }

        public static ArcUV FromList(List<double> list)
        {
            UV startpo = UV.FromList(list.GetRange(0, 2));
            UV midpo = UV.FromList(list.GetRange(3, 5));
            UV endpo = UV.FromList(list.GetRange(6, 8));
            var arc = new ArcUV(startpo, endpo, midpo);
            return arc;
        }

        public bool TransformTo(TransformXYZ transform, out ArcUV transformed)
        {
            List<UV> pos = new List<UV>();
            pos.Add(StartPoint);
            pos.Add(EndPoint);
            pos.Add(MidPoint);
            List<UV> postrans = transform.ApplyToPoints(pos);
            transformed = new ArcUV(postrans[0], postrans[1], postrans[2]);
            return true;
        }
        public bool TransformTo(TransformXYZ transform, out ITransformable transformed)
        {
            List<UV> pos = new List<UV>();
            pos.Add(StartPoint);
            pos.Add(EndPoint);
            pos.Add(MidPoint);
            List<UV> postrans = transform.ApplyToPoints(pos);
            transformed = new ArcUV(postrans[0], postrans[1], postrans[2]);
            return true;
        }

        public bool IsLine()
        {
            return false;
        }

        public bool IsArc()
        {
            return true;
        }

        public bool IsPolyline()
        {
            return false;
        }

        public bool IsPolyCurve()
        {
            return false;
        }

        /// <summary>
        /// 点到直线的最近点返回结果仅限于直线段内部，输出点到直线内部点的最小distance
        /// </summary>
        /// <param name="source"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public UV GetClosestPointWithinLine(UV source, out double distance)
        {
            double t;
            ClosestPoint(source, out t);
            UV po = GetPointAtDist(t);
            distance = po.DistanceTo(source);
            return po;

        }

        /// <summary>
        /// testPoint在线上投影点在线内部，则返回true，并返回距离起始点的长度；否则返回false
        /// </summary>
        /// <param name="testPoint"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool ClosestPoint(UV testPoint, out double t)
        {
           
            UV dir = testPoint - p_center;
            UV Dpoint = p_center + dir.Normalize() * d_radius;
            if (clock_wise)
            {
                double angle1 = (testPoint - p_center).AngleTo(EndRadiusVector);
                double angle2 = (StartRadiusVector).AngleTo(EndRadiusVector);
                if (angle1 <= angle2)
                {
                    t = (1 - angle1 / angle2) * Length;
                    return true;
                }
                else if (angle1 > angle2 && angle1 <= Math.PI + AngleRadians / 2)
                {
                    t = 0;
                    return false;
                }
                else
                {
                    t = Length;
                    return false;
                }

            }
            else
            {
                double angle1 = (testPoint - p_center).AngleTo(StartRadiusVector);
                double angle2 = (EndRadiusVector).AngleTo(StartRadiusVector);
                if (angle1 <= angle2)
                {
                    t = (angle1 / angle2) * Length;
                    return true;
                }
                else if (angle1 > angle2 && angle1 <= Math.PI + AngleRadians / 2)
                {
                    t = Length;
                    return false;
                }
                else
                {
                    t = 0;
                    return false;
                }
            }
        }
    }
    [Serializable]
    public class ArcRS : Base, ICurve
    {

        private RS p_center;
        private double d_radius;
        /// <summary>
        /// 相对于plane的X轴逆时针转过的角度的弧度值,范围[0,2π]
        /// </summary>
        private double start_angle;
        private double end_angle;
        /// <summary>
        /// 是否顺时针
        /// </summary>
        private bool clock_wise;
        public RS StartRadiusVector => new RS(start_angle);
        public RS StartTangentVector => clock_wise ? new RS(start_angle + Math.PI / 2 * 3) : new RS(start_angle + Math.PI / 2);
        public RS EndRadiusVector => new RS(end_angle);
        public RS EndTangentVector => clock_wise ? new RS(end_angle + Math.PI / 2 * 3) : new RS(end_angle + Math.PI / 2);
        public RS StartPoint => p_center + (StartRadiusVector * d_radius);
        public RS EndPoint => p_center + (EndRadiusVector * d_radius);
        public Interval AngelInterval => new Interval(start_angle, end_angle);
        /// <summary>
        /// 起点到终点转过的角度，×××①均为正值。②逆时针为正值，顺时针为负值×××。
        /// </summary>
        public double AngleRadians => clock_wise ? -((start_angle -end_angle).RadiansTo0_2PI()) : (end_angle - start_angle).RadiansTo0_2PI();
        /// <summary>
        /// 圆弧中点.
        /// </summary>
        public RS MidPoint => p_center + (StartRadiusVector.RotateByDegree(AngleRadians / 2) * d_radius);

        public RS Center => p_center;
        public double Radius => d_radius;

        public double Length => Math.Abs(AngleRadians * d_radius);
        public double StartAngle => start_angle. RadiansTo0_2PI();
        public double EndAngle => end_angle.RadiansTo0_2PI();
        public double StartAngleDegree => start_angle * 180 / Math.PI;
        public double EndAngleDegree => end_angle * 180 / Math.PI;
        public bool ClockWise => clock_wise;


        public ArcRS()
        {
            p_center = new RS();
            d_radius = 1;
            start_angle = Math.PI;
            end_angle = 0;
            clock_wise = true;
            GenerateId();
        }

        public ArcRS(RS center, double radius, double startAngle, double endAngle, bool clockWise)
        {
            d_radius = radius;
            start_angle = startAngle;
            end_angle = endAngle;
            p_center = center;
            clock_wise = clockWise;
            GenerateId();
        }
        /// <summary>
        /// 用圆弧上三点创建圆弧
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="pointOnArc"></param>
        public ArcRS(RS startPoint, RS endPoint, RS pointOnArc)
        {
            UV start = startPoint.ConvertToUV();
            UV end = endPoint.ConvertToUV();
            UV po = pointOnArc.ConvertToUV();
            LineUV li13 = new LineUV(start, end);
            LineUV li12 = new LineUV(start, po);
            XLineUV vertical13 = li13.GetMiddlePerpendicularXLine();
            XLineUV vertical12 = li12.GetMiddlePerpendicularXLine();
            RS centertemp = (vertical12.GetIntersectPoint(vertical13)).ConvertToRS();
            p_center = centertemp;
            d_radius = p_center.DistanceTo(startPoint);
            start_angle = (startPoint - centertemp).Angle;
            end_angle = (endPoint - centertemp).Angle;
            GenerateId();
            double poang = (pointOnArc - centertemp).Angle;
            Interval angIn = new Interval(start_angle, end_angle);
            if (angIn.IsForward())//起始角度小于终止角度
            {
                if (angIn.IsValueInInterval(poang))//圆上点角度在角度范围内
                {
                    clock_wise = false;
                }
                else
                {
                    clock_wise = true;
                }

            }
            else
            {
                if (!angIn.IsValueInInterval(poang))
                {
                    clock_wise = false;
                }
                else
                {
                    clock_wise = true;
                }
            }

        }

        public ArcRS(RS center, RS startPoint, RS endPoint, bool clockwise = false)
        {
            p_center = center;
            d_radius = center.DistanceTo(startPoint);
            start_angle = (startPoint - center).Angle;
            end_angle = (endPoint - center).Angle;
            clock_wise = clockwise;
            GenerateId();
        }


        /// <summary>
        /// 根据两条切线上的三个点和半径求圆弧
        /// </summary>
        /// <param name="firstPoint">起点切线上任一点</param>
        /// <param name="intersectPoint">切线交点</param>
        /// <param name="thirdPoint">终点切线上任一点</param>
        /// <param name="radius"></param>
        public ArcRS(RS firstPoint, RS intersectPoint, RS thirdPoint, double radius)
        {
            RS li21 = firstPoint - intersectPoint;
            RS li23 = thirdPoint - intersectPoint;
            double flag = Math.Abs(li23.Angle - (li21.Angle));
            double angli2o;
            if (Math.Abs(flag - Math.PI) < 1E-9 || Math.Abs(flag) < 1E-9)
            {
                throw new Exception("三点共线");
            }
            else
            {
                clock_wise = (li23.Angle - (intersectPoint - firstPoint).Angle).RadiansTo0_2PI() > Math.PI;
                if (clock_wise)
                {
                    start_angle = ((intersectPoint - firstPoint).Angle + Math.PI / 2).RadiansTo0_2PI();
                    end_angle = ((thirdPoint - intersectPoint).Angle + Math.PI / 2).RadiansTo0_2PI();
                }
                else
                {
                    start_angle = ((intersectPoint - firstPoint).Angle + Math.PI / 2 * 3).RadiansTo0_2PI();
                    end_angle = ((thirdPoint - intersectPoint).Angle + Math.PI / 2 * 3).RadiansTo0_2PI();
                }
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
                RS li2o = new RS(angli2o);
                RS oPoint = intersectPoint + (li2o * len2o);
                p_center = oPoint;
                GenerateId();
            }


        }

        public RS GetPointAtDist(double dist)
        {
            double roangle = clock_wise ? -dist / d_radius : dist / d_radius;
            RS result = p_center + (new RS(start_angle + roangle)) * d_radius;
            return result;
        }
        public bool IsPointOnCurve(RS source)
        {
            bool result = false;
            double poAng = (source - p_center).Angle;
            double poAngHelper = clock_wise ? -((poAng - end_angle).RadiansTo0_2PI()) : (poAng - start_angle).RadiansTo0_2PI();
            Interval angIn = new Interval(0, AngleRadians);

            if (angIn.IsValueInInterval(poAngHelper))//圆上点角度在角度范围内
            {
                result = true;
            }
            //if (Math.Abs(source.DistanceTo(p_center) - d_radius) < 1E-9)
            //{
            //    if (clock_wise)//顺时针
            //    {
            //        if (angIn.IsBackward() && angIn.IsValueInInterval(poAng))//圆上点角度在角度范围内
            //        {
            //            result = true;
            //            return result;
            //        }
            //        else if(angIn.IsForward() && !angIn.IsValueInInterval(poAng))
            //        {
            //            result = true;
            //            return result;
            //        }
            //    }
            //    else//逆时针
            //    {
            //        if (angIn.IsForward() && angIn.IsValueInInterval(poAng))//圆上点角度在角度范围内
            //        {
            //            result = true;
            //            return result;
            //        }
            //        else if (angIn.IsBackward() && !angIn.IsValueInInterval(poAng))
            //        {
            //            result = true;
            //            return result;
            //        }

            //    }

            return result;
        }

        public double GetDistAtPoint(RS source)
        {
            double result = 0;
            bool isOnCurve = this.IsPointOnCurve(source);
            if (!isOnCurve)
            {
                throw new Exception("点不在线上");
            }
            else
            {
                double angop = (source - p_center).Angle;
                double angle = clock_wise ? (start_angle - angop).RadiansTo0_2PI() : (angop - start_angle).RadiansTo0_2PI();
                result = Math.Abs(angle * d_radius);
            }

            return result;
        }
        //尚未测试20220419
        public RS GetClosestPoint(RS source)
        {

            RS dir = source - p_center;
            RS Dpoint = p_center + dir.Normalize() * d_radius;
            if (clock_wise)
            {
                double angle1 = (source - p_center).AngleTo(EndRadiusVector);
                double angle2 = (StartRadiusVector).AngleTo(EndRadiusVector);
                if (angle1 <= angle2)
                {
                    return Dpoint;
                }
                else if (angle1 > angle2 && angle1 <= Math.PI + AngleRadians / 2)
                {
                    return StartPoint;
                }
                else
                {
                    return EndPoint;
                }

            }
            else
            {
                double angle1 = (source - p_center).AngleTo(StartRadiusVector);
                double angle2 = (EndRadiusVector).AngleTo(StartRadiusVector);
                if (angle1 <= angle2)
                {
                    return Dpoint;
                }
                else if (angle1 > angle2 && angle1 <= Math.PI + AngleRadians / 2)
                {
                    return EndPoint;
                }
                else
                {
                    return StartPoint;
                }
            }
        }


        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0},{1},{2},{3},{4},{5},{6})", new object[7]
            {
            "Center:"+this.Center.ToString(format, provider)+"\n",
            "Radius:"+this.Radius.ToString(format, provider)+"\n",
            "StartPoint:"+this.StartPoint.ToString(format, provider)+"\n",
            "EndPoint:"+this.EndPoint.ToString(format, provider)+"\n",
            "StartAngle:"+this.StartAngleDegree.ToString(format, provider)+"\n",
            "EndAngle:"+this.EndAngleDegree.ToString(format, provider)+"\n",
           "ClockWise?:"+ this.ClockWise.ToString()
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
        public List<double> ToList()
        {
            var list = new List<double>();

            list.AddRange(StartPoint.ToList());
            list.AddRange(MidPoint.ToList());
            list.AddRange(EndPoint.ToList());
            list.Insert(0, CurveTypeEncoding.Arc);
            list.Insert(0, list.Count);
            return list;
        }

        public static ArcRS FromList(List<double> list)
        {
            RS startpo = RS.FromList(list.GetRange(0, 2));
            RS midpo = RS.FromList(list.GetRange(3, 5));
            RS endpo = RS.FromList(list.GetRange(6, 8));
            var arc = new ArcRS(startpo, endpo, midpo);
            return arc;
        }
    }

}
