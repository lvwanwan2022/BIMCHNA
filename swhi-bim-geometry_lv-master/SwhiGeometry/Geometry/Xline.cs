using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Geometry
{
    [Serializable]
    public class XLineXYZ:Base,ITransformable<XLineXYZ>
    {
        private XYZ base_point;
        private XYZ XLine_direction;
        /// <summary>
        /// 基点为XLine上距离原点最近的点
        /// </summary>
        public XYZ BasePoint => base_point;
        public XYZ DirectionPoint => base_point+ XLine_direction;
        /// <summary>
        /// 为单位向量
        /// </summary>
        public XYZ Vector => XLine_direction;
        public XYZ Direction => XLine_direction;
        public LineXYZ UnitLine => LineXYZ.CreateByStartPointAndVector(base_point, XLine_direction);
        public XYZ this[int idx] => idx switch
        {
            1 => XLine_direction,
            0 => base_point,
            _ => throw new Exception("索引错误"),
        };
        public XLineXYZ()
        {
            base_point = new XYZ();
            XLine_direction = new XYZ(1, 0, 0);
        }
        public XLineXYZ(XYZ pointOnXLine,XYZ vector)
        {
            LineXYZ line = new LineXYZ(pointOnXLine, pointOnXLine + vector);
            base_point = line.GetProjectPoint(new XYZ());
            XLine_direction = vector.Normalize();
        }
        public static XLineXYZ CreateBy2Point(XYZ point1, XYZ point2)
        {
            return new XLineXYZ(point1, point2 - point1);
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
        public bool IsPerpendicular(XLineXYZ source)
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
        public XLineXYZ Translation(XYZ vector)
        {
            XYZ p_start1 = base_point + vector;
            return new XLineXYZ(base_point, XLine_direction);
        }
        public XLineXYZ RotateByAxisAndDegree(XYZ axis, double degreeRadian)
        {
            if (degreeRadian <= Math.PI && degreeRadian >= -Math.PI)
            {
                //XYZ zaxis = new XYZ(0,0,1);
                XYZ base_point1 = base_point.RotateByAxisAndDegree(axis, degreeRadian);
                XYZ XLine_direction1 = XLine_direction.RotateByAxisAndDegree(axis, degreeRadian);
                return new XLineXYZ(base_point1, XLine_direction1);
            }
            else
            {
                throw new Exception("角度范围不在-π~π内");
            }
        }
        public void Reverse()
        {
            XLine_direction = -XLine_direction;
        }
        public bool IsPointOnCurve(XYZ source)
        {
            bool result = false;
            XYZ pointvector =(source - base_point).Normalize();
            if (pointvector.IsAlmostEqualTo(XLine_direction) || pointvector.IsAlmostEqualTo(new XYZ()))
            {
                result = true;
            }
            return result;
        }

        public double DistanceTo(XYZ source)
        {
            double result = 0;
            XYZ AO = source - base_point;
            XYZ AB = XLine_direction;
            result = (AB.CrossProduct(AO).GetLength()) / (AB.GetLength());
            return result;
        }
        public XYZ GetClosestPoint(XYZ source)
        {
            XYZ result;
            XYZ ABline = XLine_direction;
            XYZ AOline = source - base_point;
            XYZ planeNorm = (ABline.CrossProduct(AOline)).Normalize();
            XYZ DOnorm = (ABline.RotateByAxisAndDegree(planeNorm, Math.PI / 2)).Normalize();//
            double lengthDO = DistanceTo(source);
            XYZ Dpoint = source - (DOnorm * lengthDO);
            //double temp = 0;
            if (!IsPointOnCurve(Dpoint))
            {
                Dpoint = source + (DOnorm * lengthDO);
            }
            result = Dpoint;
            return result;
        }
        //以下方法为GetClosestXYZ同功能方法，重写方法名，便于识别；
        public XYZ GetProjectPoint(XYZ source)
        {
            XYZ result;
            XYZ ABline = XLine_direction;
            XYZ AOline = source - base_point;
            XYZ planeNorm = (ABline.CrossProduct(AOline)).Normalize();
            XYZ DOnorm = (ABline.RotateByAxisAndDegree(planeNorm, Math.PI / 2)).Normalize();//
            double lengthDO = DistanceTo(source);
            XYZ Dpoint = source - (DOnorm * lengthDO);
            if (!IsPointOnCurve(Dpoint))
            {
                Dpoint = source + (DOnorm * lengthDO);
            }
            result = Dpoint;
            return result;
        }

        public bool IsOnPlane(PlaneXYZ plane)
        {
            bool result = plane.IsPointOnPlane(base_point) && plane.IsPointOnPlane(base_point+XLine_direction);
            return result;
        }
        public bool IsOnOnePlane(XLineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            bool result = false;
            XYZ dr = Direction;
            XYZ aPoint = base_point;
            XYZ cPoint = source.BasePoint;
            XYZ dPoint = source.BasePoint+source.Direction;
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
        /// <summary>
        /// 判断同一平面内是否相交
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool IsIntersectedOnPlane(XLineXYZ source)
        {
            bool result = IsOnOnePlane(source) && !IsParallel(source);
            return result;
        }
        public XYZ GetIntersectPoint(XLineXYZ source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //C在直线AB上投影为c，D在直线AB上投影为d；
            //d1=cC.getLength();d2=dD.getLength();
            if (!IsIntersectedOnPlane(source))
            {
                throw new Exception("直线不相交");
            }
            XYZ CPoint = source.BasePoint;
            XYZ DPoint = source.BasePoint+source.Direction;
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
        public LineXYZ ToLineUnitLength()
        {
           return LineXYZ.CreateByStartPointAndVector(base_point, XLine_direction);
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0},{1})", new object[2]
            {
            "BasePoint:"+this[0].ToString(format, provider)+"\n",
            "Vector:"+this[1].ToString(format, provider)
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

        

        public bool TransformTo(TransformXYZ transform, out XLineXYZ transformed)
        {
            XYZ basetrans  ;
            bool a=base_point.TransformTo(transform, out basetrans);
            XYZ dirtrans;
            bool b=XLine_direction.TransformTo(transform, out dirtrans);
            if(a && b)
            {
                transformed = new XLineXYZ(basetrans, dirtrans);
                return true;
            }
            else
            {
                transformed = this;
                return false;
            }
           
        }
    }
    [Serializable]    
    public class XLineUV : Base
    {
        private UV base_point;
        private UV XLineUV_direction;
        /// <summary>
        /// 基点为XLineUV上任意点
        /// </summary>
        public UV BasePoint => base_point;
        public UV DirectionPoint => base_point + XLineUV_direction;
        /// <summary>
        /// 为单位向量
        /// </summary>
        public UV Vector => XLineUV_direction;
        public UV Direction => XLineUV_direction.Normalize();
        public UV this[int idx] => idx switch
        {
            1 => XLineUV_direction,
            0 => base_point,
            _ => throw new Exception("索引错误"),
        };
        public XLineUV()
        {
            base_point = new UV();
            XLineUV_direction = new UV(1, 0);
        }
        public XLineUV(UV pointOnXLineUV, UV vector)
        {
            //LineUV line = new LineUV(pointOnXLineUV, pointOnXLineUV + vector);
            //base_point = line.GetProjectPoint(new UV());
            base_point = pointOnXLineUV;
            XLineUV_direction = vector.Normalize();
        }
        public static XLineUV CreateBy2Point(UV point1, UV point2)
        {
            return new XLineUV(point1, point2 - point1);
        }

        //平行
        public bool IsParallel(XLineUV source)
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
        public bool IsPerpendicular(XLineUV source)
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
        public XLineUV Translation(UV vector)
        {
            UV p_start1 = base_point + vector;
            return new XLineUV(base_point, XLineUV_direction);
        }
        public XLineUV RotateByDegree(double degreeRadian)
        {
            //if (degreeRadian <= Math.PI && degreeRadian >= -Math.PI)
            //{
                //UV zaxis = new UV(0,0,1);
                UV base_point1 = base_point.RotateByDegree(degreeRadian);
                UV XLineUV_direction1 = XLineUV_direction.RotateByDegree(degreeRadian);
                return new XLineUV(base_point1, XLineUV_direction1);
            //}
            //else
            //{
            //    throw new Exception("角度范围不在-π~π内");
            //}
        }
        public void Reverse()
        {
            XLineUV_direction = -XLineUV_direction;
        }
        public bool IsPointOnCurve(UV source)
        {
            bool result = false;
            UV pointvector = (source - base_point).Normalize();
            if (pointvector.IsAlmostEqualTo(XLineUV_direction) || pointvector.IsAlmostEqualTo(new UV()))
            {
                result = true;
            }
            return result;
        }

        public double DistanceTo(UV source)
        {
            double result = 0;
            UV AO = source - base_point;
            UV AB = XLineUV_direction;
            result = (AB.CrossProduct(AO))/ (AB.GetLength());
            return result;
        }
        public UV GetClosestPoint(UV source)
        {
            UV result;
            UV ABline = XLineUV_direction;
            UV AOline = source - base_point;

            UV DOnorm = (ABline.RotateByDegree(Math.PI / 2)).Normalize();//
            double lengthDO = DistanceTo(source);
            UV Dpoint = source - (DOnorm * lengthDO);
            if (!IsPointOnCurve(Dpoint))
            {
                Dpoint = source + (DOnorm * lengthDO);
            }
            result = Dpoint;
            return result;
        }
        //以下方法为GetClosestUV同功能方法，重写方法名，便于识别；
        public UV GetProjectPoint(UV source)
        {
            UV result;
            UV ABline = XLineUV_direction;
            UV AOline = source - base_point;
           
            UV DOnorm = (ABline.RotateByDegree(Math.PI / 2)).Normalize();//
            double lengthDO = DistanceTo(source);
            UV Dpoint = source - (DOnorm * lengthDO);
            if (!IsPointOnCurve(Dpoint))
            {
                Dpoint = source + (DOnorm * lengthDO);
            }
            result = Dpoint;
            return result;
        }

        
       
        /// <summary>
        /// 判断同一平面内是否相交
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool IsIntersected(XLineUV source)
        {
            bool result = !IsParallel(source);
            return result;
        }
        public UV GetIntersectPoint(XLineUV source)
        {
            //直线1的端点为AB，直线2的端点为CD
            //C在直线AB上投影为c，D在直线AB上投影为d；
            //d1=cC.getLength();d2=dD.getLength();
            if (!IsIntersected(source))
            {
                throw new Exception("直线不相交");
            }
            UV CPoint = source.BasePoint;
            UV DPoint = source.BasePoint + (source.Direction);
            UV cPoint = GetProjectPoint(CPoint);
            UV dPoint = GetProjectPoint(DPoint);
            
            UV Cc = cPoint - CPoint;
            UV dD = DPoint - dPoint;
            double d1 = Cc.U;
            double d2 = dD.U;
            UV result = (d1 / (d1 + d2)) * dPoint + (d2 / (d1 + d2)) * cPoint;
            return result;
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0},{1})", new object[2]
            {
            "BasePoint:"+this[0].ToString(format, provider)+"\n",
            "Vector:"+this[1].ToString(format, provider)
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
