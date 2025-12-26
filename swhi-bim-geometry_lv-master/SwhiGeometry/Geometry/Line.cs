using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
    [Serializable]
    public class LineXYZ:Base, ITransformable<LineXYZ>,ICurveXYZ,ITransformable
    {
        
        private XYZ p_start;
        private XYZ p_end;
        public XYZ StartPoint => p_start;
        public XYZ EndPoint => p_end;
        public XYZ MiddlePoint => p_start+ (p_end.Subtract(p_start))/2;
        public XYZ Direction=>(p_end.Subtract(p_start)).Normalize();
        public XYZ TangentAtStart => Direction;
        public XYZ TangentAtEnd=> Direction;
        public XYZ Vector => p_end.Subtract(p_start);
        public XYZ Origin=>p_start;
        public double Length => (p_end.Subtract(p_start)).GetLength();
        protected override void GenerateId()
        {
            this.id = this.GetType().Name + ":" + this.GetHashCode().ToString();
        }
        public XYZ this[int idx] => idx switch
        {
            1 => p_end,
            0 => p_start,
            _ => throw new Exception("索引错误"),
        };
       public LineXYZ() { }
        public LineXYZ(XYZ startPoint, XYZ endPoint)
        {
            if (startPoint.IsAlmostEqualTo(endPoint))
            {
                throw new Exception("起始点与终点不能是同一个点");
            }
            p_start= startPoint;
            p_end = endPoint;
        }
        public static LineXYZ CreateByStartPointAndVector(XYZ startPoint, XYZ Vector)
        {
            if (Vector.IsAlmostEqualTo(new XYZ()))
            {
                throw new Exception("向量不能为零向量");
            }
            XYZ p_s = startPoint;
           XYZ p_e = startPoint + Vector;
            return new LineXYZ(p_s,p_e);
        }
        public static LineXYZ CreateByStartPointAndEndPoint(XYZ startPoint, XYZ endPoint)
        {
            if (startPoint.IsAlmostEqualTo(endPoint))
            {
                throw new Exception("起始点与终点不能是同一个点");
            }
            XYZ p_s = startPoint;
            XYZ p_e = endPoint;
            return new LineXYZ(p_s, p_e);
        }
        public double GetLength()
        {
            return (p_end.Subtract(p_start)).GetLength();
        }
        //以下带change字段的函数需慎用，均会改变原直线属性
        public void ChangeLengthFixStart(double len)
        {
            if (len <= 1E-9)
            {
                throw new Exception("小值错误");
            }
            //XYZ p_start1 = p_start ;
            p_end = p_start + Direction * len;
            //return new LineC(p_start1, p_end1);
        }
        public void ChangeLengthFixEnd(double len)
        {
            if (len <= 1E-9)
            {
                throw new Exception("小值错误");
            }
            //XYZ p_start1 = p_start ;
            p_start = p_end + Direction * len;
            //return new LineC(p_start1, p_end1);
        }
        public void ChangeDirectionFixStart(XYZ dir)
        {
            if (dir.IsUnitLength())
            {
                //XYZ p_start1 = p_start;
                p_end = p_start + dir * Length;
               // return new LineC(p_start1, p_end1);
            }
            else
            {
                throw new Exception("方向向量不是单位向量");
            }
        }
        public void ChangeDirectionFixEnd(XYZ dir)
        {
            if (dir.IsUnitLength())
            {
                //XYZ p_start1 = p_start;
                p_start = p_end + dir * Length;
                // return new LineC(p_start1, p_end1);
            }
            else
            {
                throw new Exception("方向向量不是单位向量");
            }
        }
        public void ChangeStartPoint(XYZ start)
        {
            if (start.IsAlmostEqualTo(p_end))
            {
                throw new Exception("起始点与终点不能是同一个点");
            }
                p_start = start;      
        }
        public void ChangeEndPoint(XYZ end)
        {
            if (end.IsAlmostEqualTo(p_start))
            {
                throw new Exception("终点与起点不能是同一个点");
            }
            p_end = end;
        }
        public void ChangeLocation(XYZ vector)
        {
            p_start = p_start + vector;
            p_end = p_end + vector;
        }
        public LineXYZ Translation(XYZ vector)
        {
           XYZ p_start1 = p_start + vector;
           XYZ p_end1 = p_end + vector;
            return new LineXYZ(p_start1, p_end1);
        }
        public LineXYZ RotateByAxisAndDegree(XYZ axis, double degreeRadian)
        {
            //20220418lvwan取消角度限制
            //if(degreeRadian <= Math.PI*2 && degreeRadian >= 0)
            //{
                //XYZ zaxis = new XYZ(0,0,1);
                XYZ p_start1 = p_start.RotateByAxisAndDegree(axis,degreeRadian);
                XYZ p_end1=p_end.RotateByAxisAndDegree(axis, degreeRadian);
                return new LineXYZ(p_start1, p_end1);
            //}
            //else
            //{
            //    throw new Exception("角度范围不在0~2π内");
            //}
        }
        public void Reverse()
        {
            XYZ temp = p_start;
            p_start = p_end;
            p_end = temp;
        }

        /// <summary>
        /// position为0-1表示点在线内部,返回true；
        /// >1在线末端延长线上，<0表示在起始段延长线上，且返回false；
        /// NAN表示不在线上，且返回false；
        /// </summary>
        /// <param name="source"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsPointOnCurve(XYZ source,out double position)
        {
            bool result = false;
            XYZ st = p_start;
            XYZ ed = p_end;
            XYZ AB = ed-st;
            XYZ Cvector = source - st;
            XYZ CvNor = Cvector.Normalize();
            XYZ zero= new XYZ(0,0,0);
            //XYZ crosProduct = Cvector.CrossProduct(Direction);
            //起点
            if (CvNor.IsAlmostEqualTo(zero) )
            {
                result = true;       
                position = 0;
                return result;
            }
            else if(CvNor.IsAlmostEqualTo(Direction) && Cvector.GetLength() <= Length)//线内部
            {
                result = true;
                position = Cvector.GetLength() / (AB.GetLength());
                return result;
            }
            else if (CvNor.IsAlmostEqualTo(Direction) && Cvector.GetLength() > Length)//线末端延长线上
            {
                result = false;
                position = Cvector.GetLength() /Length;
                return result;
            }
            else if (CvNor.IsParallelTo(Direction))//线起始段延长线
            {
                result = false;
                position = -Cvector.GetLength() / Length;
                return result;
            }
            else//不在线上
            {
                result = false;
                position =double.NaN;
                return result;
            }
        }
        /// <summary>
        /// 点在线内部,返回true；
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool IsPointOnCurve(XYZ source)
        {
            bool result = false;
            XYZ st = p_start;
            XYZ ed = p_end;
            XYZ AB =ed - st;
            XYZ ABnor = AB.Normalize();
            XYZ Cvector = source - st;
            XYZ nor = Cvector.Normalize();
            XYZ zero = new XYZ(0, 0, 0);
            if (nor.IsAlmostEqualTo(zero) || (ABnor.IsAlmostEqualTo(nor) && Cvector.GetLength()<=Length))
            {
                result = true;
            }    
            return result;
        }
        /// <summary>
        /// 点到支线的距离
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public double DistanceTo(XYZ source)
        {
            double result = 0;
            XYZ AO = source - p_start;
            XYZ AB = p_end - p_start;
            result = (AB.CrossProduct(AO).GetLength()) / (AB.GetLength()) ;
            return result;
        }

        public double AngleTo(LineXYZ line)
        {
            XYZ dir1 = Direction;
            XYZ dir2 = line.Direction;
            return dir1.AngleTo(dir2);
        }
        /// <summary>
        /// 点在支线上的投影点
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public XYZ GetClosestPoint(XYZ source)
        {
            XYZ result;
            XYZ ABline = p_end - p_start;
            XYZ AOline = source - p_start;
            XYZ planeNorm = (ABline.CrossProduct(AOline)).Normalize();
            XYZ DOnorm = (ABline.RotateByAxisAndDegree(planeNorm, Math.PI / 2)).Normalize();//
            double lengthDO = DistanceTo(source);
            XYZ DPoint = source - (DOnorm * lengthDO);
            double temp = 0;
            IsPointOnCurve(DPoint, out temp);
            if (double.IsNaN(temp))
            {
                DPoint = source + (DOnorm * lengthDO);
            }
            result = DPoint;
            return result;
        }
        //以下方法为GetClosestXYZ同功能方法，重写方法名，便于识别；
        /// <summary>
        /// 点在支线上的投影点
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public XYZ GetProjectPoint(XYZ source)
        {
            XYZ result;
            XYZ ABline = p_end - p_start;
            XYZ AOline = source - p_start;
            XYZ planeNorm = (ABline.CrossProduct(AOline)).Normalize();
            XYZ DOnorm = (ABline.RotateByAxisAndDegree(planeNorm, Math.PI / 2)).Normalize();//
            double lengthDO = DistanceTo(source);
            XYZ DPoint = source - (DOnorm * lengthDO);
            double temp = 0;
            IsPointOnCurve(DPoint, out temp);
            if (double.IsNaN(temp))
            {
                DPoint = source + (DOnorm * lengthDO);
            }
            result = DPoint;
            return result;
        }
        /// <summary>
        /// 点到直线的最近点返回结果仅限于直线段内部，输出点到直线内部点的最小distance
        /// </summary>
        /// <param name="source"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public XYZ GetClosestPointWithinLine(XYZ source,out double distance)
        {
            XYZ result;

            XYZ DXYZ = GetProjectPoint(source); ;
            result = DXYZ;
            distance = (source - DXYZ).GetLength();
            double position;
           bool flag= IsPointOnCurve(DXYZ, out position);
            //distance = position * Length;
            if (!double.IsNaN(position))
            {
                if (position > 1)
                {
                    result = p_end;
                    distance = (source - p_end).GetLength();
                }
                else if (position < 0)
                {
                    result = p_start;
                    distance = (source - p_start).GetLength();
                }
            }
            return result;
        }
        /// <summary>
        /// testPoint在线上投影点在线内部，则返回true，并返回距离起始点的长度；否则返回false
        /// </summary>
        /// <param name="testPoint"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool ClosestPoint(XYZ testPoint, out double t)
        {
            bool result=false;
            
            XYZ DXYZ = GetProjectPoint(testPoint);
            double position = 0;
            if (IsPointOnCurve(DXYZ, out position))
            {     
                    result = true;
                    t = position*Length;
                return result;
            }
            else if (!double.IsNaN(position))
            {
                if (position > 1)
                {
                    result = false ;
                    t = Length;
                    return result;
                }
                else if (position < 0)
                {
                    result = false;
                    t = 0;
                    return result;
                }
            }
            t = double.NaN;
            return result;
        }
        public XYZ GetPointAtDist(double dist)
        {
            XYZ st = p_start;
            XYZ result = st + Direction * dist;
            return result;
        }
        public double GetDistAtPoint(XYZ po)
        {
            double result=0;
            double position;
            bool isOnCurve = this.IsPointOnCurve(po, out position);
            if(!isOnCurve)
            {
                throw new Exception("点不在线上");
            }
            else
            {
                    result = this.Length * position;
                
            }
            
            return result;
        }
        /// <summary>判断直线1与直线2的关系.</summary>
        /// <remarks>返回结果：
        /// -2=>不相交(即不同平面相交)；
        /// -1=>不相交，但垂直(即不同平面垂直)；
        /// 0=>平行；
        /// 1=>相交，交点在1直线和2直线的内部（包含边界）；
        /// 2=>相交，交点在1直线内部，在2直线延长线上；
        /// 3=>相交，交点在2直线内部，在1直线延长线上；
        /// 4=>相交，交点均在1、2直线延长线上.
        /// </remarks>
        /// //该函数较复杂，建议拆分
        public int RelationWithAnother(LineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //int result = -1;
            throw new Exception("此函数弃用，建议使用其他相关函数");
          }
     


        //平行
        public bool IsParallel(LineXYZ source)
        {
            bool result = false;
            XYZ norm0 = this.Direction;
            XYZ norm1 = source.Direction;
            if (norm0.IsParallelTo(norm1))
            {
                result = true;
            }
            return result;
        }
        //平行
        public bool IsParallel(XLineXYZ source)
        {
            bool result = false;
            XYZ norm0 = this.Direction;
            XYZ norm1 = source.Direction;
            if (norm0.IsParallelTo(norm1))
            {
                result = true;
            }
            return result;
        }
        //垂直
        public bool IsPerpendicular(LineXYZ source)
        {
            bool result = false;
            XYZ norm0 = this.Direction;
            XYZ norm1 = source.Direction;
            if (norm0.IsPerpendicularTo(norm1))
            {
                result = true;
            }
            return result;
        }
        public bool IsOnOnePlane(LineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            bool result = false;
            XYZ dr = Direction;
            XYZ aPoint = p_start;
            XYZ cPoint = source.StartPoint;
            XYZ dPoint = source.EndPoint;
            XYZ acLine = cPoint - aPoint;
            XYZ adLine = dPoint - aPoint;
            XYZ abcNorm = dr.CrossProduct(acLine);
            XYZ dabNorm = adLine.CrossProduct(Direction);
            //共面判断
            if (abcNorm.IsParallelTo(dabNorm))
            {
                result = true;
            }
            return result;
        }
        public bool IsOnOnePlane(XLineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            bool result = false;
            XYZ dr = Direction;
            XYZ aPoint = p_start;
            XYZ cPoint = source.BasePoint;
            XYZ dPoint = source.DirectionPoint;
            XYZ acLine = cPoint - aPoint;
            XYZ adLine = dPoint - aPoint;
            XYZ abcNorm = dr.CrossProduct(acLine);
            XYZ dabNorm = adLine.CrossProduct(Direction);
            //共面判断
            if (abcNorm.IsParallelTo(dabNorm))
            {
                result = true;
            }
            return result;
        }

        public bool IsOnPlane(PlaneXYZ plane)
        {
            bool result = plane.IsPointOnPlane(p_start) && plane.IsPointOnPlane(p_end);
            return result;
        }
        public bool IsOnPlane(XPlane plane)
        {
            bool result = plane.IsPointOnPlane(p_start) && plane.IsPointOnPlane(p_end);
            return result;
        }
        //判断是否相交
        public bool IsIntersectedOnPlaneExtended(LineXYZ source)
        {
            bool result = IsOnOnePlane(source) && !IsParallel(source);
            return result;
        }
        public bool IsIntersectedOnPlaneExtended(XLineXYZ source)
        {
            bool result = IsOnOnePlane(source) && !IsParallel(source);
            return result;
        }

        /// <summary>判断直线1是否跨越直线2，即直线1的AB端点在直线2的两侧.</summary>        
        public bool IsCrossOver(LineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            bool result = false;
            if (IsIntersectedOnPlaneExtended(source))
            {
                XYZ cdLine = source.Direction;
                XYZ aPoint = p_start;
                XYZ bPoint = p_end;
                XYZ cPoint = source.StartPoint;
                XYZ dPoint = source.EndPoint;
                XYZ caLine = aPoint - cPoint;
                XYZ cbLine = bPoint - cPoint;
                XYZ acdNorm = caLine.CrossProduct(cdLine);
                XYZ bcdNorm = cbLine.CrossProduct(cdLine);
                if (acdNorm.DotProduct(bcdNorm) <= 0)
                {
                    result = true;
                }
            }
            
            return result;
        }
        public bool IsCrossOver(XLineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            bool result = false;
            if (IsIntersectedOnPlaneExtended(source))
            {
                XYZ cdLine = source.Direction;
                XYZ aPoint = p_start;
                XYZ bPoint = p_end;
                XYZ cPoint = source.BasePoint;
                XYZ dPoint = source.Direction;
                XYZ caLine = aPoint - cPoint;
                XYZ cbLine = bPoint - cPoint;
                XYZ acdNorm = caLine.CrossProduct(cdLine);
                XYZ bcdNorm = cbLine.CrossProduct(cdLine);
                if (acdNorm.DotProduct(bcdNorm) <= 0)
                {
                    result = true;
                }
            }

            return result;
        }
        /// <summary>判断直线1是否被直线2跨越，即直线2的CD端点在直线1的两侧.</summary>        
        public bool IsBeCrossedOver(LineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            bool result = false;
            if (IsIntersectedOnPlaneExtended(source))
            {
                XYZ abLine = Direction;
                XYZ aXYZ = p_start;
                XYZ bXYZ = p_end;
                XYZ cXYZ = source.StartPoint;
                XYZ dXYZ = source.EndPoint;
                XYZ acLine = cXYZ - aXYZ;
                XYZ adLine = dXYZ - aXYZ;
                XYZ cabNorm = acLine.CrossProduct(abLine);
                XYZ dabNorm = adLine.CrossProduct(abLine);
                //共面判断
                if (cabNorm.DotProduct(dabNorm) <= 0)
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>判断直线1是否与直线2相互跨越，即直线1的AB端点在直线2的两侧，直线2的CD端点在直线1的两侧.</summary>        
        public bool IsCrossedEachOther(LineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            bool result = false;
            if (IsIntersectedOnPlaneExtended(source))
            {
                //共面判断
                if (IsCrossOver(source) && IsBeCrossedOver(source))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>判断直线1是否与直线2相交.</summary>        
        /// 同IsCrossedEachOther功能一致，此函数应会比较常用，所以使用易懂名称重写；
        public bool IsIntersected(LineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            bool result = false;
            if (IsIntersectedOnPlaneExtended(source))
            {
                //共面判断
                if (IsCrossOver(source) && IsBeCrossedOver(source))
                {
                    result = true;
                }
            }
            return result;
        }
        public XYZ GetIntersectPointExtended(LineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //C在直线AB上投影为c，D在直线AB上投影为d；
            //d1=cC.getLength();d2=dD.getLength();
            if (!IsIntersectedOnPlaneExtended(source))
            {
                throw new Exception("直线不共面");
            }
                XYZ CXYZ = source.p_start;
            XYZ DXYZ = source.p_end;
            XYZ cXYZ = GetProjectPoint(CXYZ);
            XYZ dXYZ = GetProjectPoint(DXYZ);
            double d1 = DistanceTo(CXYZ);
            double d2 = DistanceTo(DXYZ);
            XYZ result = (d1 / (d1 + d2)) * dXYZ + (d2 / (d1 + d2)) * cXYZ;
            return result;
        }
        public XYZ GetIntersectPoint(LineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //C在直线AB上投影为c，D在直线AB上投影为d；
            //d1=cC.getLength();d2=dD.getLength();
            if (!IsCrossedEachOther(source))
            {
                throw new Exception("直线不相交");
            }
            XYZ CPoint = source.p_start;
            XYZ DPoint = source.p_end;
            XYZ cPoint = GetProjectPoint(CPoint);
            XYZ dPoint = GetProjectPoint(DPoint);
            //double d1 = DistanceTo(CPoint);
            //double d2 = DistanceTo(DPoint);
            XYZ Cc = cPoint - CPoint;
            XYZ dD = DPoint - dPoint;
            double d1 = Cc.X;
            double d2 = dD.X;
            XYZ result = (d1 / (d1 + d2)) * dPoint + (d2 / (d1 + d2)) * cPoint;
            return result;
        }
        public XYZ GetIntersectPoint(XLineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //C在直线AB上投影为c，D在直线AB上投影为d；
            //d1=cC.getLength();d2=dD.getLength();
            if (!IsCrossOver(source))
            {
                throw new Exception("直线不相交");
            }
            XYZ CPoint = source.BasePoint;
            XYZ DPoint = source.DirectionPoint;
            XYZ cPoint = GetProjectPoint(CPoint);
            XYZ dPoint = GetProjectPoint(DPoint);
            //double d1 = DistanceTo(CPoint);
            //double d2 = DistanceTo(DPoint);
            XYZ Cc = cPoint - CPoint;
            XYZ dD = DPoint - dPoint;
            double d1 = Cc.X;
            double d2 = dD.X;
            XYZ result = (d1 / (d1 + d2)) * dPoint + (d2 / (d1 + d2)) * cPoint;
            return result;
        }
        public bool IsAlmostEqualTo(LineXYZ source)
        {
            if(p_start.IsAlmostEqualTo(source.StartPoint) && p_end.IsAlmostEqualTo(source.EndPoint))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 判断直线是否重合，包括方向相反的重合
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool IsOverLoaded(LineXYZ source)
        {
            LineXYZ reverseLine = source;
            reverseLine.Reverse();
            if (IsAlmostEqualTo(source) || IsAlmostEqualTo(reverseLine))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public XLineXYZ ToXLine()
        {
            return new XLineXYZ(StartPoint, Direction);
        }
        /// <summary>
        /// //line的中垂线
        /// </summary>
        /// <returns></returns>
        public  XLineXYZ GetMiddlePerpendicularXLineOnPlane(PlaneXYZ plane)
        {

            if (IsOnPlane(plane))
            {
                XYZ midp = MiddlePoint;
                XYZ norm = plane.Normal;
                XYZ perpendicularVector = RotateByAxisAndDegree(norm,Math.PI/2).Direction;
                return new XLineXYZ(midp, perpendicularVector);
            }
            else
            {
                throw new Exception("直线不在平面上");
            }
        }
        public XLineXYZ GetMiddlePerpendicularXLineOnPlane(XPlane plane)
        {

            if (IsOnPlane(plane))
            {
                XYZ midp = MiddlePoint;
                XYZ norm = plane.Normal;
                XYZ perpendicularVector = RotateByAxisAndDegree(norm, Math.PI / 2).Direction;
                return new XLineXYZ(midp, perpendicularVector);
            }
            else
            {
                throw new Exception("直线不在平面上");
            }
        }
        /// <summary>
        /// 结果可能为空
        /// </summary>
        /// <param name="startDirection"></param>
        /// <param name="endDirection"></param>
        /// <param name="offsetDistance"></param>
        /// <returns></returns>
        public LineXYZ OffsetBy2Direction(XYZ startDirection,XYZ endDirection,double offsetDistance)
        {
             
            XYZ normal1 = startDirection.CrossProduct(Direction);
            XYZ normal2 = endDirection.CrossProduct(Direction);
            //以下注释代码限制平行偏移
            //if (!normal1.IsParallelTo(normal2))
            //{
            //    throw new Exception("给定偏移方向与直线不在同一平面");
            //}
            //sinθ1
            double sin1 = normal1.Length / startDirection.Length;
            double sin2 = normal2.Length / endDirection.Length;
            double l1 = offsetDistance / sin1;
            double l2 = offsetDistance / sin2;
            XYZ offsetPoint1 = StartPoint + startDirection.Normalize() * l1;
            XYZ offsetPoint2 = EndPoint + endDirection.Normalize() * l2;
            if (offsetPoint1.IsAlmostEqualTo(offsetPoint2))
            {
                return null;
            }
            else
            {
                LineXYZ lineResult = new LineXYZ(offsetPoint1, offsetPoint2);
                return lineResult;
                //以下注释代码限制平行偏移
                //if (lineResult.Direction.IsAlmostEqualTo(Direction))
                //{
                //    return lineResult;
                //}
                //else
                //{
                //    return null;
                //}
            }

        }
        public LineXYZ OffsetByDistance(double offsetDis, PlaneXYZ planeOffset)
        {
            XYZ offdir = Direction.CrossProduct(planeOffset.Normal);
            XYZ startOffPo = StartPoint + offdir.Normalize() * offsetDis;
            XYZ endOffPo=EndPoint + offdir.Normalize() * offsetDis;
            return new LineXYZ(startOffPo, endOffPo);
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0},{1},{2})", new object[3]
            {
            "StartPoint:"+this[0].ToString(format, provider)+"\n",
            "EndPoint:"+this[1].ToString(format, provider)+"\n",
            "Length:"+Length.ToString(format, provider)
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
            list.AddRange(EndPoint.ToList());
            list.Insert(0, CurveTypeEncoding.Line);
            list.Insert(0, list.Count);
            return list;
        }

        public static LineXYZ FromList(List<double> list)
        {
            var startPt = new XYZ(list[2], list[3], list[4]);
            var endPt = new XYZ(list[5], list[6], list[7]);
            var line = new LineXYZ(startPt, endPt);
            return line;
        }

        public bool TransformTo(TransformXYZ transform, out LineXYZ line)
        {
            line = new LineXYZ(transform.ApplyToPoint(StartPoint), transform.ApplyToPoint(EndPoint));
            return true;
        }
        public bool TransformTo(TransformXYZ transform, out ITransformable line)
        {
            line = new LineXYZ(transform.ApplyToPoint(StartPoint), transform.ApplyToPoint(EndPoint));
            return true;
        }

        public bool IsLine()
        {
            return true;
        }

        public bool IsArc()
        {
            return false;
        }

        public bool IsPolyline()
        {
            return false;
        }

        public bool IsPolyCurve()
        {
            return false;
        }

        public XYZ TangentAt(double dist)
        {
            return TangentAtStart;
        }
    }

    //二维直线
    [Serializable]
    public class LineUV : Base,ICurve,ICurveUV, ITransformable<LineUV>,ITransformable,IParameterizable
    {

        private UV p_start;
        private UV p_end;
        private PlaneXYZ base_plane;
        public UV StartPoint => p_start;
        public UV EndPoint => p_end;
        public UV MidPoint => (p_start+p_end)/2;
        public UV Direction => (p_end.Subtract(p_start)).Normalize();
        public UV TangentAtStart => Direction;
        public UV TangentAtEnd => Direction;
        public UV Origin => p_start;
        public double Length => (p_end.Subtract(p_start)).GetLength();
        protected override void GenerateId()
        {
            this.id = this.GetType().Name + ":" + this.GetHashCode().ToString();
        }
        public UV this[int idx] => idx switch
        {
            1 => p_end,
            0 => p_start,
            _ => throw new Exception("索引错误"),
        };

        public LineUV(UV startPoint, UV endPoint)
        {
            if (startPoint.IsAlmostEqualTo(endPoint))
            {
                throw new Exception("起始点与终点不能是同一个点");
            }
            p_start = startPoint;
            p_end = endPoint;
            base_plane = new PlaneXYZ();
        }
        public LineUV(PlaneXYZ baseplane, UV startPoint, UV endPoint)
        {
            if (startPoint.IsAlmostEqualTo(endPoint))
            {
                throw new Exception("起始点与终点不能是同一个点");
            }
            p_start = startPoint;
            p_end = endPoint;
            base_plane = baseplane;
        }
        public static LineUV CreateByStartPointAndVector(UV startPoint, UV Vector)
        {
            if (Vector.IsAlmostEqualTo(new UV()))
            {
                throw new Exception("向量不能为零向量");
            }
            UV p_s = startPoint;
            UV p_e = startPoint + Vector;
            return new LineUV(p_s, p_e);
        }
        public static LineUV CreateByStartPointAndEndPoint(UV startPoint, UV endPoint)
        {
            if (startPoint.IsAlmostEqualTo(endPoint))
            {
                throw new Exception("起始点与终点不能是同一个点");
            }
            UV p_s = startPoint;
            UV p_e = endPoint;
            return new LineUV(p_s, p_e);
        }

        public static LineUV CreateByStartPointAndVector(PlaneXYZ baseplane, UV startPoint, UV Vector)
        {
            if (Vector.IsAlmostEqualTo(new UV()))
            {
                throw new Exception("向量不能为零向量");
            }
            UV p_s = startPoint;
            UV p_e = startPoint + Vector;
            return new LineUV(baseplane,p_s, p_e);
        }
        public static LineUV CreateByStartXYZAndEndXYZ(PlaneXYZ baseplane, UV startPoint, UV endPoint)
        {
            if (startPoint.IsAlmostEqualTo(endPoint))
            {
                throw new Exception("起始点与终点不能是同一个点");
            }
            UV p_s = startPoint;
            UV p_e = endPoint;
            return new LineUV(baseplane, p_s, p_e);
        }
        //以下带change字段的函数需慎用，均会改变原直线属性
    
        public void ChangeLengthFixStart(double len)
        {
            if (len <= 1E-9)
            {
                throw new Exception("小值错误");
            }
            //UV p_start1 = p_start ;
            p_end = p_start + Direction * len;
            //return new LineC(p_start1, p_end1);
        }
        public void ChangeLengthFixEnd(double len)
        {
            if (len <= 1E-9)
            {
                throw new Exception("小值错误");
            }
            //UV p_start1 = p_start ;
            p_start = p_end + Direction * len;
            //return new LineC(p_start1, p_end1);
        }
        public void ChangeDirectionFixStart(UV dir)
        {
            if (dir.IsUnitLength())
            {
                //UV p_start1 = p_start;
                p_end = p_start + dir * Length;
                // return new LineC(p_start1, p_end1);
            }
            else
            {
                throw new Exception("方向向量不是单位向量");
            }
        }
        public void ChangeDirectionFixEnd(UV dir)
        {
            if (dir.IsUnitLength())
            {
                //UV p_start1 = p_start;
                p_start = p_end + dir * Length;
                // return new LineC(p_start1, p_end1);
            }
            else
            {
                throw new Exception("方向向量不是单位向量");
            }
        }
        public void ChangeStartPoint(UV start)
        {
            if (start.IsAlmostEqualTo(p_end))
            {
                throw new Exception("起始点与终点不能是同一个点");
            }
            p_start = start;
        }
        public void ChangeEndPoint(UV end)
        {
            if (end.IsAlmostEqualTo(p_start))
            {
                throw new Exception("终点与起点不能是同一个点");
            }
            p_end = end;
        }
        public void ChangeLocation(UV vector)
        {
            p_start = p_start + vector;
            p_end = p_end + vector;
        }
        public void ChangeBasePlane(PlaneXYZ baseplane)
        {
            base_plane = baseplane;
        }
        public LineUV Translation(UV vector)
        {
            UV p_start1 = p_start + vector;
            UV p_end1 = p_end + vector;
            return new LineUV(p_start1, p_end1);
        }
        public LineUV RotateByOrgin(double degreeRadian)
        {
            //if (degreeRadian <= Math.PI && degreeRadian >= -Math.PI)
            //{
                //UV zaxis = new UV(0,0,1);
                UV p_start1 = p_start.RotateByDegree(degreeRadian);
                UV p_end1 = p_end.RotateByDegree(degreeRadian);
                return new LineUV(p_start1, p_end1);
            //}
            //else
            //{
            //    throw new Exception("角度范围不在-π~π内");
            //}
        }
        public void Reverse()
        {
            UV temp = p_start;
            p_start = p_end;
            p_end = temp;
        }
        public bool IsPointOnCurve(UV source, out double position)
        {
            bool result = false;
            UV st = p_start;
            UV ed = p_end;
            UV AB = ed - st;
            UV Cvector = source - st;
            UV CvNor = Cvector.Normalize();
            UV zero = new UV(0, 0);
            //XYZ crosProduct = Cvector.CrossProduct(Direction);
            //起点
            if (CvNor.IsAlmostEqualTo(zero))
            {
                result = true;
                position = 0;
                return result;
            }
            else if (CvNor.IsAlmostEqualTo(Direction) && Cvector.GetLength() <= Length)//线内部
            {
                result = true;
                position = Cvector.GetLength() / (AB.GetLength());
                return result;
            }
            else if (CvNor.IsAlmostEqualTo(Direction) && Cvector.GetLength() > Length)//线末端延长线上
            {
                result = false;
                position = Cvector.GetLength() / Length;
                return result;
            }
            else if (CvNor.IsParallelTo(Direction))//线起始段延长线
            {
                result = false;
                position = -Cvector.GetLength() / Length;
                return result;
            }
            else//不在线上
            {
                result = false;
                position = double.NaN;
                return result;
            }
        }
        /// <summary>
        /// 点在线内部,返回true；
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool IsPointOnCurve(UV source)
        {
            bool result = false;
            UV st = p_start;
            UV ed = p_end;
            UV AB = ed - st;
            UV ABnor = AB.Normalize();
            UV Cvector = source - st;
            UV nor = Cvector.Normalize();
            UV zero = new UV(0, 0);
            if (nor.IsAlmostEqualTo(zero) || (ABnor.IsAlmostEqualTo(nor) && Cvector.GetLength() <= Length))
            {
                result = true;
            }
            return result;
        }
        public double DistanceTo(UV source)
        {
            double result = 0;
            UV AO = source - p_start;
            UV AB = p_end - p_start;
            result = (AB.CrossProduct(AO)) / (AB.GetLength());
            return result;
        }
        public UV GetClosestPoint(UV source)
        {
            UV result;
            UV ABline = p_end - p_start;
            UV AOline = source - p_start;
            UV DOnorm = (ABline.RotateByDegree(Math.PI / 2)).Normalize();//
            double lengthDO = DistanceTo(source);
            UV DPoint = source - (DOnorm * lengthDO);
            double temp = 0;
            if (!IsPointOnCurve(DPoint, out temp))
            {
                DPoint = source + (DOnorm * lengthDO);
            }
            result = DPoint;
            return result;
        }
        //以下方法为GetClosestXYZ同功能方法，重写方法名，便于识别；
        public UV GetProjectPoint(UV source)
        {
            UV result;
            UV ABline = p_end - p_start;
            UV AOline = source - p_start;
            UV DOnorm = (ABline.RotateByDegree(Math.PI / 2)).Normalize();//
            double lengthDO = DistanceTo(source);
            UV DPoint = source - (DOnorm * lengthDO);
            double temp = 0;
            IsPointOnCurve(DPoint, out temp);
            if (double.IsNaN(temp))
            {
                DPoint = source + (DOnorm * lengthDO);
            }
            result = DPoint;
            return result;

        }
        public UV GetClosestPointWithinLine(UV source, out double distance)
        {
            UV result;
            UV ABline = p_end - p_start;
            UV AOline = source - p_start;
            UV DOnorm = (ABline.RotateByDegree(Math.PI / 2)).Normalize();
            double lengthDO = DistanceTo(source);
            UV DPoint = source - DOnorm * lengthDO;
            result = DPoint;
            distance = (source - DPoint).GetLength();
            double position;
            if (this.IsPointOnCurve(DPoint, out position))
            {
                if (position > 1)
                {
                    result = p_end;
                    distance = (source - p_end).GetLength();
                }
                else if (position < 0)
                {
                    result = p_start;
                    distance = (source - p_start).GetLength();
                }
            }
            return result;
        }

        public UV GetPointAtDist(double dist)
        {
            UV st = p_start;
            UV result = st + Direction * dist;
            return result;
        }
        public double GetDistAtPoint(UV po)
        {
            double result = 0;
            double position;
            bool isOnCurve = this.IsPointOnCurve(po, out position);
            if (!isOnCurve)
            {
                throw new Exception("点不在线上");
            }
            else
            {
                if (position < 0 || position > 1)
                {
                    throw new Exception("点不在线上");
                }
                else
                {
                    result = this.Length * position;
                }

            }

            return result;
        }
        /// <summary>判断直线1与直线2的关系.</summary>
        /// <remarks>返回结果：
        /// -2=>不相交(即不同平面相交)；
        /// -1=>不相交，但垂直(即不同平面垂直)；
        /// 0=>平行；
        /// 1=>相交，交点在1直线和2直线的内部（包含边界）；
        /// 2=>相交，交点在1直线内部，在2直线延长线上；
        /// 3=>相交，交点在2直线内部，在1直线延长线上；
        /// 4=>相交，交点均在1、2直线延长线上.
        /// </remarks>
        /// //该函数较复杂，建议拆分
        public int RelationWithAnother(LineUV source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //int result = -1;
            throw new Exception("此函数弃用，建议使用其他相关函数");
            //UV dr = Direction;
            //UV drSource = source.Direction;
            //UV aXYZ = p_start;
            //UV bXYZ = p_end;
            //UV cXYZ = source.StartXYZ;
            //UV dXYZ = source.EndXYZ;
            //UV acLine = cXYZ - aXYZ;
            //UV adLine = dXYZ - aXYZ;
            //UV abcNorm = dr.CrossProduct(acLine);
            //UV dabNorm = adLine.CrossProduct(Direction);
            ////共面判断
            //if (abcNorm.IsParallelWithAnother(dabNorm))
            //{
            //    //平行
            //    if (dr.IsParallelWithAnother(drSource))
            //    {
            //        result = 0;
            //        goto IL003lv;
            //    }
            //    if (dr.IsPerpendicularWithAnother(drSource))
            //    {

            //    }
            //}
        }



        //平行
        public bool IsParallelTo(LineUV source)
        {
            bool result = false;
            UV norm0 = this.Direction;
            UV norm1 = source.Direction;
            if (norm0.IsParallelTo(norm1))
            {
                result = true;
            }
            return result;
        }
        //垂直
        public bool IsPerpendicular(LineUV source)
        {
            bool result = false;
            UV norm0 = this.Direction;
            UV norm1 = source.Direction;
            if (norm0.IsPerpendicularTo(norm1))
            {
                result = true;
            }
            return result;
        }
   
        /// <summary>判断直线1是否跨越直线2，即直线1的AB端点在直线2的两侧.</summary>        
        public bool IsCrossOver(LineUV source)
        {
            //直线1的端点为AB，直线2的端点为CD
            bool result = false;
        
                UV cdLine = source.Direction;
                UV aPoint = p_start;
                UV bPoint = p_end;
                UV cPoint = source.StartPoint;
                UV dPoint = source.EndPoint;
                UV caLine = aPoint - cPoint;
                UV cbLine = bPoint - cPoint;
                double acdNorm = caLine.CrossProduct(cdLine);
                double bcdNorm = cbLine.CrossProduct(cdLine);
                if (acdNorm*bcdNorm <= 0)
                {
                    result = true;
                }


            return result;
        }
        /// <summary>判断直线1是否被直线2跨越，即直线2的CD端点在直线1的两侧.</summary>        
        public bool IsBeCrossedOver(LineUV source)
        {
            //直线1的端点为AB，直线2的端点为CD
            bool result = false;

                UV abLine = Direction;
                UV aPoint = p_start;
                UV bPoint = p_end;
                UV cPoint = source.StartPoint;
                UV dPoint = source.EndPoint;
                UV acLine = cPoint - aPoint;
                UV adLine = dPoint - aPoint;
                double cabNorm = acLine.CrossProduct(abLine);
                double dabNorm = adLine.CrossProduct(abLine);
                //共面判断
                if (cabNorm*dabNorm<= 0)
                {
                    result = true;
                }

            return result;
        }
        /// <summary>判断直线1是否与直线2相互跨越，即直线1的AB端点在直线2的两侧，直线2的CD端点在直线1的两侧.</summary>        
        public bool IsCrossedEachOther(LineUV source)
        {
            //直线1的端点为AB，直线2的端点为CD
            bool result = false;

                //共面判断
                if (IsCrossOver(source) && IsBeCrossedOver(source))
                {
                    result = true;
                }
            
            return result;
        }

        /// <summary>判断直线1是否与直线2相交.</summary>        
        /// 同IsCrossedEachOther功能一致，此函数应会比较常用，所以使用易懂名称重写；
        public bool IsIntersected(LineUV source)
        {
            //直线1的端点为AB，直线2的端点为CD
            bool result = false;

                //共面判断
                if (IsCrossOver(source) && IsBeCrossedOver(source))
                {
                    result = true;
                }
            
            return result;
        }
        public UV GetIntersectPointExtended(LineUV source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //C在直线AB上投影为c，D在直线AB上投影为d；
            //d1=cC.getLength();d2=dD.getLength();
            UV CPoint = source.p_start;
            UV DPoint = source.p_end;
            UV cPoint = GetProjectPoint(CPoint);
            UV dPoint = GetProjectPoint(DPoint);
            double d1 = DistanceTo(CPoint);
            double d2 = DistanceTo(DPoint);
            UV result = (d1 / (d1 + d2)) * dPoint + (d2 / (d1 + d2)) * cPoint;
            return result;
        }
        public UV GetIntersectPoint(LineUV source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //C在直线AB上投影为c，D在直线AB上投影为d；
            //d1=cC.getLength();d2=dD.getLength();
            if (!IsCrossedEachOther(source))
            {
                throw new Exception("直线不相交");
            }
            UV CPoint = source.p_start;
            UV DPoint = source.p_end;
            UV cPoint = GetProjectPoint(CPoint);
            UV dPoint = GetProjectPoint(DPoint);
            //double d1 = DistanceTo(CPoint);
            //double d2 = DistanceTo(DPoint);
            UV Cc = cPoint - CPoint;
            UV dD = DPoint - dPoint;
            double d1 = Cc.U;
            double d2 = dD.U;
            UV result = (d1 / (d1 + d2)) * dPoint + (d2 / (d1 + d2)) * cPoint;
            return result;
        }
        public bool IsCrossOver(XLineUV source)
        {
            //直线1的端点为AB，直线2的端点为CD
            bool result = false;
            
                UV cdLine = source.Direction;
                UV aPoint = p_start;
                UV bPoint = p_end;
                UV cPoint = source.BasePoint;
                UV dPoint = source.Direction;
                UV caLine = aPoint - cPoint;
                UV cbLine = bPoint - cPoint;
                double acdNorm = caLine.CrossProduct(cdLine);
                double bcdNorm = cbLine.CrossProduct(cdLine);
                if (acdNorm*bcdNorm <= 0)
                {
                    result = true;
                }
           

            return result;
        }
        public UV GetIntersectPoint(XLineUV source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //C在直线AB上投影为c，D在直线AB上投影为d；
            //d1=cC.getLength();d2=dD.getLength();
            if (!IsCrossOver(source))
            {
                throw new Exception("直线不相交");
            }
            UV CPoint = source.BasePoint;
            UV DPoint = source.DirectionPoint;
            UV cPoint = GetProjectPoint(CPoint);
            UV dPoint = GetProjectPoint(DPoint);
            //double d1 = DistanceTo(CPoint);
            //double d2 = DistanceTo(DPoint);
            UV Cc = cPoint - CPoint;
            UV dD = DPoint - dPoint;
            double d1 = Cc.U;
            double d2 = dD.U;
            UV result = (d1 / (d1 + d2)) * dPoint + (d2 / (d1 + d2)) * cPoint;
            return result;
        }
        public XLineUV GetMiddlePerpendicularXLine()
        {
            LineUV verticalline = RotateByOrgin(Math.PI / 2); 
            return new XLineUV(MidPoint, verticalline.Direction);
        }

        public LineUV OffsetBy2Direction(UV startDirection, UV endDirection, double offsetDis)
        {

            double normal1 = startDirection.CrossProduct(Direction);
            double  normal2 = endDirection.CrossProduct(Direction);

          
            //sinθ1
            double sin1 = normal1 / startDirection.Length;
            double sin2 = normal2 / endDirection.Length;
            double l1 = offsetDis / sin1;
            double l2 = offsetDis / sin2;
            UV offsetPoint1 = StartPoint + startDirection.Normalize() * l1;
            UV offsetPoint2 = EndPoint + endDirection.Normalize() * l2;
            if (offsetPoint1.IsAlmostEqualTo(offsetPoint2))
            {
                return null;
            }
            else
            {
                LineUV lineResult = new LineUV(offsetPoint1, offsetPoint2);
                if (lineResult.Direction.IsAlmostEqualTo(Direction))
                {
                    return lineResult;
                }
                else
                {
                    return null;
                }
            }

        }
        public XLineUV ToXLineUV()
        {
            UV XLineDir =Direction;
            return new XLineUV(StartPoint, XLineDir);
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0},{1},{2})", new object[3]
            {
            "StartPoint:"+this[0].ToString(format, provider)+"\n",
            "EndPoint:"+this[1].ToString(format, provider)+"\n",
            "Length:"+Length.ToString(format, provider)
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
            list.AddRange(p_start.ToList());
            list.AddRange(p_end.ToList());
            list.Insert(0, CurveTypeEncoding.Line);
            list.Insert(0, list.Count);
            return list;
        }

        public static LineUV FromList(List<double> list)
        {
            var startPt = new UV(list[0], list[1]);
            var endPt = new UV(list[2], list[3]);
            var line = new LineUV(startPt, endPt);
            return line;
        }

        public bool TransformTo(TransformXYZ transform, out LineUV line)
        {
            line = new LineUV( transform.ApplyToUV(p_start),transform.ApplyToUV(p_end));
            return true;
        }

        public bool TransformTo(TransformXYZ transform, out ITransformable transformed)
        {
            transformed = new LineUV(transform.ApplyToUV(p_start), transform.ApplyToUV(p_end));
            return true;
        }

        public bool IsLine()
        {
            return true;
        }

        public bool IsArc()
        {
            return false;
        }

        public bool IsPolyline()
        {
            return false;
        }

        public bool IsPolyCurve()
        {
            return false;
        }

        public bool ClosestPoint(UV testPoint, out double t)
        {
            bool result = false;

            UV DXYZ = GetProjectPoint(testPoint);
            double position = 0;
            if (IsPointOnCurve(DXYZ, out position))
            {
                result = true;
                t = position * Length;
                return result;
            }
            else if (!double.IsNaN(position))
            {
                if (position > 1)
                {
                    result = false;
                    t = Length;
                    return result;
                }
                else if (position < 0)
                {
                    result = false;
                    t = 0;
                    return result;
                }
            }
            t = double.NaN;
            return result;
        }

   
    }

}
